#include "GXOVnT_TPMS_BLE_JohnDow.h"
using namespace GXOVnTLib::shared;

bool GXOVnT_TPMS_BLE_JohnDow::setupBLEDeviceSpecific(const char *deviceId) {
  // Must be 5 characters long
  if (strlen(deviceId) != 5) {
    return false;
  }
  // Set the sensor type
  setBLESensorType(TPMS_BLE_SENSOR_TYPE_JOHN_DOW);

  // Get the last 4 chars of the hex string and convert those into int values for comparison
  // later when needed to check if the message belongs to this sensor
  std::string s_deviceId(deviceId);
  m_DeviceIdPart1 = ConvertHexToByte(s_deviceId.substr(1, 2));
  m_DeviceIdPart2 = ConvertHexToByte(s_deviceId.substr(3, 2));

  ESP_LOGI(LOG_TAG, "Device Id parts [%s] translates to %d %d \n", s_deviceId.substr(1, 4).c_str(), m_DeviceIdPart1, m_DeviceIdPart2);

  return true;
}

enum GXOVNT_TPMS_BLE_PARSE_RESULT GXOVnT_TPMS_BLE_JohnDow::parseBLEAdvertisementDeviceSpecific(BLEAdvertisedDevice advertisedDevice, GXOVnT_TPMS_BLE_VALUES *values) {
  
  // If the sensor has not been initialized
  if (!getInitialized()) {
    return TPMS_BLE_PARSE_RESULT_NOT_INITIALIZED;
  }
  
  // Get the payload from the message
  int payloadLength = advertisedDevice.getPayloadLength();
  uint8_t *payload = advertisedDevice.getPayload();

  // Check the length of the payload
  if (payloadLength <= 27) {
    return TPMS_BLE_PARSE_RESULT_NOT_MINE;  
  }
  
  // Check if the address positions match that of the setup values
  if (payload[25] != m_DeviceIdPart1 || payload[26] != m_DeviceIdPart2) {
    return TPMS_BLE_PARSE_RESULT_NOT_MINE;  
  }

  if (m_debugPrintMode != DEBUG_PRINT_NONE) {
    serialPrintDebug(payload, payloadLength);
  }

  // Check that the message contains the correct amount of data
  if (payloadLength < 52) {
    return TPMS_BLE_PARSE_RESULT_NOTHING_UPDATED;  
  }

  // Get the advertised values
  float newPressure = payload[47] * 0.3625;
  float newTemperature = payload[48] - 40.0;
  float newBatteryVoltage = 0.0;
  int newSignalStrength = advertisedDevice.getRSSI();
  
  bool changesMade = false;

  // Compare and set each of the values retrieved
  if (!CompareFloatWithFixedPrecision(values->PressureValue, newPressure)) {
    values->PressureValue = newPressure;
    changesMade = true;
  }
  
  if (!CompareFloatWithFixedPrecision(values->TemperatureValue, newTemperature)) {
    values->TemperatureValue = newTemperature;
    changesMade = true;
  }

  if (!CompareFloatWithFixedPrecision(values->BatteryVoltage, newBatteryVoltage)) {
    values->BatteryVoltage = newBatteryVoltage;
    changesMade = true;
  }

  // Don't check for changes on the signal strength, this is not something we care about
  values->SignalStrength = newSignalStrength;
  
  // The results of this message is created once per scan and there may be multiple
  // messages for that scan. The changes made indicator must include previous 
  // changes made and when the scan is complete, the indicator must be reset.
  values->ValuesChanged = values->ValuesChanged || changesMade;

  // If changes were not made and the values stayed the same, return nothing updated
  if (!changesMade) {
    return TPMS_BLE_PARSE_RESULT_NOTHING_UPDATED;
  }

  // Return device updated
  return TPMS_BLE_PARSE_RESULT_UPDATED;
}

std::string GXOVnT_TPMS_BLE_JohnDow::SensorTypeMatchAdvertisementDeviceMessage(BLEAdvertisedDevice advertisedDevice) {
  // First check the service Id's. This will tell us if this sensor is a beacon type
  uint8_t uuidServiceIdCount = advertisedDevice.getServiceUUIDCount();

  // Should have at least 8 service Id's
  if (uuidServiceIdCount <= 8)  {
    return "";
  }
    
  uint8_t numberOfRequiredFound = 0;
  
  // Count the number of known service Id's
  for (int i = 0; i < uuidServiceIdCount; i++) {
    std::string serviceId(advertisedDevice.getServiceUUID(i).toString().c_str());
    if (serviceId.find("00005898") != 0 || serviceId.find("0000218a") != 0 ||
        serviceId.find("00002d14") != 0 || serviceId.find("00000014") != 0 ||
        serviceId.find("0000178a") != 0 || serviceId.find("00006000") != 0 ||
        serviceId.find("0000b23e") != 0 || serviceId.find("00007786") != 0) {
      numberOfRequiredFound++;
    }
  }

  if (numberOfRequiredFound < 8) { 
    return "";
  }

  // Now get the raw payload length of the device, if it's less than 36 this is not the sensor we are looking for
  const size_t payloadLength = advertisedDevice.getPayloadLength();
  if (payloadLength < 36) {
    return "";
  }
  // Compare the values that we know are the same for all the devices
  const uint8_t *payload = advertisedDevice.getPayload();
  if (payload[29] == 156 && payload[30] == 21 && payload[31] == 3 && payload[32] == 152 && payload[33] == 88 && payload[34] == 138) {
    std::string deviceId_Part_0 = ConvertByteToHex(payload[25]);
    std::string deviceId_Part_1 = ConvertByteToHex(payload[26]);

    return "1" + deviceId_Part_0 + deviceId_Part_1;
  }
  return "";
}

void GXOVnT_TPMS_BLE_JohnDow::serialPrintDebug(uint8_t* payloadRaw, size_t payloadLength) {
  #ifdef GXOVnT_DEBUG_ENABLE
    
    enum GXOVNT_TPMS_BLE_DEBUG_PRINT printOption = getDebugPrintMode();
    if (printOption == DEBUG_PRINT_NONE) { return; }

    int printStartPosition = 0;
    // Calculate the start position of the print characters
    if (printOption == DEBUG_PRINT_TRUNCATED || printOption == DEBUG_PRINT_KNOWN) {
      printStartPosition = 27;
    }

    // For each of the values in the payload
    for (int ip = printStartPosition; ip < payloadLength; ip++) {
      // If the print option is for known or full, just output the values
      if (printOption == DEBUG_PRINT_KNOWN || printOption == DEBUG_PRINT_FULL) {
        PrintDebugFixed(payloadRaw[ip]);
      } else {
        // This will be a truncated payload string and we only need to print certain positions
        // Check the position what needs to be printed
        bool printPosition = false;
        bool printElipses = false;
        
        if (ip == 27 || ip == 28 || ip == 38 || ip == 47 || ip >= 48) {
          printPosition = true;
        } 
        else if (ip == 29 || ip == 39) {
          printElipses = true;
        }

        // If we need to print the position value, do so
        if (printPosition == true) {
          // Check to see if we need to print flags
          if (ip == 38 || ip == 50 || ip == 51) {
            PrintDebugHex(payloadRaw[ip]);
          } else {
              PrintDebugFixed(payloadRaw[ip]);
          }

        }
        if (printElipses == true) {
          Serial.print("... ");
        }
      }
    }
    Serial.println();
  #endif
}

