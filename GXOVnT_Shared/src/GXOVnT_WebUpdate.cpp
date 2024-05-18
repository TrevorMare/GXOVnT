#include "GXOVnT_WebUpdate.h"

void GXOVnT_WebUpdate::checkForUpdatesAndInstall() {

  if (!SPIFFS.begin(true)) {
    ESP_LOGE(LOG_TAG, "SPIFFS Mount Failed, Unable to continue");
    return;
  }

  openWifiConnection();

  downloadSystemFirmwareVersions();

  const GVOVnT_SystemFirmware *latestFirmware = getLatestFirmwareForSystem(GVOVNT_SYSTEM_FIRMWARE_SYSTEM_TYPE_CLIENT);

  if (latestFirmware == nullptr)
    return;

  getFileFromServer(latestFirmware);

  performOTAUpdateFromSPIFFS();

}




void GXOVnT_WebUpdate::openWifiConnection() {

  // Begin connecting to WiFi using the provided SSID and password
  WiFi.begin(ssid, password);

#ifdef Heltec_Screen
  Heltec.display->clear();
  Heltec.display->drawString(0, 0, "Hello from lib");
  Heltec.display->display();
#endif

  ESP_LOGI(LOG_TAG, "Connecting to WiFi");

  // Display connection progress
  Serial.print("Connecting to WiFi");
  
  // Wait until WiFi is connected
  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
  }
  
  // Print confirmation message when WiFi is connected
  Serial.println("");
  Serial.println("WiFi connected");

  // Creating the secure wifi client
  m_wifiClientSecure = new WiFiClientSecure();
  m_wifiClientSecure->setInsecure();

}

void GXOVnT_WebUpdate::downloadSystemFirmwareVersions() {
  
  Serial.println("Downloading available firmware versions");

  // Clear the existing list as we will re-build this list
  clearFirmwareVersionList();

  // Download the firmware versions list
  HTTPClient http;
  http.useHTTP10(true);
  http.setFollowRedirects(HTTPC_FORCE_FOLLOW_REDIRECTS);
  http.begin(*m_wifiClientSecure, FIRMWARE_LIST_URL);
  http.GET();

  
  
  // Now parse the Json response
  JsonDocument doc;
  deserializeJson(doc, http.getStream());
  // Disconnect
  http.end();

  // Build a list of firmware versions
  if (doc.isNull())  {
    // Could not parse the document
    Serial.println("Firmware versions did not return a valid response, root is null");
  } else {
    // For each of the objects in json, build a new struct represenation
    for (JsonObject FirmwareVersion : doc["FirmwareVersions"].as<JsonArray>()) {
      const char* FirmwareVersion_FirmwareName = FirmwareVersion["FirmwareName"]; 
      const char* FirmwareVersion_DownloadUrl = FirmwareVersion["DownloadUrl"]; 
      int FirmwareVersion_FirmwareType = FirmwareVersion["FirmwareType"]; 
      int FirmwareVersion_SystemType = FirmwareVersion["SystemType"]; 
      const char* FirmwareVersion_VersionNumber = FirmwareVersion["VersionNumber"]; 

      GVOVnT_SystemFirmware *systemFirmware = new GVOVnT_SystemFirmware(FirmwareVersion_FirmwareName, FirmwareVersion_DownloadUrl, FirmwareVersion_VersionNumber,
        FirmwareVersion_FirmwareType, FirmwareVersion_SystemType);

      m_availableFirmwareVersions.push_back(systemFirmware);
    }
  }

  for (size_t i = 0; i < m_availableFirmwareVersions.size(); i++)
  {
    Serial.printf("Found version [%s] \n", m_availableFirmwareVersions[i]->FirmwareName.c_str());
  }
}

void GXOVnT_WebUpdate::clearFirmwareVersionList() {
    size_t firmwareListItemCount = m_availableFirmwareVersions.size();
    if (firmwareListItemCount > 0) {
        
        for (size_t deleteIndex = firmwareListItemCount -1; deleteIndex >= 0; deleteIndex--)
        {
            delete m_availableFirmwareVersions[deleteIndex];
        }
        
        m_availableFirmwareVersions.clear();
    }
}

const GVOVnT_SystemFirmware* GXOVnT_WebUpdate::getLatestFirmwareForSystem(enum GVOVNT_SYSTEM_FIRMWARE_SYSTEM_TYPE systemType, bool includeAlphaRelease, bool includeBetaRelease) {
  if (m_availableFirmwareVersions.size() == 0) 
    return nullptr;

  GVOVnT_SystemFirmware* latestFirmwareVersion = nullptr;
  for (size_t indexFirmwareVersion = 0; indexFirmwareVersion < m_availableFirmwareVersions.size(); indexFirmwareVersion++) {
    // Get the iteration firmware version
    GVOVnT_SystemFirmware* iterationFirmwareVersion = m_availableFirmwareVersions[indexFirmwareVersion];
    // Apply the filtering checks, we are only interested in firmwares matching the system type
    if (iterationFirmwareVersion->SystemType == systemType) {
      // Check if the release type matches the input filter
      if (iterationFirmwareVersion->FirmwareType == GVOVNT_SYSTEM_FIRMWARE_RELEASE_TYPE_RELEASE || 
         (iterationFirmwareVersion->FirmwareType == GVOVNT_SYSTEM_FIRMWARE_RELEASE_TYPE_ALPHA && includeAlphaRelease) || 
         (iterationFirmwareVersion->FirmwareType == GVOVNT_SYSTEM_FIRMWARE_RELEASE_TYPE_BETA && includeBetaRelease)) {
          // First version we encounter that is valid, just set it to the latest
          if (latestFirmwareVersion == nullptr) {
            latestFirmwareVersion = iterationFirmwareVersion;
          } else {
            // Compare the version, we are interested in the latest version
            if (latestFirmwareVersion->Version < iterationFirmwareVersion->Version) {
              latestFirmwareVersion = iterationFirmwareVersion;
            }
          }
      }
    }
  }
  // Return the latest firmware version
  return latestFirmwareVersion;
}

void GXOVnT_WebUpdate::getFileFromServer(const GVOVnT_SystemFirmware *firmwareVersion) {



  WiFiClientSecure client;
  //client.setInsecure(); // Set client to allow insecure connections
  client.setCACert(GXOVnT::security::SSL_BALTIMORE);
  

  if (client.connect(FIRMWARE_DOWNLOAD_HOST, FIRMWARE_DOWNLOAD_PORT)) { // Connect to the server
    
    Serial.printf("Connected to server. Attempting to download file [%s] \n", firmwareVersion->DownloadUrl.c_str());

    
    client.print("GET " + String(firmwareVersion->DownloadUrl.c_str()) + " HTTP/1.1\r\n"); // Send HTTP GET request
    client.print("Host: " + String(FIRMWARE_DOWNLOAD_HOST) + "\r\n"); // Specify the host
    client.println("Connection: keep-alive\r\n"); // Close connection after response
    client.println(); // Send an empty line to indicate end of request headers

    File file = SPIFFS.open("/" + String(FILE_NAME), FILE_WRITE); // Open file in SPIFFS for writing
    if (!file) {
      Serial.println("Failed to open file for writing");
      return;
    }

    bool endOfHeaders = false;
    String headers = "";
    String http_response_code = "error";
    const size_t bufferSize = 1024; // Buffer size for reading data
    uint8_t buffer[bufferSize];

    // Loop to read HTTP response headers
    while (client.connected() && !endOfHeaders) {
      if (client.available()) {
        char c = client.read();
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
      Serial.println("Response code did not indicate success");
      return;
    }

    Serial.println("HTTP response code: " + http_response_code); // Print received headers
    Serial.println("Starting download");

    int iDownload = 0;
    int totalFileSize = 0;


    // Loop to read and write raw data to file
    while (client.connected()) {
      if (client.available()) {
        size_t bytesRead = client.readBytes(buffer, bufferSize);
        file.write(buffer, bytesRead); // Write data to file
        
        totalFileSize += bytesRead;
      } else {
        client.stop();
      }
    }

    Serial.println("Closing file");
    file.close(); // Close the file
    //Serial.println("Stopping client");
    //client.stop(); // Close the client connection


    Serial.printf("File saved successfully. Size [%d] bytes \n", totalFileSize);
  }
  else {
    Serial.println("Failed to connect to server");
  }
}

void GXOVnT_WebUpdate::performOTAUpdateFromSPIFFS() {
  // Open the firmware file in SPIFFS for reading
  File file = SPIFFS.open("/" + String(FILE_NAME), FILE_READ);
  if (!file) {
    Serial.println("Failed to open file for reading");
    return;
  }

  Serial.println("Starting update..");
  size_t fileSize = file.size(); // Get the file size
  Serial.println(fileSize);

  // Begin OTA update process with specified size and flash destination
  if (!Update.begin(fileSize, U_FLASH)) {
    Serial.println("Cannot do the update");
    return;
  }

  // Write firmware data from file to OTA update
  Update.writeStream(file);

  // Complete the OTA update process
  if (Update.end()) {
    Serial.println("Successful update");
  }
  else {
    Serial.println("Error Occurred:" + String(Update.getError()));
    return;
  }

  file.close(); // Close the file
  Serial.println("Reset in 4 seconds....");
  delay(4000);
  ESP.restart(); // Restart ESP32 to apply the update
}