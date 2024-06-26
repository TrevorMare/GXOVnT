/////////////////////////////////////////////////////////////////
#pragma once
#ifndef _GXOVNT_SETTINGS_CONFIGSECTIONWIFI_H_
#define _GXOVNT_SETTINGS_CONFIGSECTIONWIFI_H_
#include "shared/Definitions.h"
#include "ConfigSectionBase.hpp"

// Includes
/////////////////////////////////////////////////////////////////
#include <ArduinoJson.h>
using namespace GXOVnTLib::shared;

namespace GXOVnTLib::settings {

    class ConfigSectionWiFi : ConfigSectionBase
	{
        private:
            std::string m_ssid = "";
            std::string m_password = "";

        public:
            // Constructor
            ConfigSectionWiFi() : ConfigSectionBase(sectionKeys::SectionName_WiFiSettings) {};
            ConfigSectionWiFi(JsonDocument &document) : ConfigSectionBase(sectionKeys::SectionName_WiFiSettings, document) {}
            ~ConfigSectionWiFi() {}

            // Method to write the section settings to a json document
            void writeSettingsToJson(JsonDocument &document) override {
                writeValue(document, sectionKeys::FieldName_WiFiPassword, m_password);
                writeValue(document, sectionKeys::FieldName_WiFiSsid, m_ssid);
            };

            // Method to read the section from a json document
            void readSettingsFromJson(JsonDocument &document) override {
                m_password = readValueString(document, sectionKeys::FieldName_WiFiPassword);
                m_ssid = readValueString(document, sectionKeys::FieldName_WiFiSsid);
            }

            // Getters
            std::string WiFiPassword() { return m_password; };
            std::string WiFiSsid() { return m_ssid; }

            // Setters
            void WiFiPassword(std::string input) { m_password = input; }
            void WiFiSsid(std::string input) { m_ssid = input; }
	};


}

#endif