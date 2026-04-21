using Chytra_domacnost.Data;
using Chytra_domacnost.Models;
using Chytra_domacnost.Enums;
using System.Diagnostics;

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
            Console.WriteLine("7. [ADMIN] Správa pravidel");
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
            case "7":
                if (currentUser.Role == UserRole.Spravce)
                    ManageRules();
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

    private static void ManageRules()
    {
        Console.WriteLine("\n═══ SPRÁVA PRAVIDEL ═══");
        Console.WriteLine("1. Přidat pravidlo");
        Console.WriteLine("2. Zobrazit pravidla");
        Console.WriteLine("3. Vymazat pravidlo");
        Console.WriteLine("0. Zpět");
        Console.Write("\nVolba: ");

        switch (Console.ReadLine())
        {
            case "1":
                AddRule();
                break;
            case "2":
                ShowRules();
                break;
            case "3":
                DeleteRule();
                break;
            case "0":
                return;
            default:
                Console.WriteLine("Neplatná volba!");
                ManageRules();
                break;
        }
    }

    private static void DeleteRule()
    {
        if (db.Rules.Count == 0)
        {
            Console.WriteLine("\nŽádná pravidla k vymazání.");
            Console.WriteLine("\nStiskni ENTER...");
            Console.ReadLine();
            return;
        }

        Console.WriteLine("\n═══ VYMAZAT PRAVIDLO ═══");
        for (int i = 0; i < db.Rules.Count; i++)
        {
            Console.WriteLine($"[{db.Rules[i].Id}] {db.Rules[i].Name}");
        }

        Console.Write("\nID pravidla k vymazání (0 = zpět): ");
        var id = Console.ReadLine();
        var ruleToDelete = db.Rules.FirstOrDefault(r => r.Id == int.Parse(id ?? "0"));
        if (ruleToDelete != null)
        {
            db.Rules.Remove(ruleToDelete);
            db.Save();
            Console.WriteLine("✓ Pravidlo vymazáno!");
        }
        else
        {
            Console.WriteLine("Neplatné ID!");
        }
    }

    private static void ShowRules()
    {
        Console.WriteLine("\n═══ SEZNAM PRAVIDEL ═══");

        if (db.Rules.Count == 0)
        {
            Console.WriteLine("Žádná pravidla.");
            Console.WriteLine("\nStiskni ENTER...");
            Console.ReadLine();
            return;
        }

        for (int i = 0; i < db.Rules.Count; i++)
        {
            var rule = db.Rules[i];
            Console.WriteLine($"\n📋 [{rule.Id}] {rule.Name}");

            try
            {
                bool triggerState = EvaluateRuleTrigger(rule);
                Console.WriteLine($"    Stav triggeru: {(triggerState ? "✅ AKTIVNÍ" : "⏸️  Neaktivní")}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"    Stav triggeru: ❌ Chyba ({ex.Message})");
            }
        }

        Console.WriteLine("\nStiskni ENTER...");
        Console.ReadLine();
    }

    private static void AddRule()
    {
        Console.WriteLine("\nKdyž");
        Console.WriteLine("1. Zařízení");
        Console.WriteLine("2. Místnost");
        Console.WriteLine("3. Domácnost");
        Console.Write("\nVolba: ");
        var triggerType = Console.ReadLine();
        switch (triggerType)
        {
            case "1":
                {
                    var trigger = SetDeviceTrigger();
                    var action = SetRuleAction();
                    var name = SetRuleName();
                    var newId = SetRuleId();
                    var rule = new Rule(name, newId, trigger, action);
                    db.Rules.Add(rule);
                    db.Save();

                    break;
                }
            case "2":
                {
                    var trigger = SetRoomTrigger();
                    var action = SetRuleAction();
                    var name = SetRuleName();
                    var newId = SetRuleId();
                    var rule = new Rule(name, newId, trigger, action);
                    db.Rules.Add(rule);
                    db.Save();

                    break;
                }
            case "3":
                {
                    var trigger = SetHouseholdTrigger();
                    var action = SetRuleAction();
                    var name = SetRuleName();
                    var newId = SetRuleId();
                    var rule = new Rule(name, newId, trigger, action);
                    db.Rules.Add(rule);
                    db.Save();

                    break;
                }
        }

    }

    private static int SetRuleId()
    {
        Console.WriteLine("\nID pravidla: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Neplatné ID!");
            return SetRuleId();
        }
        else
        {
            return id;
        }
    }

    private static string SetRuleName()
    {
        Console.WriteLine("\nNázev pravidla: ");
        var name = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(name))
        {
            Console.WriteLine("Název nesmí být prázdný!");
            return SetRuleName();
        }
        else
        {
            return name;
        }
    }

    private static RuleActionDefinition SetRuleAction()
    {
        Console.WriteLine("\nPak");
        Console.WriteLine("1. Zapni zařízení");
        Console.WriteLine("2. Vypni zařízení");
        Console.WriteLine("3. Deaktivuj zařízení");
        Console.WriteLine("4. Nastav hodnotu zařízení");
        Console.WriteLine("5. Zapni všechna zařízení v místnosti");
        Console.WriteLine("6. Vypni všechna zařízení v místnosti");
        Console.Write("\nVolba: ");
        var volba = Console.ReadLine();

        switch (volba)
        {
            case "1":
                {
                    Console.WriteLine("\nID zařízení: ");
                    if (!int.TryParse(Console.ReadLine(), out int id))
                    {
                        Console.WriteLine("Neplatné ID zařízení!");
                        return SetRuleAction();
                    }

                    return new RuleActionDefinition
                    {
                        ActionType = RuleActionType.TurnOnDevice,
                        DeviceId = id
                    };
                }
            case "2":
                {
                    Console.WriteLine("\nID zařízení: ");
                    if (!int.TryParse(Console.ReadLine(), out int id))
                    {
                        Console.WriteLine("Neplatné ID zařízení!");
                        return SetRuleAction();
                    }

                    return new RuleActionDefinition
                    {
                        ActionType = RuleActionType.TurnOffDevice,
                        DeviceId = id
                    };
                }
            case "3":
                {
                    Console.WriteLine("\nID zařízení: ");
                    if (!int.TryParse(Console.ReadLine(), out int id))
                    {
                        Console.WriteLine("Neplatné ID zařízení!");
                        return SetRuleAction();
                    }

                    return new RuleActionDefinition
                    {
                        ActionType = RuleActionType.DeactivateDevice,
                        DeviceId = id
                    };
                }
            case "4": // Set device value
                {
                    Console.WriteLine("\nID zařízení: ");
                    if (!int.TryParse(Console.ReadLine(), out int id))
                    {
                        Console.WriteLine("Neplatné ID zařízení!");
                        return SetRuleAction();
                    }

                    Console.WriteLine("\nNová hodnota: ");
                    if (!int.TryParse(Console.ReadLine(), out int value))
                    {
                        Console.WriteLine("Neplatná hodnota!");
                        return SetRuleAction();
                    }

                    return new RuleActionDefinition
                    {
                        ActionType = RuleActionType.SetDeviceValue,
                        DeviceId = id,
                        Value = value
                    };
                }
            case "5": // Turn on all devices in room
                {
                    Console.WriteLine("\nMístnosti:");
                    foreach (var r in db.Rooms)
                        Console.WriteLine($"{r.Id}. {r.Name}");
                    Console.Write("\nID místnosti: ");

                    if (!int.TryParse(Console.ReadLine(), out int roomId))
                    {
                        Console.WriteLine("Neplatné ID místnosti!");
                        return SetRuleAction();
                    }

                    return new RuleActionDefinition
                    {
                        ActionType = RuleActionType.TurnOnAllDevicesInRoom,
                        RoomId = roomId
                    };
                }
            case "6": // Turn off all devices in room
                {
                    Console.WriteLine("\nMístnosti:");
                    foreach (var r in db.Rooms)
                        Console.WriteLine($"{r.Id}. {r.Name}");
                    Console.Write("\nID místnosti: ");

                    if (!int.TryParse(Console.ReadLine(), out int roomId))
                    {
                        Console.WriteLine("Neplatné ID místnosti!");
                        return SetRuleAction();
                    }

                    return new RuleActionDefinition
                    {
                        ActionType = RuleActionType.TurnOffAllDevicesInRoom,
                        RoomId = roomId
                    };
                }
            default:
                Console.WriteLine("Neplatná volba!");
                return SetRuleAction();
        }
    }

    private static RuleTriggerDefinition SetDeviceTrigger()
    {
        Console.WriteLine("\nTyp triggeru:");
        Console.WriteLine("1. Power consumption (spotřeba)");
        Console.WriteLine("2. State (stav zařízení)");
        Console.WriteLine("3. Current value (aktuální hodnota)");
        Console.Write("\nVolba: ");
        var triggerType = Console.ReadLine();

        // Ask for device ID
        Console.WriteLine("\nID zařízení: ");
        if (!int.TryParse(Console.ReadLine(), out int deviceId))
        {
            Console.WriteLine("Neplatné ID zařízení!");
            return SetDeviceTrigger();
        }

        switch (triggerType)
        {
            case "1": // Power consumption
                {
                    Console.WriteLine("\n1. > (větší než)");
                    Console.WriteLine("2. < (menší než)");
                    Console.WriteLine("3. == (rovná se)");
                    Console.Write("\nVolba: ");
                    var condition = Console.ReadLine();

                    Console.WriteLine("\nHodnota (W): ");
                    if (!int.TryParse(Console.ReadLine(), out int value))
                    {
                        Console.WriteLine("Neplatná hodnota!");
                        return SetDeviceTrigger();
                    }

                    var comparison = ParseComparison(condition);
                    if (comparison == RuleComparisonType.None)
                    {
                        Console.WriteLine("Neplatná volba!");
                        return SetDeviceTrigger();
                    }

                    return new RuleTriggerDefinition
                    {
                        TargetType = RuleTargetType.Device,
                        Metric = RuleTriggerMetric.PowerConsumption,
                        Comparison = comparison,
                        DeviceId = deviceId,
                        Value = value
                    };
                }
            case "2": // State
                {
                    Console.WriteLine("\nStav:");
                    Console.WriteLine("1. Zapnuto");
                    Console.WriteLine("2. Vypnuto");
                    Console.WriteLine("3. Deaktivovano");
                    Console.Write("\nVolba: ");
                    var stateChoice = Console.ReadLine();

                    var targetState = stateChoice switch
                    {
                        "1" => DeviceState.Zapnuto,
                        "2" => DeviceState.Vypnuto,
                        "3" => DeviceState.Deaktivovano,
                        _ => (DeviceState?)null
                    };

                    if (!targetState.HasValue)
                    {
                        Console.WriteLine("Neplatná volba!");
                        return SetDeviceTrigger();
                    }

                    return new RuleTriggerDefinition
                    {
                        TargetType = RuleTargetType.Device,
                        Metric = RuleTriggerMetric.State,
                        Comparison = RuleComparisonType.Equal,
                        DeviceId = deviceId,
                        TargetState = targetState.Value
                    };
                }
            case "3": // Current value
                {
                    Console.WriteLine("\n1. > (větší než)");
                    Console.WriteLine("2. < (menší než)");
                    Console.WriteLine("3. == (rovná se)");
                    Console.Write("\nVolba: ");
                    var condition = Console.ReadLine();

                    Console.WriteLine("\nHodnota: ");
                    if (!int.TryParse(Console.ReadLine(), out int value))
                    {
                        Console.WriteLine("Neplatná hodnota!");
                        return SetDeviceTrigger();
                    }

                    var comparison = ParseComparison(condition);
                    if (comparison == RuleComparisonType.None)
                    {
                        Console.WriteLine("Neplatná volba!");
                        return SetDeviceTrigger();
                    }

                    return new RuleTriggerDefinition
                    {
                        TargetType = RuleTargetType.Device,
                        Metric = RuleTriggerMetric.CurrentValue,
                        Comparison = comparison,
                        DeviceId = deviceId,
                        Value = value
                    };
                }
            default:
                {
                    Console.WriteLine("Neplatná volba!");
                    return SetDeviceTrigger();
                }
        }
    }

    private static RuleTriggerDefinition SetRoomTrigger()
    {
        Console.WriteLine("\nMístnosti:");
        foreach (var r in db.Rooms)
            Console.WriteLine($"{r.Id}. {r.Name}");

        Console.Write("\nID místnosti: ");
        if (!int.TryParse(Console.ReadLine(), out int roomId))
        {
            Console.WriteLine("Neplatné ID místnosti!");
            return SetRoomTrigger();
        }

        Console.WriteLine("\nTyp triggeru:");
        Console.WriteLine("1. Celková spotřeba místnosti");
        Console.WriteLine("2. Počet zapnutých zařízení");
        Console.WriteLine("3. Všechna zařízení vypnuta");
        Console.Write("\nVolba: ");
        var triggerType = Console.ReadLine();

        switch (triggerType)
        {
            case "1": // Total power
                {
                    Console.WriteLine("\n1. > (větší než)");
                    Console.WriteLine("2. < (menší než)");
                    Console.WriteLine("3. == (rovná se)");
                    Console.Write("\nVolba: ");
                    var condition = Console.ReadLine();

                    Console.WriteLine("\nHodnota (W): ");
                    if (!int.TryParse(Console.ReadLine(), out int value))
                    {
                        Console.WriteLine("Neplatná hodnota!");
                        return SetRoomTrigger();
                    }

                    var comparison = ParseComparison(condition);
                    if (comparison == RuleComparisonType.None)
                    {
                        Console.WriteLine("Neplatná volba!");
                        return SetRoomTrigger();
                    }

                    return new RuleTriggerDefinition
                    {
                        TargetType = RuleTargetType.Room,
                        Metric = RuleTriggerMetric.PowerConsumption,
                        Comparison = comparison,
                        RoomId = roomId,
                        Value = value
                    };
                }
            case "2": // Active devices count
                {
                    Console.WriteLine("\n1. > (více než)");
                    Console.WriteLine("2. < (méně než)");
                    Console.WriteLine("3. == (přesně)");
                    Console.Write("\nVolba: ");
                    var condition = Console.ReadLine();

                    Console.WriteLine("\nPočet zařízení: ");
                    if (!int.TryParse(Console.ReadLine(), out int value))
                    {
                        Console.WriteLine("Neplatná hodnota!");
                        return SetRoomTrigger();
                    }

                    var comparison = ParseComparison(condition);
                    if (comparison == RuleComparisonType.None)
                    {
                        Console.WriteLine("Neplatná volba!");
                        return SetRoomTrigger();
                    }

                    return new RuleTriggerDefinition
                    {
                        TargetType = RuleTargetType.Room,
                        Metric = RuleTriggerMetric.ActiveDeviceCount,
                        Comparison = comparison,
                        RoomId = roomId,
                        Value = value
                    };
                }
            case "3": // All devices off
                {
                    return new RuleTriggerDefinition
                    {
                        TargetType = RuleTargetType.Room,
                        Metric = RuleTriggerMetric.AllDevicesOff,
                        Comparison = RuleComparisonType.None,
                        RoomId = roomId
                    };
                }
            default:
                {
                    Console.WriteLine("Neplatná volba!");
                    return SetRoomTrigger();
                }
        }
    }

    private static RuleTriggerDefinition SetHouseholdTrigger()
    {
        Console.WriteLine("\nTyp triggeru:");
        Console.WriteLine("1. Celková spotřeba domácnosti");
        Console.WriteLine("2. Celkový počet zapnutých zařízení");
        Console.WriteLine("3. Všechna zařízení vypnuta");
        Console.Write("\nVolba: ");
        var triggerType = Console.ReadLine();

        switch (triggerType)
        {
            case "1": // Total household power
                {
                    Console.WriteLine("\n1. > (větší než)");
                    Console.WriteLine("2. < (menší než)");
                    Console.WriteLine("3. == (rovná se)");
                    Console.Write("\nVolba: ");
                    var condition = Console.ReadLine();

                    Console.WriteLine("\nHodnota (W): ");
                    if (!int.TryParse(Console.ReadLine(), out int value))
                    {
                        Console.WriteLine("Neplatná hodnota!");
                        return SetHouseholdTrigger();
                    }

                    var comparison = ParseComparison(condition);
                    if (comparison == RuleComparisonType.None)
                    {
                        Console.WriteLine("Neplatná volba!");
                        return SetHouseholdTrigger();
                    }

                    return new RuleTriggerDefinition
                    {
                        TargetType = RuleTargetType.Household,
                        Metric = RuleTriggerMetric.PowerConsumption,
                        Comparison = comparison,
                        Value = value
                    };
                }
            case "2": // Total active devices
                {
                    Console.WriteLine("\n1. > (více než)");
                    Console.WriteLine("2. < (méně než)");
                    Console.WriteLine("3. == (přesně)");
                    Console.Write("\nVolba: ");
                    var condition = Console.ReadLine();

                    Console.WriteLine("\nPočet zařízení: ");
                    if (!int.TryParse(Console.ReadLine(), out int value))
                    {
                        Console.WriteLine("Neplatná hodnota!");
                        return SetHouseholdTrigger();
                    }

                    var comparison = ParseComparison(condition);
                    if (comparison == RuleComparisonType.None)
                    {
                        Console.WriteLine("Neplatná volba!");
                        return SetHouseholdTrigger();
                    }

                    return new RuleTriggerDefinition
                    {
                        TargetType = RuleTargetType.Household,
                        Metric = RuleTriggerMetric.ActiveDeviceCount,
                        Comparison = comparison,
                        Value = value
                    };
                }
            case "3": // All devices off
                {
                    return new RuleTriggerDefinition
                    {
                        TargetType = RuleTargetType.Household,
                        Metric = RuleTriggerMetric.AllDevicesOff,
                        Comparison = RuleComparisonType.None
                    };
                }
            default:
                {
                    Console.WriteLine("Neplatná volba!");
                    return SetHouseholdTrigger();
                }
        }
    }

    private static RuleComparisonType ParseComparison(string? condition)
    {
        return condition switch
        {
            "1" => RuleComparisonType.GreaterThan,
            "2" => RuleComparisonType.LessThan,
            "3" => RuleComparisonType.Equal,
            _ => RuleComparisonType.None
        };
    }

    private static bool CompareValues(int left, RuleComparisonType comparison, int? right)
    {
        if (!right.HasValue)
        {
            return false;
        }

        return comparison switch
        {
            RuleComparisonType.GreaterThan => left > right.Value,
            RuleComparisonType.LessThan => left < right.Value,
            RuleComparisonType.Equal => left == right.Value,
            _ => false
        };
    }

    private static bool EvaluateRuleTrigger(Rule rule)
    {
        var trigger = rule.TriggerDefinition;
        if (trigger == null)
        {
            return false;
        }

        switch (trigger.TargetType)
        {
            case RuleTargetType.Device:
                {
                    if (!trigger.DeviceId.HasValue)
                    {
                        return false;
                    }

                    var device = db.Devices.FirstOrDefault(d => d.Id == trigger.DeviceId.Value);
                    if (device == null)
                    {
                        return false;
                    }

                    return trigger.Metric switch
                    {
                        RuleTriggerMetric.PowerConsumption => CompareValues(device.PowerConsumption, trigger.Comparison, trigger.Value),
                        RuleTriggerMetric.State => trigger.TargetState.HasValue && device.State == trigger.TargetState.Value,
                        RuleTriggerMetric.CurrentValue => device.CurrentValue.HasValue && CompareValues(device.CurrentValue.Value, trigger.Comparison, trigger.Value),
                        _ => false
                    };
                }
            case RuleTargetType.Room:
                {
                    if (!trigger.RoomId.HasValue)
                    {
                        return false;
                    }

                    var roomDevices = db.Devices.Where(d => d.RoomId == trigger.RoomId.Value).ToList();

                    return trigger.Metric switch
                    {
                        RuleTriggerMetric.PowerConsumption => CompareValues(roomDevices.Sum(d => d.PowerConsumption), trigger.Comparison, trigger.Value),
                        RuleTriggerMetric.ActiveDeviceCount => CompareValues(roomDevices.Count(d => d.State == DeviceState.Zapnuto), trigger.Comparison, trigger.Value),
                        RuleTriggerMetric.AllDevicesOff => roomDevices.All(d => d.State == DeviceState.Vypnuto),
                        _ => false
                    };
                }
            case RuleTargetType.Household:
                {
                    return trigger.Metric switch
                    {
                        RuleTriggerMetric.PowerConsumption => CompareValues(db.Devices.Sum(d => d.PowerConsumption), trigger.Comparison, trigger.Value),
                        RuleTriggerMetric.ActiveDeviceCount => CompareValues(db.Devices.Count(d => d.State == DeviceState.Zapnuto), trigger.Comparison, trigger.Value),
                        RuleTriggerMetric.AllDevicesOff => db.Devices.All(d => d.State == DeviceState.Vypnuto),
                        _ => false
                    };
                }
            default:
                return false;
        }
    }

    private static void ExecuteRuleAction(Rule rule)
    {
        var action = rule.ActionDefinition;
        if (action == null)
        {
            return;
        }

        switch (action.ActionType)
        {
            case RuleActionType.TurnOnDevice:
                {
                    if (!action.DeviceId.HasValue)
                    {
                        return;
                    }

                    var device = db.Devices.FirstOrDefault(d => d.Id == action.DeviceId.Value);
                    if (device != null)
                    {
                        device.State = DeviceState.Zapnuto;
                        Console.WriteLine($"🔔 Pravidlo: {device.Name} zapnuto");
                    }
                    break;
                }
            case RuleActionType.TurnOffDevice:
                {
                    if (!action.DeviceId.HasValue)
                    {
                        return;
                    }

                    var device = db.Devices.FirstOrDefault(d => d.Id == action.DeviceId.Value);
                    if (device != null)
                    {
                        device.State = DeviceState.Vypnuto;
                        Console.WriteLine($"🔔 Pravidlo: {device.Name} vypnuto");
                    }
                    break;
                }
            case RuleActionType.DeactivateDevice:
                {
                    if (!action.DeviceId.HasValue)
                    {
                        return;
                    }

                    var device = db.Devices.FirstOrDefault(d => d.Id == action.DeviceId.Value);
                    if (device != null)
                    {
                        device.State = DeviceState.Deaktivovano;
                        Console.WriteLine($"🔔 Pravidlo: {device.Name} deaktivováno");
                    }
                    break;
                }
            case RuleActionType.SetDeviceValue:
                {
                    if (!action.DeviceId.HasValue || !action.Value.HasValue)
                    {
                        return;
                    }

                    var device = db.Devices.FirstOrDefault(d => d.Id == action.DeviceId.Value);
                    if (device != null)
                    {
                        device.CurrentValue = action.Value.Value;
                        Console.WriteLine($"🔔 Pravidlo: {device.Name} nastaveno na {action.Value.Value}");
                    }
                    break;
                }
            case RuleActionType.TurnOnAllDevicesInRoom:
                {
                    if (!action.RoomId.HasValue)
                    {
                        return;
                    }

                    var devicesInRoom = db.Devices.Where(d => d.RoomId == action.RoomId.Value).ToList();
                    foreach (var device in devicesInRoom)
                    {
                        device.State = DeviceState.Zapnuto;
                    }
                    Console.WriteLine($"🔔 Pravidlo: Všechna zařízení v místnosti zapnuta ({devicesInRoom.Count} zařízení)");
                    break;
                }
            case RuleActionType.TurnOffAllDevicesInRoom:
                {
                    if (!action.RoomId.HasValue)
                    {
                        return;
                    }

                    var devicesInRoom = db.Devices.Where(d => d.RoomId == action.RoomId.Value).ToList();
                    foreach (var device in devicesInRoom)
                    {
                        device.State = DeviceState.Vypnuto;
                    }
                    Console.WriteLine($"🔔 Pravidlo: Všechna zařízení v místnosti vypnuta ({devicesInRoom.Count} zařízení)");
                    break;
                }
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

        Console.WriteLine($"\n{stateIcon} {device.Name}");
        Console.WriteLine($"    ID: {device.Id}");
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

        Console.Write("ID zařízení: ");
        if (!int.TryParse(Console.ReadLine(), out int deviceId) || deviceId <= 0)
        {
            Console.WriteLine("Neplatné ID zařízení!");
            return;
        }

        if (db.Devices.Any(d => d.Id == deviceId))
        {
            Console.WriteLine("Zařízení s tímto ID už existuje!");
            return;
        }

        var device = new Device
        {
            Id = deviceId,
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
