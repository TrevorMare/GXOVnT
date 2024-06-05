/*
  John Dow TPMS Sensor library. 
    Monitored Temperature Range : -4°F to 185°F, or -20°C to 85°C
    Monitored Pressure Range : 0 ~ 92psi | 0 ~ 640kPa

  The sensor sends data with the following sample data
  0   1   2   3   4   5   6   7   8   9   10  11  12  13  14  15  16  17  18  19  20  21  22  23  24  25  26  27  28  29  30  31  32  33  34  35  36  37  38  39  40  41  42  43  44  45  46  47  48  49  50  51   
  002 001 006 026 255 076 000 002 021 181 074 220 000 103 249 017 217 150 105 008 000 032 012 154 102 241 207 191 081 156 021 003 152 088 138 033 020 045 012 000 000 000 138 023 241 207 000 081 063 178 129 114 

  Initial tests: 
  Position: 0-24 never changes and are always the same and can be ignored. 
  Position: 25, 26 is the last 4 digits of the device identifier
  Position: 29-37 never changes and are always the same and can be ignored. 
  Position: 39-46 never changes and are always the same and can be ignored. 

  Position: 27 Changes
  Position: 28 and 47 changes and both are the same 
  
  Position: 47 This is the pressure measured. Value should be multiplied by 0.3625 to get psi
  Position: 48 Temperature in degree Celcius, needs offset due to negative values e.g 64 equal 24 deg celcius
  
  Position: 50, 51 are expected to be flags

*/
#pragma once
#ifndef _GXOVNT_TPMS_BLE_JohnDow_H
#define _GXOVNT_TPMS_BLE_JohnDow_H
#include "shared/Definitions.h"
#include "GXOVnT_TPMS_BLE.h"

class GXOVnT_TPMS_BLE_JohnDow : public GXOVnT_TPMS_BLE
{

  private:
      uint8_t m_DeviceIdPart1;
      uint8_t m_DeviceIdPart2;
      void serialPrintDebug(uint8_t* payloadRaw, size_t payloadLength);
      
  public:

      // Setup for the John Dow device
      bool setupBLEDeviceSpecific(const char *deviceId) override;
      
      // Parsing of the BLE Advertisement Device
      enum GXOVNT_TPMS_BLE_PARSE_RESULT parseBLEAdvertisementDeviceSpecific(BLEAdvertisedDevice advertisedDevice, GXOVnT_TPMS_BLE_VALUES *values) override;
      
      // Checks if the BLEAdvertisedDevice message matches the John Dow TPMS Type. This will return the device id
      // if it matches
      static std::string SensorTypeMatchAdvertisementDeviceMessage(BLEAdvertisedDevice advertisedDevice);
};

#endif


