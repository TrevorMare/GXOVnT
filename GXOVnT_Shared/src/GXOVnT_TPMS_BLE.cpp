#include "GXOVnT_TPMS_BLE.h"

GXOVnT_TPMS_BLE::GXOVnT_TPMS_BLE() {}

GXOVnT_TPMS_BLE::GXOVnT_TPMS_BLE(const char *deviceId) {
  setupBLEDevice(deviceId);
}

GXOVnT_TPMS_BLE::GXOVnT_TPMS_BLE(char *deviceId) {
  setupBLEDevice(deviceId);
}

GXOVnT_TPMS_BLE::~GXOVnT_TPMS_BLE() {
  delete m_Values;
};

void GXOVnT_TPMS_BLE::setupBLEDevice(char *deviceId, int isAliveSecondsOffset) {
  std::string s_deviceId(deviceId);
  setupBLEDevice(s_deviceId.c_str(), isAliveSecondsOffset);
}

void GXOVnT_TPMS_BLE::setupBLEDevice(const char *deviceId, int isAliveSecondsOffset) {
  // Calculate the offset time that this device needs to respond in before clearing the is alive indicator
  m_isAliveMilliSecondsOffset = (isAliveSecondsOffset * 1000);
  
  if (m_Initialized == true) {
    ESP_LOGE(LOG_TAG, "The device has already been initialized and cannot be initialized again"); 
    throw new DeviceAlreadyInitializedException();
  }

  if (!setupBLEDeviceSpecific(deviceId)) {
    return;
  }

  // Temporary convert to Arduino string to be able to convert it to lower case
  String s_DeviceId = String(deviceId);
  s_DeviceId.toLowerCase();

  // Save the lower case device Id 
  m_deviceId = std::string(s_DeviceId.c_str());

  // Print the information about the instance
  ESP_LOGI(LOG_TAG, "Device with Id %s initialized \n", m_deviceId.c_str()); 

  // Finally set the initialized indicator
  m_Initialized = true;
}

enum GXOVNT_TPMS_BLE_PARSE_RESULT GXOVnT_TPMS_BLE::parseBLEAdvertisement(BLEAdvertisedDevice advertisedDevice) {
  // Parse the device specific data  
  GXOVNT_TPMS_BLE_PARSE_RESULT parseResult = parseBLEAdvertisementDeviceSpecific(advertisedDevice, m_Values);
  
  // If the sensor did receive a message known, set the last message received time
  if (parseResult == TPMS_BLE_PARSE_RESULT_NOTHING_UPDATED || parseResult == TPMS_BLE_PARSE_RESULT_UPDATED) {
    m_Values->LastMessageReceived = millis();
    m_Values->IsAlive = true;
  }

  // If the current sensor is alive, check if it is pased the expiry time and set it to false
  if (m_Values->IsAlive) {
    unsigned long offsetTime = millis() - m_isAliveMilliSecondsOffset;
    if (m_Values->LastMessageReceived < offsetTime) {
      m_Values->IsAlive = false;
    }
  }
  
  return parseResult;
}

// Override this to set up the device specific values
bool GXOVnT_TPMS_BLE::setupBLEDeviceSpecific(const char *deviceId) {
  return (strlen(deviceId) != 0);
}

// Override this to set parse the incoming advertised data
enum GXOVNT_TPMS_BLE_PARSE_RESULT GXOVnT_TPMS_BLE::parseBLEAdvertisementDeviceSpecific(BLEAdvertisedDevice advertisedDevice, GXOVnT_TPMS_BLE_VALUES *values) {
  return TPMS_BLE_PARSE_RESULT_NOT_MINE;
}