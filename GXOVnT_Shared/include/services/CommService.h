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
			bool decode_pb_string(pb_istream_t *stream, const pb_field_t *field, void **arg);
		public:
			CommService();
			~CommService();

			void start();
			void stop();
		};
	}
}

#endif