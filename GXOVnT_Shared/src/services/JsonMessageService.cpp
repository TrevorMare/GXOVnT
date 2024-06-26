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

    BaseMessageModel baseModel(inputDocument);
    uint8_t messageTypeId = baseModel.MessageTypeId();

    ESP_LOGI(LOG_TAG, "Processing Json for message Id %d and message type %d", requestCommMessageId, messageTypeId);

    switch (messageTypeId)
    {
        case MsgType_KeepAliveRequest:
        {
            ESP_LOGI(LOG_TAG, "Creating response for Keep Alive");
            StatusResponse *responseModel = new StatusResponse(requestCommMessageId, 200, "OK");
            return responseModel->Json();
        }
        case MsgType_EchoRequest:
        {
            ESP_LOGI(LOG_TAG, "Creating response for Echo");
            EchoRequest requestModel(inputDocument);
            std::string requestModelEchoMessage = requestModel.EchoMessage();
            EchoResponse *responseModel = new EchoResponse(requestCommMessageId);
            responseModel->EchoMessage("Echo - " + requestModelEchoMessage);
            return responseModel->Json();
        }
        case MsgType_GetSystemSettingsRequest:
        {

            ESP_LOGI(LOG_TAG, "Creating response for get system settings");

            GetSystemSettingsResponse *responseModel = new GetSystemSettingsResponse(requestCommMessageId);
            
            responseModel->FirmwareVersion(GXOVnT.config->Settings.SystemSettings.FirmwareVersion());
            responseModel->SystemConfigured(GXOVnT.config->Settings.SystemSettings.SystemConfigured());
            responseModel->SystemId(GXOVnT.config->Settings.SystemSettings.SystemId());
            responseModel->SystemName(GXOVnT.config->Settings.SystemSettings.SystemName());
            responseModel->SystemType(static_cast<int>(GXOVnT.config->Settings.SystemSettings.SystemType()));

            responseModel->WifiSsid(GXOVnT.config->Settings.WiFiSettings.WiFiSsid());
            responseModel->WifiPassword(GXOVnT.config->Settings.WiFiSettings.WiFiPassword());

            return responseModel->Json();
        }
        case MsgType_SetSystemSettingsRequest:
        {

            ESP_LOGI(LOG_TAG, "Creating response for set system settings");

            SetSystemSettingsRequest requestModel(inputDocument);
            GXOVnT.config->Settings.SystemSettings.SystemName(requestModel.SystemName());
            GXOVnT.config->Settings.SystemSettings.SystemConfigured(requestModel.SystemConfigured());
            GXOVnT.config->Settings.WiFiSettings.WiFiSsid(requestModel.WifiSsid());
            GXOVnT.config->Settings.WiFiSettings.WiFiPassword(requestModel.WifiPassword());

            StatusResponse *responseModel = new StatusResponse(requestCommMessageId, 200, "OK");
            return responseModel->Json();
        }
        case MsgType_TestWiFiSettingsRequest:
        {
            TestWiFiSettingsRequest requestModel(inputDocument);
            // Test the WiFi Settings
            
            ESP_LOGI(LOG_TAG, "Set the test WiFi configuration to test settings on next boot for SSID %s", requestModel.WifiSsid().c_str());

            GXOVnT.config->Settings.TestWiFiSettings.WiFiSsid(requestModel.WifiSsid());
            GXOVnT.config->Settings.TestWiFiSettings.WiFiPassword(requestModel.WifiPassword());
            GXOVnT.config->Settings.TestWiFiSettings.Tested(false);
            GXOVnT.config->Settings.TestWiFiSettings.Success(false);
            GXOVnT.config->Settings.TestWiFiSettings.StatusCode(0);
            GXOVnT.config->Settings.TestWiFiSettings.StatusMessage("");
            GXOVnT.config->Settings.TestWiFiSettings.TestOnNextBoot(true);

            GXOVnT.config->saveConfiguration();

            StatusResponse *responseModel = new StatusResponse(requestCommMessageId, 200, "OK");
            return responseModel->Json();
        }
        case MsgType_GetTestWiFiSettingsResultRequest:
        {

            GetTestWiFiSettingsResponse *responseModel = new GetTestWiFiSettingsResponse(requestCommMessageId);

            responseModel->StatusCode(GXOVnT.config->Settings.TestWiFiSettings.StatusCode());
            responseModel->StatusMessage(GXOVnT.config->Settings.TestWiFiSettings.StatusMessage());
            responseModel->WifiSsid(GXOVnT.config->Settings.TestWiFiSettings.WiFiSsid());
            responseModel->WifiPassword(GXOVnT.config->Settings.TestWiFiSettings.WiFiPassword());
            responseModel->Success(GXOVnT.config->Settings.TestWiFiSettings.Success());

            return responseModel->Json();
        }
        case MsgType_SaveConfigurationRequest:
        {
            ESP_LOGI(LOG_TAG, "Creating response for save configuration");

            GXOVnT.config->saveConfiguration();
            StatusResponse *responseModel = new StatusResponse(requestCommMessageId, 200, "OK");
            return responseModel->Json();
        }
        case MsgType_RebootRequest:
        {
            ESP_LOGI(LOG_TAG, "Rebooting device");
            delay(1000);
            ESP.restart();
        }
        case MsgType_DeleteSystemSettingsRequest:
        {

            DeleteSystemSettingsRequest requestModel(inputDocument);

            ESP_LOGI(LOG_TAG, "Deleting the system configuration");

            std::string currentSystemPassword = GXOVnT.config->Settings.SystemSettings.SystemPassword();
            std::string requestSystemPassword = requestModel.SystemPassword();

            if (currentSystemPassword.compare(requestSystemPassword) != 0) {
                StatusResponse *responseModel = new StatusResponse(requestCommMessageId, 1, "The input password did not match the system password");
                return responseModel->Json();
            } else {
                GXOVnT.config->deleteConfigurationFile();
                StatusResponse *responseModel = new StatusResponse(requestCommMessageId, 200, "OK");
                return responseModel->Json();
            }
        }
        default:
        {
            ESP_LOGE(LOG_TAG, "Message type could not be handled due to unknown message type Id");
            break;
        }
    }
    // Return no reply default
    return nullptr;
}

