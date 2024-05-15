#include "settings/GXOVnT_Settings.h"

void GXOVnT_Settings::readSettingsFromJson(JsonDocument document) {

    ESP_LOGI(LOG_TAG, "Reading of settings from configuration file \n");

    #if GXOVnT_Settings_HAS_WIFI_SETTINGS
        WiFiSettings.readSettingsFromJson(document);
    #endif
    #if GXOVnT_Settings_HAS_BLE_TPMS_SETTINGS
        BLETPMSSettings.readSettingsFromJson(document);
    #endif

    ESP_LOGI(LOG_TAG, "Reading of settings completed \n");
}

void GXOVnT_Settings::writeSettingsToJson(JsonDocument document) {
    ESP_LOGI(LOG_TAG, "Writing of settings to configuration file \n");

    #if GXOVnT_Settings_HAS_WIFI_SETTINGS
        WiFiSettings.writeSettingsToJson(document);
    #endif
    #if GXOVnT_Settings_HAS_BLE_TPMS_SETTINGS
        BLETPMSSettings.writeSettingsToJson(document);
    #endif
    
    ESP_LOGI(LOG_TAG, "Writing of settings completed \n");
}

bool GXOVnT_Settings::settingsHasChanges() {
    bool result = false;
    
    #if GXOVnT_Settings_HAS_WIFI_SETTINGS
        result |= WiFiSettings.getSettingsChanged();
    #endif
     #if GXOVnT_Settings_HAS_BLE_TPMS_SETTINGS
        result |= BLETPMSSettings.getSettingsChanged();
    #endif

    
    
    return result;
}