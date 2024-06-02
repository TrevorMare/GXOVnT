/////////////////////////////////////////////////////////////////
#pragma once
#ifndef _GXOVNT_COMMSERVICE_H_
#define _GXOVNT_COMMSERVICE_H_

#include "services/BLECommService.h"
#include "messages/CommMessageReceiveHandler.h"
#include "messages/CommMessage.h"


#include <ArduinoJson.h>



using namespace GXOVnT::messages;

namespace GXOVnT
{
	namespace services
	{

		class CommService : public CommMessageReceiveHandler
		{
		private:
			/* data */
			BleCommService *m_BleCommService = nullptr;
			uint16_t m_sendMessageId = 1;


			std::vector<CommMessage*> m_messagesReceived;
			std::vector<CommMessage*> m_messagesToSend;
			std::mutex m_messagesReceivedLock;
			std::mutex m_messagesToSendLock;

			TaskHandle_t m_ProcessMessagesTaskHandle;
			
			
			// Trigger for when a new comm service message is received from a channel. It adds
			// a message into the received messages
			void onMessageReceived(CommMessage *commMessage) override;
			void ProcessMessagesTask();
			static void startProcessMessagesTask(void* _this);

			
		public:
			CommService();
			~CommService();
			
			// Sends a message to the comm service
			bool sendMessage(uint8_t *buffer, size_t messageSize, enum GXOVnT_COMM_SERVICE_TYPE commServiceType);
			bool sendMessage(CommMessage *commMessage);

			void start();
			void stop();
			
		};
	}
}

#endif