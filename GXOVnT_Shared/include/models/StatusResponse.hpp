/////////////////////////////////////////////////////////////////
#pragma once
#ifndef _GXOVNT_MODELS_STATUSRESPONSE_H_
#define _GXOVNT_MODELS_STATUSRESPONSE_H_
#include <ArduinoJson.h>
#include "JsonMessageConstants.h"
#include "BaseMessageModel.hpp"
#include "shared/Definitions.h"

using namespace GXOVnTLib::models::constants;

namespace GXOVnTLib::models {

class StatusResponse : public BaseMessageModel
{
    private:
		uint8_t m_statusCode = 200;
		std::string m_statusMessage = "OK";
	public:
		StatusResponse(uint16_t requestCommMessageId = 0, uint8_t statusCode = 200, std::string statusMessage = "OK") 
				: BaseMessageModel(MsgType_StatusResponse, requestCommMessageId) {
            StatusCode(statusCode);
            StatusMessage(statusMessage);
		};
		~StatusResponse() {};
		uint8_t StatusCode() { return m_statusCode; }
		std::string StatusMessage() { return m_statusMessage; }
		void StatusCode(uint8_t value) {  
            m_statusCode = value;
            m_jsonDocument[JsonFieldStatusCode] = m_statusCode;
		}
		void StatusMessage(std::string value) { 
            m_statusMessage = value;
            m_jsonDocument[JsonFieldStatusMessage] = m_statusMessage;
		}
};

}

#endif