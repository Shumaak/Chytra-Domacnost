using System.Text.Json;
using Chytra_domacnost.Models;

namespace Chytra_domacnost.Data;

/// <summary>
/// Jednoduchá JSON databáze pro ukládání dat
/// </summary>
public class JsonDatabase
{
    private const string DataFile = "data.json";

    public List<User> Users { get; set; } = new();
    public List<Household> Households { get; set; } = new();
    public List<Room> Rooms { get; set; } = new();
    public List<Device> Devices { get; set; } = new();
    public List<Rule> Rules { get; set; } = new();


    /// <summary>
    /// Načte data ze souboru
    /// </summary>
    public static JsonDatabase Load()
    {
        if (!File.Exists(DataFile))
        {
            return new JsonDatabase();
        }

        try
        {
            var json = File.ReadAllText(DataFile);
            var db = JsonSerializer.Deserialize<JsonDatabase>(json);
            return db ?? new JsonDatabase();
        }
        catch
        {
            Console.WriteLine("⚠️ Chyba při načítání dat, vytvářím novou databázi");
            return new JsonDatabase();
        }
    }

    /// <summary>
    /// Uloží data do souboru
    /// </summary>
    public void Save()
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true // Pro hezký formát
        };

        var json = JsonSerializer.Serialize(this, options);
        File.WriteAllText(DataFile, json);
    }

    /// <summary>
    /// Vytvoří testovací data
    /// </summary>
    public void CreateTestData()
    {
        // Testovací uživatelé
        Users.Add(new User
        {
            Id = 1,
            Username = "admin",
            Password = "admin123",
            Role = Enums.UserRole.Spravce
        });

        Users.Add(new User
        {
            Id = 2,
            Username = "petr",
            Password = "petr123",
            Role = Enums.UserRole.Obyvatel
        });

        Users.Add(new User
        {
            Id = 3,
            Username = "host",
            Password = "host123",
            Role = Enums.UserRole.Host
        });

        // Testovací domácnost
        Households.Add(new Household
        {
            Id = 1,
            Name = "Můj dům",
            UserId = 1
        });

        // Testovací místnosti
        Rooms.Add(new Room { Id = 1, Name = "Obývák", HouseholdId = 1 });
        Rooms.Add(new Room { Id = 2, Name = "Ložnice", HouseholdId = 1 });
        Rooms.Add(new Room { Id = 3, Name = "Kuchyně", HouseholdId = 1 });

        // Testovací zařízení
        Devices.Add(new Device
        {
            Id = 1,
            Name = "Světlo obývák",
            Type = Enums.DeviceType.Svetlo,
            State = Enums.DeviceState.Vypnuto,
            PowerConsumption = 60,
            RoomId = 1
        });

        Devices.Add(new Device
        {
            Id = 2,
            Name = "Termostat",
            Type = Enums.DeviceType.Termostat,
            State = Enums.DeviceState.Zapnuto,
            PowerConsumption = 15,
            CurrentValue = 22,
            RoomId = 1
        });

        Devices.Add(new Device
        {
            Id = 3,
            Name = "Světlo ložnice",
            Type = Enums.DeviceType.Svetlo,
            State = Enums.DeviceState.Vypnuto,
            PowerConsumption = 40,
            RoomId = 2
        });

        Save();
    }
}
