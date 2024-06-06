/////////////////////////////////////////////////////////////////
#pragma once
#ifndef _GXOVNT_BLECOMMSERVICE_H_
#define _GXOVNT_BLECOMMSERVICE_H_

/////////////////////////////////////////////////////////////////
#include <BLEServer.h>
#include <BLEDevice.h>
#include <BLE2902.h>
#include <memory>
#include <mutex>
#include "shared/Definitions.h"
#include "messages/CommMessage.h"
#include "messages/CommMessagePacket.h"
#include "messages/CommMessageReceiveHandler.h"
#include "messages/CommMessageSendHandler.h"
#include "messages/CommMessageReceivedCompleteHandler.h"

using namespace GXOVnTLib::messages;

/////////////////////////////////////////////////////////////////
namespace GXOVnTLib
{

	namespace services
	{
		
		/////////////////////////////////////////////////////////////////
		class BleCommService : public BLEServerCallbacks, public BLECharacteristicCallbacks, public CommMessageSendHandler, public CommMessageReceivedCompleteHandler
		{
		private:
			
			BLEServer *m_bleServer = nullptr;
			BLEService *m_bleService = nullptr;
			/// @brief Characteristic for incoming messages relative to this device
			BLECharacteristic *m_incomingMessagesCharacteristic = nullptr;
			/// @brief Characteristic for outgoing messages relative to this device
			BLECharacteristic *m_outgoingMessagesCharacteristic = nullptr;
			BLEAdvertising *m_bleAdvertising = nullptr;
			/// @brief Callback handler for when a complete message has been built and the last packet has been received
			CommMessageReceiveHandler *m_messageHandler = nullptr;
			/// @brief A list of messages in the process of being built up while packet chunks are being sent
			std::vector<CommMessage*> m_commMessages;
			/// @brief Lock object for receiving messages
			std::mutex m_mutexLock;
			/// @brief A value to indicate if the stop was requested explicitly, if not the service will start advertising again after a device disconnects
			bool m_stopRequested = false;
			/// @brief Internal value to indicate if the bluetooth server is connected
			bool m_serverConnected = false;
			/// @brief Internal value to show the service connection Id
			int m_serverConnectionId = -1;

			// Callback when the Bluetooth server has been connected
			void onConnect(BLEServer *pServer);
			// Callback when the Bluetooth server has been disconnected
			void onDisconnect(BLEServer *pServer);
			// Callback method triggered when the BLE Characteristic has been written from another system. This is handled with the m_incomingMessagesCharacteristic
			void onWrite(BLECharacteristic *bleCharacteristic);
			// Method to setup the BLE server
			void initBleServer();
			// Method to setup the BLE service and the characteristics
			void initBleService();
			// Starts the advertising for discovery
			void startAdvertising();
			// Parses and builds a CommMessage component from a received m_incomingMessagesCharacteristic 
			CommMessage *processIncomingCharacteristicMessage(uint8_t* buffer, size_t messageLength);
			/// @brief Writes the Comm Message data in chunks to the m_outgoingMessagesCharacteristic
			/// @param commMessage 
			/// @return 
			bool processOutgoingCharacteristicMessage(CommMessage *commMessage);
			// Removes a message from the internal list of messages 
			void removeProcessedMessage(uint16_t messageId);
		public:
			BleCommService();
			~BleCommService();
			/// @brief Method to write an outgoing message to the m_outgoingMessagesCharacteristic 
			/// @param commMessage 
			/// @return 
			bool sendMessage(CommMessage *commMessage) override;
			/// @brief Method to clean up after a comm message has been handled
			/// @param commMessageId 
			void receivedMessageHandled(uint16_t commMessageId) override;
			/// @brief Starts the bluetooth services and characteristics for reading and writing
			/// @param messageHandler 
			void start(CommMessageReceiveHandler *messageHandler);
			/// @brief Stops the bluetooth services
			void stop();
		};
	}
}

#endif