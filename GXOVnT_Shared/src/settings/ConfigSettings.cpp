#include "settings/ConfigSettings.h"
using namespace GXOVnTLib::settings;


void ConfigSettings::readSettingsFromJson(JsonDocument &document) {

    ESP_LOGI(LOG_TAG, "Reading of settings from configuration file \n");

    SystemSettings.readSettingsFromJson(document);
    WiFiSettings.readSettingsFromJson(document);
    TestWiFiSettings.readSettingsFromJson(document);
    BLETPMSSettings.readSettingsFromJson(document);
}

void ConfigSettings::writeSettingsToJson(JsonDocument &document) {
    ESP_LOGI(LOG_TAG, "Writing of settings to configuration file \n");

    SystemSettings.writeSettingsToJson(document);
    TestWiFiSettings.writeSettingsToJson(document);
    WiFiSettings.writeSettingsToJson(document);
    BLETPMSSettings.writeSettingsToJson(document);
}

bool ConfigSettings::settingsHasChanges() {
    return true;
}