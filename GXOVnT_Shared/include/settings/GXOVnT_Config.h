/////////////////////////////////////////////////////////////////
#pragma once
#ifndef _GXOVNT_CONFIG_H_
#define _GXOVNT_CONFIG_H_

// Include deps
/////////////////////////////////////////////////////////////////
#include <SPIFFS.h>
#include "shared/GXOVnT_Shared.h"
#include "settings/GXOVnT_Settings.h"

// Class to handle the configuration
/////////////////////////////////////////////////////////////////
class GXOVnT_Config
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
        GXOVnT_Settings Settings;

        // Constructor
        GXOVnT_Config();
        
        // Deconstructor
        ~GXOVnT_Config();

        // Checks if the configration file exists
        bool configurationFileExists(); 

        // Reads the configuration from the file system
        bool readConfiguration();

        // Saves the configuration to the file system
        void saveConfiguration();
};


#endif
