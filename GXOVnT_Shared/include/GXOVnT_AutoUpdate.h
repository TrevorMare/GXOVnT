#ifndef _GXOVnT_AUTOUPDATE_H
#define _GXOVnT_AUTOUPDATE_H

#include <WiFi.h>
#include <SPIFFS.h>
#include "Update.h"
#include <WiFiClientSecure.h>
#include <ArduinoJson.h>

#define ssid "HouseMare"
#define password "X@Kbi-Rh3$"

// Define server details and file path
#define HOST "github.com"
#define PATH "/TrevorMare/GXOVnT/releases/download/Alpha/client_firmware.bin"
#define PORT 443
// Define the name for the downloaded firmware file
#define FILE_NAME "client_firmware.bin"


struct GVOVnT_SystemFirmware {
    std::string FirmwareVersionName = "";
    int FirmwareVersionNumber = 0;
    bool IsLatest = false;
    bool IsInstalled = false;
};

class GVOVnT_AutoUpdate {
    public:
        void checkForUpdatesAndInstall();

    private:
        void getFileFromServer();
        void performOTAUpdateFromSPIFFS();
        void openWifiConnection();

        void downloadSystemFirmwareVersions();

};

#endif