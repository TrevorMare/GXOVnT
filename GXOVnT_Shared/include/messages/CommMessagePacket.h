/////////////////////////////////////////////////////////////////
#pragma once
#ifndef _GXOVNT_COMMMESSAGEPACKET_H_
#define _GXOVNT_COMMMESSAGEPACKET_H_

#include "shared/Shared.h"

namespace GXOVnT
{
    namespace messages
    {
      /////////////////////////////////////////////////////////////////
      class CommMessagePacket
      {
        private:
            /* data */
            uint16_t m_messageId = 0;
            uint8_t m_packetId;
            bool m_packetStart = false;
            bool m_packetEnd = false;
            std::vector<uint8_t> m_packetBuffer;
            bool m_validPacket = false;
            size_t m_packetBufferSize = 0;

            void parseMessageBytes(const uint8_t *messageBytes, size_t messageSize);

        public:
            /// @brief Gets the message Id of the packet
            /// @return 
            uint16_t MessageId();
            /// @brief Gets the packet Id
            /// @return 
            uint8_t PacketId();
            /// @brief Gets a value indicating if this is the first packet
            /// @return 
            bool PacketStart();
            /// @brief Gets a value indicating if this is the last packet
            /// @return 
            bool PacketEnd();
            /// @brief Gets a value indicating if the packet is valid
            /// @return 
            bool ValidPacket();
            /// @brief Gets the parsed buffer of the message bytes. This excludes all the meta data included in the original message
            /// @return 
            std::vector<uint8_t> PacketBuffer();
            size_t PacketBufferSize();
            /// @brief Prints the content of the message packet
            void PrintPacket();
            uint8_t *GetData() ;
            // Constructors
            CommMessagePacket();
            CommMessagePacket(const uint8_t *messageBytes, size_t messageSize);
            // DeConstructor
            ~CommMessagePacket();
      };
    }
}

#endif