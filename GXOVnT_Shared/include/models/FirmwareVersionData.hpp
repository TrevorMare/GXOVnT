/////////////////////////////////////////////////////////////////
#pragma once
#ifndef _GXOVNT_MODELS_FIRMWAREVERSIONDATA_H_
#define _GXOVNT_MODELS_FIRMWAREVERSIONDATA_H_
#include "JsonMessageConstants.h"
#include "shared/Definitions.h"

using namespace GXOVnTLib::models::constants;

namespace GXOVnTLib::models {

enum GXOVnT_FIRMWARE_VERSION_TYPE {
    FIRMWARE_VERSION_TYPE_ALPHA = 0, 
    FIRMWARE_VERSION_TYPE_BETA = 1, 
    FIRMWARE_VERSION_TYPE_RELEASE = 2
};

class FirmwareVersionDetail
{
    public:
        public:
        // Version Major part
        int Major = 0;
        // Version Minor part
        int Minor = 0;
        // Version Revision part
        int Revision = 0;
        // Version Build number part
        int BuildNumber = 0;

        FirmwareVersionDetail() {};
        // Constructs a new version from a string
        FirmwareVersionDetail(std::string version) {
            sscanf(version.c_str(), "%d.%d.%d.%d", &Major, &Minor, &Revision, &BuildNumber);
        }
        // Operator less than
        bool operator<(const FirmwareVersionDetail &other) {
            return performOperatorCheck(other, false);
        }
        // Checks if this version is greater than a compared version
        bool operator>(const FirmwareVersionDetail &other) {
            return performOperatorCheck(other, true);
        }
        // Operator equal
        bool operator==(const FirmwareVersionDetail &other) {
            return (Major == other.Major && Minor == other.Minor && Revision == other.Revision && BuildNumber == other.BuildNumber);
        }
    private:
        bool performOperatorCheck(const FirmwareVersionDetail &other, bool amIGreaterCheck) {
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

class FirmwareVersionData
{
    public:
        std::string FirmwareName = "";
        std::string DownloadUrl = "";
        std::string VersionNumber = "";
        FirmwareVersionDetail Version;
        std::string Host = "";
        int HostPort = 443;
        enum GXOVnT_FIRMWARE_VERSION_TYPE FirmwareType = FIRMWARE_VERSION_TYPE_RELEASE;
        
        // Constructor
        FirmwareVersionData(std::string firmwareName, std::string downloadUrl, std::string versionNumber, int firmwareType, std::string host, int hostPort) {
            FirmwareName = firmwareName;
            DownloadUrl = downloadUrl;
            VersionNumber = versionNumber;
            Version = FirmwareVersionDetail(versionNumber);
            FirmwareType = (GXOVnT_FIRMWARE_VERSION_TYPE)firmwareType;
            Host = host;
            HostPort = hostPort;
        }
};

}

#endif