/////////////////////////////////////////////////////////////////
#pragma once
#ifndef _GXOVNT_MODELS_CHECKFIRMWAREUPDATEREQUEST_H_
#define _GXOVNT_MODELS_CHECKFIRMWAREUPDATEREQUEST_H_
#include <ArduinoJson.h>
#include "JsonMessageConstants.h"
#include "BaseMessageModel.hpp"
#include "shared/Definitions.h"

using namespace GXOVnTLib::models::constants;

namespace GXOVnTLib::models {

class CheckFirmwareUpdateRequest : public BaseMessageModel
{
    private: 
        std::string m_wifiSsid;
        std::string m_wifiPassword;
	public:
			CheckFirmwareUpdateRequest(uint16_t requestCommMessageId = 0) : BaseMessageModel(MsgType_CheckFirmwareUpdateRequest, requestCommMessageId) {};
			CheckFirmwareUpdateRequest(JsonDocument &doc, uint16_t requestCommMessageId = 0) : BaseMessageModel(doc, requestCommMessageId) {
				if (m_jsonDocument.containsKey(JsonFieldWiFiSsid)) {
					const char *wifiSsid = m_jsonDocument[JsonFieldWiFiSsid];
					m_wifiSsid = std::string(wifiSsid);
				} 
				if (m_jsonDocument.containsKey(JsonFieldWiFiPassword)) {
					const char *wifiPassword = m_jsonDocument[JsonFieldWiFiPassword];
					m_wifiPassword = std::string(wifiPassword);
				} 
			};
			std::string WifiSsid() { return m_wifiSsid; }
			std::string WifiPassword() { return m_wifiPassword; }
			~CheckFirmwareUpdateRequest() {};        
};

}

#endif