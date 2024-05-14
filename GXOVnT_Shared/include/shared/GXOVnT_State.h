/////////////////////////////////////////////////////////////////
#pragma once
#ifndef _GXOVNT_STATE_H
#define _GXOVNT_STATE_H

/////////////////////////////////////////////////////////////////
#include "GXOVnT_Shared.h"
/////////////////////////////////////////////////////////////////

/////////////////////////////////////////////////////////////////
// Enum for the state object
enum GXOVnT_States { 
    // Unknown state
    GXOVNT_STATES_UNKNOWN, 
    // Initial state is used for when the system starts up
    GXOVNT_STATES_INITIAL_STATE,
    // Listen commands is the state used for when the system should listen to incoming commands and act on them
    GXOVNT_STATES_LISTEN_COMMANDS,
    // State to provision the WiFi connection via BLE
    GXOVNT_STATES_PROVISION_WIFI_VIA_BLE,
    // State to download the configuration of the device
    GXOVNT_STATES_DOWNLOAD_CONFIG,
    // State to upload the configuration of the device
    GXOVNT_STATES_UPLOAD_CONFIG,
    // State to set the configuration of the device
    GXOVNT_STATES_SET_CONFIG,
    // State to download the latest firmware to the system
    GXOVNT_STATES_DOWNLOAD_FIRMWARE,
    // State to install the latest firmware to the system
    GXOVNT_STATES_INSTALL_FIRMWARE,
    // State to scan known types of ble devices
    GXOVNT_STATES_BLE_SCAN_TYPES,
    // State to monitor and send known device messages
    GXOVNT_STATES_BLE_MONITOR
};

// Base state class that serves as the core of the various states the system can be in
class GXOVnT_State
{
    private:
        enum GXOVnT_States m_stateValue = GXOVNT_STATES_UNKNOWN;

    protected:
        

    public:
        GXOVnT_State(enum GXOVnT_States stateValue) {
            m_stateValue = stateValue;
        }
       
        ~GXOVnT_State();

        void initState(...);
        void deinitState();
        void run();
        
        enum GXOVnT_States getStateEnum() {
            return m_stateValue;
        }

};




#endif