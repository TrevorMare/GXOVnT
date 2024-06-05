/////////////////////////////////////////////////////////////////
#pragma once
#ifndef _GXOVNT_JSONMESSAGESERVICE_H_
#define _GXOVNT_JSONMESSAGESERVICE_H_
#include <ArduinoJson.h>
#include "shared/Definitions.h"
#include "messages/CommMessage.h"
#include "models/JsonModels.hpp"


using namespace GXOVnT::messages;
using namespace GXOVnT::models;


namespace GXOVnT 
{
    namespace services
    {
        class JsonMessageService {
        private:
            
            
            JsonDocument *processJsonMessage(JsonDocument &inputDocument, uint16_t requestCommMessageId = 0) {
                BaseModel baseModel(inputDocument);
                uint8_t messageTypeId = baseModel.MessageTypeId();
// TODO: Figure out linking
                // switch (messageTypeId)
                // {
                // case JSON_MSG_TYPE_REQUEST_KEEP_ALIVE: {
                //    StatusResponseModel *responseModel = new StatusResponseModel(requestCommMessageId, 200, "OK");
                //    return responseModel->Json();
                // }
                // case JSON_MSG_TYPE_REQUEST_ECHO: {
                //     EchoModel requestModel(inputDocument);
                //     std::string requestModelEchoMessage = requestModel.EchoMessage();
                //     EchoModel *responseModel = new EchoModel(requestCommMessageId);
                //     responseModel->EchoMessage("Echo - " + requestModelEchoMessage);
                //     return responseModel->Json();
                // }
                // case JSON_MSG_TYPE_REQUEST_GET_SYSTEM_SETTINGS: {

                //     GXOVnT::settings::Config config = *(GXOVnT::services::ServiceLocator::Config());


                //     GetSystemSettingsResponseModel *responseModel = new GetSystemSettingsResponseModel(requestCommMessageId);
                //     responseModel->FirmwareVersion(config.Settings.SystemSettings.FirmwareVersion());
                //     responseModel->SystemConfigured(config.Settings.SystemSettings.SystemConfigured());
                //     responseModel->SystemId(config.Settings.SystemSettings.SystemId());
                //     responseModel->SystemName(config.Settings.SystemSettings.SystemName());
                //     responseModel->SystemType(static_cast<int>(config.Settings.SystemSettings.SystemType()));

                //     return responseModel->Json();
                // }
                // case JSON_MSG_TYPE_REQUEST_SET_SYSTEM_SETTINGS: {
                   
                //     SetSystemSettingsRequestModel requestModel(inputDocument);

                //     GXOVnT::settings::Config config = *(GXOVnT::services::ServiceLocator::Config());

                //     config.Settings.SystemSettings.SystemName(requestModel.SystemName());
                //     config.Settings.SystemSettings.SystemConfigured(requestModel.SystemConfigured());

                //    StatusResponseModel *responseModel = new StatusResponseModel(requestCommMessageId, 200, "OK");
                //    return responseModel->Json();
                // }
                // case JSON_MSG_TYPE_REQUEST_SAVE_CONFIGURATION: {

                //     GXOVnT::settings::Config config = *(GXOVnT::services::ServiceLocator::Config());

                //     config.saveConfiguration();
                //     StatusResponseModel *responseModel = new StatusResponseModel(requestCommMessageId, 200, "OK");
                //     return responseModel->Json();
                // }
                // case JSON_MSG_TYPE_REQUEST_REBOOT: {
                //    ESP.restart();
                // }
                // default:
                //     break;
                // }
                // Return no reply default
                return nullptr;    
            }
            
        public:
            JsonMessageService() {};
            ~JsonMessageService() {};

            JsonDocument *handleJsonMessage(CommMessage *commMessage) {
                const uint8_t *messageBuffer = commMessage->Read();
                JsonDocument doc;
                deserializeJson(doc, messageBuffer);
                return processJsonMessage(doc, commMessage->MessageId());
            };

            JsonDocument *handleJsonMessage(JsonDocument &inputDocument, uint16_t requestCommMessageId = 0) {
                return processJsonMessage(inputDocument, requestCommMessageId);
            };

        };

    }
}


#endif
