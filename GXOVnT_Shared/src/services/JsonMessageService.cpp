#include "services/JsonMessageService.h"

using namespace GXOVnTLib::services;
using namespace GXOVnTLib::models;

#include <WiFi.h>
#include <WiFiClient.h>

JsonMessageService::JsonMessageService() { }
JsonMessageService::~JsonMessageService() { }

JsonDocument *JsonMessageService::handleJsonMessage(GXOVnTLib::messages::CommMessage *commMessage) {
    const uint8_t *messageBuffer = commMessage->Read();
    JsonDocument doc;
    deserializeJson(doc, messageBuffer);
    return processJsonMessage(doc, commMessage->MessageId());
}

JsonDocument *JsonMessageService::handleJsonMessage(JsonDocument &inputDocument, uint16_t requestCommMessageId) {
    return processJsonMessage(inputDocument, requestCommMessageId);
}

JsonDocument *JsonMessageService::processJsonMessage(JsonDocument &inputDocument, uint16_t requestCommMessageId) {
    BaseModel baseModel(inputDocument);
    uint8_t messageTypeId = baseModel.MessageTypeId();

    switch (messageTypeId)
    {
    case JSON_MSG_TYPE_REQUEST_KEEP_ALIVE:
    {
        StatusResponseModel *responseModel = new StatusResponseModel(requestCommMessageId, 200, "OK");
        return responseModel->Json();
    }
    case JSON_MSG_TYPE_REQUEST_ECHO:
    {
        EchoModel requestModel(inputDocument);
        std::string requestModelEchoMessage = requestModel.EchoMessage();
        EchoModel *responseModel = new EchoModel(requestCommMessageId);
        responseModel->EchoMessage("Echo - " + requestModelEchoMessage);
        return responseModel->Json();
    }
    case JSON_MSG_TYPE_REQUEST_GET_SYSTEM_SETTINGS:
    {
        GetSystemSettingsResponseModel *responseModel = new GetSystemSettingsResponseModel(requestCommMessageId);
        
        responseModel->FirmwareVersion(GXOVnT.config->Settings.SystemSettings.FirmwareVersion());
        responseModel->SystemConfigured(GXOVnT.config->Settings.SystemSettings.SystemConfigured());
        responseModel->SystemId(GXOVnT.config->Settings.SystemSettings.SystemId());
        responseModel->SystemName(GXOVnT.config->Settings.SystemSettings.SystemName());
        responseModel->SystemType(static_cast<int>(GXOVnT.config->Settings.SystemSettings.SystemType()));

        responseModel->WifiSSID(GXOVnT.config->Settings.WiFiSettings.SSID());
        responseModel->WifiPassword(GXOVnT.config->Settings.WiFiSettings.Password());

        return responseModel->Json();
    }
    case JSON_MSG_TYPE_REQUEST_SET_SYSTEM_SETTINGS:
    {

        SetSystemSettingsRequestModel requestModel(inputDocument);
        GXOVnT.config->Settings.SystemSettings.SystemName(requestModel.SystemName());
        GXOVnT.config->Settings.SystemSettings.SystemConfigured(requestModel.SystemConfigured());
        GXOVnT.config->Settings.WiFiSettings.SSID(requestModel.WifiSSID());
        GXOVnT.config->Settings.WiFiSettings.Password(requestModel.WifiPassword());

        StatusResponseModel *responseModel = new StatusResponseModel(requestCommMessageId, 200, "OK");
        return responseModel->Json();
    }
    case JSON_MSG_TYPE_REQUEST_TEST_WIFI_SETTINGS:
    {
        TestWiFiSettingsRequestModel requestModel(inputDocument);
        // Test the WiFi Settings

        try
        {
            WiFi.begin(requestModel.WifiSSID().c_str(), requestModel.WifiPassword().c_str());    
            WiFi.disconnect();
            
            StatusResponseModel *responseModel = new StatusResponseModel(requestCommMessageId, 200, "OK");
            return responseModel->Json();
        }
        catch(const std::exception& e)
        {
            StatusResponseModel *responseModel = new StatusResponseModel(requestCommMessageId, 500, "Failed");
            return responseModel->Json();
        }
    }
    case JSON_MSG_TYPE_REQUEST_SAVE_CONFIGURATION:
    {
        GXOVnT.config->saveConfiguration();
        StatusResponseModel *responseModel = new StatusResponseModel(requestCommMessageId, 200, "OK");
        return responseModel->Json();
    }
    case JSON_MSG_TYPE_REQUEST_REBOOT:
    {
        ESP.restart();
    }
    default:
        break;
    }
    // Return no reply default
    return nullptr;
}

