/////////////////////////////////////////////////////////////////
#pragma once
#ifndef _GXOVNT_BLECOMMSERVICE_H_
#define _GXOVNT_BLECOMMSERVICE_H_

/////////////////////////////////////////////////////////////////
#include <BLEServer.h>
#include <BLEDevice.h>
#include <BLE2902.h>
#include "shared/Shared.h"


/////////////////////////////////////////////////////////////////
namespace GXOVnT {
    namespace services {

        class BleCommService: public BLEServerCallbacks, public BLECharacteristicCallbacks
        {
            private:
                /* data */
                BLEServer *m_bleServer = nullptr;
                BLEService *m_bleService = nullptr;
                BLECharacteristic *m_protoCharacteristic = nullptr;
                BLEAdvertising *m_bleAdvertising = nullptr;

                bool m_serverConnected = false;
                int m_serverConnectionId = -1;

                // BLEServerCallbacks
                void onConnect(BLEServer* pServer);
                // BLEServerCallbacks
                void onDisconnect(BLEServer* pServer);
                // BLECharacteristicCallbacks
                void onWrite(BLECharacteristic* pLedCharacteristic);

                void initBleServer();
                void initBleService();
                void startAdvertising();

            public:
                BleCommService();
                ~BleCommService();

                void start();
                void stop();

        };
    }
}







#endif