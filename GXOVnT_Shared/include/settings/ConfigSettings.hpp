/////////////////////////////////////////////////////////////////
#pragma once
#ifndef _GXOVNT_SETTINGS_H_
#define _GXOVNT_SETTINGS_H_

#include "ConfigSectionSystem.hpp"
#include "ConfigSectionTestWiFi.hpp"
#include "ConfigSectionWiFi.hpp"
#include "ConfigSectionTPMS.hpp"
#include "ConfigSectionCheckFirmware.hpp"

#include "shared/Definitions.h"
#include <ArduinoJson.h>

/////////////////////////////////////////////////////////////////
// Pre-Processor definitions depending on the system type
/////////////////////////////////////////////////////////////////

#define GXOVnT_Settings_HAS_WIFI_SETTINGS true
#define GXOVnT_Settings_HAS_BLE_TPMS_SETTINGS true
#define GXOVnT_Settings_HAS_SYSTEM_SETTINGS true

/////////////////////////////////////////////////////////////////


namespace GXOVnTLib::settings {
  class ConfigSettings {
    public:

    ConfigSectionWiFi WiFiSettings;
    ConfigSectionTestWiFi TestWiFiSettings;
    ConfigSectionTPMS BLETPMSSettings;
    ConfigSectionSystem SystemSettings;
    ConfigSectionCheckFirmware CheckFirmwareSettings; 

    ConfigSettings() { };
    ConfigSettings(JsonDocument &document) { readSettingsFromJson(document); }
    ~ConfigSettings() {};

    // Method to write the section settings to a json document
    void writeSettingsToJson(JsonDocument &document) {
      ESP_LOGI(LOG_TAG, "Writing of settings to configuration file \n");

      SystemSettings.writeSettingsToJson(document);
      TestWiFiSettings.writeSettingsToJson(document);
      WiFiSettings.writeSettingsToJson(document);
      BLETPMSSettings.writeSettingsToJson(document);  
      CheckFirmwareSettings.writeSettingsToJson(document);    

    }

    // Method to read the section from a json document
    void readSettingsFromJson(JsonDocument &document) {
      ESP_LOGI(LOG_TAG, "Reading of settings from configuration file \n");

      SystemSettings.readSettingsFromJson(document);
      WiFiSettings.readSettingsFromJson(document);
      TestWiFiSettings.readSettingsFromJson(document);
      BLETPMSSettings.readSettingsFromJson(document);
      CheckFirmwareSettings.readSettingsFromJson(document);
      
    }
  };
}

#endif