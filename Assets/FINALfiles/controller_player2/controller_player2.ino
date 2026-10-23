// Custom Arduino Uno keyboard-remap controller — PLAYER 2 BOARD VERSION
// Same as controller.ino, except Button 1's switch is on pin A5 instead of pin 2.
// 4 buttons (each with an LED) + 5-way directional pad (Up/Down/Left/Right/Center)
// Sends "NAME:1" on press and "NAME:0" on release over Serial.
// A PC-side Python script turns these into real keypresses.

// ----- Pin definitions -----
const int BTN1_PIN = A5;  // Button 1  -> J (Player 2)   [moved from pin 2]
const int LED1_PIN = 3;

const int BTN2_PIN = 4;   // Button 2  -> K
const int LED2_PIN = 5;

const int BTN3_PIN = 6;   // Button 3  -> Esc
const int LED3_PIN = 7;

const int BTN4_PIN = 8;   // Button 4  -> L
const int LED4_PIN = 9;

const int UP_PIN     = 10;  // -> Arrow Up
const int LED_UP_PIN = A1;

const int DOWN_PIN     = 11;  // -> Arrow Down
const int LED_DOWN_PIN = A2;

const int LEFT_PIN     = 12;  // -> Arrow Left
const int LED_LEFT_PIN = A3;

const int RIGHT_PIN     = 13;  // -> Arrow Right
const int LED_RIGHT_PIN = A4;

const int CENTER_PIN = A0;  // -> 2 (no LED)

const int NUM_INPUTS = 9;
const unsigned long DEBOUNCE_MS = 20;

struct InputPin {
  int pin;
  int ledPin;        // -1 if this input has no LED
  const char* name;  // must match the KEY_MAP names in the Python script
  int lastState;
  unsigned long lastChangeTime;
};

InputPin inputs[NUM_INPUTS] = {
  { BTN1_PIN,   LED1_PIN,     "BTN1",   HIGH, 0 },
  { BTN2_PIN,   LED2_PIN,     "BTN2",   HIGH, 0 },
  { BTN3_PIN,   LED3_PIN,     "BTN3",   HIGH, 0 },
  { BTN4_PIN,   LED4_PIN,     "BTN4",   HIGH, 0 },
  { UP_PIN,     LED_UP_PIN,   "UP",     HIGH, 0 },
  { DOWN_PIN,   LED_DOWN_PIN, "DOWN",   HIGH, 0 },
  { LEFT_PIN,   LED_LEFT_PIN, "LEFT",   HIGH, 0 },
  { RIGHT_PIN,  LED_RIGHT_PIN,"RIGHT",  HIGH, 0 },
  { CENTER_PIN, -1,           "CENTER", HIGH, 0 },
};

void setup() {
  Serial.begin(9600);

  for (int i = 0; i < NUM_INPUTS; i++) {
    pinMode(inputs[i].pin, INPUT_PULLUP);   // switch to GND, pressed = LOW
    if (inputs[i].ledPin != -1) {
      pinMode(inputs[i].ledPin, OUTPUT);
      digitalWrite(inputs[i].ledPin, HIGH);  // LED stays on all the time
    }
  }
}

void loop() {
  unsigned long now = millis();

  for (int i = 0; i < NUM_INPUTS; i++) {
    int reading = digitalRead(inputs[i].pin);

    if (reading != inputs[i].lastState &&
        (now - inputs[i].lastChangeTime) > DEBOUNCE_MS) {

      inputs[i].lastChangeTime = now;
      inputs[i].lastState = reading;

      bool pressed = (reading == LOW);

      Serial.print(inputs[i].name);
      Serial.print(':');
      Serial.println(pressed ? 1 : 0);
    }
  }
}
