#include "GXOVnT_TPMS_BLE_Manager.h"

GXOVnT_TPMS_BLE_Manager::GXOVnT_TPMS_BLE_Manager() {}

GXOVnT_TPMS_BLE_Manager::~GXOVnT_TPMS_BLE_Manager() {
  delete m_BLEScan;
}

void GXOVnT_TPMS_BLE_Manager::initializeManager(bool activeScan, int scanInterval, int scanWindow, int scanTimeSeconds) {

  BLEDevice::init("");
  m_BLEScan = BLEDevice::getScan();

  m_BLEScan->setAdvertisedDeviceCallbacks(this, true);
  m_BLEScan->setActiveScan(activeScan); 
  m_BLEScan->setInterval(scanInterval);
  m_BLEScan->setWindow(scanWindow);  // less or equal setInterval value

  // Set the scan time of the ble scan
  m_scanTime = scanTimeSeconds;

  // Set the initialized value
  m_initialized = true;
}

void GXOVnT_TPMS_BLE_Manager::run() {

  // If we are in an idle mode, just return
  if (m_managerScanMode == BLE_MANAGER_SCAN_MODE_IDLE) return;

  // Check if the class has been initialized
  if (!m_initialized) { 
    #ifdef GXOVnT_DEBUG_ENABLE
      Serial.println("GXOVnT_TPMS_BLE_Manager not initialized");
    #endif
    return; 
  }
  // Now check in what mode we are before running 
  m_BLEScan->start(m_scanTime, false);
  m_BLEScan->clearResults();   // delete results fromBLEScan buffer to release memory
}

void GXOVnT_TPMS_BLE_Manager::clear() {
  
  // Delete any results from the scan types vector
  for (int index = m_TPMSScanTypeResults.size() - 1; index >= 0; --index) {
    delete m_TPMSScanTypeResults[index];
  }
  m_TPMSScanTypeResults.clear();

}

void GXOVnT_TPMS_BLE_Manager::onResult(BLEAdvertisedDevice advertisedDevice) {
  if (m_managerScanMode == BLE_MANAGER_SCAN_MODE_READ_TPMS) {

  } else if (m_managerScanMode == BLE_MANAGER_SCAN_MODE_SEARCH_TPMS) {
    processDeviceMessageScanTPMSTypes(advertisedDevice);
  }
}

void GXOVnT_TPMS_BLE_Manager::processDeviceMessageScanTPMSTypes(BLEAdvertisedDevice advertisedDevice) {
  // Lock the mutex until we have time to process the next message
  std::lock_guard<std::mutex> lck(m_processMutex);

  std::string deviceMacAddress = advertisedDevice.getAddress().toString();

  // Check if this a John Dow TPMS sensor
  enum GXOVNT_TPMS_BLE_SENSOR_TYPE deviceSensorType = TPMS_BLE_SENSOR_TYPE_UNKNOWN;

  // Check if it's a John Dow TPMS Sensor type
  std::string jdDeviceId = GXOVnT_TPMS_BLE_JohnDow::SensorTypeMatchAdvertisementDeviceMessage(advertisedDevice);

  if (jdDeviceId != "") {
    deviceSensorType = TPMS_BLE_SENSOR_TYPE_JOHN_DOW;
  }

  // If we could parse the sensor type
  if (deviceSensorType != TPMS_BLE_SENSOR_TYPE_UNKNOWN) {
    // Get the mac address of the sensor

    // Now that we can check if we have not already inserted the item before
    int scannedDeviceIdIndex = macAddressInScanTypesIndex(jdDeviceId);

    // Get the index if already inserted
    if (scannedDeviceIdIndex == -1) {

        #ifdef GXOVnT_DEBUG_ENABLE
          Serial.printf("GXOVnT_TPMS_BLE_Manager: Adding known sensor type with address [%s] and Id [%s] to scan results. \n", deviceMacAddress.c_str(), jdDeviceId.c_str());
        #endif

        BLE_MANAGER_SCAN_TYPE_RESULT *typeFoundResult = new BLE_MANAGER_SCAN_TYPE_RESULT(deviceSensorType, 
        deviceMacAddress.c_str(), jdDeviceId.c_str(), advertisedDevice.toString().c_str());
        
        m_TPMSScanTypeResults.push_back(typeFoundResult);
    }
  }
}

int GXOVnT_TPMS_BLE_Manager::macAddressInScanTypesIndex(std::string inputDeviceId) {
  // Now check if it's not already added
  int resultIndex = -1;

  std::vector<BLE_MANAGER_SCAN_TYPE_RESULT*>::iterator it;
  int index = 0;

  for (it = m_TPMSScanTypeResults.begin(); it != m_TPMSScanTypeResults.end(); it++, index++) {
    BLE_MANAGER_SCAN_TYPE_RESULT item = (*m_TPMSScanTypeResults[index]);

    bool itemExists = inputDeviceId.compare(item.DeviceId) == 0; 

    if (itemExists) {
      resultIndex = index;
      break;
    }
  }

  return resultIndex;
}

