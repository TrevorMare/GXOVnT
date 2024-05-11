#include <vector>
#ifndef _GXOVNT_TPMS_BLE_MANAGER_H
#define _GXOVNT_TPMS_BLE_MANAGER_H

#include "GXOVnT_Shared.h"
#include "GXOVnT_TPMS_BLE.h"
#include "GXOVnT_TPMS_BLE_JohnDow.h"

#include <BLEDevice.h>
#include <BLEUtils.h>
#include <BLEScan.h>
#include <BLEAdvertisedDevice.h>
#include <mutex>

enum BLE_MANAGER_SCAN_MODE {
  // The manager will scan for configured TPMS devices
  BLE_MANAGER_SCAN_MODE_READ_TPMS,
  // The manager will search for known types of TPMS devices
  BLE_MANAGER_SCAN_MODE_SEARCH_TPMS,
  // The manager will not do any work
  BLE_MANAGER_SCAN_MODE_IDLE
};

struct BLE_MANAGER_SCAN_TYPE_RESULT {
  enum GXOVNT_TPMS_BLE_SENSOR_TYPE TPMSSensorType = TPMS_BLE_SENSOR_TYPE_UNKNOWN;
  std::string MacAddress = "";
  std::string DeviceId = "";
  std::string ManufacturerData = "";

  BLE_MANAGER_SCAN_TYPE_RESULT(enum GXOVNT_TPMS_BLE_SENSOR_TYPE tpmsSensorType, std::string macAddress, std::string deviceId, std::string manufacturerData) {
    TPMSSensorType = tpmsSensorType;
    MacAddress = macAddress;
    ManufacturerData = manufacturerData;
    DeviceId = deviceId;
  }
};

class GXOVnT_TPMS_BLE_Manager: public BLEAdvertisedDeviceCallbacks {
  private:
    // Mutex to try and stop duplicate messages being parsed at the same time
    std::mutex m_processMutex;

    // List of configured TPMS Sensors to scan
    std::vector<GXOVnT_TPMS_BLE*> m_BLETPMSSensors;
    
    // List of known TPMS Sensors that match a specific type
    std::vector<BLE_MANAGER_SCAN_TYPE_RESULT*> m_TPMSScanTypeResults;

    // BLE Scan instance used for scanning device messages
    BLEScan* m_BLEScan;
    
    // A value to indicate if the manager has been initialized
    bool m_initialized;
    
    // Current scan mode of the manager
    enum BLE_MANAGER_SCAN_MODE m_managerScanMode = BLE_MANAGER_SCAN_MODE_SEARCH_TPMS;
    
    // Scan time in seconds
    int m_scanTime = 5;

    // Checks if the Mac address is defined in the list of scanned for type results
    int macAddressInScanTypesIndex(std::string macAddress);

    // Handles an incoming BLE Advertised Device message
    void onResult(BLEAdvertisedDevice advertisedDevice);
    
    // Processes the incoming device message when scanning for known TPMS device types
    void processDeviceMessageScanTPMSTypes(BLEAdvertisedDevice advertisedDevice);

  public:
    // Constructs a new instance of the BLE TPMS Manager class
    GXOVnT_TPMS_BLE_Manager();
    
    // Disposes the BLE TPMS Manager class
    ~GXOVnT_TPMS_BLE_Manager();
    
    // Initializes the manager object and creates the BLE Scan internals
    void initializeManager(bool activeScan = true, int scanInterval = 100, int scanWindow = 99, int scanTimeSeconds = 5);

    // Should be called in the main loop to start a scan and process results
    void run();

    // Should be called in the main loop after the results has been processed
    void clear();

    std::vector<BLE_MANAGER_SCAN_TYPE_RESULT*> getScanTypeResults() { return m_TPMSScanTypeResults; }

    // Gets the current mode of the Scan Manager
    enum BLE_MANAGER_SCAN_MODE getManagerScanMode() { return m_managerScanMode; }
    
    // Sets the scan mode of the manager
    void setManagerScanMode(enum BLE_MANAGER_SCAN_MODE managerScanMode) { m_managerScanMode = managerScanMode; }

};


#endif