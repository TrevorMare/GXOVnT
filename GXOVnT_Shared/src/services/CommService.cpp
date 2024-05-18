#include "services/CommService.h"
#include "services/BLECommService.h"

using namespace GXOVnT::services;

CommService::CommService() {}
CommService::~CommService() {}

void CommService::start() {
    if (m_BleCommService != nullptr) return;

    m_BleCommService = new BleCommService();
    m_BleCommService->start();

}
void CommService::stop() {
    if (m_BleCommService == nullptr) return;

    m_BleCommService->stop();

}