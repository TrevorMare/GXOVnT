/////////////////////////////////////////////////////////////////
#include "messages/CommMessagePacket.h"

using namespace GXOVnTLib::shared;
using namespace GXOVnTLib::messages;

// Constructors
/////////////////////////////////////////////////////////////////
CommMessagePacket::CommMessagePacket() {}
CommMessagePacket::~CommMessagePacket() {
    m_packetBuffer.clear();
}

// Property Getters
uint16_t CommMessagePacket::MessageId() { return m_messageId; }
uint8_t CommMessagePacket::PacketId() { return m_packetId; }
bool CommMessagePacket::PacketStart() { return m_packetStart; }
bool CommMessagePacket::PacketEnd() { return m_packetEnd; }
bool CommMessagePacket::ValidPacket() { return m_validPacket; }
uint8_t *CommMessagePacket::GetData() { return m_packetBuffer.data(); }
std::vector<uint8_t> CommMessagePacket::GetDataVector() { return m_packetBuffer; }
size_t CommMessagePacket::PacketBufferSize() { return m_packetBufferSize; }
enum GXOVnT_COMM_MESSAGE_DIRECTION CommMessagePacket::MessageDirection() { return m_commMessageDirection; }
// Helper Methods
/////////////////////////////////////////////////////////////////
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

// Packet builder methods
/////////////////////////////////////////////////////////////////
void CommMessagePacket::buildOutgoingPacketData(uint16_t messageId, uint8_t packetId, bool isStartPacket, bool isEndPacket, const uint8_t *messageBytes, size_t messageSize) {
    m_commMessageDirection = COMM_MESSAGE_DIRECTION_OUTGOING;
    m_messageId = messageId;
    m_packetId = packetId;
    m_packetStart = isStartPacket;
    m_packetEnd = isEndPacket;
    // Add 4 bytes to the packet buffer size for meta data
    m_packetBufferSize = messageSize + 4;
    // Start writing the buffer;
    uint8_t messageIdPart1 = (messageId << 8);
    uint8_t messageIdPart2 = (uint8_t)messageId;

    uint8_t flags = 0;
    if (isStartPacket) flags += 1;
    if (isEndPacket) flags += 2;

    m_packetBuffer.push_back(messageIdPart1);
    m_packetBuffer.push_back(messageIdPart2);
    m_packetBuffer.push_back(m_packetId);
    m_packetBuffer.push_back(flags);

    for (size_t i = 0; i < messageSize; i++) {
        m_packetBuffer.push_back(messageBytes[i]);
    }
    m_validPacket = m_packetBufferSize > 4;
}

void CommMessagePacket::buildIncomingPacketData(const uint8_t *messageBytes, size_t messageSize) {
    m_commMessageDirection = COMM_MESSAGE_DIRECTION_INCOMING;

    if (messageSize <= 4) {
		ESP_LOGW(LOG_TAG, "CommMessagePacket: Packet length to short to build the packet");
		return;
	}
	// First two bytes is message is message Id
	m_messageId = ((uint16_t)messageBytes[0] << 8) | messageBytes[1];
	m_packetId = messageBytes[2];
	uint8_t packetDetail = messageBytes[3];
    // Flags:
	//  Position 0 - Is message start packet
	//  Position 1 - Is message end packet
	m_packetStart = GetFlag(packetDetail, 0);
	m_packetEnd = GetFlag(packetDetail, 1);
    // ESP_LOGI(LOG_TAG, "Parsed package MessageId: %d, PacketId: %d, PacketDetail: %d, IsStart: %s, IsEnd: %s",
    // m_messageId, m_packetId, packetDetail, (m_packetStart ? "true" : "false"), (m_packetStart ? "true" : "false"));
    m_packetBuffer.reserve(20);
    // Create a vector of the remaining bytes. This represents the actual data in the packet
	for (int i = 4; i < messageSize; i++) {
        uint8_t indexValue = messageBytes[i];
		m_packetBuffer.push_back(indexValue);
	}
    m_packetBufferSize = messageSize - 4;
    m_validPacket = true;
}
