/////////////////////////////////////////////////////////////////
#pragma once
#ifndef _GXOVNT_MODELS_GETFIRMWAREUPDATERESULTRESPONSE_H_
#define _GXOVNT_MODELS_GETFIRMWAREUPDATERESULTRESPONSE_H_
#include <ArduinoJson.h>
#include "JsonMessageConstants.h"
#include "BaseMessageModel.hpp"
#include "shared/Definitions.h"

using namespace GXOVnTLib::models::constants;

namespace GXOVnTLib::models {

struct FirmwareVersionResult {
    public:
        std::string FirmwareVersion = "";
        std::string DownloadUrl = "";
        std::string VersionNumber = "";
        int SystemType = 1; 
        bool InstalledVersion = false;
        
};

class GetFirmwareUpdateResultResponse : public BaseMessageModel
{

    public:
        GetFirmwareUpdateResultResponse(uint16_t requestCommMessageId = 0) : BaseMessageModel(MsgType_GetSystemSettingsResponse, requestCommMessageId) {};
        ~GetFirmwareUpdateResultResponse() {};

        void AddFirmwareVersionResult(FirmwareVersionResult &firmwareVersionResult) {
            JsonObject settingsObject = m_jsonDocument[JsonFieldFirmwareVersions].add<JsonObject>();

            settingsObject[JsonFieldFirmwareVersion] = firmwareVersionResult.FirmwareVersion;
            settingsObject[JsonFieldDownloadUrl] = firmwareVersionResult.DownloadUrl;
            settingsObject[JsonFieldFirmwareVersionInstalled] = firmwareVersionResult.InstalledVersion;
            settingsObject[JsonFieldVersionNumber] = firmwareVersionResult.VersionNumber;
            settingsObject[JsonFieldSystemType] = firmwareVersionResult.SystemType;

        }

        void AddFirmwareVersionResult(std::string firmwareVersion, std::string downloadUrl, std::string versionNumber, int systemType, bool installedVersion) {
            JsonObject settingsObject = m_jsonDocument[JsonFieldFirmwareVersions].add<JsonObject>();

            settingsObject[JsonFieldFirmwareVersion] = firmwareVersion;
            settingsObject[JsonFieldDownloadUrl] = downloadUrl;
            settingsObject[JsonFieldFirmwareVersionInstalled] = installedVersion;
            settingsObject[JsonFieldVersionNumber] = versionNumber;
            settingsObject[JsonFieldSystemType] = systemType;
        }

};

}

#endif