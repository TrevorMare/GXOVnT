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
	public:
			SetSystemSettingsRequest(uint16_t requestCommMessageId = 0) : BaseMessageModel(MsgType_SetSystemSettingsRequest, requestCommMessageId) {};
			SetSystemSettingsRequest(JsonDocument &doc, uint16_t requestCommMessageId = 0) : BaseMessageModel(doc, requestCommMessageId) {
				if (m_jsonDocument.containsKey("systemName")) {
					const char *systemName = m_jsonDocument["systemName"];
					m_systemName = std::string(systemName);
				} 
				if (m_jsonDocument.containsKey("systemId")) {
					m_systemConfigured = m_jsonDocument["systemConfigured"];
				}
				if (m_jsonDocument.containsKey(JsonFieldWiFiSsid)) {
					const char *wifiSsid = m_jsonDocument[JsonFieldWiFiSsid];
					m_wifiSsid = std::string(wifiSsid);
				} 
				if (m_jsonDocument.containsKey(JsonFieldWiFiPassword)) {
					const char *wifiPassword = m_jsonDocument[JsonFieldWiFiPassword];
					m_wifiPassword = std::string(wifiPassword);
				} 
			};
			std::string SystemName() { return m_systemName; }
			bool SystemConfigured() { return m_systemConfigured; }
			std::string WifiSsid() { return m_wifiSsid; }
			std::string WifiPassword() { return m_wifiPassword; }
			~SetSystemSettingsRequest() {};        
};

}

#endif