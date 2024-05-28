/////////////////////////////////////////////////////////////////
#pragma once
#ifndef _GXOVNT_COMMSERVICE_H_
#define _GXOVNT_COMMSERVICE_H_

#include "services/BLECommService.h"
#include "messages/CommMessageHandler.h"
#include "messages/CommMessage.h"

#include "messages/gxovnt.messaging.container.pb.h"


#include "messages/pb_encode.h"
#include "messages/pb_decode.h"
#include "messages/pb_common.h"

// #include "messages/pb_common.h"

using namespace GXOVnT::messages;

namespace GXOVnT
{
	namespace services
	{

		class CommService : public CommMessageHandler
		{
		private:
			/* data */
			BleCommService *m_BleCommService = nullptr;
			// Handles the incoming comm message
			void handleMessage(CommMessage *commMessage) override;
			std::vector<CommMessage*> m_messagesToRun;
			std::mutex m_mutexLock;
			void processMessage(CommMessage *commMessage);

			
			
		public:
			CommService();
			~CommService();

			void start();
			void stop();
			void run();
		};
	}
}

#endif