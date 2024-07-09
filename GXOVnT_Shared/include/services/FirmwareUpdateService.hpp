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

struct InstallFirmwareVersionResult {
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
            ESP_LOGE(LOG_TAG, "%s", message, 299);
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
        FirmwareVersionData* GetLatestVersion() {
            if (AvailableVersions.size() == 0) return nullptr;

            uint8_t resultIndex = -1;
            FirmwareVersionDetail result = AvailableVersions.at(0)->Version;

            for (size_t i = 1; i < AvailableVersions.size(); i++)
            {
                if (AvailableVersions.at(i)->Version > result)
                {
                    result = AvailableVersions.at(i)->Version;
                    resultIndex = i;
                }
            }
            return AvailableVersions.at(resultIndex);
        }

        FirmwareVersionData* GetVersion(std::string firmwareVersion) {
            if (AvailableVersions.size() == 0) return nullptr;
            
            FirmwareVersionData *result = nullptr;

            for (size_t i = 1; i < AvailableVersions.size(); i++)
            {
                if (AvailableVersions.at(i)->VersionNumber.compare(firmwareVersion) == 0)
                {
                    result = AvailableVersions.at(i);
                    break;
                }
            }
            return result;
        }
};

class FirmwareUpdateService
{
    private:
        const char *firmware_list_fileName = "/firmwareVersions.json";
        
        std::string m_wifiSsid;
        std::string m_wifiPassword;
        std::string m_firmwareListUrl = std::string(GXOVNT_FIRMWARE_LIST_URL);
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
                    const char* host = jsonFirmwareVersion[JsonFieldHostName]; 
                    int hostPort = jsonFirmwareVersion[JsonFieldHostPort]; 

                    FirmwareVersionData *firmwareVersion = new FirmwareVersionData(firmwareName, 
                        downloadUrl, versionNumber, firmwareType, host, hostPort);

                    result->AvailableVersions.push_back(firmwareVersion);
                }
            }
            catch(...)
            {
                result->SetError("An unknown error occured reading the firmware versions json");
            }
            return result;
        }

        FirmwareServiceLoadListResult *loadFirmwareListFromFile(bool downloadLatestList) {
            
            if (downloadLatestList) {
                downloadFirmwareVersionsList();
            }
            
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
        
        InstallFirmwareVersionResult *downloadFirmwareVersion(std::string firmwareVersion) {
            
            InstallFirmwareVersionResult *result = new InstallFirmwareVersionResult();
            FirmwareServiceLoadListResult *firmwareList = loadFirmwareListFromFile(true);

            if (firmwareList->Success == false) {
                result->SetError(firmwareList->StatusMessage);
                return result;
            }

            FirmwareVersionData* installVersion = firmwareList->GetVersion(firmwareVersion);
            if (installVersion == nullptr) {
                result->SetError("Could not find the specified version to install");
                return result;
            }

            FirmwareServiceOpenWiFiResult *openWiFiResult = openWiFiClient();

            if (openWiFiResult->Success == false) {
                result->SetError(openWiFiResult->StatusMessage);
                return result;
            }

            ESP_LOGI(LOG_TAG, "Downloading and installing version %s for the system", installVersion->VersionNumber.c_str());

            const char *host = installVersion->Host.c_str();
            int hostPort = installVersion->HostPort;

            if (m_wiFiClient->connect(host, hostPort)) { // Connect to the server
                m_wiFiClient->print("GET " + String(installVersion->DownloadUrl.c_str()) + " HTTP/1.1\r\n"); // Send HTTP GET request
                m_wiFiClient->print("Host: " + String(host) + "\r\n"); // Specify the host
                m_wiFiClient->println("Connection: keep-alive\r\n"); // Close connection after response
                m_wiFiClient->println(); // Send an empty line to indicate end of request headers

                File file = SPIFFS.open("/" + String(GXOVNT_FIRMWARE_FILE_NAME), FILE_WRITE); // Open file in SPIFFS for writing
                if (!file) {
                    result->SetError("Could not open the firmware file for writing");
                    return result;
                }

                bool endOfHeaders = false;
                String headers = "";
                String http_response_code = "error";
                const size_t bufferSize = 1024; // Buffer size for reading data
                uint8_t buffer[bufferSize];

                // Loop to read HTTP response headers
                while (m_wiFiClient->connected() && !endOfHeaders) {
                    if (m_wiFiClient->available()) {
                        char c = m_wiFiClient->read();
                        headers += c;
                        if (headers.startsWith("HTTP/1.1")) {
                            http_response_code = headers.substring(9, 12);
                        }
                        if (headers.endsWith("\r\n\r\n")) { // Check for end of headers
                            endOfHeaders = true;
                        }
                    }
                }

                if (http_response_code != "200") {
                    result->SetError("Response code did not indicate success");
                    return result;
                }

                ESP_LOGI(LOG_TAG, "HTTP response code: %s", http_response_code); // Print received headers
                ESP_LOGI(LOG_TAG, "Starting download"); 

                int iDownload = 0;
                int totalFileSize = 0;

                // Loop to read and write raw data to file
                while (m_wiFiClient->connected()) {
                    if (m_wiFiClient->available()) {
                        size_t bytesRead = m_wiFiClient->readBytes(buffer, bufferSize);
                        file.write(buffer, bytesRead); // Write data to file
                        totalFileSize += bytesRead;
                    } else {
                        m_wiFiClient->stop();
                    }
                }

                Serial.println("Closing file");
                file.close(); // Close the file

                ESP_LOGI(LOG_TAG, "File saved successfully. Size [%d] bytes \n", totalFileSize);
            }
            else {
                result->SetError("Could not connect to the download server");
            }
            return result;
        }

        InstallFirmwareVersionResult *installFirmwareVersion() {
            // Open the firmware file in SPIFFS for reading
            InstallFirmwareVersionResult *result = new InstallFirmwareVersionResult();
            File file = SPIFFS.open("/" + String(GXOVNT_FIRMWARE_FILE_NAME), FILE_READ);
            if (!file) {
                result->SetError("Failed to open file for reading");
                return result;
            }
            
            size_t fileSize = file.size(); // Get the file size

            // Begin OTA update process with specified size and flash destination
            if (!Update.begin(fileSize, U_FLASH)) {
                result->SetError("An error occured during the update");
                return result;
            }

            // Write firmware data from file to OTA update
            Update.writeStream(file);

            // Complete the OTA update process
            if (Update.end()) {
                Serial.println("Successful update");
            }
            else {
                String updateError = "An error occured during the update: " + String(Update.getError());
                result->SetError(std::string(updateError.c_str()));
                return result;
            }

            file.close(); // Close the file

            return result;
        }

        InstallFirmwareVersionResult *downloadAndInstallFirmware(std::string firmwareVersion) {
            
            InstallFirmwareVersionResult *result = downloadFirmwareVersion(firmwareVersion);

            if (result->Success == false) {
                return result;
            }

            return installFirmwareVersion();
        }

        InstallFirmwareVersionResult *downloadAndInstallLatestFirmware() {
            
            InstallFirmwareVersionResult *result = new InstallFirmwareVersionResult();
            FirmwareServiceLoadListResult *availableVersionResult = loadFirmwareListFromFile(true);

            if (availableVersionResult->Success == false) {
                result->SetError(availableVersionResult->StatusMessage);
                return result;
            }

            FirmwareVersionData* latestVersion = availableVersionResult->GetLatestVersion();
            
            if (latestVersion == nullptr) {
                result->SetError("Could not find the latest firmware version");
                return result;
            }

            return downloadAndInstallFirmware(latestVersion->VersionNumber);
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

        InstallFirmwareVersionResult *InstallFirmwareVersion(std::string firmwareVersion) {
            return downloadAndInstallFirmware(firmwareVersion);
        }  

        InstallFirmwareVersionResult *InstallLatestFirmwareVersion(std::string firmwareVersion) {
            return downloadAndInstallLatestFirmware();
        }      

        ~FirmwareUpdateService() {
            closeWiFiClient();
        };
    };

}
#endif