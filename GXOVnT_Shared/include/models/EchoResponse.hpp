/////////////////////////////////////////////////////////////////
#pragma once
#ifndef _GXOVNT_MODELS_ECHORESPONSE_H_
#define _GXOVNT_MODELS_ECHORESPONSE_H_
#include <ArduinoJson.h>
#include "JsonMessageConstants.h"
#include "BaseMessageModel.hpp"
#include "shared/Definitions.h"

using namespace GXOVnTLib::models::constants;

namespace GXOVnTLib::models {

class EchoResponse : public BaseMessageModel
{
    private:
		std::string m_echoMessage;
	public:
		EchoResponse(uint16_t requestCommMessageId = 0, std::string echoMessage = "") 
				: BaseMessageModel(MsgType_EchoResponse, requestCommMessageId) {
            EchoMessage(echoMessage);
		};
		~EchoResponse() {};
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