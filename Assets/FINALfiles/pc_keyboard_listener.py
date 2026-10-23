"""
PC-side listener for the custom Arduino controller.

Reads "NAME:1" / "NAME:0" lines from the Arduino over serial and turns
them into real key presses/releases using the `keyboard` library.

Install dependencies first:
    pip install pyserial keyboard

Windows note: the `keyboard` library needs the script to run with admin
rights to inject key events system-wide. Run your terminal "as Administrator"
if key presses don't seem to register in games/other windows.
"""   

import time
import serial
import keyboard
# ---- CONFIG ----
PORT = "COM7"   # Change this to your Arduino's port (see Device Manager > Ports)
BAUD = 9600

KEY_MAP = {
    "BTN1":   "space",
    "BTN2":   "e",
    "BTN3":   "tab",
    "BTN4":   "q",
    "UP":     "w",
    "DOWN":   "s",
    "LEFT":   "a",
    "RIGHT":  "d",
    "CENTER": "1",
}


def main():
    ser = serial.Serial(PORT, BAUD, timeout=1)
    time.sleep(2)  # Arduino resets when the serial port opens; give it time to boot
    print(f"Listening on {PORT}... (Ctrl+C to quit)")

    try:
        while True:
            raw = ser.readline().decode(errors="ignore").strip()
            if not raw or ":" not in raw:
                continue

            name, state = raw.split(":", 1)
            key = KEY_MAP.get(name)
            if key is None:
                continue

            if state == "1":
                keyboard.press(key)
            else:
                keyboard.release(key)

    except KeyboardInterrupt:
        print("\nStopping.")
    finally:
        # Make sure nothing is left "stuck" held down
        for key in KEY_MAP.values():
            keyboard.release(key)
        ser.close()


if __name__ == "__main__":
    main()
