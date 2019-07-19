#ifndef AGROSMART_H_
#define AGROSMART_H_

#include <DHT.h>
#include <DHT_U.h>
#include<ESP8266WiFi.h>
#include <ESP8266HTTPClient.h>
#include <ArduinoJson.h>

#define SSID_NAME "MSFTGUEST"
#define ERROR 1
#define SUCCESS 0
#define ECONNECT 2
#define EPOST 3
#define ETEMP 4
#define EHUMIDITY 5
#define MAX_RETRIES 30
#define DHTTYPE DHT11
#define DHTPIN 0
#define CONTENT_TYPE "application/json"

int ConnectWifi();
int HTTPInit(HTTPClient *http, char *endpoint);
int PostSensorData(HTTPClient *http);
int PostSettings(HTTPClient *http, String settings);

#endif
