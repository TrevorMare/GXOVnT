/////////////////////////////////////////////////////////////////
#pragma once
#ifndef _GXOVNT_BLECOMMSERVICE_H_
#define _GXOVNT_BLECOMMSERVICE_H_

/////////////////////////////////////////////////////////////////
#include <BLEServer.h>
#include <BLEDevice.h>
#include <BLE2902.h>
#include <memory>
#include "shared/Shared.h"
#include <mutex>
#include "messages/CommMessage.h"
#include "messages/CommMessagePacket.h"
#include "messages/CommMessageReceiveHandler.h"
#include "messages/CommMessageSendHandler.h"
#include "messages/CommMessageReceivedCompleteHandler.h"

using namespace GXOVnT::messages;

/////////////////////////////////////////////////////////////////
namespace GXOVnT
{
	namespace services
	{
		
		/////////////////////////////////////////////////////////////////
		class BleCommService : public BLEServerCallbacks, public BLECharacteristicCallbacks, public CommMessageSendHandler, public CommMessageReceivedCompleteHandler
		{
		private:
			/* data */
			BLEServer *m_bleServer = nullptr;
			BLEService *m_bleService = nullptr;
			BLECharacteristic *m_protoReadCharacteristic = nullptr;
			BLECharacteristic *m_protoWriteCharacteristic = nullptr;
			BLEAdvertising *m_bleAdvertising = nullptr;
			CommMessageReceiveHandler *m_messageHandler = nullptr;
			std::vector<CommMessage*> m_commMessages;
			std::mutex m_mutexLock;
			
			bool m_stopRequested = false;
			bool m_serverConnected = false;
			int m_serverConnectionId = -1;

			// Callback when the Bluetooth server has been connected
			void onConnect(BLEServer *pServer);
			// Callback when the Bluetooth server has been disconnected
			void onDisconnect(BLEServer *pServer);
			// Callback method triggered when the BLE Characteristic has been written from another connection
			void onWrite(BLECharacteristic *bleCharacteristic);
			// Method to setup the BLE server
			void initBleServer();
			// Method to setup the BLE service and the characteristics
			void initBleService();
			// Starts the advertising for discovery
			void startAdvertising();
			// Parses a Characteristic message
			CommMessage *processReadCharacteristicMessage(uint8_t* buffer, size_t messageLength);
			bool processWriteCharacteristicMessage(CommMessage *commMessage);
			// Removes a message from the buffer 
			void removeProcessedMessage(uint16_t messageId);
		public:
			BleCommService();
			~BleCommService();
			bool sendMessage(CommMessage *commMessage) override;
			void receivedMessageHandled(uint16_t commMessageId) override;
			void start(CommMessageReceiveHandler *messageHandler);
			void stop();
		};
	}
}

#endif