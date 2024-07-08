// Root class to provide functionality to the calling library. 
// This provides the setup and run 
/////////////////////////////////////////////////////////////////
#pragma once
#ifndef _GXOVNT_H_
#define _GXOVNT_H_

#include "shared/Definitions.h"
#include "services/CommService.h"
#include "services/FirmwareUpdateService.hpp"
#include "settings/Config.h"

// Forward declaration of the services
/////////////////////////////////////////////////////////////////
namespace GXOVnTLib::services {
    class CommService;
    
}
namespace GXOVnTLib::settings {
    class Config;
}


class GXOVnTRoot {
    private:
        void testWiFiConnection();
        void downloadLatestFirmwareList();
        void installFirmware();
    public:
        GXOVnTLib::services::CommService *commService = nullptr;
        GXOVnTLib::services::FirmwareUpdateService *firmwareUpdateService = nullptr;
        GXOVnTLib::settings::Config *config = nullptr;
        void Initialize();
        GXOVnTRoot();
};

extern GXOVnTRoot GXOVnT;

#endif

