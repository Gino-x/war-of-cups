"""
PC-side listener for TWO custom Arduino controllers (Player 1 + Player 2).

Each Arduino runs the exact same controller.ino sketch — the boards don't
need to know about each other. This script opens both serial ports at once
(one thread per board) and applies a SEPARATE key map to each, so the same
physical button can mean different things on each controller.

Install dependencies first:
    pip install pyserial keyboard mouse

Windows note: run your terminal as Administrator if key/mouse events don't
register in games or other windows.
"""

import time
import threading
import serial
import keyboard
import mouse


# ---- CONFIG: set these to your two Arduinos' ports ----
# Check Device Manager > Ports (COM & LPT). Plug in one board at a time if
# you're not sure which port belongs to which physical controller.
PLAYER1_PORT = "COM7"
PLAYER2_PORT = "COM6"
BAUD = 9600

# A mapped value can be either:
#   - a plain string  -> a keyboard key, e.g. "space", "e", "up"
#   - a tuple ("mouse", button) -> a mouse button, e.g. ("mouse", "right")
PLAYER1_KEY_MAP = {
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

PLAYER2_KEY_MAP = {
    "BTN1":   "j",
    "BTN2":   "k",
    "BTN3":   "esc",
    "BTN4":   "l",
    "UP":     "t",
    "DOWN":   "g",
    "LEFT":   "f",
    "RIGHT":  "h",
    "CENTER": "2",
}


def dispatch(mapped, pressed):
    """Send a key press/release or a mouse button press/release."""
    if mapped is None:
        return
    if isinstance(mapped, tuple) and mapped[0] == "mouse":
        button = mapped[1]
        if pressed:
            mouse.press(button=button)
        else:
            mouse.release(button=button)
    else:
        key = mapped
        if pressed:
            keyboard.press(key)
        else:
            keyboard.release(key)


def release_all(key_map):
    for mapped in key_map.values():
        dispatch(mapped, pressed=False)


def listen(port, key_map, label):
    try:
        ser = serial.Serial(port, BAUD, timeout=1)
    except serial.SerialException as e:
        print(f"[{label}] Could not open {port}: {e}")
        return

    time.sleep(2)  # Arduino resets when the serial port opens; give it time to boot
    print(f"[{label}] Listening on {port}...")

    try:
        while True:
            raw = ser.readline().decode(errors="ignore").strip()
            if not raw or ":" not in raw:
                continue

            name, state = raw.split(":", 1)
            mapped = key_map.get(name)
            if mapped is None:
                continue

            dispatch(mapped, pressed=(state == "1"))

    except KeyboardInterrupt:
        pass
    finally:
        release_all(key_map)
        ser.close()
        print(f"[{label}] Stopped.")


def main():
    t1 = threading.Thread(target=listen, args=(PLAYER1_PORT, PLAYER1_KEY_MAP, "P1"), daemon=True)
    t2 = threading.Thread(target=listen, args=(PLAYER2_PORT, PLAYER2_KEY_MAP, "P2"), daemon=True)

    t1.start()
    t2.start()

    print("Both controllers running. Press Ctrl+C to quit.")
    try:
        while t1.is_alive() or t2.is_alive():
            time.sleep(0.5)
    except KeyboardInterrupt:
        print("\nStopping...")


if __name__ == "__main__":
    main()
