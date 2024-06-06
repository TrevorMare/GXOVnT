/////////////////////////////////////////////////////////////////
#pragma once
#ifndef _GXOVNT_COMMMESSAGERECEIVEDCOMPLETEHANDLER_H_
#define _GXOVNT_COMMMESSAGERECEIVEDCOMPLETEHANDLER_H_

#include "shared/Definitions.h"

namespace GXOVnTLib::messages
{
	/// @brief Callback handler definition when either a LORA message or bluetooth message has been received
	class CommMessageReceivedCompleteHandler
	{
		public:
			virtual void receivedMessageHandled(uint16_t commMessageId) {};
	};
}
#endif