#include "settings/GXOVnT_Client_Settings.h"


void GXOVnT_Client_Settings::readFromJsonDocument(JsonDocument doc) {
    if (doc.isNull())
        return;

    // Read the wifi settings
    if (doc.containsKey("WiFiSettings")) {
        JsonObject wifiSettingsObject = (doc["WiFiSettings"].as<JsonObject>());

        if (!wifiSettingsObject.isNull()) {
            const char *ssid = wifiSettingsObject["SSID"];
            const char *password = wifiSettingsObject["Password"];

        }

    }

}

JsonDocument GXOVnT_Client_Settings::writeToJsonDocument() const {

}

void GXOVnT_Client_Settings::setWiFiSettings(std::string ssid, std::string password) {
    m_wifiSettings.Password = password;
    m_wifiSettings.SSID = ssid;
    _changesInSettings = true;
}