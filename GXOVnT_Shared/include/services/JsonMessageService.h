/////////////////////////////////////////////////////////////////
#pragma once
#ifndef _GXOVNT_JSONMESSAGESERVICE_H_
#define _GXOVNT_JSONMESSAGESERVICE_H_
#include <ArduinoJson.h>
#include "shared/Definitions.h"
#include "messages/CommMessage.h"

#include "models/BaseMessageModel.hpp"
#include "models/EchoRequest.hpp"
#include "models/EchoResponse.hpp"
#include "models/GetSystemSettingsRequest.hpp"
#include "models/GetSystemSettingsResponse.hpp"
#include "models/GetTestWiFiSettingsRequest.hpp"
#include "models/GetTestWiFiSettingsResponse.hpp"
#include "models/KeepAliveRequest.hpp"
#include "models/RebootRequest.hpp"
#include "models/SaveConfigurationRequest.hpp"
#include "models/SetSystemSettingsRequest.hpp"
#include "models/StatusResponse.hpp"
#include "models/TestWiFiSettingsRequest.hpp"

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
