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
#include "messages/CommMessageHandler.h"

using namespace GXOVnT::messages;

/////////////////////////////////////////////////////////////////
namespace GXOVnT
{
	namespace services
	{
		
		/////////////////////////////////////////////////////////////////
		class BleCommService : public BLEServerCallbacks, public BLECharacteristicCallbacks
		{
		private:
			/* data */
			BLEServer *m_bleServer = nullptr;
			BLEService *m_bleService = nullptr;
			BLECharacteristic *m_protoCharacteristic = nullptr;
			BLEAdvertising *m_bleAdvertising = nullptr;
			CommMessageHandler *m_messageHandler = nullptr;
			std::vector<CommMessage*> m_commMessages;
			std::mutex m_mutexLock;
			
			bool m_stopRequested = false;
			bool m_serverConnected = false;
			int m_serverConnectionId = -1;

			// Callback when the Bluetooth server has been connected
			void onConnect(BLEServer *pServer);
			// Callback when the Bluetooth server has been disconnected
			void onDisconnect(BLEServer *pServer);
			// Callback method triggered when the GRPC BLE Characteristic has been written
			void onWrite(BLECharacteristic *pLedCharacteristic);
			// Method to setup the BLE server
			void initBleServer();
			// Method to setup the BLE service and the characteristics
			void initBleService();
			// Starts the advertising for discovery
			void startAdvertising();
			// Parses a Characteristic message
			CommMessage *processCharacteristicMessage(uint8_t* buffer, size_t messageLength); 
			void removeProcessedMessage(uint16_t messageId);
		public:
			BleCommService();
			~BleCommService();

			void start(CommMessageHandler *messageHandler);
			void stop();
		};
	}
}

#endif