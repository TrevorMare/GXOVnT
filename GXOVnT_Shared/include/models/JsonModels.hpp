/////////////////////////////////////////////////////////////////
#pragma once
#ifndef _GXOVNT_MODELS_JSONMODELS_H_
#define _GXOVNT_MODELS_JSONMODELS_H_
#include <ArduinoJson.h>

namespace GXOVnT
{
    namespace models
    {
        // JSON Message Type Id's
        static const uint8_t JSON_MSG_TYPE_REQUEST_PROGRESS = 1;
        static const uint8_t JSON_MSG_TYPE_REPLY_PROGRESS = 2;
        static const uint8_t JSON_MSG_TYPE_REQUEST_ECHO = 3;
        static const uint8_t JSON_MSG_TYPE_REPLY_ECHO = 4;

#pragma region BaseModel 
class BaseModel
        {
            /// @brief Value for the type of model
            uint8_t m_messageTypeId = 0;
            /// @brief Value to indicate that this model is a response to another model
            uint16_t m_replyMessageId = 0;

        protected:
            /// @brief A reference to the json document for this model
            JsonDocument m_jsonDocument; 
            void MessageTypeId(uint8_t value) { 
                m_messageTypeId = value; 
                m_jsonDocument["messageTypeId"] = m_messageTypeId;
            }
            void ReplyMessageId(uint16_t value) { 
                m_replyMessageId = value;
                m_jsonDocument["replyMessageId"] = m_replyMessageId;            
            }
        public:
            /// @brief Default constructor
            BaseModel() {};
            /// @brief Constructs a new model with the specified message type Id
            /// @param messageTypeId 
            BaseModel(uint8_t messageTypeId, uint16_t replyMessageId = 0) { 
                MessageTypeId(messageTypeId);
                ReplyMessageId(replyMessageId);
            };
            BaseModel(JsonDocument &doc, uint16_t replyMessageId = 0) { 
                if (doc != nullptr) {
                    m_jsonDocument = doc;

                    if (m_jsonDocument.containsKey("messageTypeId")) {
                        m_messageTypeId = m_jsonDocument["messageTypeId"];
                    }

                    if (m_jsonDocument.containsKey("replyMessageId")) {
                        m_replyMessageId = m_jsonDocument["replyMessageId"];
                    }
                }
            };
            ~BaseModel() {};
            JsonDocument *Json() { return &m_jsonDocument; }
            uint8_t MessageTypeId() { return m_messageTypeId; }
            uint16_t ReplyMessageId() { return m_replyMessageId; }
        };
#pragma endregion

        

        class EchoModel: public BaseModel
        {
        private:    
            std::string m_echoMessage;
        public:
            EchoModel(uint16_t requestCommMessageId = 0) : BaseModel(JSON_MSG_TYPE_REQUEST_ECHO, requestCommMessageId) {};
            EchoModel(JsonDocument &doc, uint16_t requestCommMessageId = 0) : BaseModel(doc, requestCommMessageId) {
                if (m_jsonDocument.containsKey("echoMessage")) {
                    const char *echoMessage = m_jsonDocument["echoMessage"];
                    m_echoMessage = std::string(echoMessage);
                } 
            };
            ~EchoModel() {};
            std::string EchoMessage() {
                return m_echoMessage;
            }
            void EchoMessage(std::string value) {
                m_echoMessage = value;
                m_jsonDocument["echoMessage"] = m_echoMessage;
            }
        };


    }
}


#endif
