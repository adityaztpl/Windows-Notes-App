using Dapper;
using Microsoft.Data.Sqlite;
using WinPet.App.Models;

namespace WinPet.App.Data;

public sealed class Database
{
    private readonly string _connectionString;

    public Database()
    {
        var directory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "WinPet");
        Directory.CreateDirectory(directory);
        _connectionString = new SqliteConnectionStringBuilder
        {
            DataSource = Path.Combine(directory, "winpet.db"),
            Mode = SqliteOpenMode.ReadWriteCreate,
            Cache = SqliteCacheMode.Shared
        }.ToString();
    }

    private SqliteConnection OpenConnection() => new(_connectionString);

    public async Task InitializeAsync()
    {
        await using var connection = OpenConnection();
        await connection.OpenAsync();
        await connection.ExecuteAsync("PRAGMA journal_mode=WAL; PRAGMA foreign_keys=ON;");
        await connection.ExecuteAsync("""
            CREATE TABLE IF NOT EXISTS pet_state (
                id TEXT PRIMARY KEY,
                name TEXT NOT NULL,
                level INTEGER NOT NULL,
                experience INTEGER NOT NULL,
                energy INTEGER NOT NULL,
                happiness INTEGER NOT NULL,
                technical INTEGER NOT NULL,
                knowledge INTEGER NOT NULL,
                creativity INTEGER NOT NULL,
                discipline INTEGER NOT NULL,
                exploration INTEGER NOT NULL,
                social INTEGER NOT NULL,
                created_at TEXT NOT NULL,
                updated_at TEXT NOT NULL
            );
            CREATE TABLE IF NOT EXISTS activity_sessions (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                process_name TEXT NOT NULL,
                category TEXT NOT NULL,
                started_at TEXT NOT NULL,
                ended_at TEXT NOT NULL,
                duration_seconds INTEGER NOT NULL
            );
            CREATE INDEX IF NOT EXISTS ix_activity_started ON activity_sessions(started_at);
            """);
    }

    public async Task<PetState> GetOrCreatePetAsync()
    {
        await using var connection = OpenConnection();
        await connection.OpenAsync();
        var pet = await connection.QuerySingleOrDefaultAsync<PetState>("SELECT * FROM pet_state LIMIT 1");
        if (pet is not null) return pet;

        pet = new PetState { Id = Guid.NewGuid() };
        await connection.ExecuteAsync("""
            INSERT INTO pet_state (id,name,level,experience,energy,happiness,technical,knowledge,creativity,discipline,exploration,social,created_at,updated_at)
            VALUES (@Id,@Name,@Level,@Experience,@Energy,@Happiness,@Technical,@Knowledge,@Creativity,@Discipline,@Exploration,@Social,@CreatedAt,@UpdatedAt)
            """, pet);
        return pet;
    }

    public async Task SavePetAsync(PetState pet)
    {
        pet.UpdatedAt = DateTime.UtcNow;
        await using var connection = OpenConnection();
        await connection.OpenAsync();
        await connection.ExecuteAsync("""
            UPDATE pet_state SET name=@Name,level=@Level,experience=@Experience,energy=@Energy,happiness=@Happiness,
            technical=@Technical,knowledge=@Knowledge,creativity=@Creativity,discipline=@Discipline,
            exploration=@Exploration,social=@Social,updated_at=@UpdatedAt WHERE id=@Id
            """, pet);
    }

    public async Task AddActivityAsync(ActivitySession activity)
    {
        await using var connection = OpenConnection();
        await connection.OpenAsync();
        await connection.ExecuteAsync("""
            INSERT INTO activity_sessions (process_name,category,started_at,ended_at,duration_seconds)
            VALUES (@ProcessName,@Category,@StartedAt,@EndedAt,@DurationSeconds)
            """, activity);
    }
}
