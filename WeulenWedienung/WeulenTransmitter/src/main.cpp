#include <Arduino.h>
#include <RH_ASK.h>
#include <SPI.h> // Not actually used but needed to compile

void statusBlinking();
RH_ASK driver(2000, D3, D5, 4); // tx, rx, ptt, ptt2

void statusBlinking();

void setup()
{
	Serial.begin(74880);
	pinMode(LED_BUILTIN, OUTPUT);
	Serial.println("");
	Serial.println("Setup done.");
	if (!driver.init())
		Serial.println("init failed");
}

void loop()
{
	statusBlinking();
    const char *msg = "Hello World!";
    driver.send((uint8_t *)msg, strlen(msg));
    driver.waitPacketSent();
    delay(1000);
}

static unsigned long s_previousMillis = 0;
static bool s_ledOn = false;

void statusBlinking()
{
	Serial.println("Blink");
	if (millis() - s_previousMillis > 500)
	{
		if (s_ledOn)
			digitalWrite(LED_BUILTIN, HIGH);
		else
			digitalWrite(LED_BUILTIN, LOW);
		s_ledOn = !s_ledOn;
		s_previousMillis = millis();
	}
}