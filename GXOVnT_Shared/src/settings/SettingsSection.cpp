#include "settings/SettingsSection.h"
using namespace GXOVnT::settings;

/////////////////////////////////////////////////////////////////
// Base class for the settings sections
/////////////////////////////////////////////////////////////////
BaseSettingsSection::BaseSettingsSection() {}
BaseSettingsSection::BaseSettingsSection(JsonDocument &document) { readSettingsFromJson(document); }
void BaseSettingsSection::setSettingsChanged(bool settingsChanged, std::string settingName) {
    m_settingsChanged = settingsChanged;
    if (m_settingsChanged) {
        if (settingName == "") {
            ESP_LOGI(LOG_TAG, "Configuration setting [%s] changed.", settingName.c_str());
        } else {
            ESP_LOGI(LOG_TAG, "Configuration setting changed.");
        }
    }
}
bool BaseSettingsSection::getCustomSettingsChanged() { return false; }
bool BaseSettingsSection::getSettingsChanged() { 
    return m_settingsChanged || getCustomSettingsChanged();
}
void BaseSettingsSection::writeSettingsToJson(JsonDocument &document) {}
void BaseSettingsSection::readSettingsFromJson(JsonDocument &document) {}

/////////////////////////////////////////////////////////////////
// System Settings section class
/////////////////////////////////////////////////////////////////
SytemSettingsSection::SytemSettingsSection() {
    #if GXOVNT_SYSTEM_TYPE == GXOVNT_SYSTEM_TYPE_CLIENT
        m_SystemType = SYSTEM_TYPE_CLIENT;
    #endif                
    #if GXOVNT_SYSTEM_TYPE == GXOVNT_SYSTEM_TYPE_SERVER   
        m_SystemType = SYSTEM_TYPE_SERVER;
    #endif
    m_SystemId = DeviceMACAddress();
    m_FirmwareVersion = GXOVnT_FIRMWARE_VERSION;
};
std::string SytemSettingsSection::SystemName() { return m_SystemName; }
std::string SytemSettingsSection::SystemId() { return m_SystemId; }
std::string SytemSettingsSection::FirmwareVersion() { return m_FirmwareVersion; }
GXOVnT_SYSTEM_TYPE SytemSettingsSection::SystemType() { return m_SystemType; }
void SytemSettingsSection::SystemName(std::string input) { 
    if (m_SystemName.compare(input) == 0) return; 
    m_SystemName = input;
    setSettingsChanged(true, "SystemSettings:SystemName");;
}
void SytemSettingsSection::writeSettingsToJson(JsonDocument &document) {
    document[m_sectionName][m_valueName_SystemName] = m_SystemName;
    // Reset the settings changed
    setSettingsChanged(false);
}
void SytemSettingsSection::readSettingsFromJson(JsonDocument &document) {
    JsonObject sectionJsonObject;
    
    // Reset the settings changed
    setSettingsChanged(false);

    // Check if the document already contains the section name object. If found, get a reference,
    // else we need to create it
    if (document.containsKey(m_sectionName)) {
        // Get section json object
        sectionJsonObject = document[m_sectionName];
        const char *settingsSystemName = sectionJsonObject[m_valueName_SystemName];
        m_SystemName = CharToString(settingsSystemName);
    }
}

/////////////////////////////////////////////////////////////////
// WiFi Settings section class
/////////////////////////////////////////////////////////////////
void WiFiSettingsSection::writeSettingsToJson(JsonDocument &document) {
    // Now write the settings
    document[m_sectionName][m_valueName_Password] = m_Password;
    document[m_sectionName][m_valueName_SSID] = m_SSID;
    // Reset the settings changed
    setSettingsChanged(false);
}
void WiFiSettingsSection::readSettingsFromJson(JsonDocument &document) {
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
void WiFiSettingsSection::SSID(std::string input) {
    if (m_SSID.compare(input) == 0) return;
    m_SSID = input;
    setSettingsChanged(true, "WiFiSettings:SSID");
}
void WiFiSettingsSection::Password(std::string input) {
    if (m_Password.compare(input) == 0) return;
    m_Password = input;
    setSettingsChanged(true, "WiFiSettings:Password");
}
std::string WiFiSettingsSection::SSID() { return m_SSID; }
std::string WiFiSettingsSection::Password() { return m_Password; }

/////////////////////////////////////////////////////////////////
// BLE TPMS Settings section class
/////////////////////////////////////////////////////////////////

int BleTPMSSettingsSection::getConfigurationIndex(std::string sensorId) {
    int result = -1;
    for (size_t i = 0; i < m_bleTPMSSettings.size(); i++) {
        if (m_bleTPMSSettings[i].SensorId().compare(sensorId) == 0) {
            result = i;
            break;
        }
    }
    return result;
}
bool BleTPMSSettingsSection::getCustomSettingsChanged() {
    bool result = false;
    for (size_t i = 0; i < m_bleTPMSSettings.size(); i++) {
        if (m_bleTPMSSettings[i].ChangesMade()) {
            result = true;
            break;
        }
    }
    return result;
}
void BleTPMSSettingsSection::writeSettingsToJson(JsonDocument &document) {
    for (size_t i = 0; i < m_bleTPMSSettings.size(); i++)
    {
        JsonObject settingsObject = document[m_sectionName].add<JsonObject>();
        settingsObject[m_valueName_CustomPosition] = m_bleTPMSSettings[i].CustomPosition();
        settingsObject[m_valueName_SensorId] = m_bleTPMSSettings[i].SensorId();
        settingsObject[m_valueName_SensorPosition] = (int)m_bleTPMSSettings[i].SensorPosition();
        settingsObject[m_valueName_SensorType] = (int)m_bleTPMSSettings[i].SensorType();
        // Reset the changes made
        m_bleTPMSSettings[i].ChangesMade(false);
    }

    // Reset the settings changed
    setSettingsChanged(false);
}
void BleTPMSSettingsSection::readSettingsFromJson(JsonDocument &document) {
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
void BleTPMSSettingsSection::addTPMSSettings(BLE_TPMS_SETTINGS settings) {
    m_bleTPMSSettings.push_back(settings);
    setSettingsChanged(true);
};
void BleTPMSSettingsSection::removeTPMSSettings(int removeAt) {
    m_bleTPMSSettings.erase(m_bleTPMSSettings.begin() + removeAt);
    setSettingsChanged(true);
}
void BleTPMSSettingsSection::removeTPMSSettings(std::string sensorId) {
    int removeAtIndex = getConfigurationIndex(sensorId);
    if (removeAtIndex < 0) return;
    removeTPMSSettings(removeAtIndex);
}
BLE_TPMS_SETTINGS BleTPMSSettingsSection::operator[](int index) {
    return m_bleTPMSSettings[index];
}
std::vector<BLE_TPMS_SETTINGS> BleTPMSSettingsSection::getBLETPMSSettings() const {
    return m_bleTPMSSettings;
}