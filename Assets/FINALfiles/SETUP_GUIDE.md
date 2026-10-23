# Custom Arduino Keyboard Controller — Setup Guide

## What you need (parts list)
- 1x Arduino Uno + USB cable
- 4x momentary push buttons (for BTN1–BTN4)
- 8x LEDs (any color) — one per button and one per direction (Up/Down/Left/Right); Center has no LED
- 8x 220–330 ohm resistors (for the LEDs)
- 5x momentary switches for the directional pad (Up/Down/Left/Right/Center)
- Breadboard or perfboard
- Jumper wires

## Wiring

**Every switch** (all 9 of them — 4 buttons + 5-way pad) is wired the same way:
- One leg → the Arduino pin listed below
- Other leg → GND

The code uses `INPUT_PULLUP`, so pins read HIGH when the switch is open and LOW when pressed — no external resistor needed for the switches themselves.

| Input          | Arduino pin |
|-----------------|-------------|
| Button 1 (Space)| 2           |
| Button 2 (E)    | 4           |
| Button 3 (Esc)  | 6           |
| Button 4 (1)    | 8           |
| Up (W)          | 10          |
| Down (S)        | 11          |
| Left (A)        | 12          |
| Right (D)       | 13          |
| Center (Enter)  | A0          |

**Each LED** (8 total: all 4 buttons + Up/Down/Left/Right — Center has no LED):
- Long leg (anode) → 220–330 ohm resistor → Arduino pin listed below
- Short leg (cathode) → GND

| LED         | Arduino pin |
|-------------|-------------|
| LED 1 (Button 1) | 3     |
| LED 2 (Button 2) | 5     |
| LED 3 (Button 3) | 7     |
| LED 4 (Button 4) | 9     |
| LED Up      | A1          |
| LED Down    | A2          |
| LED Left    | A3          |
| LED Right   | A4          |

All 8 LEDs now turn on the moment the Arduino boots and stay on permanently — they don't turn off when you press the matching button/direction.

Note: pins 0 and 1 are deliberately left free — those are used internally for USB/Serial communication with your PC, so wiring anything to them will interfere.

## Uploading the Arduino code
1. Open `controller.ino` in the Arduino IDE.
2. Select **Tools > Board > Arduino Uno** and the correct **Port**.
3. Click Upload.
4. Open the Serial Monitor (9600 baud) and test each switch — you should see lines like `BTN1:1` on press and `BTN1:0` on release.

## Running the PC-side script
1. Install Python 3 if you don't have it.
2. Install dependencies:
   ```
   pip install pyserial keyboard
   ```
3. In `pc_keyboard_listener.py`, set `PORT` to your Arduino's COM port (check Device Manager > Ports (COM & LPT) — close the Arduino Serial Monitor first, only one program can use the port at a time).
4. Run it:
   ```
   python pc_keyboard_listener.py
   ```
5. On Windows, if key presses aren't registering in games, run your terminal as Administrator — the `keyboard` library needs elevated rights to inject system-wide input.

## Current key mapping
| Input   | Key   |
|---------|-------|
| Button 1| Space |
| Button 2| E     |
| Button 3| Tab   |
| Button 4| Q     |
| Up      | W     |
| Down    | S     |
| Left    | A     |
| Right   | D     |
| Center  | 1     |

To change a mapping later, edit the `KEY_MAP` dictionary in `pc_keyboard_listener.py` — no need to touch the Arduino code or re-upload.

## Optional: auto-start the script
If you don't want to manually run the Python script every time, you can turn it into a `.bat` file (Windows) that runs it at startup, or set it up as a scheduled task. Ask if you want that set up too.

## Two-player setup (2x Arduino Uno)
Each controller needs 13 pins (9 switches + 4 LEDs), and a Uno only has 18 usable pins — so one board per player, not one board for both.

- Flash **the exact same `controller.ino`** to both Unos, unchanged. Wiring is identical on each board — follow the same pin table above for both Player 1 and Player 2.
- Plug both boards in. Each shows up as its own COM port (e.g. COM3 and COM4) in Device Manager > Ports (COM & LPT). If you're not sure which port is which board, plug them in one at a time and see which port appears/disappears.
- Use `dual_controller_listener.py` instead of the single-controller script — it opens both ports at once and applies a separate key map to each, so the same physical button can do something different on each controller.
- Set `PLAYER1_PORT` and `PLAYER2_PORT` at the top of `dual_controller_listener.py` to match your two COM ports.
- This script also needs the `mouse` package (for Player 2's right-click button):
  ```
  pip install pyserial keyboard mouse
  ```

### Player 1 mapping
| Input   | Key   |
|---------|-------|
| Button 1| Space |
| Button 2| E     |
| Button 3| Tab   |
| Button 4| Q     |
| Up      | W     |
| Down    | S     |
| Left    | A     |
| Right   | D     |
| Center  | 1     |

### Player 2 mapping
| Input   | Key           |
|---------|---------------|
| Button 1| Right mouse click |
| Button 2| K             |
| Button 3| Esc           |
| Button 4| L             |
| Up      | Arrow Up      |
| Down    | Arrow Down    |
| Left    | Arrow Left    |
| Right   | Arrow Right   |
| Center  | 2             |
