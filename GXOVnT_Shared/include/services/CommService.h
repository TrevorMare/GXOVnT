/////////////////////////////////////////////////////////////////
#pragma once
#ifndef _GXOVNT_COMMSERVICE_H_
#define _GXOVNT_COMMSERVICE_H_

#include "messages/CommMessageHandler.h"
#include "BLECommService.h"

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
			void handleMessage(CommMessage *commMessage) override;

		public:
			CommService();
			~CommService();

			void start();
			void stop();
		};
	}
}

#endif