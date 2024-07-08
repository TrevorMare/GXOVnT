/////////////////////////////////////////////////////////////////
#pragma once
#ifndef _GXOVNT_SERVICES_FIRMWAREUPDATESERVICE_H_
#define _GXOVNT_SERVICES_FIRMWAREUPDATESERVICE_H_

#include "security/Certificates.h"
#include "models/JsonMessageConstants.h"
#include "models/FirmwareVersionData.hpp"

#include <ArduinoJson.h>
#include <WiFiClientSecure.h>
#include <HTTPClient.h>
#include <SPIFFS.h>
#include <Update.h>


using namespace GXOVnTLib::models;
using namespace GXOVnTLib::models::constants;

namespace GXOVnTLib::services {

struct FirmwareServiceOpenWiFiResult {
    int StatusCode = 200;
    std::string StatusMessage = "OK";
    bool Success = true;;
    void SetError(std::string message) {
        StatusCode = 500;
        Success = false;
        StatusMessage = message;
        ESP_LOGE(LOG_TAG, "%s", message);
    }
};

struct FirmwareServiceDownloadListResult {
    public:
        std::string JsonPayload = "";
        int StatusCode = 200;
        std::string StatusMessage = "OK";
        bool Success;
        void SetError(std::string message) {
            StatusCode = 500;
            StatusMessage = message;
            ESP_LOGE(LOG_TAG, "%s", message);
        }
};

struct FirmwareServiceLoadListResult {
    public:
        std::vector<FirmwareVersionData*> AvailableVersions;
        int StatusCode = 200;
        std::string StatusMessage = "OK";
        bool Success;
        void SetError(std::string message) {
            StatusCode = 500;
            StatusMessage = message;
            ESP_LOGE(LOG_TAG, "%s", message);
        }
};

class FirmwareUpdateService
{
    private:
        const char *firmware_list_fileName = "/firmwareVersions.json";
        
        std::string m_wifiSsid;
        std::string m_wifiPassword;
        std::string m_firmwareListUrl = "https://github.com/TrevorMare/GXOVnT/raw/main/firmware_versions.json";
        bool m_connectedToWiFi = false;
        WiFiClientSecure *m_wiFiClient = nullptr;

        FirmwareServiceOpenWiFiResult *openWiFiClient() {
            
            FirmwareServiceOpenWiFiResult *result = new FirmwareServiceOpenWiFiResult();
            
            if (m_connectedToWiFi) 
                return result;

            if (m_wifiSsid.compare("") == 0) {
                result->SetError("Could not connect to the WiFi, No SSID provided");
                return result;
            }

            bool connectionSuccess = false;
            bool connectionFailed = false;

            WiFi.begin(m_wifiSsid.c_str(), m_wifiPassword.c_str());

            // Wait until the connection has a response
            while (!connectionSuccess && !connectionFailed) {
                if (WiFi.status() == WL_CONNECT_FAILED) { connectionFailed = true; }
                else if (WiFi.status() == WL_CONNECTED) { connectionSuccess = true; }
            }

            if (connectionFailed) {
                WiFi.disconnect(true);
                result->SetError("Could not connect to the WiFi, No SSID provided");
                return result;
            }

            if (!connectionSuccess) {
                WiFi.disconnect(true);
                result->SetError("Could not connect to the WiFi, the state is neither failed nor success");
                return result;
            }

            m_connectedToWiFi = true;

            m_wiFiClient = new WiFiClientSecure();
            m_wiFiClient->setInsecure();
            m_wiFiClient->setCACert(GXOVnTLib::security::SSL_BALTIMORE);

            return result;
        }

        void closeWiFiClient() {
            if (!m_connectedToWiFi) return;

            if (m_wiFiClient != nullptr) {
                m_wiFiClient->stop();
                m_wiFiClient = nullptr;
            }

            WiFi.disconnect(true);
            m_connectedToWiFi = false;
        }

        FirmwareServiceDownloadListResult *downloadFirmwareVersionsList() {
            
            FirmwareServiceDownloadListResult *result = new FirmwareServiceDownloadListResult();
            FirmwareServiceOpenWiFiResult *openWiFiResult = openWiFiClient();

            if (openWiFiResult->Success == false) {
                result->SetError(openWiFiResult->StatusMessage);
                return result;
            }
            
            // No longer needed
            delete openWiFiResult;
            
            try
            {
                // Download the content
                HTTPClient http;
                http.useHTTP10(true);
                http.setFollowRedirects(HTTPC_FORCE_FOLLOW_REDIRECTS);
                http.begin(*m_wiFiClient, m_firmwareListUrl.c_str());
                http.GET();

                // Now parse the Json response
                JsonDocument jsonDocument;
                deserializeJson(jsonDocument, http.getStream());
                // Disconnect
                http.end();
                
                // Validate the json content
                if (jsonDocument.isNull()) {
                    result->SetError("Could not parse the the firmware list.");
                } else {
                    // Save the firmware versions file to disk
                    std::string firmwareVersionPayload;
                    serializeJson(jsonDocument, firmwareVersionPayload);

                    result->JsonPayload = firmwareVersionPayload;

                    if(!SPIFFS.begin(FORMAT_SPIFFS_IF_FAILED)){
                        result->SetError("Could not open the file system to write the content.");
                    } else {
                        File file = SPIFFS.open(firmware_list_fileName, FILE_WRITE);
                        file.print(String(firmwareVersionPayload.c_str()));
                        file.close();
                    }

                     SPIFFS.end();
                }
            }
            catch(...)
            {
                result->SetError("An error occured downloading the firmware list.");
            }

            closeWiFiClient();

            return result;
        }

        FirmwareServiceLoadListResult *loadFirmwareListFromJsonPayload(std::string jsonPayload) {
            FirmwareServiceLoadListResult *result = new FirmwareServiceLoadListResult();
            
            try
            {
                JsonDocument jsonDocument;
                std::vector<FirmwareVersionData*> firmwareVersions;

                deserializeJson(jsonDocument, jsonPayload);

                // For each of the objects in json, build a new struct represenation
                for (JsonObject jsonFirmwareVersion : jsonDocument[JsonFieldFirmwareVersions].as<JsonArray>()) {
                    
                    const char* firmwareName = jsonFirmwareVersion[JsonFieldFirmwareName]; 
                    const char* downloadUrl = jsonFirmwareVersion[JsonFieldDownloadUrl]; 
                    int firmwareType = jsonFirmwareVersion[JsonFieldFirmwareType]; 
                    const char* versionNumber = jsonFirmwareVersion[JsonFieldVersionNumber]; 

                    FirmwareVersionData *firmwareVersion = new FirmwareVersionData(firmwareName, 
                        downloadUrl, versionNumber, firmwareType);

                    result->AvailableVersions.push_back(firmwareVersion);
                }

            }
            catch(...)
            {
                result->SetError("An unknown error occured reading the firmware versions json");
            }
            return result;
        }

        FirmwareServiceLoadListResult *loadFirmwareListFromFile() {
            std::string filePayload = "";

            if(!SPIFFS.begin(FORMAT_SPIFFS_IF_FAILED)) {
                FirmwareServiceLoadListResult *result = new FirmwareServiceLoadListResult();
                result->SetError("Could not open the file system to read the content.");
                return result;

            } else {
                File file = SPIFFS.open(firmware_list_fileName, FILE_READ);
                if (file.available()) {
                    filePayload = std::string(file.readString().c_str());
                } 
                file.close();
            }
            SPIFFS.end();

            return loadFirmwareListFromJsonPayload(filePayload);
        }
        
    public:
        FirmwareUpdateService() {}
        FirmwareUpdateService(std::string wifiSsid, std::string wifiPassword = "", std::string firmwareListUrl = "") {
           Setup(wifiSsid, wifiPassword, firmwareListUrl);
        };
        void Setup(std::string wifiSsid, std::string wifiPassword = "", std::string firmwareListUrl = "") {
            m_wifiPassword = wifiPassword;
            m_wifiSsid = wifiSsid;
            if (firmwareListUrl.compare("") != 0) {
                m_firmwareListUrl = firmwareListUrl;
            }
        }

        FirmwareServiceDownloadListResult *DownloadLatestFirmwareList() {
            return downloadFirmwareVersionsList();
        } 

        ~FirmwareUpdateService() {
            closeWiFiClient();
        };
    };

}
#endif