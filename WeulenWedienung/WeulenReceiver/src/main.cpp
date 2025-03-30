#include <Arduino.h>
#include <RFReceiver.h>

void statusBlinking();
RFReceiver receiver(D3);

void setup()
{
  Serial.begin(115200);
  pinMode(LED_BUILTIN, OUTPUT);
  Serial.print("Setup done.");
  receiver.begin();
}

void loop()
{
  statusBlinking();
  byte data[MAX_PACKAGE_SIZE];
  byte from = 0;
  byte packageId = 0;
  if (!receiver.ready())
    return;

  Serial.println("Waiting");
  byte len = receiver.recvPackage(data, &from, &packageId);

  Serial.println("");
  Serial.print("Package: ");
  Serial.println(packageId);
  Serial.println("");
}

static unsigned long s_previousMillis = 0;
static bool s_ledOn = false;

void statusBlinking()
{
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