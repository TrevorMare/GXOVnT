/////////////////////////////////////////////////////////////////
#pragma once
#ifndef _GXOVNT_MODELS_SAVECONFIGURATIONREQUEST_H_
#define _GXOVNT_MODELS_SAVECONFIGURATIONREQUEST_H_
#include <ArduinoJson.h>
#include "JsonMessageConstants.h"
#include "BaseMessageModel.hpp"
#include "shared/Definitions.h"

using namespace GXOVnTLib::models::constants;

namespace GXOVnTLib::models {

class SaveConfigurationRequest : public BaseMessageModel
{
    public:
        SaveConfigurationRequest(uint16_t requestCommMessageId = 0) : BaseMessageModel(MsgType_SaveConfigurationRequest, requestCommMessageId) {};
        ~SaveConfigurationRequest() {};
};

}

#endif