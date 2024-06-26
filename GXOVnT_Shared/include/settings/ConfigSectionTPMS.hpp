/////////////////////////////////////////////////////////////////
#pragma once
#ifndef _GXOVNT_SETTINGS_CONFIGSECTIONTPMS_H_
#define _GXOVNT_SETTINGS_CONFIGSECTIONTPMS_H_
#include "shared/Definitions.h"
#include "ConfigSectionBase.hpp"

// Includes
/////////////////////////////////////////////////////////////////
#include <ArduinoJson.h>
using namespace GXOVnTLib::shared;

namespace GXOVnTLib::settings
{

    struct TPMSSettings
    {
    public:
        std::string SensorId = "";
        std::string CustomPosition = "";
        GXOVnT_BLE_TPMS_TYPE SensorType = BLE_TPMS_TYPE_UNKNOWN;
        GXOVnT_TPMS_POSITION SensorPosition = TPMS_POSITION_UNKNOWN;
    };

    class ConfigSectionTPMS : ConfigSectionBase
    {
    private:
        std::vector<TPMSSettings> m_tpmsSettings;

    public:
        // Constructor
        ConfigSectionTPMS() : ConfigSectionBase(sectionKeys::SectionName_TPMSSettings){};
        ConfigSectionTPMS(JsonDocument &document) : ConfigSectionBase(sectionKeys::SectionName_TPMSSettings, document) {}
        ~ConfigSectionTPMS() {}

        // Method to write the section settings to a json document
        void writeSettingsToJson(JsonDocument &document) override
        {

            document[m_sectionName].clear();

            for (size_t i = 0; i < m_tpmsSettings.size(); i++)
            {
                JsonObject settingsObject = document[m_sectionName].add<JsonObject>();
                settingsObject[sectionKeys::FieldName_TPMSCustomPosition] = m_tpmsSettings[i].CustomPosition;
                settingsObject[sectionKeys::FieldName_TPMSSensorId] = m_tpmsSettings[i].SensorId;
                settingsObject[sectionKeys::FieldName_TPMSSensorPosition] = (int)m_tpmsSettings[i].SensorPosition;
                settingsObject[sectionKeys::FieldName_TPMSSensorType] = (int)m_tpmsSettings[i].SensorType;
            }
        };

        // Method to read the section from a json document
        void readSettingsFromJson(JsonDocument &document) override
        {
            JsonArray sectionJsonObject;

            m_tpmsSettings.clear();

            // Get section json object
            sectionJsonObject = document[m_sectionName];

            for (JsonObject jsonSetting : document[m_sectionName].as<JsonArray>())
            {
                TPMSSettings bleTPMSSettings;

                const char *sensorId = jsonSetting[sectionKeys::FieldName_TPMSSensorId];
                const char *customPosition = jsonSetting[sectionKeys::FieldName_TPMSCustomPosition];
                GXOVnT_TPMS_POSITION sensorPosition = (GXOVnT_TPMS_POSITION)jsonSetting[sectionKeys::FieldName_TPMSSensorPosition].as<int>();
                GXOVnT_BLE_TPMS_TYPE tpmsSensorType = (GXOVnT_BLE_TPMS_TYPE)jsonSetting[sectionKeys::FieldName_TPMSSensorType].as<int>();

                bleTPMSSettings.CustomPosition = CharToString(customPosition);
                bleTPMSSettings.SensorId = CharToString(sensorId);
                bleTPMSSettings.SensorPosition = sensorPosition;
                bleTPMSSettings.SensorType = tpmsSensorType;

                m_tpmsSettings.push_back(bleTPMSSettings);
            }
        }

        int getConfigurationIndex(std::string sensorId)
        {
            int result = -1;
            for (size_t i = 0; i < m_tpmsSettings.size(); i++)
            {
                if (m_tpmsSettings[i].SensorId.compare(sensorId) == 0)
                {
                    result = i;
                    break;
                }
            }
            return result;
        }

        void addTPMSSettings(TPMSSettings settings)
        {
            m_tpmsSettings.push_back(settings);
        }

        void removeTPMSSettings(int removeAt)
        {
            m_tpmsSettings.erase(m_tpmsSettings.begin() + removeAt);
        }

        void removeTPMSSettings(std::string sensorId)
        {
            int removeAtIndex = getConfigurationIndex(sensorId);
            if (removeAtIndex < 0)
                return;
            removeTPMSSettings(removeAtIndex);
        }

        TPMSSettings operator[](int index)
        {
            return m_tpmsSettings[index];
        }

        std::vector<TPMSSettings> getBLETPMSSettings() const
        {
            return m_tpmsSettings;
        }
    };

}

#endif