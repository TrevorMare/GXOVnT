/////////////////////////////////////////////////////////////////
#pragma once
#ifndef _GXOVNT_MODELS_GETFIRMWAREUPDATERESULTREQUEST_H_
#define _GXOVNT_MODELS_GETFIRMWAREUPDATERESULTREQUEST_H_
#include <ArduinoJson.h>
#include "JsonMessageConstants.h"
#include "BaseMessageModel.hpp"
#include "shared/Definitions.h"

using namespace GXOVnTLib::models::constants;

namespace GXOVnTLib::models {

class GetFirmwareUpdateResultRequest : public BaseMessageModel
{
    public:
        GetFirmwareUpdateResultRequest(uint16_t requestCommMessageId = 0) : BaseMessageModel(MsgType_GetSystemSettingsRequest, requestCommMessageId) {};
        ~GetFirmwareUpdateResultRequest() {};
};

}

#endif