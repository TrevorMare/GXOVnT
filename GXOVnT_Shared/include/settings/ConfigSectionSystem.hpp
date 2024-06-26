/////////////////////////////////////////////////////////////////
#pragma once
#ifndef _GXOVNT_SETTINGS_CONFIGSECTIONSYSTEM_H_
#define _GXOVNT_SETTINGS_CONFIGSECTIONSYSTEM_H_
#include "shared/Definitions.h"
#include "ConfigSectionBase.hpp"

// Includes
/////////////////////////////////////////////////////////////////
#include <ArduinoJson.h>
using namespace GXOVnTLib::shared;

namespace GXOVnTLib::settings {

    class ConfigSectionSystem : ConfigSectionBase
	{
        private:
            std::string m_systemName = "GXOVnTDevice";
            std::string m_systemId = DeviceMACAddress();
            std::string m_firmwareVersion = GXOVnT_FIRMWARE_VERSION;
            std::string m_systemPassword = "";
            GXOVnT_SYSTEM_TYPE m_systemType = SYSTEM_TYPE_UN_INITIALIZED;
            bool m_systemConfigured = false;

        public:
            // Constructor
            ConfigSectionSystem() : ConfigSectionBase(sectionKeys::SectionName_SystemSettings) {};
            ConfigSectionSystem(JsonDocument &document) : ConfigSectionBase(sectionKeys::SectionName_SystemSettings, document) {}
            ~ConfigSectionSystem() {}

            // Method to write the section settings to a json document
            void writeSettingsToJson(JsonDocument &document) override {
                writeValue(document, sectionKeys::FieldName_SystemName, m_systemName);
                writeValue(document, sectionKeys::FieldName_SystemId, m_systemId);
                writeValue(document, sectionKeys::FieldName_FirmwareVersion, m_firmwareVersion);
                writeValue(document, sectionKeys::FieldName_SystemPassword, m_systemPassword);
                writeValue(document, sectionKeys::FieldName_SystemConfigured, m_systemConfigured);
                writeValue(document, sectionKeys::FieldName_SystemType, (int)m_systemType);
            };

            // Method to read the section from a json document
            void readSettingsFromJson(JsonDocument &document) override {
                m_systemName = readValueString(document, sectionKeys::FieldName_SystemName, "GXOVnTDevice");
                m_systemPassword = readValueString(document, sectionKeys::FieldName_SystemPassword, "1234admin");
                m_systemConfigured = readValueBool(document, sectionKeys::FieldName_SystemConfigured);
                m_systemType = (GXOVnT_SYSTEM_TYPE)readValueInt(document, sectionKeys::FieldName_SystemType);
            }

            // Getters
            std::string SystemName() { return m_systemName; };
            std::string SystemId() { return m_systemId; }
            std::string FirmwareVersion() { return m_firmwareVersion; }
            std::string SystemPassword() { return m_systemPassword; }
            bool SystemConfigured() { return m_systemConfigured; }
            GXOVnT_SYSTEM_TYPE SystemType() { return m_systemType; }

            // Setters
            void SystemName(std::string input) { m_systemName = input; }
            void SystemPassword(std::string input) { m_systemPassword = input; }
            void SystemConfigured(bool input) { m_systemConfigured = input; }
            void SystemType(GXOVnT_SYSTEM_TYPE input) { m_systemType = input; }
	};


}

#endif