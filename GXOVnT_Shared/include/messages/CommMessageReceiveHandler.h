/////////////////////////////////////////////////////////////////
#pragma once
#ifndef _GXOVNT_COMMMESSAGERECEIVEHANDLER_H_
#define _GXOVNT_COMMMESSAGERECEIVEHANDLER_H_

#include "CommMessage.h"

namespace GXOVnT
{
    namespace messages
    {
		// Callback handler definition when either a LORA message or bluetooth message has been received
		class CommMessageReceiveHandler
		{
			public:
				virtual void onMessageReceived(CommMessage *commMessage) {};
		};
    }
}
#endif