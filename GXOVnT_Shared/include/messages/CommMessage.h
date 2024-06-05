/////////////////////////////////////////////////////////////////
#pragma once
#ifndef _GXOVNT_COMMMESSAGE_H_
#define _GXOVNT_COMMMESSAGE_H_

#include "shared/Definitions.h"
#include "CommMessagePacket.h"

using namespace GXOVnT::shared;

namespace GXOVnT
{
    namespace messages 
    {
				/////////////////////////////////////////////////////////////////
				class CommMessage
				{
					private:
						/// @brief Unique Id of the message
						uint16_t m_messageId = 0;
						/// @brief Timestamp for the last time this message received a packet
						unsigned long m_lastPacketReceivedTimeStamp = millis();
						/// @brief Gets the total size in bytes of all the combined packets
						size_t m_totalSize = 0;
						/// @brief A value to indicate if the ending packet has been received
						bool m_receivedAllPackets = false;
						/// @brief An array containing the message packets for this message
						std::vector<CommMessagePacket*> m_messagePackets;
						/// @brief The comm service that this comm message is intended for
						enum GXOVnT_COMM_SERVICE_TYPE m_commServiceType = COMM_SERVICE_TYPE_BLE;
						// Helper buffer to combine all the packets into
						std::vector<uint8_t> m_messageBuffer;
						// A value to indicate if this message has been processed
						bool m_IsProcessed = false;
					public:

						/// @brief Constructor
						/// @param The Comm Service is packet is intended or received from
						CommMessage(enum GXOVnT_COMM_SERVICE_TYPE commServiceType);
						~CommMessage();
						/// @brief Adds a new message packet to the list of packages
						/// @param messagePacket 
						void AddIncomingPackage(CommMessagePacket *messagePacket);
						/// @brief Gets the source service that created this message
						/// @return 
						enum GXOVnT_COMM_SERVICE_TYPE GetSourceService();
						/// @brief Gets a value indicating if all the packages has been recieved for this message
						/// @return 
						bool ReceivedAllPackets();
						/// @brief Gets a value indicating if this message has expired
						/// @return 
						bool IsExpired();
						/// @brief Gets a value to indicate if the message has been processed
						/// @return 
						bool IsProcessed();
						/// @brief Sets the is processed indicator
						/// @param processed 
						void IsProcessed(bool processed);
						/// @brief Gets the message Id associated with the message
						/// @return 
						uint16_t MessageId();
						/// @brief Gets the total size of all the message packets
						/// @return 
						size_t TotalSize();
						/// @brief Reads the parsed packets into a buffer
						/// @return 
						const uint8_t *Read();
						/// @brief Writes the message buffer in chunks to the packets
						/// @param buffer 
						/// @param messageSize 
						std::vector<CommMessagePacket*> *Write(uint16_t messageId, uint8_t *buffer, size_t messageSize, uint16_t chunkSize = 20);
						/// @brief Prints the debug message to the output
						void PrintMessage();
						/// @brief Gets the message packets in the message
						/// @return 
						std::vector<CommMessagePacket*> *MessagePackets();
				};
    }
}

#endif