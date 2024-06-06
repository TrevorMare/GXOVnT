#include "services/CommService.h"

using namespace GXOVnTLib::services;
using namespace GXOVnTLib::messages;


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
    delete m_BleCommService;
    delete m_jsonMessageService;
}

// Tasks and management
/////////////////////////////////////////////////////////////////
void CommService::startProcessMessagesTask(void* _this) {
    static_cast<CommService*>(_this)->ProcessMessagesTask();
}

void CommService::ProcessMessagesTask() {
    
    while(1) {
        processReceivedMessages();
        processSendMessages();
        vTaskDelay(200);
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

// Message handling
//////////////////////////////////////////////////////////////
void CommService::processReceivedMessages() {
    std::lock_guard<std::mutex> lck(m_messagesReceivedLock);
    size_t numberOfIncomingMessages = m_messagesReceived.size();
    if (numberOfIncomingMessages > 0) {

       for (int iMessageToProcess = 0; iMessageToProcess < numberOfIncomingMessages; iMessageToProcess++) {
            CommMessage *commMessage = m_messagesReceived[iMessageToProcess];
            
            ESP_LOGI(LOG_TAG, "processReceivedMessages: Comm message Id - %d", commMessage->MessageId());

            // Handle the json document and get a response
            JsonDocument *responseDocument = m_jsonMessageService->handleJsonMessage(commMessage);
            
            // If there is a response to be written to the comm service, add it to the outgoing messages
            if (responseDocument != nullptr) {
                // Build the outgoing message response
                std::string responseJsonString = "";
                serializeJson(*responseDocument, responseJsonString);
                uint8_t *responseBuffer = CharPtrToUInt8Ptr(responseJsonString.c_str());
                // Add the message to the outgoing queue
                sendMessage(responseBuffer, responseJsonString.length(), commMessage->GetSourceService());
            }

            // Lastly mark the message as completed in the applicable service
            if (commMessage->GetSourceService() == COMM_SERVICE_TYPE_BLE) {
                m_BleCommService->receivedMessageHandled(commMessage->MessageId());
            }
        }
        // Do not delete the object here as we are not the owner, rather let the owner service know that we are 
        // done processing the message
        m_messagesReceived.clear();
    }
}

void CommService::processSendMessages() {
    std::lock_guard<std::mutex> guard(m_messagesToSendLock);
    size_t numberOfOutgoingMessages = m_messagesToSend.size();

    if (numberOfOutgoingMessages > 0) {
        ESP_LOGI(LOG_TAG, "Preparing to send %d message(s)", numberOfOutgoingMessages);
        
        for (size_t i = 0; i < numberOfOutgoingMessages; i++) {
            CommMessage *messageToSend = m_messagesToSend[i];
            if (messageToSend->GetSourceService() == COMM_SERVICE_TYPE_BLE) {
                m_BleCommService->sendMessage(messageToSend);
            } else {
                ESP_LOGW(LOG_TAG, "Message could not be sent, source service type not implemented");
            }
        }
        // Clear the outgoing messages
        for (CommMessage* obj : m_messagesToSend)
            delete obj;

        m_messagesToSend.clear();
    }
}
