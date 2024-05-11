/*
  Base class for Bluetooth TPMS Sensors
*/

#ifndef _GXOVNT_TPMS_BLE_H
#define _GXOVNT_TPMS_BLE_H

#include "GXOVnT_Shared.h"
#include <BLEAdvertisedDevice.h>

// Used for converting hex to uint_8
#include <sstream>
#include <iostream>
#include <iomanip>

enum GXOVNT_TPMS_BLE_DEBUG_PRINT { DEBUG_PRINT_NONE, DEBUG_PRINT_TRUNCATED, DEBUG_PRINT_FULL, DEBUG_PRINT_KNOWN };
enum GXOVNT_TPMS_BLE_SENSOR_TYPE { TPMS_BLE_SENSOR_TYPE_UNKNOWN, TPMS_BLE_SENSOR_TYPE_JOHN_DOW };
enum GXOVNT_TPMS_BLE_PARSE_RESULT { TPMS_BLE_PARSE_RESULT_NOT_MINE, TPMS_BLE_PARSE_RESULT_NOT_INITIALIZED, TPMS_BLE_PARSE_RESULT_NOTHING_UPDATED, TPMS_BLE_PARSE_RESULT_UPDATED };

struct GXOVnT_TPMS_BLE_VALUES {
  public:
    // Value for the pressure of the sensor
    float PressureValue = 0;
    // Value for the temperature of the sensor
    float TemperatureValue = 0;
    // Value for the battery voltage of the sensor
    float BatteryVoltage = 0;
    // Value for the signal strength
    int SignalStrength = -100;
    // Value to indicate if the last scan made changes
    bool ValuesChanged = false;
    // Value to indicate if this sensor received any messages in the specified time frame
    bool IsAlive = false;
    // Value to indicate last update time
    unsigned long LastMessageReceived = 0;
};

// Base class for Bluetooth TPMS Sensors
class GXOVnT_TPMS_BLE {
  private:
    
    std::string m_deviceId;
    enum GXOVNT_TPMS_BLE_SENSOR_TYPE m_BLESensorType = TPMS_BLE_SENSOR_TYPE_UNKNOWN;
    GXOVnT_TPMS_BLE_VALUES *m_Values = new GXOVnT_TPMS_BLE_VALUES();
    
    // Value to indicate if the sensor has been initialized by calling the setupBLEDevice method
    bool m_Initialized;
    // Value in seconds that specifies how long after the last message is received the is alive indicator should
    // be cleared
    int m_isAliveMilliSecondsOffset = (60 * 1000);

  public:
    // Default constructor, setupBLEDevice must be called to initialize this instance 
    GXOVnT_TPMS_BLE();
    
    // Constructor that initializes the device with the given device Id
    GXOVnT_TPMS_BLE(const char *deviceId);
    
    // Constructor that initializes the device with the given device Id
    GXOVnT_TPMS_BLE(char *deviceId);
    
    ~GXOVnT_TPMS_BLE();

    // Initializes this instance with the device Id
    void setupBLEDevice(const char *deviceId, int isAliveSecondsOffset = 60);
    
    // Initializes this instance with the device Id
    void setupBLEDevice(char *deviceId, int isAliveSecondsOffset = 60);

    // Gets a value indicating if the device has been initialized
    bool getInitialized() { return m_Initialized; }

    // Gets the Bluetooth Sensor type
    enum GXOVNT_TPMS_BLE_SENSOR_TYPE getSensorType() { return m_BLESensorType; }
    
    // Sets the debug print mode on the instance
    void setDebugPrintMode(enum GXOVNT_TPMS_BLE_DEBUG_PRINT debugPrintMode) { m_debugPrintMode = debugPrintMode; }

    enum GXOVNT_TPMS_BLE_DEBUG_PRINT getDebugPrintMode() { return m_debugPrintMode; }    

    // Gets the latest values for the sensor
    const GXOVnT_TPMS_BLE_VALUES* getValue() { return m_Values; };

    // Parses the device message and returns an enum result if this was a success
    enum GXOVNT_TPMS_BLE_PARSE_RESULT parseBLEAdvertisement(BLEAdvertisedDevice advertisedDevice);

    // Resets the changed indicators after the scan completes
    void resetAfterScan() {
      m_Values->ValuesChanged = false;
    }
    
  protected:
    // A value indicating the current debug print options
    enum GXOVNT_TPMS_BLE_DEBUG_PRINT m_debugPrintMode = DEBUG_PRINT_NONE;

    // Sets the instance initialized indicator
    void setInitialized(bool initialized) { m_Initialized = initialized; }

    // Sets the Bluetooth sensor type
    void setBLESensorType(enum GXOVNT_TPMS_BLE_SENSOR_TYPE sensorType) { m_BLESensorType = sensorType; }

    // This method needs to overwritten to set up the device specific details
    virtual bool setupBLEDeviceSpecific(const char *deviceId);

    // This method needs to overwritten to parse the device specific message
    virtual enum GXOVNT_TPMS_BLE_PARSE_RESULT parseBLEAdvertisementDeviceSpecific(BLEAdvertisedDevice advertisedDevice, GXOVnT_TPMS_BLE_VALUES *values);

};

#endif