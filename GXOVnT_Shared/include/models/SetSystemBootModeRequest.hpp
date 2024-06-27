/////////////////////////////////////////////////////////////////
#pragma once
#ifndef _GXOVNT_MODELS_SETSYSTEMBOOTMODEREQUEST_H_
#define _GXOVNT_MODELS_SETSYSTEMBOOTMODEREQUEST_H_
#include <ArduinoJson.h>
#include "JsonMessageConstants.h"
#include "BaseMessageModel.hpp"
#include "shared/Definitions.h"

using namespace GXOVnTLib::models::constants;

namespace GXOVnTLib::models {

class SetSystemBootModeRequest : public BaseMessageModel
{
    private: 
		GXOVnT_BOOT_MODE m_systemBootMode = BOOT_MODE_SYSTEM_BLE_MODE;

	public:
			SetSystemBootModeRequest(uint16_t requestCommMessageId = 0) : BaseMessageModel(MsgType_SetSystemBootModeRequest, requestCommMessageId) {};
			SetSystemBootModeRequest(JsonDocument &doc, uint16_t requestCommMessageId = 0) : BaseMessageModel(doc, requestCommMessageId) {
				if (m_jsonDocument.containsKey(JsonFieldSystemBootMode)) {
					m_systemBootMode = (GXOVnT_BOOT_MODE)m_jsonDocument[JsonFieldSystemBootMode].as<int>();
				}
				
			};
	
			GXOVnT_BOOT_MODE SystemBootMode() { return m_systemBootMode; }
			~SetSystemBootModeRequest() {};        
};

}

#endif