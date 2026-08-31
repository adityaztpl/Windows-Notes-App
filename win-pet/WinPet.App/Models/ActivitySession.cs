namespace WinPet.App.Models;

public sealed class ActivitySession
{
    public long Id { get; set; }
    public string ProcessName { get; set; } = string.Empty;
    public string Category { get; set; } = "Other";
    public DateTime StartedAt { get; set; }
    public DateTime EndedAt { get; set; }
    public int DurationSeconds { get; set; }
}
