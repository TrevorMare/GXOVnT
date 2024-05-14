/////////////////////////////////////////////////////////////////
#pragma once
#ifndef _GXOVNT_CONFIG_H
#define _GXOVNT_CONFIG_H

// Include deps
/////////////////////////////////////////////////////////////////
#include <SPIFFS.h>
#include <ArduinoJson.h>
#include "shared/GXOVnT_Shared.h"

// Other fields
/////////////////////////////////////////////////////////////////

const char *config_file_name = "/config.json";

// Class to handle the configuration
/////////////////////////////////////////////////////////////////
class GXOVnT_Config
{
    private:
        bool m_initialized = false;

        // Reads the content of the configuration file
        String readConfigurationContent();

    public:
        // Constructor
        GXOVnT_Config();
        
        // Deconstructor
        ~GXOVnT_Config();
        
        // Initializes the configuration state
        bool initializeState();
        
        // Disposes the configuration state
        bool disposeState();
        
        // Gets a value indicating if the state has been initialized
        bool getStateInitialized() { return m_initialized; }

        // Checks if the configration file exists
        bool configurationFileExists(); 


};







#endif
