#include "agrosmart.h"

DHT_Unified dht(DHTPIN, DHTTYPE);
sensors_event_t TempHumidity;

int PostSensorData(HTTPClient *http)
{
        char *data="";

        dht.temperature().getEvent(&TempHumidity);
        if (isnan(TempHumidity.temperature)) {
                Serial.println("Error reading temperature Data");
                return -ETEMP;
        }
        dht.humidity().getEvent(&TempHumidity);
        if(isnan(TempHumidity.relative_humidity)) {
                Serial.println("Error reading RH data");
                return -EHUMIDITY;
        }

        Serial.println(String("{ \"temperature\": "+ String(TempHumidity.temperature)+"}"));
        if (!http)
                return -EPOST;
        http->POST(String("{ \"temperature\": "+ String(TempHumidity.temperature)+"}"));
        return SUCCESS;
}

void setup()
{ 
        Serial.begin(9600);
        delay(500);
 
        //Serial.println();
        //Serial.print("MAC: ");
        //Serial.println(WiFi.macAddress());

        if ( ConnectWifi() == SUCCESS )
                Serial.println("Connected to WIFI Successfully");
        else
                Serial.println("Wifi Connection failed");
}
 
void loop() 
{
        if(WiFi.status()== WL_CONNECTED) {   //Check WiFi connection status
                HTTPClient http;
                HTTPInit(&http, "http://10.104.253.106:8080/");
                Serial.println(PostSettings(&http,"Hello"));
                PostSensorData(&http);
        }
        else {
                Serial.println("Error in WiFi connection");   
        }
        delay(30000);  //Send a request every 30 seconds
}
