/////////////////////////////////////////////////////////////////
#pragma once
#ifndef _GXOVNT_SETTINGS_H
#define _GXOVNT_SETTINGS_H

#include <ArduinoJson.h>

/////////////////////////////////////////////////////////////////
// Common structures shared between implemented settings
/////////////////////////////////////////////////////////////////
struct GXOVnT_WiFiSettings {
    std::string SSID = "";
    std::string Password = "";
};

class GXOVnT_Settings
{
    protected:
        bool _changesInSettings = false;
        
        virtual void readFromJsonDocument(JsonDocument doc);

        virtual JsonDocument writeToJsonDocument() const;
    public:
        GXOVnT_Settings() {};
        
        GXOVnT_Settings(JsonDocument *doc) { readFromJsonDocument(*doc); }
        
        ~GXOVnT_Settings() {};
        
        // Gets a value indicating if there were changes made to the settings
        bool changesInSettings() { return _changesInSettings; }


};

#endif