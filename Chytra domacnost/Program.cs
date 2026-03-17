using Chytra_domacnost.Data;
using Chytra_domacnost.Models;
using Chytra_domacnost.Enums;

namespace Chytra_domacnost;

class Program
{
    static User? currentUser = null;
    static JsonDatabase db = null!;

    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine("╔════════════════════════════════════════╗");
        Console.WriteLine("║  SIMULÁTOR CHYTRÉ DOMÁCNOSTI           ║");
        Console.WriteLine("╚════════════════════════════════════════╝\n");

        // Načtení dat z JSON souboru
        InitializeDatabase();

        // Hlavní smyčka programu
        while (true)
        {
            if (currentUser == null)
            {
                ShowLoginMenu();
            }
            else
            {
                ShowMainMenu();
            }
        }
    }

    static void InitializeDatabase()
    {
        Console.WriteLine("Načítám data...");

        db = JsonDatabase.Load();

        // Pokud je databáze prázdná, vytvoříme testovací data
        if (db.Users.Count == 0)
        {
            Console.WriteLine("Vytvářím testovací data...");
            db.CreateTestData();
            Console.WriteLine("✓ Testovací data vytvořena!\n");
        }
        else
        {
            Console.WriteLine($"✓ Načteno: {db.Users.Count} uživatelů, {db.Rooms.Count} místností, {db.Devices.Count} zařízení\n");
        }
    }

    static void ShowLoginMenu()
    {
        Console.WriteLine("\n═══ PŘIHLÁŠENÍ ═══");
        Console.WriteLine("1. Přihlásit se");
        Console.WriteLine("2. Zobrazit testovací účty");
        Console.WriteLine("0. Ukončit program");
        Console.Write("\nVolba: ");

        var choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
                Login();
                break;
            case "2":
                ShowTestAccounts();
                break;
            case "0":
                db.Save(); // Uložíme před ukončením
                Environment.Exit(0);
                break;
            default:
                Console.WriteLine("Neplatná volba!");
                break;
        }
    }

    static void ShowTestAccounts()
    {
        Console.WriteLine("\n═══ TESTOVACÍ ÚČTY ═══");
        Console.WriteLine("Správce:  admin / admin123");
        Console.WriteLine("Obyvatel: petr / petr123");
        Console.WriteLine("Host:     host / host123");
        Console.WriteLine("\nStiskni ENTER pro pokračování...");
        Console.ReadLine();
    }

    static void Login()
    {
        Console.Write("\nUživatelské jméno: ");
        var username = Console.ReadLine();

        Console.Write("Heslo: ");
        var password = Console.ReadLine();

        var user = db.Users.FirstOrDefault(u => u.Username == username && u.Password == password);

        if (user != null)
        {
            currentUser = user;
            user.LastLogin = DateTime.Now;
            db.Save(); // Uložíme změnu

            Console.WriteLine($"\n✓ Přihlášen jako {user.Username} ({user.Role})");
        }
        else
        {
            Console.WriteLine("\n✗ Špatné přihlašovací údaje!");
        }
    }

    static void ShowMainMenu()
    {
        Console.WriteLine($"\n═══ HLAVNÍ MENU ({currentUser!.Role}) ═══");
        Console.WriteLine("1. Zobrazit místnosti");
        Console.WriteLine("2. Zobrazit zařízení");
        Console.WriteLine("3. Přidat místnost");
        Console.WriteLine("4. Přidat zařízení");
        Console.WriteLine("5. Ovládat zařízení");

        if (currentUser.Role == UserRole.Spravce)
        {
            Console.WriteLine("6. [ADMIN] Správa uživatelů");
        }

        Console.WriteLine("0. Odhlásit se");
        Console.Write("\nVolba: ");

        var choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
                ShowRooms();
                break;
            case "2":
                ShowDevices();
                break;
            case "3":
                if (currentUser.Role >= UserRole.Obyvatel)
                    AddRoom();
                else
                    Console.WriteLine("Nemáte oprávnění!");
                break;
            case "4":
                if (currentUser.Role >= UserRole.Obyvatel)
                    AddDevice();
                else
                    Console.WriteLine("Nemáte oprávnění!");
                break;
            case "5":
                ControlDevice();
                break;
            case "6":
                if (currentUser.Role == UserRole.Spravce)
                    ManageUsers();
                else
                    Console.WriteLine("Nemáte oprávnění!");
                break;
            case "0":
                db.Save(); // Uložíme před odhlášením
                currentUser = null;
                Console.WriteLine("\n✓ Odhlášen");
                break;
            default:
                Console.WriteLine("Neplatná volba!");
                break;
        }
    }

    static void ShowRooms()
    {
        Console.WriteLine("\n═══ MÍSTNOSTI ═══");

        if (db.Rooms.Count == 0)
        {
            Console.WriteLine("Žádné místnosti.");
            Console.WriteLine("\nStiskni ENTER...");
            Console.ReadLine();
            return;
        }

        foreach (var room in db.Rooms)
        {
            var devicesInRoom = db.Devices.Where(d => d.RoomId == room.Id).ToList();
            var activeDevices = devicesInRoom.Count(d => d.State == DeviceState.Zapnuto);

            Console.WriteLine($"\n🏠 [{room.Id}] {room.Name}");
            Console.WriteLine($"    Počet zařízení: {devicesInRoom.Count}");
            Console.WriteLine($"    Zapnuto: {activeDevices}");
        }

        Console.WriteLine("\nStiskni ENTER...");
        Console.ReadLine();
    }

    static void ShowDevices()
    {
        Console.WriteLine("\n═══ ZAŘÍZENÍ ═══");

        if (db.Devices.Count == 0)
        {
            Console.WriteLine("Žádná zařízení.");
            Console.WriteLine("\nStiskni ENTER...");
            Console.ReadLine();
            return;
        }

        foreach (var device in db.Devices)
        {
            ShowDevice(device);
        }

        Console.WriteLine("\n1. Vyhledat zařízení");
        Console.WriteLine("0. zpět");
        Console.Write("\nVolba: ");

        var choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
                SearchDevice();
                break;
            case "0":
                return;
            default:
                Console.WriteLine("Neplatná volba!");
                ShowDevices();
                break;
        }
    }

    private static void SearchDevice()
    {

        Console.WriteLine("\n Vyhledat podle:");
        Console.WriteLine("1. Název");
        Console.WriteLine("2. Typ");
        Console.WriteLine("3. Místnost");
        Console.WriteLine("4. Stav");
        Console.WriteLine("5. Abecedně");
        Console.WriteLine("6. spotřeba");
        Console.WriteLine("0. Zpět");

        Console.Write("\nVolba: ");
        var choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
                SearchDeviceByName();
                break;
            case "2":
                SearchDeviceByType();
                break;
            case "3":
                SearchDeviceByRoom();
                break;
            case "4":
                SearchDeviceByState();
                break;
            case "5":
                SearchDeviceByAlphabet();
                break;
            case "6":
                SearchDeviceByPower();
                break;
            case "0":
                ShowDevices();
                break;
            default:
                Console.WriteLine("Neplatná volba!");
                SearchDevice();
                break;
        }

    }


    private static void SearchDeviceByName()
    {
        Console.WriteLine("\nNázev zařízení: ");
        var name = Console.ReadLine();
        var devices = db.Devices.Where(d => d.Name.Contains(name ?? "", StringComparison.OrdinalIgnoreCase)).ToList();

        if (devices.Count == 0)
        {
            Console.WriteLine("Nenalezeno žádné zařízení!");
            return;
        }

        foreach (var device in devices)
        {
            ShowDevice(device);
        }
    }

    private static void SearchDeviceByState()
    {
        Console.WriteLine("\nStav:");
        Console.WriteLine("1. Zapnuto");
        Console.WriteLine("2. Vypnuto");
        Console.WriteLine("3. Deaktivováno");
        Console.Write("\nVolba: ");
        var choice = Console.ReadLine();

        List<Device> devices = new List<Device>();

        switch (choice)
        {
            case "1":
                devices = db.Devices.Where(d => d.State == DeviceState.Zapnuto).ToList();
                break;
            case "2":
                devices = db.Devices.Where(d => d.State == DeviceState.Vypnuto).ToList();
                break;
            case "3":
                devices = db.Devices.Where(d => d.State == DeviceState.Deaktivovano).ToList();
                break;
            default:
                Console.WriteLine("Neplatná volba!");
                SearchDeviceByState();
                break;
        }
        if (devices.Count == 0)
        {
            Console.WriteLine("Nenalezeno žádné zařízení");
            return;
        }
        foreach (var device in devices)
        {
            ShowDevice(device);
        }
    }

    private static void SearchDeviceByRoom()
    {
        Console.WriteLine("\nMístnosti:");
        foreach (var r in db.Rooms)
            Console.WriteLine($"{r.Id}. {r.Name}");

        Console.Write("\nID místnosti: ");
        if (!int.TryParse(Console.ReadLine(), out int roomId) || !db.Rooms.Any(r => r.Id == roomId))
        {
            Console.WriteLine("Neplatné ID místnosti!");
            SearchDeviceByRoom();
            return;
        }
        else
        {
            var devices = db.Devices.Where(d => d.RoomId == roomId).ToList();
            if(devices.Count == 0)
        {
            Console.WriteLine("Nenalezeno žádné zařízení!");
            return;
        }
            foreach (var device in devices)
            {
                ShowDevice(device);
            }
        }
    }

    private static void SearchDeviceByType()
    {
        Console.WriteLine("\nTypy zařízení:");
        Console.WriteLine("0. Světlo");
        Console.WriteLine("1. Zásuvka");
        Console.WriteLine("2. Senzor");
        Console.WriteLine("3. Kamera");
        Console.WriteLine("4. Termostat");
        Console.WriteLine("5. Zabezpečení");
        Console.WriteLine("6. Zvonek");
        Console.WriteLine("7. Žaluzie");
        Console.Write("\nTyp: ");
        
        if (!int.TryParse(Console.ReadLine(), out int typeNum) || typeNum < 0 || typeNum > 7)
        {
            Console.WriteLine("Neplatný typ!");
            SearchDeviceByType();
            return;
        }
        else
        {
            var devices = db.Devices.Where(d => (int)d.Type == typeNum).ToList();
            foreach (var device in devices)
            {
                ShowDevice(device);
            }
        }
    }

    private static void SearchDeviceByAlphabet()
    {
        Console.WriteLine("\nZařízení seřazena podle abecedy:");
        var devices = db.Devices.OrderBy(d => d.Name).ToList();
        foreach (var device in devices)
        {
            ShowDevice(device);
        }
    }
    private static void SearchDeviceByPower()
    {
       Console.WriteLine("\nZařízení seřazena podle spotřeby:");
        var devices = db.Devices.OrderBy(d => d.PowerConsumption).ToList();
        foreach (var device in devices)
        {
            ShowDevice(device);
        }
    }

    private static void ShowDevice(Device device)
    {
        var room = db.Rooms.FirstOrDefault(r => r.Id == device.RoomId);
        var stateIcon = device.State == DeviceState.Zapnuto ? "🟢" :
                       device.State == DeviceState.Vypnuto ? "🔴" : "⚫";

        Console.WriteLine($"\n{stateIcon} [{device.Id}] {device.Name}");
        Console.WriteLine($"    Typ: {device.Type}");
        Console.WriteLine($"    Místnost: {room?.Name ?? "Neznámá"}");
        Console.WriteLine($"    Stav: {device.State}");
        Console.WriteLine($"    Spotřeba: {device.PowerConsumption}W");

        if (device.CurrentValue.HasValue)
            Console.WriteLine($"    Hodnota: {device.CurrentValue}");
    }

    static void AddRoom()
    {
        Console.Write("\nNázev místnosti: ");
        var name = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(name))
        {
            Console.WriteLine("Název nesmí být prázdný!");
            return;
        }

        var newId = db.Rooms.Count > 0 ? db.Rooms.Max(r => r.Id) + 1 : 1;

        var room = new Room
        {
            Id = newId,
            Name = name,
            HouseholdId = 1 // Pro jednoduchost používáme první domácnost
        };

        db.Rooms.Add(room);
        db.Save(); // Uložíme změnu

        Console.WriteLine($"✓ Místnost '{name}' přidána!");
    }

    static void AddDevice()
    {
        if (db.Rooms.Count == 0)
        {
            Console.WriteLine("Nejdřív vytvořte místnost!");
            return;
        }

        Console.WriteLine("\nMístnosti:");
        foreach (var r in db.Rooms)
            Console.WriteLine($"{r.Id}. {r.Name}");

        Console.Write("\nID místnosti: ");
        if (!int.TryParse(Console.ReadLine(), out int roomId) || !db.Rooms.Any(r => r.Id == roomId))
        {
            Console.WriteLine("Neplatné ID místnosti!");
            return;
        }

        Console.Write("Název zařízení: ");
        var name = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(name))
        {
            Console.WriteLine("Název nesmí být prázdný!");
            return;
        }

        Console.WriteLine("\nTypy zařízení:");
        Console.WriteLine("0. Světlo");
        Console.WriteLine("1. Zásuvka");
        Console.WriteLine("2. Senzor");
        Console.WriteLine("3. Kamera");
        Console.WriteLine("4. Termostat");
        Console.WriteLine("5. Zabezpečení");
        Console.WriteLine("6. Zvonek");
        Console.WriteLine("7. Žaluzie");
        Console.Write("\nTyp: ");

        if (!int.TryParse(Console.ReadLine(), out int typeNum) || typeNum < 0 || typeNum > 7)
        {
            Console.WriteLine("Neplatný typ!");
            return;
        }

        Console.Write("Spotřeba (W): ");
        if (!int.TryParse(Console.ReadLine(), out int power) || power < 0)
        {
            Console.WriteLine("Neplatná spotřeba!");
            return;
        }

        var newId = db.Devices.Count > 0 ? db.Devices.Max(d => d.Id) + 1 : 1;

        var device = new Device
        {
            Id = newId,
            Name = name,
            Type = (DeviceType)typeNum,
            State = DeviceState.Vypnuto,
            PowerConsumption = power,
            RoomId = roomId
        };

        db.Devices.Add(device);
        db.Save(); // Uložíme změnu

        Console.WriteLine($"✓ Zařízení '{device.Name}' přidáno!");
    }

    static void ControlDevice()
    {
        if (db.Devices.Count == 0)
        {
            Console.WriteLine("Žádná zařízení k ovládání!");
            Console.WriteLine("\nStiskni ENTER...");
            Console.ReadLine();
            return;
        }

        Console.WriteLine("\n═══ OVLÁDÁNÍ ZAŘÍZENÍ ═══");
        foreach (var d in db.Devices)
        {
            var room = db.Rooms.FirstOrDefault(r => r.Id == d.RoomId);
            var icon = d.State == DeviceState.Zapnuto ? "🟢" :
                      d.State == DeviceState.Vypnuto ? "🔴" : "⚫";
            Console.WriteLine($"{icon} {d.Id}. {d.Name} ({room?.Name}) - {d.State}");
        }

        Console.Write("\nID zařízení (0 = zpět): ");
        if (!int.TryParse(Console.ReadLine(), out int deviceId))
            return;

        if (deviceId == 0)
            return;

        var device = db.Devices.FirstOrDefault(d => d.Id == deviceId);
        if (device == null)
        {
            Console.WriteLine("Zařízení nenalezeno!");
            return;
        }

        if (device.State == DeviceState.Deaktivovano)
        {
            Console.WriteLine("Zařízení je deaktivované!");
            return;
        }

        Console.WriteLine("\n1. Zapnout");
        Console.WriteLine("2. Vypnout");

        if (currentUser!.Role == UserRole.Spravce)
        {
            Console.WriteLine("3. Deaktivovat");
        }

        Console.Write("\nAkce: ");

        var action = Console.ReadLine();

        switch (action)
        {
            case "1":
                device.State = DeviceState.Zapnuto;
                Console.WriteLine($"✓ {device.Name} ZAPNUTO");
                db.Save();
                break;
            case "2":
                device.State = DeviceState.Vypnuto;
                Console.WriteLine($"✓ {device.Name} VYPNUTO");
                db.Save();
                break;
            case "3":
                if (currentUser!.Role == UserRole.Spravce)
                {
                    device.State = DeviceState.Deaktivovano;
                    Console.WriteLine($"✓ {device.Name} DEAKTIVOVÁNO");
                    db.Save();
                }
                else
                {
                    Console.WriteLine("Pouze správce může deaktivovat zařízení!");
                }
                break;
        }
    }

    static void ManageUsers()
    {
        Console.WriteLine("\n═══ SPRÁVA UŽIVATELŮ ═══");

        foreach (var u in db.Users)
        {
            Console.WriteLine($"[{u.Id}] {u.Username} - {u.Role}");
            Console.WriteLine($"    Poslední přihlášení: {u.LastLogin}");
        }

        Console.WriteLine("\n1. Přidat uživatele");
        Console.WriteLine("0. Zpět");
        Console.Write("\nVolba: ");

        if (Console.ReadLine() != "1")
            return;

        Console.Write("\nUživatelské jméno: ");
        var username = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(username))
        {
            Console.WriteLine("Jméno nesmí být prázdné!");
            return;
        }

        if (db.Users.Any(u => u.Username == username))
        {
            Console.WriteLine("Uživatel již existuje!");
            return;
        }

        Console.Write("Heslo: ");
        var password = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(password))
        {
            Console.WriteLine("Heslo nesmí být prázdné!");
            return;
        }

        Console.WriteLine("\nRole:");
        Console.WriteLine("0. Host");
        Console.WriteLine("1. Obyvatel");
        Console.WriteLine("2. Správce");
        Console.Write("Volba: ");

        if (!int.TryParse(Console.ReadLine(), out int roleNum) || roleNum < 0 || roleNum > 2)
        {
            Console.WriteLine("Neplatná role!");
            return;
        }

        var newId = db.Users.Max(u => u.Id) + 1;

        var newUser = new User
        {
            Id = newId,
            Username = username,
            Password = password,
            Role = (UserRole)roleNum
        };

        db.Users.Add(newUser);
        db.Save();

        Console.WriteLine($"✓ Uživatel '{newUser.Username}' vytvořen!");
    }
}
