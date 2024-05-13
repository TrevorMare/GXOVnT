/*
 * Shared functions and library includes used by the GXOVnT system
*/

#ifndef _GXOVNT_SHARED_H
#define _GXOVNT_SHARED_H

#define LOG_TAG "GXOVnT"

// Logging libraries for the ESP
#include <esp_err.h>
#include <esp_log.h>

// Arduino API library
#include <Arduino.h>

// String conversion libraries
#include <iomanip>
#include <sstream>
#include <string>

// List libraries
#include <vector>

// System firmware version
static const char *GXOVnT_FIRMWARE_VERSION = "v1.0.2";

// Enum to provide the exception type
enum GXOVnT_Exception_Code {
  ERROR_UNKNOWN = 0,
  ERROR_DEVICE_ALREADY_INITIALIZED = 1
};

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

#endif



