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

    ESP_LOGI(LOG_TAG, "Processing Json for message Id %d and message type %d", requestCommMessageId, messageTypeId);

    switch (messageTypeId)
    {
        case JSON_MSG_TYPE_REQUEST_KEEP_ALIVE:
        {

            ESP_LOGI(LOG_TAG, "Creating response for Keep Alive");

            StatusResponseModel *responseModel = new StatusResponseModel(requestCommMessageId, 200, "OK");
            return responseModel->Json();
        }
        case JSON_MSG_TYPE_REQUEST_ECHO:
        {

            ESP_LOGI(LOG_TAG, "Creating response for Echo");

            EchoModel requestModel(inputDocument);
            std::string requestModelEchoMessage = requestModel.EchoMessage();
            EchoModel *responseModel = new EchoModel(requestCommMessageId);
            responseModel->EchoMessage("Echo - " + requestModelEchoMessage);
            return responseModel->Json();
        }
        case JSON_MSG_TYPE_REQUEST_GET_SYSTEM_SETTINGS:
        {

            ESP_LOGI(LOG_TAG, "Creating response for get system settings");

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

            ESP_LOGI(LOG_TAG, "Creating response for set system settings");

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
            
            ESP_LOGI(LOG_TAG, "Set the test WiFi configuration to test settings on next boot for SSID %s", requestModel.WifiSSID().c_str());

            GXOVnT.config->Settings.TestWiFiSettings.SSID(requestModel.WifiSSID());
            GXOVnT.config->Settings.TestWiFiSettings.Password(requestModel.WifiPassword());
            GXOVnT.config->Settings.TestWiFiSettings.Tested(false);
            GXOVnT.config->Settings.TestWiFiSettings.Success(false);
            GXOVnT.config->Settings.TestWiFiSettings.TestResultCode(0);
            GXOVnT.config->Settings.TestWiFiSettings.TestResultMessage("");
            GXOVnT.config->Settings.TestWiFiSettings.TestOnNextBoot(true);

            GXOVnT.config->saveConfiguration();

            StatusResponseModel *responseModel = new StatusResponseModel(requestCommMessageId, 200, "OK");
            return responseModel->Json();
        }
        case JSON_MSG_TYPE_REQUEST_SAVE_CONFIGURATION:
        {
            ESP_LOGI(LOG_TAG, "Creating response for save configuration");

            GXOVnT.config->saveConfiguration();
            StatusResponseModel *responseModel = new StatusResponseModel(requestCommMessageId, 200, "OK");
            return responseModel->Json();
        }
        case JSON_MSG_TYPE_REQUEST_REBOOT:
        {
            ESP_LOGI(LOG_TAG, "Rebooting device");
            delay(1000);
            ESP.restart();
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

