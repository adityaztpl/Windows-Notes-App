using System.Drawing;
using System.Windows;
using Forms = System.Windows.Forms;

namespace WinPet.App.Services;

public sealed class TrayService : IDisposable
{
    private readonly Forms.NotifyIcon _icon;
    private readonly Window _window;

    public TrayService(Window window)
    {
        _window = window;
        _icon = new Forms.NotifyIcon
        {
            Text = "WinPet",
            Icon = SystemIcons.Application,
            Visible = true
        };
        var menu = new Forms.ContextMenuStrip();
        menu.Items.Add("Show WinPet", null, (_, _) => Show());
        menu.Items.Add("Exit", null, (_, _) => Application.Current.Shutdown());
        _icon.ContextMenuStrip = menu;
        _icon.DoubleClick += (_, _) => Show();
    }

    private void Show()
    {
        _window.Show();
        _window.WindowState = WindowState.Normal;
        _window.Activate();
    }

    public void Dispose()
    {
        _icon.Visible = false;
        _icon.Dispose();
    }
}