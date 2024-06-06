/////////////////////////////////////////////////////////////////
#pragma once
#ifndef _GXOVNT_CONFIG_H_
#define _GXOVNT_CONFIG_H_

// Include deps
/////////////////////////////////////////////////////////////////
#include <SPIFFS.h>
#include "shared/Definitions.h"
#include "settings/ConfigSettings.h"


namespace GXOVnTLib::settings {
/// @brief Class to handle the configuration
class Config
{
	private:
		const char *config_file_name = "/config.json";
		bool m_SPIFFS_open = false;

		// Reads the content of the configuration file content
		String readConfigurationFromFileSystem();

		void writeConfigurationToFileSystem(String content);
		
		void deleteConfigurationFile();
		
		bool openSPIFFS();

		bool closeSPIFFS();

	public:
		ConfigSettings Settings;

		// Constructor
		Config();
		
		// Deconstructor
		~Config();

		// Checks if the configration file exists
		bool configurationFileExists(); 

		// Reads the configuration from the file system
		bool readConfiguration();

		// Saves the configuration to the file system
		void saveConfiguration();
};


}



#endif
