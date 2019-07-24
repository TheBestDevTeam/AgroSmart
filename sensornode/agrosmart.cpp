#include "agrosmart.h"


int ConnectWifi()
{
        int count=0;
        #ifndef SSID_PASS
                WiFi.begin(SSID_NAME);
        #else
                WiFi.begin(SSID_NAME, SSID_PASS);
        #endif

        while (count < MAX_RETRIES && WiFi.status() != WL_CONNECTED) {
                delay(500); count++;
        }
        return (WiFi.status() == WL_CONNECTED ? SUCCESS : -ECONNECT);
}

int HTTPInit(HTTPClient *http, String endpoint)
{
        http->begin(endpoint);
        http->addHeader("Content-Type", CONTENT_TYPE);
        return SUCCESS;
}

int PostData(HTTPClient *http, String settings)
{
        if (http == NULL)
                return -EPOST;
        return http->POST(settings);
}