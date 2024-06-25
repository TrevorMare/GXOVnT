/////////////////////////////////////////////////////////////////
#pragma once
#ifndef _GXOVNT_MODELS_GETTESTWIFISETTINGSREQUEST_H_
#define _GXOVNT_MODELS_GETTESTWIFISETTINGSREQUEST_H_
#include <ArduinoJson.h>
#include "JsonMessageConstants.h"
#include "BaseMessageModel.hpp"
#include "shared/Definitions.h"

using namespace GXOVnTLib::models::constants;

namespace GXOVnTLib::models {

class GetTestWiFiSettingsRequest : public BaseMessageModel
{
    public:
        GetTestWiFiSettingsRequest(uint16_t requestCommMessageId = 0) : BaseMessageModel(MsgType_GetTestWiFiSettingsResultRequest, requestCommMessageId) {};
        ~GetTestWiFiSettingsRequest() {};
};

}

#endif