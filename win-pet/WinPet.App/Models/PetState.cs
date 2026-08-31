namespace WinPet.App.Models;

public sealed class PetState
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "Byte";
    public int Level { get; set; } = 1;
    public long Experience { get; set; }
    public int Energy { get; set; } = 100;
    public int Happiness { get; set; } = 75;
    public int Technical { get; set; }
    public int Knowledge { get; set; }
    public int Creativity { get; set; }
    public int Discipline { get; set; }
    public int Exploration { get; set; }
    public int Social { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
