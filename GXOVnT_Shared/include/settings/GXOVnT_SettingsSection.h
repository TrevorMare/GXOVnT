/////////////////////////////////////////////////////////////////
#pragma once
#ifndef _GXOVNT_SETTINGS_SECTION_H_
#define _GXOVNT_SETTINGS_SECTION_H_

// Includes
/////////////////////////////////////////////////////////////////
#include <ArduinoJson.h>
#include "shared/GXOVnT_Shared.h"

// Base class for the settings sections
/////////////////////////////////////////////////////////////////
class GXOVnT_SettingsSection
{
    protected: 
        // Value to indicate if settings section changed
        bool m_settingsChanged = false;

        // Sets the settings changed value
        void setSettingsChanged(bool settingsChanged = true, std::string settingName = "") {
            m_settingsChanged = settingsChanged;

            if (m_settingsChanged && settingName.compare("") != 0) {
                ESP_LOGI(LOG_TAG, "Configuration setting [%s] changed. \n", settingName.c_str());
            } else {
                ESP_LOGI(LOG_TAG, "Configuration setting changed. \n");
            }
        }

        virtual bool getCustomSettingsChanged() { return false; }
    public:
        // Default constructor
        GXOVnT_SettingsSection() {}
        
        // Constructor that will read the settings from a json document
        GXOVnT_SettingsSection(JsonDocument document) { readSettingsFromJson(document); }
        
        // Gets an indicator if this section values changed
        bool getSettingsChanged() { 
            return m_settingsChanged || getCustomSettingsChanged(); 
        }

        // Method to write the section settings to a json document
        virtual void writeSettingsToJson(JsonDocument document) {};
        
        // Method to read the section from a json document
        virtual void readSettingsFromJson(JsonDocument document) {};

};

/////////////////////////////////////////////////////////////////
// WiFi Settings section class
/////////////////////////////////////////////////////////////////
class GXOVnT_SettingsSection_WiFi : public GXOVnT_SettingsSection
{
    private:
        const char *m_sectionName = "WiFiSettings";
        const char *m_valueName_SSID = "SSID";
        const char *m_valueName_Password = "Password";

        std::string m_SSID = "";
        std::string m_Password = "";

    public:
        GXOVnT_SettingsSection_WiFi() : GXOVnT_SettingsSection::GXOVnT_SettingsSection() {};
        GXOVnT_SettingsSection_WiFi(JsonDocument document) : GXOVnT_SettingsSection::GXOVnT_SettingsSection(document) {};
        ~GXOVnT_SettingsSection_WiFi() {};

        // Method to write the section settings to a json document
        void writeSettingsToJson(JsonDocument document) override;
        
        // Method to read the section from a json document
        void readSettingsFromJson(JsonDocument document) override;

        // Getters and setters
        void setSSID(std::string input);
        void setPassword(std::string input);
        std::string getSSID() { return m_SSID; }
        std::string getPassword() { return m_Password; }

};

void GXOVnT_SettingsSection_WiFi::writeSettingsToJson(JsonDocument document) {

    JsonObject sectionJsonObject;
    // Check if the document already contains the section name object. If found, get a reference,
    // else we need to create it
    if (document.containsKey(m_sectionName)) {
        sectionJsonObject = document[m_sectionName];
    }
    else {
        sectionJsonObject = document.createNestedObject(m_sectionName);
    }
    // Now write the settings
    sectionJsonObject[m_valueName_Password] = m_Password;
    sectionJsonObject[m_SSID] = m_Password;

    // Reset the settings changed
    setSettingsChanged(false);
}

void GXOVnT_SettingsSection_WiFi::readSettingsFromJson(JsonDocument document) {
    JsonObject sectionJsonObject;
    
    // Reset the settings changed
    setSettingsChanged(false);

    // Check if the document already contains the section name object. If found, get a reference,
    // else we need to create it
    if (document.containsKey(m_sectionName)) {
        // Get section json object
        sectionJsonObject = document[m_sectionName];

        const char *settingsSSID = sectionJsonObject[m_valueName_SSID];
        const char *settingsPassword = sectionJsonObject[m_valueName_Password];

        m_SSID = CharToString(settingsSSID);
        m_Password = CharToString(settingsPassword);
    }
    else {
        m_SSID = "";
        m_Password = "";
    }
}

void GXOVnT_SettingsSection_WiFi::setSSID(std::string input) {
    if (m_SSID.compare(input) == 0) return;
    m_SSID = input;
    setSettingsChanged(true, "WiFiSettings:SSID");
}

void GXOVnT_SettingsSection_WiFi::setPassword(std::string input) {
    if (m_Password.compare(input) == 0) return;
    m_Password = input;
    setSettingsChanged(true, "WiFiSettings:Password");
}
/////////////////////////////////////////////////////////////////

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

class GXOVnT_SettingsSection_BLE_TPMS : public GXOVnT_SettingsSection {
    private:
        const char *m_sectionName = "BLETPMS";
        const char *m_valueName_SensorId = "SensorId";
        const char *m_valueName_SensorType = "SensorType";
        const char *m_valueName_SensorPosition = "SensorPosition";
        const char *m_valueName_CustomPosition = "CustomPosition";
        std::vector<BLE_TPMS_SETTINGS> m_bleTPMSSettings;
        int getConfigurationIndex(std::string sensorId);

    public:
        GXOVnT_SettingsSection_BLE_TPMS() : GXOVnT_SettingsSection::GXOVnT_SettingsSection() {};
        GXOVnT_SettingsSection_BLE_TPMS(JsonDocument document) : GXOVnT_SettingsSection::GXOVnT_SettingsSection(document) {};
        ~GXOVnT_SettingsSection_BLE_TPMS() {};

        bool getCustomSettingsChanged() override;

        void addTPMSSettings(BLE_TPMS_SETTINGS settings) {
            m_bleTPMSSettings.push_back(settings);
            setSettingsChanged(true);
        };

        void removeTPMSSettings(int removeAt) {
            m_bleTPMSSettings.erase(m_bleTPMSSettings.begin() + removeAt);
            setSettingsChanged(true);
        }

        void removeTPMSSettings(std::string sensorId) {
            int removeAtIndex = getConfigurationIndex(sensorId);
            if (removeAtIndex < 0) return;
            removeTPMSSettings(removeAtIndex);
        }

        BLE_TPMS_SETTINGS operator[](int index) {
            return m_bleTPMSSettings[index];
        }

        std::vector<BLE_TPMS_SETTINGS> getBLETPMSSettings() const {
            return m_bleTPMSSettings;
        }

        // Method to write the section settings to a json document
        void writeSettingsToJson(JsonDocument document) override;
        
        // Method to read the section from a json document
        void readSettingsFromJson(JsonDocument document) override;
};

int GXOVnT_SettingsSection_BLE_TPMS::getConfigurationIndex(std::string sensorId) {
    int result = -1;
    for (size_t i = 0; i < m_bleTPMSSettings.size(); i++) {
        if (m_bleTPMSSettings[i].SensorId().compare(sensorId) == 0) {
            result = i;
            break;
        }
    }
    return result;
}

bool GXOVnT_SettingsSection_BLE_TPMS::getCustomSettingsChanged() {
    bool result = false;
    for (size_t i = 0; i < m_bleTPMSSettings.size(); i++) {
        if (m_bleTPMSSettings[i].ChangesMade()) {
            result = true;
            break;
        }
    }
    return result;
}

void GXOVnT_SettingsSection_BLE_TPMS::writeSettingsToJson(JsonDocument document) {
    JsonArray sectionJsonObject;
    // Check if the document already contains the section name object. If found, get a reference,
    // else we need to create it
    if (document.containsKey(m_sectionName)) {
        sectionJsonObject = document[m_sectionName];
    }
    else {
        sectionJsonObject = document.createNestedArray(m_sectionName);
    }

    for (size_t i = 0; i < m_bleTPMSSettings.size(); i++)
    {
        JsonObject settingsObject;
        settingsObject[m_valueName_CustomPosition] = m_bleTPMSSettings[i].CustomPosition();
        settingsObject[m_valueName_SensorId] = m_bleTPMSSettings[i].SensorId();
        settingsObject[m_valueName_SensorPosition] = (int)m_bleTPMSSettings[i].SensorPosition();
        settingsObject[m_valueName_SensorType] = (int)m_bleTPMSSettings[i].SensorType();
        sectionJsonObject.add(settingsObject);

        // Reset the changes made
        m_bleTPMSSettings[i].ChangesMade(false);
    }

    // Reset the settings changed
    setSettingsChanged(false);
}

void GXOVnT_SettingsSection_BLE_TPMS::readSettingsFromJson(JsonDocument document) {
    // Reset the settings changed
    setSettingsChanged(false);

    JsonArray sectionJsonObject;
    m_bleTPMSSettings.clear();
    
    // Check if the document already contains the section name object. If found, get a reference,
    // else we need to create it
    if (document.containsKey(m_sectionName)) {
        // Get section json object
        sectionJsonObject = document[m_sectionName];
        for (JsonObject jsonSetting : document[m_sectionName].as<JsonArray>()) {
            BLE_TPMS_SETTINGS bleTPMSSettings;

            const char *sensorId = jsonSetting[m_valueName_SensorId];
            const char *customPosition = jsonSetting[m_valueName_CustomPosition];
            GXOVnT_TPMS_POSITION sensorPosition = (GXOVnT_TPMS_POSITION)jsonSetting[m_valueName_SensorPosition].as<int>();
            GXOVnT_BLE_TPMS_TYPE tpmsSensorType = (GXOVnT_BLE_TPMS_TYPE)jsonSetting[m_valueName_SensorType].as<int>();

            bleTPMSSettings.CustomPosition(CharToString(customPosition));
            bleTPMSSettings.SensorId(CharToString(sensorId));
            bleTPMSSettings.SensorPosition(sensorPosition);
            bleTPMSSettings.SensorType(tpmsSensorType);

            bleTPMSSettings.ChangesMade(false);
            m_bleTPMSSettings.push_back(bleTPMSSettings);
        }
    }
}


#endif