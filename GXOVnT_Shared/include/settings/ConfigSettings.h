/////////////////////////////////////////////////////////////////
#pragma once
#ifndef _GXOVNT_SETTINGS_H_
#define _GXOVNT_SETTINGS_H_

#include "SettingsSection.h"
#include <ArduinoJson.h>

/////////////////////////////////////////////////////////////////
// Pre-Processor definitions depending on the system type
/////////////////////////////////////////////////////////////////
#if defined(GXOVNT_SYSTEM_TYPE) && (GXOVNT_SYSTEM_TYPE == GXOVNT_SYSTEM_TYPE_CLIENT || GXOVNT_SYSTEM_TYPE == GXOVNT_SYSTEM_TYPE_SERVER)
#define GXOVnT_Settings_HAS_WIFI_SETTINGS true
#define GXOVnT_Settings_HAS_BLE_TPMS_SETTINGS true
#define GXOVnT_Settings_HAS_SYSTEM_SETTINGS true
#elif
#define GXOVnT_Settings_HAS_WIFI_SETTINGS false
#define GXOVnT_Settings_HAS_BLE_TPMS_SETTINGS false
#define GXOVnT_Settings_HAS_SYSTEM_SETTINGS false
#endif
/////////////////////////////////////////////////////////////////


namespace GXOVnT {
    namespace settings {
        class ConfigSettings
        {
        public:

        #if GXOVnT_Settings_HAS_WIFI_SETTINGS
                WiFiSettingsSection WiFiSettings;
        #endif
        #if GXOVnT_Settings_HAS_BLE_TPMS_SETTINGS
                BleTPMSSettingsSection BLETPMSSettings;
        #endif
        #if GXOVnT_Settings_HAS_SYSTEM_SETTINGS
                SytemSettingsSection SystemSettings;
        #endif

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
}

#endif