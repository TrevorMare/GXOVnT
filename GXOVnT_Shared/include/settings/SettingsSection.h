/////////////////////////////////////////////////////////////////
#pragma once
#ifndef _GXOVNT_SETTINGS_SECTION_H_
#define _GXOVNT_SETTINGS_SECTION_H_

// Includes
/////////////////////////////////////////////////////////////////
#include <ArduinoJson.h>
#include "shared/Shared.h"
using namespace GXOVnT::shared;

namespace GXOVnT {
    namespace settings {
        
        // Base class for the settings sections
        /////////////////////////////////////////////////////////////////
        class BaseSettingsSection
        {
            protected: 
                // Value to indicate if settings section changed
                bool m_settingsChanged = false;

                // Sets the settings changed value
                void setSettingsChanged(bool settingsChanged = true, std::string settingName = "");

                virtual bool getCustomSettingsChanged();
            public:
                // Default constructor
                BaseSettingsSection();
                
                // Constructor that will read the settings from a json document
                BaseSettingsSection(JsonDocument &document);
                
                // Gets an indicator if this section values changed
                bool getSettingsChanged();

                // Method to write the section settings to a json document
                virtual void writeSettingsToJson(JsonDocument &document);
                
                // Method to read the section from a json document
                virtual void readSettingsFromJson(JsonDocument &document);

        };

        /////////////////////////////////////////////////////////////////
        // System Settings section class
        /////////////////////////////////////////////////////////////////
        class SytemSettingsSection : public BaseSettingsSection {
            private:
                const char *m_sectionName = "SystemSettings";
                const char *m_valueName_systemName = "SystemName";
                const char *m_valueName_systemId = "SystemId";
                const char *m_valueName_systemType = "SystemType";
                const char *m_valueName_firmwareVersion = "FirmwareVersion";
                const char *m_valueName_systemConfigured = "SystemConfigured";

                std::string m_systemName = "GXOVnTDevice";
                std::string m_systemId = "";
                GXOVnT_SYSTEM_TYPE m_systemType = SYSTEM_TYPE_UN_INITIALIZED;
                std::string m_firmwareVersion = "";
                bool m_systemConfigured = false;
            public:
                SytemSettingsSection();
                // Method to write the section settings to a json document
                void writeSettingsToJson(JsonDocument &document) override;
                // Method to read the section from a json document
                void readSettingsFromJson(JsonDocument &document) override;
            
                std::string SystemName();
                std::string SystemId();
                std::string FirmwareVersion();
                bool SystemConfigured();
                GXOVnT_SYSTEM_TYPE SystemType();
                void SystemName(std::string input);
                void SystemConfigured(bool input);
        };

        /////////////////////////////////////////////////////////////////
        // WiFi Settings section class
        /////////////////////////////////////////////////////////////////
        class WiFiSettingsSection : public BaseSettingsSection
        {
            private:
                const char *m_sectionName = "WiFiSettings";
                const char *m_valueName_SSID = "SSID";
                const char *m_valueName_Password = "Password";

                std::string m_SSID = "";
                std::string m_Password = "";

            public:
                WiFiSettingsSection() {}; 

                // Method to write the section settings to a json document
                void writeSettingsToJson(JsonDocument &document) override;
                
                // Method to read the section from a json document
                void readSettingsFromJson(JsonDocument &document) override;

                // Getters and setters
                void SSID(std::string input);
                void Password(std::string input);
                std::string SSID();
                std::string Password();
        };

        /////////////////////////////////////////////////////////////////
        // BLE TPMS Settings section class
        /////////////////////////////////////////////////////////////////

        struct BLE_TPMS_SETTINGS {
            private: 
                std::string m_sensorId = "";
                std::string m_customPosition = "";
                GXOVnT_BLE_TPMS_TYPE m_sensorType = BLE_TPMS_TYPE_UNKNOWN;
                GXOVnT_TPMS_POSITION m_sensorPosition = TPMS_POSITION_UNKNOWN;
                bool m_changesMade = false;

            public:
                std::string SensorId() { return m_sensorId; }
                std::string CustomPosition() { return m_customPosition; }
                GXOVnT_BLE_TPMS_TYPE SensorType() { return m_sensorType; };
                GXOVnT_TPMS_POSITION SensorPosition() { return m_sensorPosition; };   
                bool ChangesMade() { return m_changesMade; }

                void SensorId(std::string input) { 
                    if (m_sensorId.compare(input) == 0) return;
                    m_sensorId = input; 
                    m_changesMade = true;
                }
                void CustomPosition(std::string input) { 
                    if (m_customPosition.compare(input) == 0) return;
                    m_customPosition = input; 
                    m_changesMade = true;
                }
                void SensorType(GXOVnT_BLE_TPMS_TYPE input) { 
                    if (m_sensorType == input) return;
                    m_sensorType = input;
                    m_changesMade = true; 
                };
                void SensorPosition(GXOVnT_TPMS_POSITION input) { 
                    if (m_sensorPosition == input) return;
                    m_sensorPosition = input; 
                    m_changesMade = true;
                };   
                void ChangesMade(bool input) { 
                    m_changesMade = input; 
                }
        };

        class BleTPMSSettingsSection : public BaseSettingsSection {
            private:
                const char *m_sectionName = "BLETPMS";
                const char *m_valueName_SensorId = "SensorId";
                const char *m_valueName_SensorType = "SensorType";
                const char *m_valueName_SensorPosition = "SensorPosition";
                const char *m_valueName_CustomPosition = "CustomPosition";
                std::vector<BLE_TPMS_SETTINGS> m_bleTPMSSettings;
                int getConfigurationIndex(std::string sensorId);

            public:
                BleTPMSSettingsSection() {};

                bool getCustomSettingsChanged() override;

                void addTPMSSettings(BLE_TPMS_SETTINGS settings);

                void removeTPMSSettings(int removeAt);

                void removeTPMSSettings(std::string sensorId);

                BLE_TPMS_SETTINGS operator[](int index);

                std::vector<BLE_TPMS_SETTINGS> getBLETPMSSettings() const;

                // Method to write the section settings to a json document
                void writeSettingsToJson(JsonDocument &document) override;
                
                // Method to read the section from a json document
                void readSettingsFromJson(JsonDocument &document) override;
        };
 
    }
}



#endif