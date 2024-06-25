/////////////////////////////////////////////////////////////////
#pragma once
#ifndef _GXOVNT_MODELS_ECHOREQUEST_H_
#define _GXOVNT_MODELS_ECHOREQUEST_H_
#include <ArduinoJson.h>
#include "JsonMessageConstants.h"
#include "BaseMessageModel.hpp"
#include "shared/Definitions.h"

using namespace GXOVnTLib::models::constants;

namespace GXOVnTLib::models {

class EchoRequest : public BaseMessageModel
{
    private:    
		std::string m_echoMessage;
	public:
		EchoRequest(uint16_t requestCommMessageId = 0) : BaseMessageModel(MsgType_EchoRequest, requestCommMessageId) {};
		EchoRequest(JsonDocument &doc, uint16_t requestCommMessageId = 0) : BaseMessageModel(doc, requestCommMessageId) {
			if (m_jsonDocument.containsKey(JsonFieldEchoMessage)) {
					const char *echoMessage = m_jsonDocument[JsonFieldEchoMessage];
					m_echoMessage = std::string(echoMessage);
			} 
		};
		~EchoRequest() {};
		std::string EchoMessage() {
			return m_echoMessage;
		}
		void EchoMessage(std::string value) {
			m_echoMessage = value;
			m_jsonDocument[JsonFieldEchoMessage] = m_echoMessage;
		}
};

}

#endif