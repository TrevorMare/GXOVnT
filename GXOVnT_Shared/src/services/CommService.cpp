#include "services/CommService.h"
#include "services/BLECommService.h"

using namespace GXOVnT::services;

CommService::CommService() {}
CommService::~CommService() {}

void CommService::start() {
    if (m_BleCommService != nullptr) return;

    m_BleCommService = new BleCommService();
    m_BleCommService->start(this);

}
void CommService::stop() {
    if (m_BleCommService == nullptr) return;

    m_BleCommService->stop();

}

void CommService::handleBLEMessage(const uint8_t *buffer, size_t size) {
    ESP_LOGI(LOG_TAG, "Handle BLE input size: %d", size);

    Serial.println("Package Content Start");
    Serial.printf("%s", (char*)buffer);
    Serial.println();
    Serial.println("Package Content End");
    
}