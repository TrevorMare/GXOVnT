#include "services/BLECommService.h"


using namespace GXOVnTLib::services;
using namespace GXOVnTLib::messages;


/////////////////////////////////////////////////////////////////
BleCommService::BleCommService() {}
BleCommService::~BleCommService() {}

// Public
/////////////////////////////////////////////////////////////////
void BleCommService::start(CommMessageReceiveHandler *messageHandler)
{
	m_stopRequested = false;
	// Check if we already started the service
	if (m_bleServer != nullptr)
	{
		ESP_LOGI(LOG_TAG, "BLE server already started");
		return;
	}

	m_messageHandler = messageHandler;
	// Read the device name from the configuration
	std::string deviceName = GXOVnT.config->Settings.SystemSettings.SystemName() + "_" + GXOVnTLib::shared::DeviceMACAddress();
	ESP_LOGI(LOG_TAG, "Starting BLE server with device name %s", deviceName.c_str());

	// Initialize the device
	BLEDevice::init(deviceName);

	initBleServer();
	initBleService();
	startAdvertising();

	ESP_LOGI(LOG_TAG, "BLE server started");
}

void BleCommService::stop()
{

	m_stopRequested = true;

	if (m_bleServer == nullptr)
		return;

	if (m_bleAdvertising != nullptr)
		m_bleAdvertising->stop();
	if (m_bleService != nullptr)
		m_bleService->stop();
	if (m_serverConnectionId != -1)
		m_bleServer->disconnect(m_serverConnectionId);

	// Delete the pointer variables
	delete m_bleAdvertising;
	delete m_incomingMessagesCharacteristic;
	delete m_outgoingMessagesCharacteristic;
	delete m_bleService;
	delete m_bleServer;

	// Set the values to null
	m_bleAdvertising = nullptr;
	m_incomingMessagesCharacteristic = nullptr;
	m_outgoingMessagesCharacteristic = nullptr;
	m_bleServer = nullptr;
	m_bleService = nullptr;
	m_serverConnectionId = -1;

	// De Init the ble device
	BLEDevice::deinit(true);
}

bool BleCommService::sendMessage(CommMessage *commMessage) {
	return processOutgoingCharacteristicMessage(commMessage);
}

void BleCommService::receivedMessageHandled(uint16_t commMessageId) {
	removeProcessedMessage(commMessageId);
}
// Bluetooth connections and methods
/////////////////////////////////////////////////////////////////
void BleCommService::initBleServer()
{
	// Set up the server
	m_bleServer = BLEDevice::createServer();
	m_bleServer->setCallbacks(this);
}

void BleCommService::initBleService()
{
	m_bleService = m_bleServer->createService(GXOVNT_BLE_SERVICE_UUID);

	m_incomingMessagesCharacteristic = m_bleService->createCharacteristic(
			GXOVNT_BLE_INCOMING_CHARACTERISTIC_UUID,
			BLECharacteristic::PROPERTY_READ |
					BLECharacteristic::PROPERTY_WRITE |
					BLECharacteristic::PROPERTY_NOTIFY |
					BLECharacteristic::PROPERTY_INDICATE);

	m_outgoingMessagesCharacteristic = m_bleService->createCharacteristic(
			GXOVNT_BLE_OUTGOING_CHARACTERISTIC_UUID,
			BLECharacteristic::PROPERTY_READ |
					BLECharacteristic::PROPERTY_WRITE |
					BLECharacteristic::PROPERTY_NOTIFY |
					BLECharacteristic::PROPERTY_INDICATE |
					BLECharacteristic::PROPERTY_WRITE_NR);		 			

	m_incomingMessagesCharacteristic->setCallbacks(this);
	m_incomingMessagesCharacteristic->addDescriptor(new BLE2902());

	m_outgoingMessagesCharacteristic->addDescriptor(new BLE2902());

	m_bleService->start();
}

void BleCommService::startAdvertising()
{
	m_bleAdvertising = BLEDevice::getAdvertising();

	// Set the manufacturer data so that the scanning devices can filter
	BLEAdvertisementData advertismentData;
	int systemType = static_cast<int>(GXOVnT.config->Settings.SystemSettings.SystemType());
	int systemConfigured = GXOVnT.config->Settings.SystemSettings.SystemConfigured() ? 1 : 0;

	std::string manufacturerData = CharToString(GXOVNT_BLE_MANUFACTURER) + 
		CharToString(GXOVNT_BLE_MANUFACTURER_DELIMITER) + 
		std::to_string(systemType) + 
		CharToString(GXOVNT_BLE_MANUFACTURER_DELIMITER) + 
		std::to_string(systemConfigured) + 
		CharToString(GXOVNT_BLE_MANUFACTURER_DELIMITER);

	advertismentData.setManufacturerData(manufacturerData);

	m_bleAdvertising->setAdvertisementData(advertismentData);

	m_bleAdvertising->addServiceUUID(GXOVNT_BLE_SERVICE_UUID);

	m_bleAdvertising->setScanResponse(false);
	m_bleAdvertising->setMinPreferred(0x0); // set value to 0x00 to not advertise this parameter
	BLEDevice::startAdvertising();
}

void BleCommService::onConnect(BLEServer *pServer)
{
	m_serverConnectionId = pServer->getConnId();
	m_serverConnected = true;
	ESP_LOGI(LOG_TAG, "Device connected with connection Id %d", m_serverConnectionId);
}

void BleCommService::onDisconnect(BLEServer *pServer)
{
	m_serverConnected = false;
	m_serverConnectionId = -1;
	ESP_LOGI(LOG_TAG, "Device disconnected");

	// If this device did not request the connection to be stopped, restart the advertising
	if (!m_stopRequested) {
		BLEDevice::startAdvertising();
	}
}

// Callback for when another system writes to the Characteristic
void BleCommService::onWrite(BLECharacteristic *bleCharacteristic)
{
	uint8_t *messageBuffer = bleCharacteristic->getData();
	size_t messageLength = bleCharacteristic->getLength();
	if (messageLength > 2) {

		CommMessage *commMessage = processIncomingCharacteristicMessage(messageBuffer, messageLength);
		if (commMessage != nullptr) {
			m_messageHandler->onMessageReceived(commMessage);
		}
	}
}

// Characteristic Parsing
/////////////////////////////////////////////////////////////////
void BleCommService::removeProcessedMessage(uint16_t messageId) {

	int commMessageIndex = -1;
	// We need to find the 
	for (int iCommMessage = 0; iCommMessage < m_commMessages.size(); iCommMessage++) {
		if (messageId == m_commMessages[iCommMessage]->MessageId()) {
			commMessageIndex = iCommMessage;
			break;
		}
	}
	if (commMessageIndex == -1) return;

	delete m_commMessages[commMessageIndex];
	m_commMessages.erase(m_commMessages.begin() + commMessageIndex);

}

bool BleCommService::processOutgoingCharacteristicMessage(CommMessage *commMessage) {
	if (m_outgoingMessagesCharacteristic == nullptr) {
		ESP_LOGE(LOG_TAG, "Could not find the write characteristic");
		return false;
	}
	size_t numberOfPackets = commMessage->MessagePackets()->size();
	for (size_t iPacket = 0; iPacket < numberOfPackets; iPacket++) {
		CommMessagePacket *packet = commMessage->MessagePackets()->at(iPacket);
		uint8_t *packetBuffer = packet->GetData();
		size_t packetLength = packet->PacketBufferSize();
		m_outgoingMessagesCharacteristic->setValue(packetBuffer, packetLength);
		m_outgoingMessagesCharacteristic->notify(true);
		delay(BLE_SERVER_WRITE_DELAY);
	}
	return true;
}

CommMessage *BleCommService::processIncomingCharacteristicMessage(uint8_t *buffer, size_t messageLength) {
	if (m_messageHandler == nullptr) { return nullptr; }
	// Build the message package
	CommMessagePacket *messagePacket = new CommMessagePacket();

	// Build the packet from the incoming data
	messagePacket->buildIncomingPacketData(buffer, messageLength);

	if (!messagePacket->ValidPacket()) {
		ESP_LOGI(LOG_TAG, "Packet not valid, quiting");
		delete messagePacket;
		return nullptr;
	}
	
	CommMessage *commMessage = nullptr;
	int commMessageIndex = -1;

	// If this is the start of the message, create a new message object and add it to the messages
	if (messagePacket->PacketStart()) {
		commMessage = new CommMessage(COMM_SERVICE_TYPE_BLE);
		m_commMessages.push_back(commMessage);
		commMessageIndex = m_commMessages.size() -1;

	} else {
		// We need to find the 
		for (int iCommMessage = 0; iCommMessage < m_commMessages.size(); iCommMessage++) {
			if (messagePacket->MessageId() == m_commMessages[iCommMessage]->MessageId()) {
				commMessage = m_commMessages[iCommMessage];
				commMessageIndex = iCommMessage;
				break;
			}
		}
	}

	// Check that we did find the message, if not ignore it 
	if (commMessage == nullptr) {
		ESP_LOGE(LOG_TAG, "Deleting message packet as message not found");
		delete messagePacket;
		return nullptr;
	}

	// Add the message packet
	commMessage->AddIncomingPackage(messagePacket);

	// If this is the last packet in the message, handle it
	if (messagePacket->PacketEnd()) {
		return commMessage;
	}
	return nullptr;
}
