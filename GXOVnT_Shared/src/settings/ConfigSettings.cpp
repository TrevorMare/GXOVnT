#include "settings/ConfigSettings.h"
using namespace GXOVnTLib::settings;


void ConfigSettings::readSettingsFromJson(JsonDocument &document) {

    ESP_LOGI(LOG_TAG, "Reading of settings from configuration file \n");

    #if GXOVnT_Settings_HAS_SYSTEM_SETTINGS
        SystemSettings.readSettingsFromJson(document);
    #endif
    #if GXOVnT_Settings_HAS_WIFI_SETTINGS
        WiFiSettings.readSettingsFromJson(document);
        TestWiFiSettings.readSettingsFromJson(document);
    #endif
    #if GXOVnT_Settings_HAS_BLE_TPMS_SETTINGS
        BLETPMSSettings.readSettingsFromJson(document);
    #endif
}

void ConfigSettings::writeSettingsToJson(JsonDocument &document) {
    ESP_LOGI(LOG_TAG, "Writing of settings to configuration file \n");

    #if GXOVnT_Settings_HAS_SYSTEM_SETTINGS
        SystemSettings.writeSettingsToJson(document);
        TestWiFiSettings.writeSettingsToJson(document);
    #endif
    #if GXOVnT_Settings_HAS_WIFI_SETTINGS
        WiFiSettings.writeSettingsToJson(document);
    #endif
    #if GXOVnT_Settings_HAS_BLE_TPMS_SETTINGS
        BLETPMSSettings.writeSettingsToJson(document);
    #endif
}

bool ConfigSettings::settingsHasChanges() {
    bool result = false;
    
    #if GXOVnT_Settings_HAS_SYSTEM_SETTINGS
        result |= SystemSettings.getSettingsChanged();
    #endif
    #if GXOVnT_Settings_HAS_WIFI_SETTINGS
        result |= WiFiSettings.getSettingsChanged();
        result |= TestWiFiSettings.getSettingsChanged();
    #endif
    #if GXOVnT_Settings_HAS_BLE_TPMS_SETTINGS
        result |= BLETPMSSettings.getSettingsChanged();
    #endif
    
    return result;
}