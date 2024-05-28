#include "services/CommService.h"

using namespace GXOVnT::services;
using namespace GXOVnT::messages;


bool commservice_processMessage_decodestring(pb_istream_t *stream, const pb_field_t *field, void **arg)
{
    //https://github.com/nanopb/nanopb/blob/master/tests/callbacks/decode_callbacks.c#L10
    uint8_t buffer[1024] = {0};
    /* We could read block-by-block to avoid the large buffer... */
    if (stream->bytes_left > sizeof(buffer) - 1)
        return false;
    ESP_LOGW(LOG_TAG, "Reading from the stream into buffer");
    if (!pb_read(stream, buffer, stream->bytes_left))
        return false;
    /* Print the string, in format comparable with protoc --decode.
     * Format comes from the arg defined in main().
     */
    ESP_LOGW(LOG_TAG, "Printing the value");
    
    printf((char*)*arg, buffer);
    return true;
}

CommService::CommService() {}
CommService::~CommService() {}

void CommService::start() {
    if (m_BleCommService != nullptr) return;

    m_BleCommService = new BleCommService();
    m_BleCommService->start(this);

}
void CommService::stop() {
    if (m_BleCommService == nullptr) return;
    m_BleCommService->stop();
}


void CommService::processMessage(CommMessage *commMessage) {
    const uint8_t *buffer = commMessage->Read();
    const size_t message_length = commMessage->TotalSize();
    gxovnt_messaging_Container containerProtoDecode = gxovnt_messaging_Container_init_zero;
    containerProtoDecode.msg.TextValue.arg = (void*)" submsg {\n  stringvalue: \"%s\"\n";
    containerProtoDecode.msg.TextValue.funcs.decode = &commservice_processMessage_decodestring;
    /* Create a stream that reads from the buffer. */
    pb_istream_t istream = pb_istream_from_buffer(buffer, message_length);
    /* Now we are ready to decode the message. */
    bool status = pb_decode(&istream, gxovnt_messaging_Container_fields, &containerProtoDecode);
    /* Check for errors... */
    if (!status) {
        ESP_LOGE(LOG_TAG, "Decoding failed: %s\n", PB_GET_ERROR(&istream));
    } 
}

bool CommService::sendMessage(uint8_t *buffer, size_t messageSize, enum GXOVnT_COMM_SERVICE_TYPE commServiceType) {
    
    CommMessage commMessage = { commServiceType };
    commMessage.Write(m_sendMessageId, buffer, messageSize);
    return sendMessage(&commMessage);
}

bool CommService::sendMessage(CommMessage *commMessage) {
    if (commMessage == nullptr) return false;
    if (commMessage->GetSourceService() == COMM_SERVICE_TYPE_BLE) {
        return m_BleCommService->sendMessage(commMessage);
    }
    m_sendMessageId++;
    return false;
}

void CommService::run() {

    std::lock_guard<std::mutex> lck(m_mutexLock);

    for (int iMessageToProcess = 0; iMessageToProcess < m_messagesToRun.size(); iMessageToProcess++) {
        CommMessage *commMessage = m_messagesToRun[iMessageToProcess];
    }
    m_messagesToRun.clear();
}

void CommService::onMessageReceived(CommMessage *commMessage) {
    std::lock_guard<std::mutex> lck(m_mutexLock);
    if (commMessage == nullptr) {
        ESP_LOGW(LOG_TAG, "Could not handle null ptr comm message");
        return;
    } 
    processMessage(commMessage);
    //m_messagesToRun.push_back(commMessage);
}