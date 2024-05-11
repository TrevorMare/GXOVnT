#ifndef _GXOVnT_AUTOUPDATE_H
#define _GXOVnT_AUTOUPDATE_H

// This class internally uses the Wifi to connect to the wifi. After that, either a 
// WiFiClientSecure or WiFiClient is created depending if the server is http or https. The
// WiFiClientSecure can optionally validate server certificates. An http client is created on top of the wifi client to perform the 
// http operations. This updater class reads from Github (https)

#include <WiFiClientSecure.h>
#include <HTTPClient.h>

#include <SPIFFS.h>
#include "Update.h"
#include <ArduinoJson.h>

#define ssid "HouseMare"
#define password "X@Kbi-Rh3$"

// Define server details and file path
#define HOST "github.com"
#define PATH "/TrevorMare/GXOVnT/releases/download/Alpha/client_firmware.bin"
#define PORT 443
// Define the name for the downloaded firmware file
#define FILE_NAME "client_firmware.bin"

// Type of release of the firmware
enum GVOVNT_SYSTEM_FIRMWARE_RELEASE_TYPE {
    GVOVNT_SYSTEM_FIRMWARE_RELEASE_TYPE_ALPHA = 1,
    GVOVNT_SYSTEM_FIRMWARE_RELEASE_TYPE_BETA = 2,
    GVOVNT_SYSTEM_FIRMWARE_RELEASE_TYPE_RELEASE = 4
};

// Target of release of the firmware
enum GVOVNT_SYSTEM_FIRMWARE_SYSTEM_TYPE {
    GVOVNT_SYSTEM_FIRMWARE_SYSTEM_TYPE_CLIENT = 1,
    GVOVNT_SYSTEM_FIRMWARE_SYSTEM_TYPE_SERVER = 2
};

/*
    Object to help parse a symantic version and perform comparisons on
*/
struct GVOVnT_Version {
    public:
        // Version Major part
        int Major = 0;
        // Version Minor part
        int Minor = 0;
        // Version Revision part
        int Revision = 0;
        // Version Build number part
        int BuildNumber = 0;

        GVOVnT_Version();
        // Constructs a new version from a string
        GVOVnT_Version(std::string version) {
            sscanf(version.c_str(), "%d.%d.%d.%d", &Major, &Minor, &Revision, &BuildNumber);
        }
        // Operator less than
        bool operator<(const GVOVnT_Version &other) {
            return performOperatorCheck(other, false);
        }
        // Checks if this version is greater than a compared version
        bool operator>(const GVOVnT_Version &other) {
            return performOperatorCheck(other, true);
        }
        // Operator equal
        bool operator==(const GVOVnT_Version &other) {
            return (Major == other.Major && Minor == other.Minor && Revision == other.Revision && BuildNumber == other.BuildNumber);
        }
    private:
        bool performOperatorCheck(const GVOVnT_Version &other, bool amIGreaterCheck) {
            // Major comparison
            if (Major < other.Major || Major > other.Major) { return amIGreaterCheck ? Major > other.Major : Major < other.Major; }
            else {
                // Minor Comparison
                if (Minor < other.Minor || Minor > other.Minor) { return amIGreaterCheck ? Minor > other.Minor : Minor < other.Minor; }
                else {
                    // Revision Comparison
                    if (Revision < other.Revision || Revision > other.Revision) { return amIGreaterCheck ? Revision > other.Revision : Revision < other.Revision; }
                    else {
                        // Build Number Comparison
                        if (BuildNumber < other.BuildNumber || BuildNumber > other.BuildNumber) { return amIGreaterCheck ? BuildNumber > other.BuildNumber : BuildNumber < other.BuildNumber; }
                        else {
                            return false;
                        }
                    }
                }
            }
        }
};

/*
    Structure representing the available firmware versions 
*/
struct GVOVnT_SystemFirmware {
    public: 
        std::string FirmwareName = "";
        std::string DownloadUrl = "";
        std::string VersionNumber = "";
        GVOVnT_Version Version;
        enum GVOVNT_SYSTEM_FIRMWARE_RELEASE_TYPE FirmwareType = GVOVNT_SYSTEM_FIRMWARE_RELEASE_TYPE_RELEASE;
        enum GVOVNT_SYSTEM_FIRMWARE_SYSTEM_TYPE SystemType = GVOVNT_SYSTEM_FIRMWARE_SYSTEM_TYPE_CLIENT;
        GVOVnT_SystemFirmware(std::string firmwareName, std::string downloadUrl, std::string versionNumber, uint8_t firmwareType, uint8_t systemType) {
            FirmwareName = firmwareName;
            DownloadUrl = downloadUrl;
            VersionNumber = versionNumber;
            Version = GVOVnT_Version(versionNumber);
            FirmwareType = static_cast<GVOVNT_SYSTEM_FIRMWARE_RELEASE_TYPE>(firmwareType);
            SystemType = static_cast<GVOVNT_SYSTEM_FIRMWARE_SYSTEM_TYPE>(systemType);
        }
};

/*
    Helper class that will check for updates for the specifed system and automatically download and install 
    the specified version.
*/
class GXOVnT_WebUpdate {
    public:

        GXOVnT_WebUpdate() {

        }

        ~GXOVnT_WebUpdate() {
            if (m_wifiClientSecure != nullptr) {
                delete m_wifiClientSecure;
            }
        }

        void checkForUpdatesAndInstall();

    private:
        void getFileFromServer();
        void performOTAUpdateFromSPIFFS();
        void openWifiConnection();
        void downloadSystemFirmwareVersions();

        WiFiClientSecure *m_wifiClientSecure = nullptr;
        std::vector<GVOVnT_SystemFirmware*> m_availableFirmwareVersions; 

        
};

#endif