#include "services/BLECommService.h"

#include "GXOVnT.h"

using namespace GXOVnT::services;
using namespace GXOVnT::messages;


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
	std::string deviceName = GXOVnTConfig.Settings.SystemSettings.SystemName() + "_" + GXOVnT::shared::DeviceMACAddress();
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
	delete m_protoReadCharacteristic;
	delete m_protoWriteCharacteristic;
	delete m_bleService;
	delete m_bleServer;

	// Set the values to null
	m_bleAdvertising = nullptr;
	m_protoReadCharacteristic = nullptr;
	m_protoWriteCharacteristic = nullptr;
	m_bleServer = nullptr;
	m_bleService = nullptr;
	m_serverConnectionId = -1;

	// De Init the ble device
	BLEDevice::deinit(true);
}

bool BleCommService::sendMessage(CommMessage *commMessage) {
	if (commMessage == nullptr) return false;
	return processWriteCharacteristicMessage(commMessage);
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

	m_protoReadCharacteristic = m_bleService->createCharacteristic(
			GXOVNT_BLE_PROTO_CHARACTERISTIC_READ_UUID,
			BLECharacteristic::PROPERTY_READ |
					BLECharacteristic::PROPERTY_WRITE |
					BLECharacteristic::PROPERTY_NOTIFY |
					BLECharacteristic::PROPERTY_INDICATE);

	m_protoWriteCharacteristic = m_bleService->createCharacteristic(
			GXOVNT_BLE_PROTO_CHARACTERISTIC_WRITE_UUID,
			BLECharacteristic::PROPERTY_READ |
					BLECharacteristic::PROPERTY_WRITE |
					BLECharacteristic::PROPERTY_NOTIFY |
					BLECharacteristic::PROPERTY_INDICATE);		 			

	m_protoReadCharacteristic->setCallbacks(this);
	m_protoReadCharacteristic->addDescriptor(new BLE2902());

	m_protoWriteCharacteristic->setCallbacks(this);
	m_protoWriteCharacteristic->addDescriptor(new BLE2902());

	m_bleService->start();
}

void BleCommService::startAdvertising()
{
	m_bleAdvertising = BLEDevice::getAdvertising();

	// Set the manufacturer data so that the scanning devices can filter
	BLEAdvertisementData advertismentData;

	int systemType = static_cast<int>(GXOVnTConfig.Settings.SystemSettings.SystemType());
	int systemConfigured = GXOVnTConfig.Settings.SystemSettings.SystemConfigured() ? 1 : 0;

	// Now build the manufacturer data for the scanning devices
	// It's in the format GXOVnT|X|Y where X and Y are 1 or 0. X represents the system type
	// and Y represents if the system has been configured
	std::string manufacturerData = CharToString(GXOVNT_BLE_MANUFACTURER) + CharToString(GXOVNT_BLE_MANUFACTURER_DELIMITER) + std::to_string(systemType) + CharToString(GXOVNT_BLE_MANUFACTURER_DELIMITER) + std::to_string(systemConfigured) + CharToString(GXOVNT_BLE_MANUFACTURER_DELIMITER);

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

void BleCommService::onWrite(BLECharacteristic *protoCharacteristic)
{
	std::lock_guard<std::mutex> lck(m_mutexLock);
	uint8_t *messageBuffer = protoCharacteristic->getData();
	size_t messageLength = protoCharacteristic->getLength();
	if (messageLength > 2) {

		CommMessage *commMessage = processReadCharacteristicMessage(messageBuffer, messageLength);
		if (commMessage != nullptr) {
			m_messageHandler->onMessageReceived(commMessage);
			//removeProcessedMessage(commMessage->MessageId());
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

	ESP_LOGI(LOG_TAG, "Message processed, deleting message at index %d", commMessageIndex);

	delete m_commMessages[commMessageIndex];
	m_commMessages.erase(m_commMessages.begin() + commMessageIndex);

}

bool BleCommService::processWriteCharacteristicMessage(CommMessage *commMessage) {
	size_t numberOfChunks = commMessage->MessagePackets()->size();

	for (size_t iChunk = 0; iChunk < numberOfChunks; iChunk++) {
		CommMessagePacket *packet = commMessage->MessagePackets()->at(0);
		uint8_t *packetBuffer = packet->GetData();
		size_t packetLength = packet->PacketBufferSize();

		m_protoWriteCharacteristic->setValue(packetBuffer, packetLength);

		

		delay(100);
	}
	return true;
}

CommMessage *BleCommService::processReadCharacteristicMessage(uint8_t *buffer, size_t messageLength)
{
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
		ESP_LOGI(LOG_TAG, "Deleting message packet as message not found");
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
