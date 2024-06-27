/////////////////////////////////////////////////////////////////
#pragma once
#ifndef _GXOVNT_MODELS_SETSYSTEMSETTINGSREQUEST_H_
#define _GXOVNT_MODELS_SETSYSTEMSETTINGSREQUEST_H_
#include <ArduinoJson.h>
#include "JsonMessageConstants.h"
#include "BaseMessageModel.hpp"
#include "shared/Definitions.h"

using namespace GXOVnTLib::models::constants;

namespace GXOVnTLib::models {

class SetSystemSettingsRequest : public BaseMessageModel
{
    private: 
			std::string m_systemName = "";
			bool m_systemConfigured = false;
			std::string m_wifiSsid;
			std::string m_wifiPassword;
			GXOVnT_SYSTEM_TYPE m_systemType = SYSTEM_TYPE_UN_INITIALIZED;

	public:
			SetSystemSettingsRequest(uint16_t requestCommMessageId = 0) : BaseMessageModel(MsgType_SetSystemSettingsRequest, requestCommMessageId) {};
			SetSystemSettingsRequest(JsonDocument &doc, uint16_t requestCommMessageId = 0) : BaseMessageModel(doc, requestCommMessageId) {
				if (m_jsonDocument.containsKey(JsonFieldSystemName)) {
					const char *systemName = m_jsonDocument[JsonFieldSystemName];
					m_systemName = CharToString(systemName);
				} 
				if (m_jsonDocument.containsKey(JsonFieldSystemConfigured)) {
					m_systemConfigured = m_jsonDocument[JsonFieldSystemConfigured];
				}
				if (m_jsonDocument.containsKey(JsonFieldSystemType)) {
					m_systemType = (GXOVnT_SYSTEM_TYPE)m_jsonDocument[JsonFieldSystemType].as<int>();
				}
				if (m_jsonDocument.containsKey(JsonFieldWiFiSsid)) {
					const char *wifiSsid = m_jsonDocument[JsonFieldWiFiSsid];
					m_wifiSsid = CharToString(wifiSsid);
				} 
				if (m_jsonDocument.containsKey(JsonFieldWiFiPassword)) {
					const char *wifiPassword = m_jsonDocument[JsonFieldWiFiPassword];
					m_wifiPassword = CharToString(wifiPassword);
				} 
			};
			std::string SystemName() { return m_systemName; }
			bool SystemConfigured() { return m_systemConfigured; }
			std::string WifiSsid() { return m_wifiSsid; }
			std::string WifiPassword() { return m_wifiPassword; }
			GXOVnT_SYSTEM_TYPE SystemType() { return m_systemType; }
			~SetSystemSettingsRequest() {};        
};

}

#endif