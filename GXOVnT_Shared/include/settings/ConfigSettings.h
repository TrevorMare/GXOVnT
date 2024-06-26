/////////////////////////////////////////////////////////////////
#pragma once
#ifndef _GXOVNT_SETTINGS_H_
#define _GXOVNT_SETTINGS_H_

#include "SettingsSection.h"
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

    WiFiSettingsSection WiFiSettings;
    TestWiFiSettingsSection TestWiFiSettings;
    BleTPMSSettingsSection BLETPMSSettings;
    SytemSettingsSection SystemSettings;

    ConfigSettings() { };
    ConfigSettings(JsonDocument &document) { readSettingsFromJson(document); }
    ~ConfigSettings() {};
              
    // Gets a value indicating if there were changes made to the settings
    bool settingsHasChanges();

    // Method to write the section settings to a json document
    void writeSettingsToJson(JsonDocument &document);

    // Method to read the section from a json document
    void readSettingsFromJson(JsonDocument &document);
  };
}

#endif