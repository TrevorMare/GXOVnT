#include "services/CommService.h"

using namespace GXOVnT::services;
using namespace GXOVnT::messages;

// Constructors
/////////////////////////////////////////////////////////////////
CommService::CommService() {
    // Create the tasks to read and write messages
    xTaskCreate(this->startProcessMessagesTask, "ProcessMessagesTask", 3072, this, 5, &m_ProcessMessagesTaskHandle);
}
CommService::~CommService() {
    if (m_ProcessMessagesTaskHandle != nullptr) {
        vTaskDelete(m_ProcessMessagesTaskHandle);
        m_ProcessMessagesTaskHandle = nullptr;
    }
}

// Tasks and management
/////////////////////////////////////////////////////////////////
void CommService::startProcessMessagesTask(void* _this) {
    static_cast<CommService*>(_this)->ProcessMessagesTask();
}

void CommService::ProcessMessagesTask() {
    
    while(1) {
        std::lock_guard<std::mutex> lck(m_messagesReceivedLock);
        size_t numberOfIncomingMessages = m_messagesReceived.size();
        if (numberOfIncomingMessages > 0) {

            try
            {
                
               
                for (int iMessageToProcess = 0; iMessageToProcess < numberOfIncomingMessages; iMessageToProcess++) {
                    CommMessage *commMessage = m_messagesReceived[iMessageToProcess];

                    const uint8_t *readBuffer = commMessage->Read();
                    const size_t message_length = commMessage->TotalSize();
                    JsonDocument doc;

                    deserializeJson(doc, readBuffer);

                    int messageTypeId = doc["messageTypeId"];
                    const char* messageDataChar = doc["messageData"];
                    std::string messageData(messageDataChar);
                    // Echo the message

                    ESP_LOGI(LOG_TAG, "Received Incoming Message: %s", messageData.c_str());
                }
            }
            catch(...)
            {
               
            }

            m_messagesReceived.clear();
        }

        
        

        std::lock_guard<std::mutex> guard(m_messagesToSendLock);
        int numberOfOutgoingMessages = m_messagesToSend.size();

        if (numberOfOutgoingMessages > 0) {
            ESP_LOGI(LOG_TAG, "Found messages to Send");
            for (size_t i = 0; i < numberOfOutgoingMessages; i++)
            {
                CommMessage *messageToSend = m_messagesToSend[i];
                if (messageToSend->GetSourceService() == COMM_SERVICE_TYPE_BLE) {
                    m_BleCommService->sendMessage(messageToSend);
                }

                ESP_LOGI(LOG_TAG, "message sent");
            }

            m_messagesToSend.clear();
            

        }


        // Delay for 500 ms to give the app time to run
        vTaskDelay(1500);
    }
}


/////////////////////////////////////////////////////////////////
void CommService::start() {
    if (m_BleCommService != nullptr) return;

    m_BleCommService = new BleCommService();
    m_BleCommService->start(this);
}

void CommService::stop() {
    if (m_BleCommService == nullptr) return;
    m_BleCommService->stop();
}

void CommService::onMessageReceived(CommMessage *commMessage) {
    if (commMessage == nullptr) {
        ESP_LOGW(LOG_TAG, "Could not handle null ptr comm message");
        return;
    } 
    std::lock_guard<std::mutex> guard(m_messagesReceivedLock);
    m_messagesReceived.push_back(commMessage);
}

/////////////////////////////////////////////////////////////////
bool CommService::sendMessage(uint8_t *buffer, size_t messageSize, enum GXOVnT_COMM_SERVICE_TYPE commServiceType) {
    CommMessage *commMessage = new CommMessage(commServiceType);
    commMessage->Write(m_sendMessageId, buffer, messageSize);
    return sendMessage(commMessage);
}

bool CommService::sendMessage(CommMessage *commMessage) {
    if (commMessage == nullptr) return false;
    std::lock_guard<std::mutex> guard(m_messagesToSendLock);
    m_messagesToSend.push_back(commMessage);
    m_sendMessageId++;
    return true; 
}


