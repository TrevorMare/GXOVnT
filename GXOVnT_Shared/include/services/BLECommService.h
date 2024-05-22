/////////////////////////////////////////////////////////////////
#pragma once
#ifndef _GXOVNT_BLECOMMSERVICE_H_
#define _GXOVNT_BLECOMMSERVICE_H_

/////////////////////////////////////////////////////////////////
#include <BLEServer.h>
#include <BLEDevice.h>
#include <BLE2902.h>
#include "shared/Shared.h"
#include <memory>

/////////////////////////////////////////////////////////////////
namespace GXOVnT
{
	namespace services
	{

		// Callback handler definition when either a LORA message or bluetooth message has been received
		class BLECommServiceMessageCallback
		{
			public:
				virtual void handleBLEMessage(const uint8_t *buffer, size_t size) {};
		};

		/////////////////////////////////////////////////////////////////
		// This structure is used to keep the buffer of the packet with some additional details
		/////////////////////////////////////////////////////////////////
		struct BLEMessagePacket {
			uint16_t MessageId = 0;
			uint8_t PacketId;
			bool PacketStart = false;
			bool PacketEnd = false;
			std::vector<uint8_t> *Data;

			BLEMessagePacket(uint16_t messageId, uint8_t packetId, bool packetStart, bool packetEnd, std::vector<uint8_t> *data) {
				MessageId = messageId;
				PacketId = packetId;
				PacketStart = packetStart;
				PacketEnd = packetEnd;
				Data = data;
			}

			const uint8_t *toArray()  {
				std::vector<uint8_t> data = *Data;
				const uint8_t *result = data.size() ? &data[0] : NULL;
				return result;
			}

		};
		/////////////////////////////////////////////////////////////////
		// This structure is used to build up a GRPC message buffer sent 
		// over the Bluetooth channel. When a message exceeds (Default: 512 bytes for esp32, 20 for all others) bytes
		// the message needs to be broken into multiple packets
		/////////////////////////////////////////////////////////////////
		struct BLEMessage {
			uint16_t MessageId = 0;
			unsigned long ExpiryMillis = 0;
			size_t MessageSize;
			std::vector<BLEMessagePacket*> MessagePackets;
		};
		/////////////////////////////////////////////////////////////////
		class BleCommService : public BLEServerCallbacks, public BLECharacteristicCallbacks
		{
		private:
			/* data */
			BLEServer *m_bleServer = nullptr;
			BLEService *m_bleService = nullptr;
			BLECharacteristic *m_protoCharacteristic = nullptr;
			BLEAdvertising *m_bleAdvertising = nullptr;
			std::vector<BLEMessage*> m_messages;
			BLECommServiceMessageCallback *m_messageHandler = nullptr;

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
			bool processCharacteristicMessage(uint8_t* buffer, size_t messageLength); 
			// Builds a message packet from a buffer
			BLEMessagePacket *buildMessagePacket(uint8_t* buffer, size_t messageLength) const;
			// Finds the index of the message in memory. Returns -1 if not found
			int getMessageIndex(uint16_t messageId);
			// Creates or updates an existing message with the new packet information
			BLEMessage* createOrUpdateMessage(int messageIndex, BLEMessagePacket *messagePacket, size_t messageLength);
			// Handles the callback when the message end is specified
			void handleMessageComplete(BLEMessage *bleMessage);
			// Handles the callback when the message end is specified
			void handleMessageComplete(const uint8_t* buffer, size_t messageLength);

		public:
			BleCommService();
			~BleCommService();

			void start(BLECommServiceMessageCallback *messageHandler);
			void stop();
		};
	}
}

#endif