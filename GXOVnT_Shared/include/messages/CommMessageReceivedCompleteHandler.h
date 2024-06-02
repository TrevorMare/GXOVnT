/////////////////////////////////////////////////////////////////
#pragma once
#ifndef _GXOVNT_COMMMESSAGERECEIVEDCOMPLETEHANDLER_H_
#define _GXOVNT_COMMMESSAGERECEIVEDCOMPLETEHANDLER_H_

namespace GXOVnT
{
    namespace messages
    {
		// Callback handler definition when either a LORA message or bluetooth message has been received
		class CommMessageReceivedCompleteHandler
		{
			public:
				virtual void receivedMessageHandled(uint16_t commMessageId) {};
		};
    }
}
#endif