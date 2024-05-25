#include "services/CommService.h"

using namespace GXOVnT::services;
using namespace GXOVnT::messages;



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

// bool decode_pb_string(pb_istream_t *stream, const pb_field_t *field, void **arg) {

//     uint8_t buffer[1024] = {0};
    
//     /* We could read block-by-block to avoid the large buffer... */
//     if (stream->bytes_left > sizeof(buffer) - 1)
//         return false;
    
//     if (!pb_read(stream, buffer, stream->bytes_left))
//         return false;
    
//     /* Print the string, in format comparable with protoc --decode.
//      * Format comes from the arg defined in main().
//      */
//     printf((char*)*arg, buffer);
//     return true;

// }

void CommService::handleMessage(CommMessage *commMessage) {

    if (commMessage == nullptr) {
        ESP_LOGW(LOG_TAG, "Could not handle null ptr comm message");
        return;
    } 
    // const uint8_t *buffer = commMessage->GetMessageBuffer();
    // const size_t message_length = commMessage->TotalSize();

    // gxovnt_messaging_Container containerProto = gxovnt_messaging_Container_init_zero;
    // //containerProto.msg.TextValue.funcs.decode = &decode_pb_string_x;


    /* Create a stream that reads from the buffer. */
    //pb_istream_t stream = pb_istream_from_buffer(buffer, message_length);
    // /* Now we are ready to decode the message. */
    // bool status = pb_decode(&stream, gxovnt_messaging_Container_fields, &containerProto);
    // if (!status) {
    //     ESP_LOGE(LOG_TAG, "Could not decode the GRPC package");
    // } else {
    //     ESP_LOGI(LOG_TAG, "GRPC package decoded!!!!");

        
    // }


    
    


    //commMessage->PrintMessage();
}