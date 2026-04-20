# Testing Rules - Step by Step Guide

## Login
1. Run the program
2. Login as: `admin` / `admin123`

---

## Test 1: Device Power Consumption Rule
**Goal:** Turn off "Světlo ložnice" when "Termostat" uses more than 10W

1. Go to Main Menu → Option `7` (Správa pravidel)
2. Choose `1` (Přidat pravidlo)
3. Choose `1` (Zařízení)
4. Choose trigger type `1` (Power consumption)
5. Enter device ID: `2` (Termostat)
6. Choose operator `1` (>)
7. Enter value: `10`
8. Choose action `2` (Vypni zařízení)
9. Enter device ID: `3` (Světlo ložnice)
10. Enter rule name: `Úspora energie`
11. Enter rule ID: `1`

**Expected:** Rule created successfully

**Test it:**
- Go to Main Menu → `7` → `3` (Testovat pravidla)
- Should trigger because Termostat has 15W (> 10W)
- Light in bedroom should turn OFF

---

## Test 2: Device State Rule
**Goal:** Turn on light when termostat is ON

1. Main Menu → `7` → `1` (Přidat pravidlo)
2. Choose `1` (Zařízení)
3. Choose trigger type `2` (State)
4. Enter device ID: `2` (Termostat)
5. Choose state `1` (Zapnuto)
6. Choose action `1` (Zapni zařízení)
7. Enter device ID: `1` (Světlo obývák)
8. Enter name: `Světlo při termostatu`
9. Enter ID: `2`

**Test it:**
- Go to Main Menu → `7` → `3`
- Should trigger because Termostat is ON
- Living room light should turn ON

---

## Test 3: Room Total Power Rule
**Goal:** Turn off all devices in living room if total power > 70W

1. Main Menu → `7` → `1`
2. Choose `2` (Místnost)
3. Choose room ID: `1` (Obývák)
4. Choose trigger type `1` (Celková spotřeba)
5. Choose operator `1` (>)
6. Enter value: `70`
7. Choose action `6` (Vypni všechna zařízení v místnosti)
8. Choose room ID: `1` (Obývák)
9. Enter name: `Limit obývák`
10. Enter ID: `3`

**Test it:**
- Living room has: Světlo (60W) + Termostat (15W) = 75W
- Should trigger because 75W > 70W
- All devices in living room should turn OFF

---

## Test 4: Room Active Devices Count
**Goal:** Alert when more than 1 device is ON in bedroom

1. Main Menu → `7` → `1`
2. Choose `2` (Místnost)
3. Choose room ID: `2` (Ložnice)
4. Choose trigger type `2` (Počet zapnutých zařízení)
5. Choose operator `1` (>)
6. Enter value: `1`
7. Choose action `2` (Vypni zařízení)
8. Enter device ID: `3` (any device)
9. Enter name: `Max zařízení ložnice`
10. Enter ID: `4`

**Test it:**
- First turn ON multiple devices in bedroom (you'll need to add more)
- Then test the rule

---

## Test 5: Household Total Power
**Goal:** If total household power > 100W, turn off a device

1. Main Menu → `7` → `1`
2. Choose `3` (Domácnost)
3. Choose trigger type `1` (Celková spotřeba)
4. Choose operator `1` (>)
5. Enter value: `100`
6. Choose action `2` (Vypni zařízení)
7. Enter device ID: `1`
8. Enter name: `Limit domácnosti`
9. Enter ID: `5`

**Test it:**
- Current total: 60W + 15W + 40W = 115W
- Should trigger because 115W > 100W
- Device 1 should turn OFF

---

## Test 6: All Devices in Room OFF
**Goal:** When all devices in living room are OFF, turn on one device

1. Main Menu → `7` → `1`
2. Choose `2` (Místnost)
3. Choose room ID: `1` (Obývák)
4. Choose trigger type `3` (Všechna zařízení vypnuta)
5. Choose action `1` (Zapni zařízení)
6. Enter device ID: `1` (Světlo obývák)
7. Enter name: `Auto zapnutí`
8. Enter ID: `6`

**Test it:**
- First turn OFF all devices in living room manually
- Then test rules
- Light should turn back ON

---

## Viewing Rules
- Main Menu → `7` → `2` (Zobrazit pravidla)
- Shows all rules with their current trigger states

## Deleting Rules
- Main Menu → `7` → `4` (Vymazat pravidlo)
- Choose rule number to delete

---

## Notes:
- Rules are stored in memory only (not saved to JSON)
- When you restart the program, all rules will be lost
- To save rules, you need to modify the Rule class structure
