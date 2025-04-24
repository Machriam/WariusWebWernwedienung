#include <Arduino.h>
#include <RCSwitch.h>

RCSwitch mySwitch = RCSwitch();

void statusBlinking();

void setup()
{
	Serial.begin(115200);
	pinMode(LED_BUILTIN, OUTPUT);
	Serial.println("");
	Serial.println("Setup done.");
	mySwitch.enableTransmit(D3);
}

void loop()
{
	mySwitch.send(1234, 24);
	delay(1000);
	statusBlinking();
	mySwitch.send(5678, 24);
	delay(1000);
	statusBlinking();
}

static unsigned long s_previousMillis = 0;
static bool s_ledOn = false;

void statusBlinking()
{
	if (millis() - s_previousMillis > 500)
	{
		Serial.println("Blink");
		if (s_ledOn)
			digitalWrite(LED_BUILTIN, HIGH);
		else
			digitalWrite(LED_BUILTIN, LOW);
		s_ledOn = !s_ledOn;
		s_previousMillis = millis();
	}
}