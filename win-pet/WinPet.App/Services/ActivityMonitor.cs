using System.Diagnostics;
using WinPet.App.Data;
using WinPet.App.Models;

namespace WinPet.App.Services;

public sealed class ActivityMonitor : IDisposable
{
    private readonly Database _database;
    private readonly Timer _timer;
    private string? _currentProcess;
    private string _currentCategory = "Other";
    private DateTime _startedAt;

    public event Action<string, string, TimeSpan>? ActivityChanged;

    public ActivityMonitor(Database database)
    {
        _database = database;
        _timer = new Timer(_ => Sample(), null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
    }

    private void Sample()
    {
        try
        {
            var process = GetForegroundProcess();
            if (process is null) return;

            var processName = process.ProcessName;
            if (string.Equals(processName, _currentProcess, StringComparison.OrdinalIgnoreCase))
            {
                ActivityChanged?.Invoke(processName, _currentCategory, DateTime.UtcNow - _startedAt);
                return;
            }

            CloseCurrentSession();
            _currentProcess = processName;
            _currentCategory = Categorize(processName);
            _startedAt = DateTime.UtcNow;
            ActivityChanged?.Invoke(processName, _currentCategory, TimeSpan.Zero);
        }
        catch { }
    }

    private void CloseCurrentSession()
    {
        if (string.IsNullOrWhiteSpace(_currentProcess) || _startedAt == default) return;
        var ended = DateTime.UtcNow;
        var seconds = Math.Max(1, (int)(ended - _startedAt).TotalSeconds);
        _database.AddActivityAsync(new ActivitySession
        {
            ProcessName = _currentProcess,
            Category = _currentCategory,
            StartedAt = _startedAt,
            EndedAt = ended,
            DurationSeconds = seconds
        }).GetAwaiter().GetResult();
    }

    private static Process? GetForegroundProcess()
    {
        var hwnd = NativeMethods.GetForegroundWindow();
        if (hwnd == IntPtr.Zero) return null;
        NativeMethods.GetWindowThreadProcessId(hwnd, out var processId);
        if (processId == 0) return null;
        return Process.GetProcessById((int)processId);
    }

    private static string Categorize(string processName)
    {
        return processName.ToLowerInvariant() switch
        {
            "devenv" or "code" or "rider64" or "idea64" or "cursor" or "windowsterminal" or "powershell" or "pwsh" or "cmd" or "git" or "dotnet" or "node" => "Development",
            "chrome" or "msedge" or "firefox" or "opera" or "brave" => "Browser",
            "explorer" => "Files",
            "winword" or "excel" or "powerpnt" or "acrord32" => "Productivity",
            "spotify" or "vlc" => "Media",
            _ => "Other"
        };
    }

    public void Dispose()
    {
        CloseCurrentSession();
        _timer.Dispose();
    }

    private static class NativeMethods
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);
    }
}
