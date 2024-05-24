/////////////////////////////////////////////////////////////////
#pragma once
#ifndef _GXOVNT_COMMMESSAGE_H_
#define _GXOVNT_COMMMESSAGE_H_

#include "shared/Shared.h"
#include "CommMessagePacket.h"

using namespace GXOVnT::shared;

namespace GXOVnT
{
    namespace messages
    {
				class CommMessage
				{
					private:
						uint16_t m_messageId = 0;
						unsigned long m_expiryMillis = millis();
						size_t m_totalSize = 0;
						bool m_receivedAllPackets = false;
						std::vector<CommMessagePacket*> m_messagePackets;
						std::vector<uint8_t> m_messageBuffer;
						enum GXOVnT_COMM_SERVICE_TYPE m_commServiceType = COMM_SERVICE_TYPE_BLE;
					public:
						CommMessage(enum GXOVnT_COMM_SERVICE_TYPE commServiceType);
						~CommMessage();
						/// @brief Adds a new message packet to the list of packages
						/// @param messagePacket 
						void AddPackage(CommMessagePacket *messagePacket);
						/// @brief Gets a value indicating if all the packages has been recieved for this message
						/// @return 
						bool ReceivedAllPackets();
						/// @brief Gets a value indicating if this message has expired
						/// @return 
						bool IsExpired();
						/// @brief Builds the complete message buffer from all of the packages
						/// @return 
						const uint8_t *GetMessageBuffer();
						/// @brief Gets the message Id associated with the message
						/// @return 
						uint16_t MessageId();
						/// @brief Gets the source service that created this message
						/// @return 
						enum GXOVnT_COMM_SERVICE_TYPE GetSourceService();
						/// @brief Prints the debug message to the output
						void PrintMessage();
				};
    }
}

#endif