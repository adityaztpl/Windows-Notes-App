using WinPet.App.Models;

namespace WinPet.App.Services;

public sealed class EvolutionService
{
    public string GetSpecies(PetState pet)
    {
        if (pet.Level < 5) return "Byte";
        if (pet.Technical >= pet.Creativity && pet.Technical >= pet.Exploration) return "CyberFox";
        if (pet.Creativity >= pet.Technical && pet.Creativity >= pet.Knowledge) return "DreamFox";
        if (pet.Discipline >= 150) return "Guardian";
        if (pet.Exploration >= 150) return "Explorer";
        return "Byte";
    }

    public string GetEmoji(PetState pet) => GetSpecies(pet) switch
    {
        "CyberFox" => "🦊",
        "DreamFox" => "🦋",
        "Guardian" => "🐺",
        "Explorer" => "🦝",
        _ => pet.Level >= 3 ? "🐥" : "🐣"
    };
}