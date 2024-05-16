/////////////////////////////////////////////////////////////////
/*
 * Shared functions and library includes used by the GXOVnT system
*/
/////////////////////////////////////////////////////////////////
#pragma once
#ifndef _GXOVNT_SHARED_H_
#define _GXOVNT_SHARED_H_

// Includes for Logging, Arduino, string conversions and lists
/////////////////////////////////////////////////////////////////
#include <esp_err.h>
#include <esp_log.h>
#include <Arduino.h>
#include <iomanip>
#include <sstream>
#include <string>
#include <vector>
#include <soc/efuse_reg.h>

// Firmware Defines
/////////////////////////////////////////////////////////////////
#define FORMAT_SPIFFS_IF_FAILED true
#define LOG_TAG "GXOVnT"

#define GXOVNT_SYSTEM_TYPE_CLIENT 1
#define GXOVNT_SYSTEM_TYPE_SERVER 2

#define GXOVNT_SYSTEM_TYPE 1

// System firmware version
/////////////////////////////////////////////////////////////////
static const char *GXOVnT_FIRMWARE_VERSION = "v1.0.2";

// Common enums
/////////////////////////////////////////////////////////////////
// Enum to provide the exception type
enum GXOVnT_Exception_Code {
  ERROR_UNKNOWN = 0,
  ERROR_DEVICE_ALREADY_INITIALIZED = 1
};

enum GXOVnT_SYSTEM_TYPE {
  SYSTEM_TYPE_CLIENT = 1, SYSTEM_TYPE_SERVER = 2
};

enum GXOVnT_BLE_TPMS_TYPE {
  BLE_TPMS_TYPE_UNKNOWN = 99, BLE_TPMS_TYPE_JD_DY_BLE_I = 1
};

enum GXOVnT_TPMS_POSITION {
  TPMS_POSITION_UNKNOWN = 99, TPMS_POSITION_FRONT_RIGHT = 0, TPMS_POSITION_FRONT_LEFT = 1, TPMS_POSITION_REAR_RIGHT = 2, TPMS_POSITION_REAR_LEFT = 3, TPMS_POSITION_FRONT_SPARE = 4, TPMS_POSITION_CUSTOM = 5
};

// Common structures
/////////////////////////////////////////////////////////////////
// Custom exception structure type for the GXOVnT 
struct GXOVnT_Exception {
  // Gets the error message
  const char *ErrorMessage = "";
  // Gets the error code
  enum GXOVnT_Exception_Code ErrorCode = ERROR_UNKNOWN;

  // Default Constructor
  GXOVnT_Exception() = default; 
  
  // Specified Constructor
  GXOVnT_Exception(const char *errorMessage, enum GXOVnT_Exception_Code errorCode) {
    ErrorMessage = errorMessage;
    ErrorCode = errorCode;
  }
};
// TPMS Device already initialized exception. A TPMS device may only be initialized once
struct DeviceAlreadyInitializedException : GXOVnT_Exception { 
  DeviceAlreadyInitializedException() : GXOVnT_Exception::GXOVnT_Exception{ "The device has already been initialized", ERROR_DEVICE_ALREADY_INITIALIZED } {}
};

// Helper methods
/////////////////////////////////////////////////////////////////
// Helper method to convert a hex string into a byte
static uint8_t ConvertHexToByte(std::string input) {
  return (uint8_t)strtol(input.c_str(), 0, 16);
}
// Helper method to convert a byte into a hex string 
static std::string ConvertByteToHex(uint8_t input) {
  std::stringstream ss;
  ss << std::hex;
  ss << std::setw(2) << std::setfill('0') << (int)input;
  return ss.str();
  //return "";
}
// Helper method to print a byte value to the serial output in the format 000 to the serial monitor
static void PrintDebugFixed(uint8_t value) {
  Serial.printf("%03d ", value);
}
// Helper method to print a byte in 8 bit flags to the serial monitor
static void PrintDebugFlags(uint8_t value) {
    Serial.printf("(%d) ", value);

    for(byte mask = 0x80; mask; mask >>= 1){
      if(mask & value)
        Serial.print('1');
      else
        Serial.print('0');
    }

    Serial.print(' ');
}
// Helper method to print a byte in hex the serial monitor
static void PrintDebugHex(uint8_t value) {
  #ifdef GXOVnT_DEBUG_ENABLE
    Serial.printf("%02X ", value);
  #endif
}
// Compares two float values by converting them to int values to 2 decimal points
static bool CompareFloatWithFixedPrecision(float fval1, float fval2) {
  return (int)(fval1 * 100) == (int)(fval2 * 100);
}
// Converts a char pointer into a string
static std::string CharToString(char *c) {
  std::string s(c);
  return s;
}
static std::string CharToString(const char *c) {
  std::string s(c);
  return s;
}
// Gets the STA MAC Address of the device
static std::string DeviceMACAddress() {
  uint8_t baseMac[6];

  esp_read_mac(baseMac, ESP_MAC_WIFI_STA);

  std::string result("");

  for (int i = 0; i < 5; i++) {
    result += ConvertByteToHex(baseMac[i]);
  }

  return result;
}

#endif



