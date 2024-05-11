#ifndef _GXOVNT_SHARED_H
#define _GXOVNT_SHARED_H

#ifdef GXOVnT_DEBUG_ENABLE
// Used for printing to the serial window
#include <HardwareSerial.h>
#endif

#include <Arduino.h>
#include <iomanip>
#include <sstream>
#include <string>

enum GXOVnT_Exception_Code {
  ERROR_UNKNOWN = 0,
  ERROR_DEVICE_ALREADY_INITIALIZED = 1
};

static const char *SoftwareVersion = "v1.0.1";

struct GXOVnT_Exception {
  const char *ErrorMessage = "";
  enum GXOVnT_Exception_Code ErrorCode = ERROR_UNKNOWN;

  // Default Constructor
  GXOVnT_Exception() = default; 
  
  // Specified Constructor
  GXOVnT_Exception(const char *errorMessage, enum GXOVnT_Exception_Code errorCode) {
    ErrorMessage = errorMessage;
    ErrorCode = errorCode;
  }
};

struct DeviceAlreadyInitializedException : GXOVnT_Exception { 
  DeviceAlreadyInitializedException() : GXOVnT_Exception::GXOVnT_Exception{ "The device has already been initialized", ERROR_DEVICE_ALREADY_INITIALIZED } {}
};

static uint8_t ConvertHexToByte(std::string input) {
  return (uint8_t)strtol(input.c_str(), 0, 16);
}

static std::string ConvertByteToHex(uint8_t input) {
  std::stringstream ss;
  ss << std::hex;
  ss << std::setw(2) << std::setfill('0') << (int)input;
  return ss.str();
  //return "";
}

static void PrintDebugFixed(uint8_t value) {
  #ifdef GXOVnT_DEBUG_ENABLE
    Serial.printf("%03d ", value);
  #endif
}

static void PrintDebugFlags(uint8_t value) {
  #ifdef GXOVnT_DEBUG_ENABLE

    Serial.printf("(%d) ", value);

    for(byte mask = 0x80; mask; mask >>= 1){
      if(mask & value)
        Serial.print('1');
      else
        Serial.print('0');
    }

    Serial.print(' ');
  #endif
}

static void PrintDebugHex(uint8_t value) {
  #ifdef GXOVnT_DEBUG_ENABLE
    Serial.printf("%02X ", value);
  #endif
}

// Compares two float values by converting them to int values
static bool CompareFloatWithFixedPrecision(float fval1, float fval2) {
  return (int)(fval1 * 100) == (int)(fval2 * 100);
}

#endif
