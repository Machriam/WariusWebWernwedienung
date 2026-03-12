#include <Arduino.h>
#include <RCSwitch.h>
#include <ESP8266WiFi.h>
#include <ESP8266WebServer.h>  
#include <secrets.h>

RCSwitch mySwitch = RCSwitch();
int On = 2913;
int Off = 5131;

ESP8266WebServer server(80);

void connectToWifi();
void handleOnRequest();
void handleOffRequest();
void setupServer();

void setup()
{
	Serial.begin(115200);
	pinMode(LED_BUILTIN, OUTPUT);
	connectToWifi();
	setupServer();
	Serial.println("");
	Serial.println("Setup done.");
	mySwitch.enableTransmit(D3);
}

void loop()
{
	server.handleClient();
}

static unsigned long s_previousMillis = 0;
static bool s_ledOn = false;

void connectToWifi(){
	Serial.print("Connecting to WiFi network: ");
    Serial.println(WIFI_SSID);
    
    WiFi.begin(WIFI_SSID, WIFI_PASSWORD);
    while (WiFi.status() != WL_CONNECTED) {
        delay(500);
        Serial.print(".");
        digitalWrite(LED_BUILTIN, !digitalRead(LED_BUILTIN));
    }
    
    Serial.println("");
    Serial.println("WiFi connected!");
    Serial.print("IP address: ");
    Serial.println(WiFi.localIP());
}

void setupServer() {
    server.on("/on", HTTP_POST, handleOnRequest);
    server.on("/off", HTTP_POST, handleOffRequest);
    
    server.begin();
    Serial.println("HTTP server started");
}

void handleOnRequest() {
    Serial.println("Received ON request");
    mySwitch.send(On, 24);
    server.send(200, "text/plain", "ON signal sent");
    digitalWrite(LED_BUILTIN, LOW);  
    delay(200);
    digitalWrite(LED_BUILTIN, HIGH); 
}

void handleOffRequest() {
    Serial.println("Received OFF request");
    mySwitch.send(Off, 24);
    server.send(200, "text/plain", "OFF signal sent");
    digitalWrite(LED_BUILTIN, LOW);  
    delay(200);
    digitalWrite(LED_BUILTIN, HIGH); 
}