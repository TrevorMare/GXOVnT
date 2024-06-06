/////////////////////////////////////////////////////////////////
#pragma once
#ifndef _GXOVNT_COMMMESSAGEPACKET_H_
#define _GXOVNT_COMMMESSAGEPACKET_H_

#include "shared/Definitions.h"

using namespace GXOVnTLib::shared;

namespace GXOVnTLib::messages
{
  /////////////////////////////////////////////////////////////////
  class CommMessagePacket
  {
    private:
        /// @brief Message Id this packet belongs to. Needs to be transmitted in the first two bytes of the packet
        uint16_t m_messageId = 0; 
        /// @brief Packet Id, needs to be transmitted as the third byte of the packet
        uint8_t m_packetId;
        /// @brief A value to indicate if this is the first packet in the message
        bool m_packetStart = false;
        /// @brief Value to indicate if this is the last packet in the message
        bool m_packetEnd = false;
        /// @brief The data in the packet. This is partial message data that needs to be combined with all the message packets
        std::vector<uint8_t> m_packetBuffer;
        /// @brief Indicator to check if this is a valid packet
        bool m_validPacket = false;
        /// @brief The total size of the number of bytes in the packet buffer
        size_t m_packetBufferSize = 0;
        /// @brief A value to indicate if this message packet was received via a channel or is being sent over a channel
        enum GXOVnT_COMM_MESSAGE_DIRECTION m_commMessageDirection = COMM_MESSAGE_DIRECTION_INCOMING;
    public:
        /////////////////////////////////////////////////////////////////
        // Packet property getters
        /////////////////////////////////////////////////////////////////

        /// @brief Gets the message direction
        enum GXOVnT_COMM_MESSAGE_DIRECTION MessageDirection();
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

        /////////////////////////////////////////////////////////////////
        // Packet builder methods
        /////////////////////////////////////////////////////////////////
        
        /// @brief Sets up this packet from incoming bytes. 
        /// @param messageBytes The data array containing the meta data bytes
        /// @param messageSize The size of the array
        void buildIncomingPacketData(const uint8_t *messageBytes, size_t messageSize);
        
        /// @brief Sets up the packet for outgoing bytes.
        /// @param messageId // The unique message Id that this packet belongs to
        /// @param packetId // The unique packet Id
        /// @param isStartPacket  A value to indicate if this is the first packet in the message
        /// @param isEndPacket A value to indicate if this is the last packet in the message
        /// @param messageBytes The data array containing the bytes without the meta data 
        /// @param messageSize The size of the array
        void buildOutgoingPacketData(uint16_t messageId, uint8_t packetId, bool isStartPacket, bool isEndPacket, const uint8_t *messageBytes, size_t messageSize);
        
        /// @brief Returns the number of bytes in the packet buffer
        /// @return Returns the number of bytes for this packet
        size_t PacketBufferSize();
        
        /// @brief Gets the parsed buffer of the message bytes. 
        /// @return Returns the underlaying vector for the buffer
        std::vector<uint8_t> GetDataVector();

        /// @brief Returns the raw bytes in the array           
        uint8_t *GetData();

        /// @brief Prints the content of the message packet
        void PrintPacket();
        
        // Default constructor
        CommMessagePacket();

        // DeConstructor
        ~CommMessagePacket();
  };
}

#endif