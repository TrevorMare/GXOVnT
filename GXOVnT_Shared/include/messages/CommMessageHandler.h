/////////////////////////////////////////////////////////////////
#pragma once
#ifndef _GXOVNT_COMMMESSAGEHANDLER_H_
#define _GXOVNT_COMMMESSAGEHANDLER_H_

#include "CommMessage.h"

namespace GXOVnT
{
    namespace messages
    {
		// Callback handler definition when either a LORA message or bluetooth message has been received
		class CommMessageHandler
		{
			public:
				virtual void handleMessage(CommMessage *commMessage) {};
		};
    }
}
#endif