#include <Arduino.h>
#include <RFReceiver.h>
#include <ESP8266WiFi.h>
#include <user_interface.h>
#include <RCSwitch.h>

RF_PRE_INIT()
{
    system_phy_set_powerup_option(4);
    wifi_set_opmode_current(NULL_MODE);
}

void statusBlinking();
void initMySwitch();
void mySwitchLoop();

const uint8_t RECEIVE_PIN = D2;
RCSwitch *mySwitch = nullptr;
bool mySwitchInitialized = false;
int On = 2913;
int Off = 5131;

void setup()
{
    pinMode(D1, OUTPUT);
    pinMode(D6, OUTPUT);
    pinMode(D7, OUTPUT);
    pinMode(D8, OUTPUT);
    digitalWrite(D6, LOW);
    digitalWrite(D1, LOW);
    Serial.begin(115200);

    pinMode(LED_BUILTIN, OUTPUT);
    Serial.println("");
    Serial.println("Setup done.");
    Serial.println(WiFi.getMode() == WIFI_OFF ? "WiFi OFF" : "WiFi ON");
    pinMode(RECEIVE_PIN, INPUT_PULLUP);
}

void loop()
{
    statusBlinking();
    initMySwitch();
    mySwitchLoop();
}

void initMySwitch()
{
    if (mySwitchInitialized || millis() < 3000)
        return;

    mySwitch = new RCSwitch();
    mySwitch->setReceiveTolerance(75);
    mySwitch->enableReceive(RECEIVE_PIN);
    mySwitchInitialized = true;
    Serial.println("RCSwitch enabled.");
}

void mySwitchLoop()
{
    if (!mySwitchInitialized || !mySwitch || !mySwitch->available())
        return;

    Serial.print("Received ");
    long receivedValue = mySwitch->getReceivedValue();
    Serial.print(receivedValue);
    Serial.print(" / ");
    Serial.print(mySwitch->getReceivedBitlength());
    Serial.print("bit ");
    Serial.print("Protocol: ");
    Serial.println(mySwitch->getReceivedProtocol());

    if (receivedValue == On)
    {
        digitalWrite(D6, LOW);
        digitalWrite(D7, HIGH);
    }
    else if (receivedValue == Off)
    {
        digitalWrite(D6, HIGH);
        digitalWrite(D7, LOW);
    }

    mySwitch->resetAvailable();
}

static unsigned long s_previousMillis = 0;
static bool s_ledOn = false;

void statusBlinking()
{
    if (millis() - s_previousMillis > 500)
    {
        Serial.println("Blink");
        if (s_ledOn)
        {
            digitalWrite(D8, HIGH);
            digitalWrite(LED_BUILTIN, HIGH);
        }
        else
        {
            digitalWrite(D8, LOW);
            digitalWrite(LED_BUILTIN, LOW);
        }
        s_ledOn = !s_ledOn;
        s_previousMillis = millis();
    }
}
