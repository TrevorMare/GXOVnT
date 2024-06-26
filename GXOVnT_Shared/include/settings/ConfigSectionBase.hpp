/////////////////////////////////////////////////////////////////
#pragma once
#ifndef _GXOVNT_SETTINGS_CONFIGSECTIONBASE_H_
#define _GXOVNT_SETTINGS_CONFIGSECTIONBASE_H_
#include "shared/Definitions.h"

// Includes
/////////////////////////////////////////////////////////////////
#include <ArduinoJson.h>
using namespace GXOVnTLib::shared;

namespace GXOVnTLib::settings {

	namespace sectionKeys {
		static const std::string SectionName_SystemSettings = "systemSettings";
		static const std::string SectionName_WiFiSettings = "wifiSettings";
		static const std::string SectionName_TestWiFiSettings = "testWiFiSettings";
		static const std::string SectionName_TPMSSettings = "tpmsSettings";

		static const std::string FieldName_SystemName = "systemName";
		static const std::string FieldName_SystemId = "systemId";
		static const std::string FieldName_FirmwareVersion = "firmwareVersion";
		static const std::string FieldName_SystemPassword = "systemPassword";
		static const std::string FieldName_SystemType = "systemType";
		static const std::string FieldName_SystemConfigured = "systemConfigured";
		static const std::string FieldName_WiFiSsid = "ssid";
		static const std::string FieldName_WiFiPassword = "password";
		static const std::string FieldName_WiFiTested = "tested";
		static const std::string FieldName_WiFiTestSuccess = "success";
		static const std::string FieldName_StatusCode = "statusCode";
		static const std::string FieldName_StatusMessage = "statusMessage";
		static const std::string FieldName_WiFiTestOnNextBoot = "testOnNextBoot";

		static const std::string FieldName_TPMSSensorId = "sensorId";
		static const std::string FieldName_TPMSSensorPosition = "sensorPosition";
		static const std::string FieldName_TPMSSensorType = "sensorType";
		static const std::string FieldName_TPMSCustomPosition = "customPosition";
	}


    class ConfigSectionBase
	{
		protected:
			// Value to indicate if settings section changed
			std::string m_sectionName;

			std::string readValueString(JsonDocument &document, std::string settingName, std::string defaultIfNullOrEmpty = "") {
				if (document.containsKey(m_sectionName)) {
					JsonObject sectionJsonObject = document[m_sectionName];
					const char *value = sectionJsonObject[settingName];
					std::string result = CharToString(value);
					if (result.compare("") == 0) return defaultIfNullOrEmpty;
					return result;
				} 
				return defaultIfNullOrEmpty;
			}

			bool readValueBool(JsonDocument &document, std::string settingName) {
				if (document.containsKey(m_sectionName)) {
					JsonObject sectionJsonObject = document[m_sectionName];
					return sectionJsonObject[settingName];
				} 
				return false;
			}

			int readValueInt(JsonDocument &document, std::string settingName) {
				if (document.containsKey(m_sectionName)) {
					JsonObject sectionJsonObject = document[m_sectionName];
					return sectionJsonObject[settingName];
				} 
				return 0;
			}

			void writeValue(JsonDocument &document, std::string settingName, std::string value) {
				document[m_sectionName][settingName] = value;
			}

			void writeValue(JsonDocument &document, std::string settingName, bool value) {
				document[m_sectionName][settingName] = value;
			}
			
			void writeValue(JsonDocument &document, std::string settingName, int value) {
				document[m_sectionName][settingName] = value;
			}

		public:
			// Default constructor
			ConfigSectionBase(std::string sectionName) {
				m_sectionName = sectionName;
			};

			// Constructor that will read the settings from a json document
			ConfigSectionBase(std::string sectionName, JsonDocument &document) {
				m_sectionName = sectionName;
				readSettingsFromJson(document);
			}
			~ConfigSectionBase() {}

			// Method to write the section settings to a json document
			virtual void writeSettingsToJson(JsonDocument &document) {};

			// Method to read the section from a json document
			virtual void readSettingsFromJson(JsonDocument &document) {};
	};


}

#endif