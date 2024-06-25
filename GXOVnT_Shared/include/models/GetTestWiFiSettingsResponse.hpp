/////////////////////////////////////////////////////////////////
#pragma once
#ifndef _GXOVNT_MODELS_GETTESTWIFISETTINGSRESPONSE_H_
#define _GXOVNT_MODELS_GETTESTWIFISETTINGSRESPONSE_H_
#include <ArduinoJson.h>
#include "JsonMessageConstants.h"
#include "BaseMessageModel.hpp"
#include "shared/Definitions.h"

using namespace GXOVnTLib::models::constants;

namespace GXOVnTLib::models {

class GetTestWiFiSettingsResponse : public BaseMessageModel
{
    private: 
		std::string m_wifiSsid = "";
		std::string m_wifiPassword = "";
		bool m_tested = false;
		bool m_success = false;
		uint8_t m_statusCode = 200;
		std::string m_statusMessage = "OK";
	public:
		GetTestWiFiSettingsResponse(uint16_t requestCommMessageId = 0) : BaseMessageModel(MsgType_GetTestWiFiSettingsResultResponse, requestCommMessageId) {};
		GetTestWiFiSettingsResponse(JsonDocument &doc, uint16_t requestCommMessageId = 0) : BaseMessageModel(doc, requestCommMessageId) {};
		
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

		bool Tested() { return m_tested; }
		void Tested(bool value) {
			m_tested = value;
			m_jsonDocument[JsonFieldTested] = m_tested;
		}

		bool Success() { return m_success; }
		void Success(bool value) {
			m_success = value;
			m_jsonDocument[JsonFieldSuccess] = m_success;
		}

		void StatusCode(uint8_t value) {  
			m_statusCode = value;
			m_jsonDocument[JsonFieldStatusCode] = m_statusCode;
		}
		void StatusMessage(std::string value) { 
			m_statusMessage = value;
			m_jsonDocument[JsonFieldStatusMessage] = m_statusMessage;
		}
		~GetTestWiFiSettingsResponse() {};
};

}

#endif