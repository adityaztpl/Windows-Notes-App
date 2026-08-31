using WinPet.App.Data;
using WinPet.App.Models;

namespace WinPet.App.Services;

public sealed class PetEngine
{
    private readonly Database _database;
    public PetState Pet { get; private set; } = new();

    public event Action<PetState>? PetChanged;

    public PetEngine(Database database) => _database = database;

    public async Task InitializeAsync() => Pet = await _database.GetOrCreatePetAsync();

    public async Task ApplyActivityAsync(string category, TimeSpan duration)
    {
        var minutes = Math.Max(0, (int)duration.TotalMinutes);
        if (minutes < 1) return;

        var xp = Math.Min(10, Math.Max(1, minutes / 5));
        Pet.Experience += xp;
        Pet.Energy = Math.Clamp(Pet.Energy - Math.Min(3, minutes / 15), 0, 100);
        Pet.Happiness = Math.Clamp(Pet.Happiness + (category == "Development" ? 1 : 0), 0, 100);

        switch (category)
        {
            case "Development": Pet.Technical += xp; Pet.Discipline += Math.Max(1, xp / 2); break;
            case "Productivity": Pet.Knowledge += xp; Pet.Discipline += Math.Max(1, xp / 2); break;
            case "Browser": Pet.Exploration += xp; break;
            case "Media": Pet.Creativity += Math.Max(1, xp / 2); Pet.Happiness += 1; break;
            default: Pet.Exploration += Math.Max(1, xp / 2); break;
        }

        var nextLevel = Pet.Level * 100L;
        if (Pet.Experience >= nextLevel)
        {
            Pet.Level++;
            Pet.Energy = Math.Min(100, Pet.Energy + 10);
            Pet.Happiness = Math.Min(100, Pet.Happiness + 10);
        }

        NormalizeStats();
        await _database.SavePetAsync(Pet);
        PetChanged?.Invoke(Pet);
    }

    private void NormalizeStats()
    {
        Pet.Technical = Math.Clamp(Pet.Technical, 0, 1000);
        Pet.Knowledge = Math.Clamp(Pet.Knowledge, 0, 1000);
        Pet.Creativity = Math.Clamp(Pet.Creativity, 0, 1000);
        Pet.Discipline = Math.Clamp(Pet.Discipline, 0, 1000);
        Pet.Exploration = Math.Clamp(Pet.Exploration, 0, 1000);
        Pet.Social = Math.Clamp(Pet.Social, 0, 1000);
    }
}
