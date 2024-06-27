/////////////////////////////////////////////////////////////////
#pragma once
#ifndef _GXOVNT_SETTINGS_CONFIGSECTIONCHECKFIRMWARE_H_
#define _GXOVNT_SETTINGS_CONFIGSECTIONCHECKFIRMWARE_H_
#include "shared/Definitions.h"
#include "ConfigSectionBase.hpp"

// Includes
/////////////////////////////////////////////////////////////////
#include <ArduinoJson.h>
using namespace GXOVnTLib::shared;

namespace GXOVnTLib::settings {

    class ConfigSectionCheckFirmware : ConfigSectionBase
	{
        private:
            std::string m_ssid = "";
            std::string m_password = "";
            bool m_success = false;
            int m_statusCode = 0;
            std::string m_statusMessage = "";

        public:
            // Constructor
            ConfigSectionCheckFirmware() : ConfigSectionBase(sectionKeys::SectionName_CheckFirmwareSettings) {};
            ConfigSectionCheckFirmware(JsonDocument &document) : ConfigSectionBase(sectionKeys::SectionName_CheckFirmwareSettings, document) {}
            ~ConfigSectionCheckFirmware(){}

            // Method to write the section settings to a json document
            void writeSettingsToJson(JsonDocument &document) override {
                writeValue(document, sectionKeys::FieldName_WiFiPassword, m_password);
                writeValue(document, sectionKeys::FieldName_WiFiSsid, m_ssid);
                writeValue(document, sectionKeys::FieldName_Success, m_success);
                writeValue(document, sectionKeys::FieldName_StatusCode, m_statusCode);
                writeValue(document, sectionKeys::FieldName_StatusMessage, m_statusMessage);
            };

            // Method to read the section from a json document
            void readSettingsFromJson(JsonDocument &document) override {
                m_password = readValueString(document, sectionKeys::FieldName_WiFiPassword);
                m_ssid = readValueString(document, sectionKeys::FieldName_WiFiSsid);
                m_success = readValueBool(document, sectionKeys::FieldName_Success);
                m_statusCode = readValueInt(document, sectionKeys::FieldName_StatusCode);
                m_statusMessage = readValueString(document, sectionKeys::FieldName_StatusMessage);
            }

            // Getters
            std::string WiFiPassword() { return m_password; };
            std::string WiFiSsid() { return m_ssid; }
            bool Success() { return m_success; }
            std::string StatusMessage() { return m_statusMessage; }
            int StatusCode() { return m_statusCode; }

            // Setters
            void WiFiPassword(std::string input) { m_password = input; }
            void WiFiSsid(std::string input) { m_ssid = input; }
            void Success(bool input) { m_success = input; }
            void StatusMessage(std::string input) { m_statusMessage = input; }
            void StatusCode(int input) { m_statusCode = input; }
	};
}

#endif