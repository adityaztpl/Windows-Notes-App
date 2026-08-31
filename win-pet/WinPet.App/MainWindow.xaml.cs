using System.Windows;
using WinPet.App.Data;
using WinPet.App.Services;

namespace WinPet.App;

public partial class MainWindow : Window
{
    private readonly Database _database = new();
    private ActivityMonitor? _monitor;
    private PetEngine? _engine;

    public MainWindow()
    {
        InitializeComponent();
        Loaded += OnLoaded;
        Closed += OnClosed;
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        await _database.InitializeAsync();
        _engine = new PetEngine(_database);
        await _engine.InitializeAsync();
        UpdateUi();
        _engine.PetChanged += _ => Dispatcher.Invoke(UpdateUi);
        _monitor = new ActivityMonitor(_database);
        _monitor.ActivityChanged += OnActivityChanged;
    }

    private async void OnActivityChanged(string process, string category, TimeSpan duration)
    {
        if (_engine is null || duration.TotalMinutes < 1) return;
        await _engine.ApplyActivityAsync(category, duration);
    }

    private void UpdateUi()
    {
        if (_engine is null) return;
        var pet = _engine.Pet;
        PetNameText.Text = pet.Name;
        LevelText.Text = $"Level {pet.Level}  •  {pet.Experience:N0} XP";
        EnergyBar.Value = pet.Energy;
        HappinessBar.Value = pet.Happiness;
        EnergyText.Text = $"{pet.Energy}%";
        HappinessText.Text = $"{pet.Happiness}%";
        TechnicalText.Text = $"Technical {pet.Technical}";
        KnowledgeText.Text = $"Knowledge {pet.Knowledge}";
        DisciplineText.Text = $"Discipline {pet.Discipline}";
    }

    private void OnClosed(object? sender, EventArgs e) => _monitor?.Dispose();
}
