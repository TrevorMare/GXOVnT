/////////////////////////////////////////////////////////////////
#include "messages/CommMessage.h"

using namespace GXOVnT::shared;
using namespace GXOVnT::messages;

/////////////////////////////////////////////////////////////////

CommMessage::CommMessage(enum GXOVnT_COMM_SERVICE_TYPE commServiceType) {
    m_commServiceType = commServiceType;
}

CommMessage::~CommMessage() {
    ESP_LOGI(LOG_TAG, "Disposing CommMessage");
    for (size_t iIndex = 0; iIndex < m_messagePackets.size(); iIndex++) {
        delete m_messagePackets[iIndex];
    }
    m_messagePackets.clear();
    m_messageBuffer.clear();
}

// Property getters
/////////////////////////////////////////////////////////////////
uint16_t CommMessage::MessageId() { return m_messageId; }
bool CommMessage::ReceivedAllPackets() { return m_receivedAllPackets; }
size_t CommMessage::TotalSize() { return m_totalSize; }
bool CommMessage::IsProcessed() { return m_IsProcessed; }
void CommMessage::IsProcessed(bool processed) { m_IsProcessed = processed; }
enum GXOVnT_COMM_SERVICE_TYPE CommMessage::GetSourceService() { return m_commServiceType; }
std::vector<CommMessagePacket*> *CommMessage::MessagePackets() { return &m_messagePackets; }
bool CommMessage::IsExpired() {
    unsigned long explicitExpiry = m_lastPacketReceivedTimeStamp + 30000;
    return explicitExpiry < millis();
}

// Packet helpers
/////////////////////////////////////////////////////////////////

void CommMessage::AddIncomingPackage(CommMessagePacket *messagePacket) {
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

    m_lastPacketReceivedTimeStamp = millis();

    if (messagePacket->PacketEnd()) {
        m_receivedAllPackets = true;
    }
}

// Helper Methods
/////////////////////////////////////////////////////////////////

void CommMessage::PrintMessage() {
    ESP_LOGI(LOG_TAG, "MessageId: %d, TotalSize: %d (bytes), NumberOfPackets: %d",
        m_messageId, m_totalSize, m_messagePackets.size());
    int numberOfPackets = m_messagePackets.size();

    for (size_t i = 0; i < numberOfPackets; i++) {
        m_messagePackets[i]->PrintPacket();
    }

    const uint8_t * messageBuffer = Read();
    
    std::string bufferValues = "";
    for (size_t iIndex = 0; iIndex < m_totalSize; iIndex++) {
        uint8_t value = messageBuffer[iIndex];
        bufferValues += " " + std::to_string(value) + " ";
    }
    
    ESP_LOGI(LOG_TAG, "Message bytes: %s", bufferValues.c_str());
}

const uint8_t* CommMessage::Read() {
    if (!ReceivedAllPackets()) {
        ESP_LOGW(LOG_TAG, "CommMessage: Read called before all packets were received");
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

std::vector<CommMessagePacket*> *CommMessage::Write(uint16_t messageId, uint8_t *buffer, size_t messageSize, uint16_t chunkSize) {
     
    m_messageId = messageId;

    uint8_t chunkBuffer[chunkSize];
    // Calculate the chunk size as each packet uses 4 bytes for meta data
    uint16_t calculatedBytesSize = chunkSize - 4;
    // Calculate the number of chunks
    uint8_t numberOfChunks = ceil(messageSize / calculatedBytesSize);
    // Now we can start by building the chunks
    for (int iChunk = 0; iChunk < numberOfChunks; iChunk++) {
        bool isPacketStart = (iChunk == 0);
        bool isPacketEnd = (iChunk == (numberOfChunks - 1));
        CommMessagePacket *commMessagePacket = new CommMessagePacket();

        commMessagePacket->buildOutgoingPacketData(messageId, iChunk, 
            isPacketStart, isPacketEnd, buffer, messageSize);

        m_messagePackets.push_back(commMessagePacket);
        m_totalSize += commMessagePacket->PacketBufferSize();
    }

    return &m_messagePackets;
}