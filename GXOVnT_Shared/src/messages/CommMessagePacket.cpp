/////////////////////////////////////////////////////////////////
#include "messages/CommMessagePacket.h"
#include "shared/Shared.h"

using namespace GXOVnT::shared;
using namespace GXOVnT::messages;

// Public Methods
/////////////////////////////////////////////////////////////////
uint16_t CommMessagePacket::MessageId() { return m_messageId; }
uint8_t CommMessagePacket::PacketId() { return m_packetId; }
bool CommMessagePacket::PacketStart() { return m_packetStart; }
bool CommMessagePacket::PacketEnd() { return m_packetEnd; }
bool CommMessagePacket::ValidPacket() { return m_validPacket; }
std::vector<uint8_t> CommMessagePacket::PacketBuffer() { return m_packetBuffer; }
size_t CommMessagePacket::PacketBufferSize() { return m_packetBufferSize; }
void CommMessagePacket::PrintPacket() {
    if (!m_validPacket) {
        ESP_LOGI(LOG_TAG, "Packet is not valid");
    } else {
        std::string bufferValues = "";
        for (size_t iIndex = 0; iIndex < m_packetBuffer.size(); iIndex++) {
            uint8_t value = m_packetBuffer.at(iIndex);
            bufferValues += " " + std::to_string(value) + " ";
        }

        ESP_LOGI(LOG_TAG, "PacketId: %d, IsStart: %s, IsEnd: %s, Buffer: %s",
            m_packetId, (m_packetStart ? "true" : "false"), (m_packetEnd ? "true" : "false"), bufferValues.c_str());
    }
}
CommMessagePacket::CommMessagePacket() {}
CommMessagePacket::CommMessagePacket(const uint8_t *messageBytes, size_t messageSize) {
    parseMessageBytes(messageBytes, messageSize);
}
CommMessagePacket::~CommMessagePacket() {
    m_packetBuffer.clear();
}

// Private methods
/////////////////////////////////////////////////////////////////
void CommMessagePacket::parseMessageBytes(const uint8_t *messageBytes, size_t messageSize) {
    if (messageSize <= 4) {
		ESP_LOGW(LOG_TAG, "CommMessagePacket: Packet length to short to build the packet");
		return;
	}
	// First two bytes is message is message Id
	m_messageId = ((uint16_t)messageBytes[1] << 8) | messageBytes[0];
	m_packetId = messageBytes[2];
	uint8_t packetDetail = messageBytes[3];
    // Flags:
	//  Position 0 - Is message start packet
	//  Position 1 - Is message end packet
	m_packetStart = GetFlag(packetDetail, 0);
	m_packetEnd = GetFlag(packetDetail, 1);
    m_packetBuffer.reserve(20);
    // Create a vector of the remaining bytes. This represents the actual data in the packet
	for (int i = 4; i < messageSize; i++) {
		m_packetBuffer.push_back(messageBytes[i]);
	}
    m_packetBufferSize = messageSize - 4;
    m_validPacket = true;
}

uint8_t *CommMessagePacket::GetData() {
    return m_packetBuffer.data();
}