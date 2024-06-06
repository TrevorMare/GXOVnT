/////////////////////////////////////////////////////////////////
#pragma once
#ifndef _GXOVNT_COMMMESSAGESENDHANDLER_H_
#define _GXOVNT_COMMMESSAGESENDHANDLER_H_

#include "CommMessage.h"
#include "shared/Definitions.h"

namespace GXOVnTLib::messages
{
		/// @brief Callback handler definition when either a LORA message or bluetooth message has been received
		class CommMessageSendHandler
		{
			public:
				virtual bool sendMessage(CommMessage *commMessage) { return true; };
		};

}
#endif