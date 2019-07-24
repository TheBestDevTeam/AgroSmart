#include "agrosmart.h"

#define MOISTURE_PIN A0

DHT_Unified dht(DHTPIN, DHTTYPE);
float min_moisture=-1, max_moisture=-1;
HTTPClient http, settings, sensor_status;
String endpoint = "http://agrosmartservice.southindia.cloudapp.azure.com:9000/api/device/";
String endpoint_s = "http://agrosmart.westus.azurecontainer.io/CropSettings/";
String endpoint_stat = "http://agrosmartservice.southindia.cloudapp.azure.com:9000/api/devicestatus/";
StaticJsonDocument<200> status_data;
String status;

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
        sensor_data["id"] = WiFi.macAddress();

        serializeJson(sensor_data, data);
        Serial.println(data);
        return PostData(http, data);
}

int RetrieveSettings(HTTPClient *http, int crop_id)
{
        endpoint_s.concat(crop_id);
        HTTPInit(http, endpoint_s);

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

int LessMoisture()
{
        float moisture = ( 100.00 - ( (analogRead(MOISTURE_PIN)/1024.00) * 100.00 ) );
        if (moisture < min_moisture)
                return 1;
        return 0;
}

int updateCropSettings()
{
        int crop_id = 0;
        if (Serial.available()) {
                crop_id = Serial.read();
                if (RetrieveSettings(&settings,crop_id) == 0)
                        return SUCCESS;
                return -ERROR;
        }
        return -ECONNECT;
}

int irrigateLand(HTTPClient *http)
{
        status_data["status"] = "Land is Currently being Irrigated";
        serializeJson(status_data, status);
        Serial.println(status);
        Serial.println();
        return PostData(http, status);
}

int NoIrrigation(HTTPClient *http)
{
        status_data["status"] = "No irrigation required. Currently Monitoring";
        serializeJson(status_data, status);
        Serial.println(status);
        Serial.println();
        return PostData(http, status);
}

void setup()
{ 
        Serial.begin(9600);
        dht.begin();
        delay(500);
        
        if ( ConnectWifi() == SUCCESS )
                Serial.println("Connected to WIFI Successfully");
        else
                Serial.println("Wifi Connection failed");

        HTTPInit(&http, endpoint);
        HTTPInit(&sensor_status, endpoint_stat);
        RetrieveSettings(&settings,3);
}

 
void loop() 
{
        status_data["id"] = WiFi.macAddress();
        if(WiFi.status()== WL_CONNECTED) {   //Check WiFi connection status
                Serial.println(endpoint);
                Serial.println(PostSensorData(&http));
                if (LessMoisture())
                        irrigateLand(&sensor_status);
                else
                        NoIrrigation(&sensor_status);
                status = "";
        }
        else {
                Serial.println("Error in WiFi connection");   
        }
        delay(20000);  //Send a request every 30 seconds
        updateCropSettings();
}
