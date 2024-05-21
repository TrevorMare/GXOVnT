#include "services/BLECommService.h"
#include "GXOVnT.h"
#include "shared/Shared.h"

using namespace GXOVnT::services;


/////////////////////////////////////////////////////////////////
BleCommService::BleCommService() {}
BleCommService::~BleCommService() {}

// Public
/////////////////////////////////////////////////////////////////
void BleCommService::start() {
    // Check if we already started the service
    if (m_bleServer != nullptr) { 
        ESP_LOGI(LOG_TAG, "BLE server already started");
        return;
    }
    
    // Read the device name from the configuration
    std::string deviceName = GXOVnTConfig.Settings.SystemSettings.SystemName() + "_" + GXOVnT::shared::DeviceMACAddress();
    ESP_LOGI(LOG_TAG, "Starting BLE server with device name %s", deviceName.c_str());
    
    // Initialize the device
    BLEDevice::init(deviceName);

    initBleServer();
    initBleService();
    startAdvertising();

    ESP_LOGI(LOG_TAG, "BLE server started");
}

void BleCommService::stop() {
    if (m_bleServer == nullptr) return;
    
    if (m_bleAdvertising != nullptr) m_bleAdvertising->stop();
    if (m_bleService != nullptr) m_bleService->stop();
    if (m_serverConnectionId != -1) m_bleServer->disconnect(m_serverConnectionId);

    // Delete the pointer variables
    delete m_bleAdvertising;
    delete m_protoCharacteristic;
    delete m_bleService;
    delete m_bleServer;

    // Set the values to null
    m_bleAdvertising = nullptr;
    m_protoCharacteristic = nullptr;
    m_bleServer = nullptr;
    m_bleService = nullptr;
    m_serverConnectionId = -1;

    // De Init the ble device
    BLEDevice::deinit(true);
}

// Private
/////////////////////////////////////////////////////////////////
void BleCommService::initBleServer() {
  // Set up the server
    m_bleServer = BLEDevice::createServer();
    m_bleServer->setCallbacks(this);
}

void BleCommService::initBleService() {
    m_bleService = m_bleServer->createService(GXOVNT_BLE_SERVICE_UUID);
    
    m_protoCharacteristic = m_bleService->createCharacteristic(
                    GXOVNT_BLE_PROTO_CHARACTERISTIC_UUID,
                    BLECharacteristic::PROPERTY_READ   |
                    BLECharacteristic::PROPERTY_WRITE  |
                    BLECharacteristic::PROPERTY_NOTIFY |
                    BLECharacteristic::PROPERTY_INDICATE
                );
    
    m_protoCharacteristic->setCallbacks(this);
    m_protoCharacteristic->addDescriptor(new BLE2902());

    m_bleService->start();
}

void BleCommService::startAdvertising() {
    m_bleAdvertising = BLEDevice::getAdvertising();

    // Set the manufacturer data so that the scanning devices can filter 
    BLEAdvertisementData advertismentData;
  
    int systemType = static_cast<int>(GXOVnTConfig.Settings.SystemSettings.SystemType());
    int systemConfigured = GXOVnTConfig.Settings.SystemSettings.SystemConfigured() ? 1 : 0;

    // Now build the manufacturer data for the scanning devices
    // It's in the format GXOVnT|X|Y where X and Y are 1 or 0. X represents the system type 
    // and Y represents if the system has been configured
    std::string manufacturerData = CharToString(GXOVNT_BLE_MANUFACTURER) + CharToString(GXOVNT_BLE_MANUFACTURER_DELIMITER) 
        + std::to_string(systemType) + CharToString(GXOVNT_BLE_MANUFACTURER_DELIMITER) 
        + std::to_string(systemConfigured) + CharToString(GXOVNT_BLE_MANUFACTURER_DELIMITER);

    advertismentData.setManufacturerData(manufacturerData);
  
    m_bleAdvertising->setAdvertisementData(advertismentData);
    
    m_bleAdvertising->addServiceUUID(GXOVNT_BLE_SERVICE_UUID);

    m_bleAdvertising->setScanResponse(false);
    m_bleAdvertising->setMinPreferred(0x0);  // set value to 0x00 to not advertise this parameter
    BLEDevice::startAdvertising();
}

void BleCommService::onConnect(BLEServer* pServer) {
    m_serverConnectionId = pServer->getConnId();
    m_serverConnected = true;
    ESP_LOGI(LOG_TAG, "Device connected with connection Id %d", m_serverConnectionId);
}

void BleCommService::onDisconnect(BLEServer* pServer) {
    m_serverConnected = false;
    m_serverConnectionId = -1;
    ESP_LOGI(LOG_TAG, "Device disconnected");
}

void BleCommService::onWrite(BLECharacteristic* protoCharacteristic) {
    std::string value = protoCharacteristic->getValue();
    if (value.length() > 0) { 
        ESP_LOGI(LOG_TAG, "BLE Received -> %s", value.c_str());
    }
}