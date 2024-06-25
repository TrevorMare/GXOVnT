/////////////////////////////////////////////////////////////////
#pragma once
#ifndef _GXOVNT_MODELS_GETSYSTEMSETTINGSRESPONSE_H_
#define _GXOVNT_MODELS_GETSYSTEMSETTINGSRESPONSE_H_
#include <ArduinoJson.h>
#include "JsonMessageConstants.h"
#include "BaseMessageModel.hpp"
#include "shared/Definitions.h"

using namespace GXOVnTLib::models::constants;

namespace GXOVnTLib::models {

class GetSystemSettingsResponse : public BaseMessageModel
{
    private:
		std::string m_systemName;
		std::string m_firmwareVersion;
		std::string m_systemId;
		std::string m_wifiSsid;
		std::string m_wifiPassword;
		bool m_systemConfigured;
		int m_systemType;

	public:
        GetSystemSettingsResponse(uint16_t requestCommMessageId = 0) : BaseMessageModel(MsgType_GetSystemSettingsResponse, requestCommMessageId) {};
        ~GetSystemSettingsResponse() {};
        
        std::string SystemName() { return m_systemName; }
        void SystemName(std::string value) {
            m_systemName = value;
            m_jsonDocument[JsonFieldSystemName] = m_systemName;
        }

        std::string SystemId() { return m_systemId; }
        void SystemId(std::string value) {
            m_systemId = value;
            m_jsonDocument[JsonFieldSystemId] = m_systemId;
        }

        bool SystemConfigured() { return m_systemConfigured; }
        void SystemConfigured(bool value) {
            m_systemConfigured = value;
            m_jsonDocument[JsonFieldSystemConfigured] = m_systemConfigured;
        }

        int SystemType() { return m_systemType; }
        void SystemType(int value) {
            m_systemType = value;
            m_jsonDocument[JsonFieldSystemType] = m_systemType;
        }

        std::string FirmwareVersion() { return m_firmwareVersion; }
        void FirmwareVersion(std::string value) {
            m_firmwareVersion = value;
            m_jsonDocument[JsonFieldFirmwareVersion] = m_firmwareVersion;
        }

        std::string WifiSsid() { return m_wifiSsid; }
        void WifiSsid(std::string value) {
            m_wifiSsid = value;
            m_jsonDocument[JsonFieldWiFiSsid] = m_wifiSsid;
        }

        std::string WifiPassword() { return m_wifiPassword; }
        void WifiPassword(std::string value) {
            m_wifiPassword = value;
            m_jsonDocument[JsonFieldWiFiPassword] = m_wifiPassword;
        }     
};

}

#endif