#include "agrosmart.h"

#define MOISTURE_PIN A0

DHT_Unified dht(DHTPIN, DHTTYPE);
long int min_moisture, max_moisture;

int PostSensorData(HTTPClient *http)
{
        String data;
        StaticJsonDocument<200> sensor_data;
        sensors_event_t event;
        dht.temperature().getEvent(&event);
        if (isnan(event.temperature)) {
                Serial.println(F("Error reading temperature!"));
                return -ETEMP;
        }
        Serial.print(F("Temperature: "));
        Serial.print(event.temperature);
        Serial.println(F("°C"));
        sensor_data["temperature"] = event.temperature;

        // Get humidity event and print its value.
        dht.humidity().getEvent(&event);
        if (isnan(event.relative_humidity)) {
                Serial.println(F("Error reading humidity!"));
                return -EHUMIDITY;
        }
        Serial.print(F("Humidity: "));
        Serial.print(event.relative_humidity);
        Serial.println(F("%"));

        sensor_data["humidity"] = event.relative_humidity;
        sensor_data["moisture"] = ( 100.00 - ( (analogRead(MOISTURE_PIN)/1024.00) * 100.00 ) );
        sensor_data["_id"] = WiFi.macAddress();

        serializeJson(sensor_data, data);
        Serial.println(data);
        if (!http)
                return -EPOST;
        http->POST(data);
        return SUCCESS;
}

int RetrieveSettings(HTTPClient *http)
{
        int status_code = http->GET();
        String payload = http->getString();
        if (status_code >= 400) {
                return -EGET;
        }
        const int capacity = JSON_OBJECT_SIZE(1) + 60;
        DynamicJsonDocument jsonBuffer(capacity);
        DeserializationError error = deserializeJson(jsonBuffer, payload);
        if (error) {
                return -ERROR;
        }
        JsonObject settings = jsonBuffer.as<JsonObject>();
        min_moisture = settings["min"].as<int>();
        max_moisture = settings["max"].as<int>();
        return SUCCESS;
} 

void setup()
{ 
        Serial.begin(9600);
        dht.begin();
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
                HTTPInit(&http, "http://10.104.253.55:8080/");
                Serial.println(PostSettings(&http,"Hello"));
                PostSensorData(&http);
        }
        else {
                Serial.println("Error in WiFi connection");   
        }
        delay(20000);  //Send a request every 30 seconds
}
