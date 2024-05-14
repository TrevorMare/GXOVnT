/////////////////////////////////////////////////////////////////
#pragma once
#ifndef _GXOVNT_CLIENT_SETTINGS_H
#define _GXOVNT_CLIENT_SETTINGS_H

#include "GXOVnT_Settings.h"

class GXOVnT_Client_Settings : public GXOVnT_Settings
{

    private:
        GXOVnT_WiFiSettings m_wifiSettings;

        

    protected:

        void readFromJsonDocument(JsonDocument doc);

        JsonDocument writeToJsonDocument() const;

    public:
        void setWiFiSettings(std::string ssid, std::string password);
};

#endif