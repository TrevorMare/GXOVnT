/////////////////////////////////////////////////////////////////
#pragma once
#ifndef _GXOVNT_MODELS_REBOOTREQUEST_H_
#define _GXOVNT_MODELS_REBOOTREQUEST_H_
#include <ArduinoJson.h>
#include "JsonMessageConstants.h"
#include "BaseMessageModel.hpp"
#include "shared/Definitions.h"

using namespace GXOVnTLib::models::constants;

namespace GXOVnTLib::models {

class RebootRequest : public BaseMessageModel
{
    public:
        RebootRequest(uint16_t requestCommMessageId = 0) : BaseMessageModel(MsgType_RebootRequest, requestCommMessageId) {};
        ~RebootRequest() {};
};

}

#endif