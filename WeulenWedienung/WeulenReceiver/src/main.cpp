#include <Arduino.h>
#include <RFReceiver.h>
#include <ESP8266WiFi.h>
#include <user_interface.h>
// #include <RCSwitch.h>

// Runs before the SDK initialises the RF hardware — truly kills the boot spike.
// option 4 = no RF calibration, no RF power-up at all.
RF_PRE_INIT()
{
    system_phy_set_powerup_option(4);
    wifi_set_opmode_current(NULL_MODE);
}

void statusBlinking();
//RCSwitch mySwitch = RCSwitch();
int On = 2913;
int Off = 5131;

void setup()
{
    pinMode(D6, OUTPUT);
    digitalWrite(D6, LOW);

    Serial.begin(115200);

    pinMode(LED_BUILTIN, OUTPUT);
    Serial.println("");
    Serial.println("Setup done.");
    Serial.println(WiFi.getMode() == WIFI_OFF ? "WiFi OFF" : "WiFi ON");
    // mySwitch.enableReceive(D2); // Receiver on interrupt 0 => that is pin #2
}

void loop()
{
    statusBlinking();
    //mySwitchLoop();
}

// void mySwitchLoop()
// {
//     if (!mySwitch.available())
//         return;
//     Serial.print("Received ");
//     long receivedValue = mySwitch.getReceivedValue();
//     Serial.print(receivedValue);
//     Serial.print(" / ");
//     Serial.print(mySwitch.getReceivedBitlength());
//     Serial.print("bit ");
//     Serial.print("Protocol: ");
//     Serial.println(mySwitch.getReceivedProtocol());
//     if (receivedValue == On)
//     {
//         digitalWrite(D6, HIGH);
//     }
//     else if (receivedValue == Off)
//     {
//         digitalWrite(D6, LOW);
//     }

//     mySwitch.resetAvailable();
// }

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