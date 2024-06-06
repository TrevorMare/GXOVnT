/////////////////////////////////////////////////////////////////
#include "messages/CommMessage.h"

using namespace GXOVnTLib::shared;
using namespace GXOVnTLib::messages;

/////////////////////////////////////////////////////////////////

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
    // Set the message Id 
    m_messageId = messageId;
    // Calculate the chunk size as each packet. Take into account that the first 4 bytes are used
    // for meta data and should be added to the total packet size
    uint16_t offsetChunkSize = chunkSize - 4;

    // Now that we have the chunk size, we can calulate the number of message packets that needs to be created
    uint8_t numberOfChunks = static_cast<uint8_t>(ceil((double)messageSize / (double)offsetChunkSize));

    // Variables required by the loops
    size_t currentWriteIndex = 0;
    
    // Loop through the number of packets indexes and create them.
    for (size_t iChunk = 0; iChunk < numberOfChunks; iChunk++) {
        bool isPacketStart = (iChunk == 0);
        bool isPacketEnd = (iChunk == (numberOfChunks - 1));
        // Calculate the number of bytes left to write for this chunk
        size_t bytesLeftThisChunk = messageSize - (iChunk * offsetChunkSize);
        if (bytesLeftThisChunk > offsetChunkSize) bytesLeftThisChunk = offsetChunkSize;
        uint8_t *chunkBuffer = new uint8_t[bytesLeftThisChunk];

        for (size_t iChunkBufferIndex = 0; iChunkBufferIndex < bytesLeftThisChunk; iChunkBufferIndex++) {
            chunkBuffer[iChunkBufferIndex] = buffer[currentWriteIndex];
            currentWriteIndex ++;
        }
            
        CommMessagePacket *commMessagePacket = new CommMessagePacket();

        commMessagePacket->buildOutgoingPacketData(messageId, iChunk, 
            isPacketStart, isPacketEnd, chunkBuffer, bytesLeftThisChunk);

        m_messagePackets.push_back(commMessagePacket);
        m_totalSize += commMessagePacket->PacketBufferSize();
    }

    return &m_messagePackets;
}

