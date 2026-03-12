#include <Arduino.h>
#include <RFReceiver.h>

void statusBlinking();
#include <RCSwitch.h>
RCSwitch mySwitch = RCSwitch();
int On = 2913;
int Off = 5131;

void setup()
{
	Serial.begin(115200);
	pinMode(LED_BUILTIN, OUTPUT);
	pinMode(D6, OUTPUT);
	digitalWrite(D6, LOW);
	Serial.println("");
	Serial.println("Setup done.");
	mySwitch.enableReceive(D3); // Receiver on interrupt 0 => that is pin #2
}

void loop()
{
	statusBlinking();
	if (mySwitch.available())
	{
		Serial.print("Received ");
		long receivedValue = mySwitch.getReceivedValue();
		Serial.print(receivedValue);
		Serial.print(" / ");
		Serial.print(mySwitch.getReceivedBitlength());
		Serial.print("bit ");
		Serial.print("Protocol: ");
		Serial.println(mySwitch.getReceivedProtocol());
		if (receivedValue == On)
		{
			digitalWrite(D6, HIGH);
		}
		else if (receivedValue == Off)
		{
			digitalWrite(D6, LOW);
		}

		mySwitch.resetAvailable();
	}
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