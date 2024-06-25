/////////////////////////////////////////////////////////////////
#pragma once
#ifndef _GXOVNT_MODELS_KEEPALIVEREQUEST_H_
#define _GXOVNT_MODELS_KEEPALIVEREQUEST_H_
#include <ArduinoJson.h>
#include "JsonMessageConstants.h"
#include "BaseMessageModel.hpp"
#include "shared/Definitions.h"

using namespace GXOVnTLib::models::constants;

namespace GXOVnTLib::models {

class KeepAliveRequest : public BaseMessageModel
{
    public:
        KeepAliveRequest(uint16_t requestCommMessageId = 0) : BaseMessageModel(MsgType_KeepAliveRequest, requestCommMessageId) {};
        ~KeepAliveRequest() {};
};

}

#endif