## Postup

### 1. Příprava

Vytvořil jsem základní strukturu složek:
- Models - datové třídy
- Data - práce s jsonem
- Enums - Enumy
- Program.cs - hlavní logika

Přidal jsem Json package:
```bash
dotnet add package System.Text.Json
```

### 2. Vytvoření enumů

Udělal jsem tři enumy:

UserRole.cs:
- Host (0) - omezený přístup
- Obyvatel (1) - standardní přístup
- Spravce (2) - plný přístup

DeviceType.cs:
- Svetlo, Zasuvka, Senzor, Kamera, Termostat, Zabezpeceni, Zvonec, Zaluzie

DeviceState.cs:
- Vypnuto (0)
- Zapnuto (1)
- Deaktivovano (2)

### 3. Vytvoření modelů

Udělal jsem čtyři třídy:

User:
- Id, Username, Password, Role, LastLogin

Household:
- Id, Name, UserId

Room:
- Id, Name, HouseholdId

Device:
- Id, Name, Type, State, PowerConsumption, CurrentValue, RoomId

### 4. Implementace JSON

Vytvořil jsem classu JsonDatabase s třemi hlavními metodami:

Load():
- Načte data ze souboru data.json
- Pokud soubor neexistuje, vrátí prázdnou databázi

Save():
- Dá data do JSON formátu
- Ukládá do souboru data.json

CreateTestData():
- Vytváří testovací uživatele, zařízení atd.

### 5. Hlavní program

Implementoval jsem hlavní logiku v Program.cs:

Globální proměnné:
- currentUser - aktuálně přihlášený uživatel
- db - instance JSON databáze

Main():
- Zobrazí úvod
- Spustí hlavní smyčku pro menu

InitializeDatabase():
- Načte data pomocí JsonDatabase.Load()
- Pokud je databáze prázdná, vytvoří testovací data

### 6. Přihlašování

ShowLoginMenu():
- Zobrazí menu
- Čeká na volbu uživatele

Login():
- Načte username a heslo
- Vyhledá uživatele v databázi pomocí LINQ (Linq je super)
- Nastaví currentUser a LastLogin, pokud vše proběhlo jak má
- Uloží změny.

### 7. Hlavní menu podle role

ShowMainMenu():
- Zobrazí různé možnosti podle role uživatele

### 8. Zobrazení dat

ShowRooms():
- Vypíše všechny místnosti s info

ShowDevices():
- Vypíše všechny zařízení s info

### 9. Přidávání dat

AddRoom():
- Načte název místnosti od uživatele
- Vygeneruje nové ID
- Přidá místnost do databáze a uloží

AddDevice():
- Kontroluje jeslti exituje místnost
- Zobrazí dostupné místnosti
- Načte zařízení (název, typ, spotřeba)
- Vygeneruje ID a přidá do databáze

### 10. Ovládání zařízení

ControlDevice():
- Zobrazí seznam všech zařízení
- Načte zařízení podle ID od uživatele
- Nabídne akce: Zapnout, Vypnout, Deaktivovat (správce)

### 11. Správa uživatelů

ManageUsers():
- Pouze pro Správce
- Zobrazí seznam uživatelů
- Umožní přidat uživatele
- Vygeneruje ID a uloží

### Použití AI

AI jsem používal na:
- Refaktorování - Commenty, Summaries, obecně přehlednost a strukturu kódu
- Vzhled Menu v konzoli
- Občasnou pomoc s kódem
- Implementace validace
- Strukturu README.md