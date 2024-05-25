/////////////////////////////////////////////////////////////////
#include "messages/CommMessage.h"

using namespace GXOVnT::shared;
using namespace GXOVnT::messages;

CommMessage::CommMessage(enum GXOVnT_COMM_SERVICE_TYPE commServiceType) {
    m_commServiceType = commServiceType;
}

CommMessage::~CommMessage() {
    for (size_t iIndex = 0; iIndex < m_messagePackets.size(); iIndex++) {
        delete m_messagePackets[iIndex];
    }
    m_messagePackets.clear();
    m_messageBuffer.clear();
}

void CommMessage::AddPackage(CommMessagePacket *messagePacket) {
    if (messagePacket == nullptr) {
        ESP_LOGW(LOG_TAG, "Message packet was null, could not add it");
    }
    
    if (!messagePacket->ValidPacket()) {
        ESP_LOGW(LOG_TAG, "Message packet was not valid, skipping the add");
    }

    if (m_messageId <= 0) {
        m_messageId = messagePacket->MessageId();
    }
    m_messagePackets.push_back(messagePacket);

    // Get the size of the packet buffer
    m_totalSize += messagePacket->PacketBufferSize();

    m_expiryMillis = millis();

    if (messagePacket->PacketEnd()) {
        m_receivedAllPackets = true;
    }
}

bool CommMessage::ReceivedAllPackets() { return m_receivedAllPackets; }

size_t CommMessage::TotalSize() { return m_totalSize; }

enum GXOVnT_COMM_SERVICE_TYPE CommMessage::GetSourceService() { return m_commServiceType; }

uint16_t CommMessage::MessageId() { return m_messageId; }

bool CommMessage::IsExpired() {
    unsigned long explicitExpiry = m_expiryMillis + 30000;
    return explicitExpiry < millis();
}

void CommMessage::PrintMessage() {
    ESP_LOGI(LOG_TAG, "MessageId: %d, TotalSize: %d (bytes), NumberOfPackets: %d",
        m_messageId, m_totalSize, m_messagePackets.size());
    int numberOfPackets = m_messagePackets.size();

    for (size_t i = 0; i < numberOfPackets; i++) {
        m_messagePackets[i]->PrintPacket();
    }

    const uint8_t * messageBuffer = GetMessageBuffer();
    
    std::string bufferValues = "";
    for (size_t iIndex = 0; iIndex < m_totalSize; iIndex++) {
        uint8_t value = messageBuffer[iIndex];
        bufferValues += " " + std::to_string(value) + " ";
    }
    
    ESP_LOGI(LOG_TAG, "Message bytes: %s", bufferValues.c_str());
}

const uint8_t* CommMessage::GetMessageBuffer() {
    if (!ReceivedAllPackets()) {
        ESP_LOGW(LOG_TAG, "CommMessage: Get buffer called before all packets were received");
    }
    m_messageBuffer.clear();
    m_messageBuffer.reserve(m_totalSize);
    int numberOfPackets = m_messagePackets.size();
    // For each of the packets
    for (int iPacket = 0; iPacket < numberOfPackets; iPacket++) {
        // Get the packet buffer and size
        const uint8_t *packetBuffer = m_messagePackets[iPacket]->GetData();
        size_t packetBufferSize = m_messagePackets[iPacket]->PacketBufferSize();
        // Copy the packet buffer data into the message buffer
        for (int iPacketBuffer = 0; iPacketBuffer < packetBufferSize; iPacketBuffer++) {
            uint8_t value = packetBuffer[iPacketBuffer];
            m_messageBuffer.push_back(value);
        }
    }
    return m_messageBuffer.data();
}