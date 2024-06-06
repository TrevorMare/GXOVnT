/////////////////////////////////////////////////////////////////
#pragma once
#ifndef _GXOVNT_JSONMESSAGESERVICE_H_
#define _GXOVNT_JSONMESSAGESERVICE_H_
#include <ArduinoJson.h>
#include "shared/Definitions.h"
#include "messages/CommMessage.h"
#include "models/JsonModels.hpp"
#include "GXOVnTRoot.h"

// Forward declaration of GXOVnTRoot


namespace GXOVnTLib::services
{
	class JsonMessageService
	{
	private:
		JsonDocument *processJsonMessage(JsonDocument &inputDocument, uint16_t requestCommMessageId = 0);
	public:
		JsonMessageService();
		~JsonMessageService();
		JsonDocument *handleJsonMessage(GXOVnTLib::messages::CommMessage *commMessage);
		JsonDocument *handleJsonMessage(JsonDocument &inputDocument, uint16_t requestCommMessageId = 0);
	};
}

#endif
