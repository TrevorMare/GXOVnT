/////////////////////////////////////////////////////////////////
#pragma once
#ifndef _GXOVNT_MODELS_JSONMODELS_H_
#define _GXOVNT_MODELS_JSONMODELS_H_
#include <ArduinoJson.h>
#include "shared/Definitions.h"

namespace GXOVnT
{
    namespace models
    {
        
        static const uint8_t JSON_MSG_TYPE_RESPONSE_STATUS = 101;
        static const uint8_t JSON_MSG_TYPE_RESPONSE_GET_SYSTEM_SETTINGS = 102;

        /// @brief Binds to Echo Model and returns same model 
        static const uint8_t JSON_MSG_TYPE_REQUEST_ECHO = 3;
        /// @brief Binds to base model and return status model
        static const uint8_t JSON_MSG_TYPE_REQUEST_KEEP_ALIVE = 4;

        /// @brief Binds to base model and returns GetSystemSettingsResponseModel
        static const uint8_t JSON_MSG_TYPE_REQUEST_GET_SYSTEM_SETTINGS = 5;
        /// @brief Binds to SetSystemSettingsRequestModel model and return status model
        static const uint8_t JSON_MSG_TYPE_REQUEST_SET_SYSTEM_SETTINGS = 6;
        /// @brief Binds to base model and return status model
        static const uint8_t JSON_MSG_TYPE_REQUEST_REBOOT = 98;
        static const uint8_t JSON_MSG_TYPE_REQUEST_SAVE_CONFIGURATION = 99;

#pragma region BaseModel 
        /// @brief Base Json model with the root fields. All json models should inherit this class
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

#pragma region Status Response Model
        /// @brief Keep-Alive model to keep the connection alive. Response is standard status response
        class StatusResponseModel: public BaseModel
        {
        private:
            uint8_t m_statusCode = 200;
            std::string m_statusMessage = "OK";
        public:
            StatusResponseModel(uint16_t requestCommMessageId = 0, uint8_t statusCode = 200, std::string statusMessage = "OK") 
                : BaseModel(JSON_MSG_TYPE_RESPONSE_STATUS, requestCommMessageId) {
                StatusCode(statusCode);
                StatusMessage(statusMessage);
            };
            ~StatusResponseModel() {};
            uint8_t StatusCode() { return m_statusCode; }
            std::string StatusMessage() { return m_statusMessage; }
            void StatusCode(uint8_t value) {  
                m_statusCode = value;
                m_jsonDocument["statusCode"] = m_statusCode;
            }
            void StatusMessage(std::string value) { 
                m_statusMessage = value;
                m_jsonDocument["statusMessage"] = m_statusMessage;
            }
        };

#pragma endregion

#pragma region Echo Model        
        /// @brief Serves as both a request and reply for an echo request message
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
#pragma endregion

#pragma region Keep Alive Model         
        /// @brief Keep-Alive model to keep the connection alive. Response is standard status response
        class KeepAliveRequestModel: public BaseModel
        {
        public:
            KeepAliveRequestModel(uint16_t requestCommMessageId = 0) : BaseModel(JSON_MSG_TYPE_REQUEST_KEEP_ALIVE, requestCommMessageId) {};
            ~KeepAliveRequestModel() {};
        };
#pragma endregion

#pragma region Get Settings Model
        /// @brief Keep-Alive model to keep the connection alive. Response is standard status response
        class GetSystemSettingsResponseModel: public BaseModel
        {
        private:
            std::string m_systemName;
            std::string m_firmwareVersion;
            std::string m_systemId;
            bool m_systemConfigured;
            int m_systemType;

        public:
            GetSystemSettingsResponseModel(uint16_t requestCommMessageId = 0) : BaseModel(JSON_MSG_TYPE_RESPONSE_GET_SYSTEM_SETTINGS, requestCommMessageId) {};
            ~GetSystemSettingsResponseModel() {};
            
            std::string SystemName() { return m_systemName; }
            void SystemName(std::string value) {
                m_systemName = value;
                m_jsonDocument["systemName"] = m_systemName;
            }

            std::string SystemId() { return m_systemId; }
            void SystemId(std::string value) {
                m_systemId = value;
                m_jsonDocument["systemId"] = m_systemId;
            }

            bool SystemConfigured() { return m_systemConfigured; }
            void SystemConfigured(bool value) {
                m_systemConfigured = value;
                m_jsonDocument["systemConfigured"] = m_systemConfigured;
            }

            int SystemType() { return m_systemType; }
            void SystemType(int value) {
                m_systemType = value;
                m_jsonDocument["systemType"] = m_systemType;
            }

            std::string FirmwareVersion() { return m_firmwareVersion; }
            void FirmwareVersion(std::string value) {
                m_firmwareVersion = value;
                m_jsonDocument["firmwareVersion"] = m_firmwareVersion;
            }

            
        };
#pragma endregion

#pragma region Get Settings Model
        /// @brief Keep-Alive model to keep the connection alive. Response is standard status response
        class SetSystemSettingsRequestModel: public BaseModel
        {
        private: 
            std::string m_systemName = "";
            bool m_systemConfigured = false;
        public:
            SetSystemSettingsRequestModel(uint16_t requestCommMessageId = 0) : BaseModel(JSON_MSG_TYPE_REQUEST_SET_SYSTEM_SETTINGS, requestCommMessageId) {};
            SetSystemSettingsRequestModel(JsonDocument &doc, uint16_t requestCommMessageId = 0) : BaseModel(doc, requestCommMessageId) {
                if (m_jsonDocument.containsKey("systemName")) {
                    const char *systemName = m_jsonDocument["systemName"];
                    m_systemName = std::string(systemName);
                } 
                if (m_jsonDocument.containsKey("systemId")) {
                    m_systemConfigured = m_jsonDocument["systemConfigured"];
                }
            };
            std::string SystemName() { return m_systemName; }
            bool SystemConfigured() { return m_systemConfigured; }
            ~SetSystemSettingsRequestModel() {};
        };
#pragma endregion

    }
}
#endif
