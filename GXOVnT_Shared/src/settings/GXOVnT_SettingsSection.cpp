#include "settings/GXOVnT_SettingsSection.h"

/////////////////////////////////////////////////////////////////
// Base class for the settings sections
/////////////////////////////////////////////////////////////////
GXOVnT_SettingsSection::GXOVnT_SettingsSection() {}
GXOVnT_SettingsSection::GXOVnT_SettingsSection(JsonDocument document) { readSettingsFromJson(document); }
void GXOVnT_SettingsSection::setSettingsChanged(bool settingsChanged, std::string settingName) {
    m_settingsChanged = settingsChanged;

    if (m_settingsChanged && settingName.compare("") != 0) {
        ESP_LOGI(LOG_TAG, "Configuration setting [%s] changed. \n", settingName.c_str());
    } else {
        ESP_LOGI(LOG_TAG, "Configuration setting changed. \n");
    }
}
bool GXOVnT_SettingsSection::getCustomSettingsChanged() { return false; }
bool GXOVnT_SettingsSection::getSettingsChanged() { 
    return m_settingsChanged || getCustomSettingsChanged();
}
void GXOVnT_SettingsSection::writeSettingsToJson(JsonDocument document) {}
void GXOVnT_SettingsSection::readSettingsFromJson(JsonDocument document) {}


/////////////////////////////////////////////////////////////////
// System Settings section class
/////////////////////////////////////////////////////////////////
GXOVnT_SettingsSection_System::GXOVnT_SettingsSection_System() {
    #if GXOVNT_SYSTEM_TYPE == GXOVNT_SYSTEM_TYPE_CLIENT
        m_SystemType = SYSTEM_TYPE_CLIENT;
    #endif                
    #if GXOVNT_SYSTEM_TYPE == GXOVNT_SYSTEM_TYPE_SERVER   
        m_SystemType = SYSTEM_TYPE_SERVER;
    #endif
    m_SystemId = DeviceMACAddress();
    m_FirmwareVersion = GXOVnT_FIRMWARE_VERSION;
};
std::string GXOVnT_SettingsSection_System::SystemName() { return m_SystemName; }
void GXOVnT_SettingsSection_System::SystemName(std::string input) { 
    if (m_SystemName.compare(input) == 0) return; 
    m_SystemName = input;
    setSettingsChanged(true);
}
void GXOVnT_SettingsSection_System::writeSettingsToJson(JsonDocument document) {

    JsonObject sectionJsonObject;
    // Check if the document already contains the section name object. If found, get a reference,
    // else we need to create it
    if (document.containsKey(m_sectionName)) {
        sectionJsonObject = document[m_sectionName];
    }
    else {
        sectionJsonObject = document.to<JsonObject>();
    }
    // Now write the settings
    sectionJsonObject[m_valueName_SystemName] = m_SystemName;

    // Reset the settings changed
    setSettingsChanged(false);
}
void GXOVnT_SettingsSection_System::readSettingsFromJson(JsonDocument document) {
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
void GXOVnT_SettingsSection_WiFi::writeSettingsToJson(JsonDocument document) {

    JsonObject sectionJsonObject;
    // Check if the document already contains the section name object. If found, get a reference,
    // else we need to create it
    if (document.containsKey(m_sectionName)) {
        sectionJsonObject = document[m_sectionName];
    }
    else {
        sectionJsonObject = document.to<JsonObject>();
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
void GXOVnT_SettingsSection_WiFi::SSID(std::string input) {
    if (m_SSID.compare(input) == 0) return;
    m_SSID = input;
    setSettingsChanged(true, "WiFiSettings:SSID");
}
void GXOVnT_SettingsSection_WiFi::Password(std::string input) {
    if (m_Password.compare(input) == 0) return;
    m_Password = input;
    setSettingsChanged(true, "WiFiSettings:Password");
}
std::string GXOVnT_SettingsSection_WiFi::SSID() { return m_SSID; }
std::string GXOVnT_SettingsSection_WiFi::Password() { return m_Password; }

/////////////////////////////////////////////////////////////////
// BLE TPMS Settings section class
/////////////////////////////////////////////////////////////////

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
        sectionJsonObject = document[m_sectionName].add<JsonArray>();
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
void GXOVnT_SettingsSection_BLE_TPMS::addTPMSSettings(BLE_TPMS_SETTINGS settings) {
    m_bleTPMSSettings.push_back(settings);
    setSettingsChanged(true);
};
void GXOVnT_SettingsSection_BLE_TPMS::removeTPMSSettings(int removeAt) {
    m_bleTPMSSettings.erase(m_bleTPMSSettings.begin() + removeAt);
    setSettingsChanged(true);
}
void GXOVnT_SettingsSection_BLE_TPMS::removeTPMSSettings(std::string sensorId) {
    int removeAtIndex = getConfigurationIndex(sensorId);
    if (removeAtIndex < 0) return;
    removeTPMSSettings(removeAtIndex);
}
BLE_TPMS_SETTINGS GXOVnT_SettingsSection_BLE_TPMS::operator[](int index) {
    return m_bleTPMSSettings[index];
}
std::vector<BLE_TPMS_SETTINGS> GXOVnT_SettingsSection_BLE_TPMS::getBLETPMSSettings() const {
    return m_bleTPMSSettings;
}