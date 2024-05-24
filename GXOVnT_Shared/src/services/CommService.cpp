#include "services/CommService.h"
#include "messages/CommMessage.h"
#include "services/BLECommService.h"

using namespace GXOVnT::services;
using namespace GXOVnT::messages;

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

void CommService::handleMessage(CommMessage *commMessage) {
    commMessage->PrintMessage();
}