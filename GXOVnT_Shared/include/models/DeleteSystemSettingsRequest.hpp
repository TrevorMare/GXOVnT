/////////////////////////////////////////////////////////////////
#pragma once
#ifndef _GXOVNT_MODELS_DELETESYSTEMSETTINGSREQUEST_H_
#define _GXOVNT_MODELS_DELETESYSTEMSETTINGSREQUEST_H_
#include <ArduinoJson.h>
#include "JsonMessageConstants.h"
#include "BaseMessageModel.hpp"
#include "shared/Definitions.h"

using namespace GXOVnTLib::models::constants;

namespace GXOVnTLib::models {

class DeleteSystemSettingsRequest : public BaseMessageModel
{
    private:
        std::string m_systemPassword = "";

    public:
     	DeleteSystemSettingsRequest(uint16_t requestCommMessageId = 0) : BaseMessageModel(MsgType_DeleteSystemSettingsRequest, requestCommMessageId) {};
		DeleteSystemSettingsRequest(JsonDocument &doc, uint16_t requestCommMessageId = 0) : BaseMessageModel(doc, requestCommMessageId) {
			if (m_jsonDocument.containsKey(JsonFieldSystemPassword)) {
                const char *systemPassword = m_jsonDocument[JsonFieldSystemPassword];
                m_systemPassword = std::string(systemPassword);
			} 
		};
        ~DeleteSystemSettingsRequest() {};
        std::string SystemPassword() {
			return m_systemPassword;
		}
		void SystemPassword(std::string value) {
			m_systemPassword = value;
			m_jsonDocument[JsonFieldSystemPassword] = m_systemPassword;
		}
};

}

#endif