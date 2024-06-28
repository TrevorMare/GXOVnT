/////////////////////////////////////////////////////////////////
#pragma once
#ifndef _GXOVNT_COMMSERVICE_H_
#define _GXOVNT_COMMSERVICE_H_

#include "shared/Definitions.h"
#include "services/BLECommService.h"
#include "services/JsonMessageService.h"
#include "messages/CommMessageReceiveHandler.h"
#include "messages/CommMessage.h"
#include <ArduinoJson.h>

using namespace GXOVnTLib::messages;

namespace GXOVnTLib::services
{

	// Forward declaration of services
	class BleCommService;
	class JsonMessageService;
	
	

	class CommService : public CommMessageReceiveHandler
	{
	private:
		/* data */
		GXOVnTLib::services::BleCommService *m_BleCommService = nullptr;
		GXOVnTLib::services::JsonMessageService *m_jsonMessageService;

		uint16_t m_sendMessageId = 1;
		std::vector<CommMessage*> m_messagesReceived;
		std::vector<CommMessage*> m_messagesToSend;
		std::mutex m_messagesReceivedLock;
		std::mutex m_messagesToSendLock;
		

		TaskHandle_t m_ProcessMessagesTaskHandle;
		/// @brief Callback handler for when a sub service received a comm message, this message will be handled in a separate task
		/// @param commMessage 
		void onMessageReceived(CommMessage *commMessage) override;
		/// @brief Handles all the messages that were received from the last processing
		void processReceivedMessages();
		/// @brief Handles all the messages to be sent from the last processing
		void processSendMessages();
		/// @brief Wrapper to handle the processing of messages in this instance
		void ProcessMessagesTask();
		/// @brief Starts the background task for handling the incoming and outgoing messages
		/// @param _this 
		static void startProcessMessagesTask(void* _this);
	public:
		CommService();
		~CommService();
		
		/// @brief Creates a new message to send and adds it to the queue to be sent via the task
		/// @param buffer 
		/// @param messageSize 
		/// @param  
		/// @return 
		bool SendMessage(uint8_t *buffer, size_t messageSize, enum GXOVnT_COMM_SERVICE_TYPE commServiceType);
		/// @brief Adds new message to send and adds it to the queue to be sent via the task
		/// @param commMessage 
		/// @return 
		bool SendMessage(CommMessage *commMessage);
		/// @brief Starts the comm services
		void Start();
		/// @brief Stops the comm services
		void Stop();
	};
}

#endif