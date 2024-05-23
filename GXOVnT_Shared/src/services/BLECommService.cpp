#include "services/BLECommService.h"
#include "GXOVnT.h"
#include "shared/Shared.h"

using namespace GXOVnT::services;

/////////////////////////////////////////////////////////////////
BleCommService::BleCommService() {}
BleCommService::~BleCommService() {}

// Public
/////////////////////////////////////////////////////////////////
void BleCommService::start(BLECommServiceMessageCallback *messageHandler)
{
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
void BleCommService::initBleServer()
{
	// Set up the server
	m_bleServer = BLEDevice::createServer();
	m_bleServer->setCallbacks(this);
}

void BleCommService::initBleService()
{
	m_bleService = m_bleServer->createService(GXOVNT_BLE_SERVICE_UUID);

	m_protoCharacteristic = m_bleService->createCharacteristic(
			GXOVNT_BLE_PROTO_CHARACTERISTIC_UUID,
			BLECharacteristic::PROPERTY_READ |
					BLECharacteristic::PROPERTY_WRITE |
					BLECharacteristic::PROPERTY_NOTIFY |
					BLECharacteristic::PROPERTY_INDICATE);

	m_protoCharacteristic->setCallbacks(this);
	m_protoCharacteristic->addDescriptor(new BLE2902());

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
}

void BleCommService::onWrite(BLECharacteristic *protoCharacteristic)
{
	uint8_t *messageBuffer = protoCharacteristic->getData();
	size_t messageLength = protoCharacteristic->getLength();
	if (messageLength > 2) {
		processCharacteristicMessage(messageBuffer, messageLength);
	}
}

// Message handling
void BleCommService::handleMessageComplete(const uint8_t *buffer, size_t messageLength)
{
	if (m_messageHandler == nullptr) { return; }
	m_messageHandler->handleBLEMessage(buffer, messageLength);
}

void BleCommService::handleMessageComplete(BLEMessage *bleMessage)
{
	if (m_messageHandler == nullptr) { return; }

	BLEMessage localMessage = *bleMessage;

	size_t totalSize = localMessage.MessageSize;
	int numberOfPackets = localMessage.MessagePackets.size();

	ESP_LOGI(LOG_TAG, "Starting the combination of all the packets");

	ESP_LOGI(LOG_TAG, "Number of packets on the message: %d", numberOfPackets);
	ESP_LOGI(LOG_TAG, "Message Buffer size on the message: %d", totalSize);

	
	std::vector<uint8_t> totalMessage;
	totalMessage.reserve(totalSize + 1);

	ESP_LOGI(LOG_TAG, "Space reserved: %d bytes", totalSize + 1);

	for (int i = 0; i < numberOfPackets; i++) {
		
		
		BLEMessagePacket messagePacket = *(localMessage.MessagePackets.at(i));
		std::vector<uint8_t> packetBuffer = *messagePacket.Data;
		
		
		totalMessage.insert(totalMessage.end(), packetBuffer.begin(), packetBuffer.end());
	}

	totalMessage.push_back('\0');

	for (size_t i = 0; i < totalMessage.size(); i++)
	{
		Serial.printf(" %d ", totalMessage[i]);
	}
	

	const uint8_t *result = totalMessage.data();
	
	if (m_messageHandler != nullptr) {
		m_messageHandler->handleBLEMessage(result, totalSize);
	}
}

// Characteristic Parsing
/////////////////////////////////////////////////////////////////

bool BleCommService::processCharacteristicMessage(uint8_t *buffer, size_t messageLength)
{
	// Build the message packet
	BLEMessagePacket *messagePacket = buildMessagePacket(buffer, messageLength);
	// If the packet could not be built, no need to continue
	if (messagePacket == nullptr) {
		ESP_LOGI(LOG_TAG, "Message received but could not parse the packet");
		return false;
	}

	// If the message packet contains the complete buffer of the message,
	// we do not need to keep this message in seperate packages and can continue
	// processing the buffer per usual
	if (messagePacket->PacketStart && messagePacket->PacketEnd) {
		ESP_LOGI(LOG_TAG, "Message packet contained all data required for processing");
		const uint8_t *packetBuffer = (*(messagePacket->Data)).data();
		handleMessageComplete(packetBuffer, (*(messagePacket->Data)).size());
		return true;
	}

	// Now we need to find the message this packet belongs to
	int messageIndex = getMessageIndex(messagePacket->MessageId);

	BLEMessage *message = createOrUpdateMessage(messageIndex, messagePacket, messageLength);
	if (message == nullptr) {
		ESP_LOGE(LOG_TAG, "Could not locate or create the message");
		return false;
	}

	// Last we need to check if this is the last packet in the message, if it is
	// we need to process the message and remove it from memory
	if (messagePacket->PacketEnd) {
		ESP_LOGI(LOG_TAG, "Message end recieved, attempting to handle the message");
		// Process message
		handleMessageComplete(message);
		ESP_LOGI(LOG_TAG, "Message handled, performing cleanup on memory");
		delete m_messages[messageIndex];
		m_messages.erase(m_messages.begin() + messageIndex);
	}

	return true;
}

// Message Vector Helper Methods
/////////////////////////////////////////////////////////////////

int BleCommService::getMessageIndex(uint16_t messageId)
{
	int index = -1;
	for (size_t iMessage = 0; iMessage < m_messages.size(); iMessage++)
	{
		// Find the message with the id
		if (m_messages[iMessage]->MessageId == messageId)
		{
			index = iMessage;
			break;
		}
	}
	return index;
}

BLEMessage *BleCommService::createOrUpdateMessage(int messageIndex, BLEMessagePacket *messagePacket, size_t messageLength)
{
	BLEMessage *bleMessage = nullptr;
	if (messageIndex != -1) {
		bleMessage = m_messages[messageIndex];
	}
	// If the message could not be found, we need to check that the packet is at least for
	// the start of the message
	if (bleMessage == nullptr && !messagePacket->PacketStart) {
		ESP_LOGW(LOG_TAG, "Message packet received without a valid message in memory.");
		return nullptr;
	}
	else if (bleMessage == nullptr && messagePacket->PacketStart) {
		ESP_LOGW(LOG_TAG, "Message packet received. Starting new message package group");
		// Create a new message in memory
		bleMessage = new BLEMessage();
		bleMessage->MessageId = messagePacket->MessageId;
		bleMessage->MessageSize = 0;
		m_messages.push_back(bleMessage);
	}

	// Calculate the new size of the message by adding the packet size
	size_t currentSize = bleMessage->MessageSize;
	bleMessage->MessageSize = currentSize + messageLength - (sizeof(uint8_t) * 4);
	// Set the expiry of the message
	bleMessage->ExpiryMillis = millis() + 30000;
	// Create the smart pointer and add to the message
	bleMessage->MessagePackets.push_back(messagePacket);

	return bleMessage;
}

BLEMessagePacket *BleCommService::buildMessagePacket(uint8_t *buffer, size_t messageLength) const
{
	if (messageLength <= 4) {
		ESP_LOGW(LOG_TAG, "buildMessagePacket: Packet length to short");
		return nullptr;
	}
	// First two bytes is message is message Id
	uint16_t messageId = ((uint16_t)buffer[1] << 8) | buffer[0];
	uint8_t packetId = buffer[2];
	uint8_t packetDetail = buffer[3];
	// Flags:
	//  Position 0 - Is message start packet
	//  Position 1 - Is message end packet
	bool isPacketStart = GetFlag(packetDetail, 0);
	bool isPacketEnd = GetFlag(packetDetail, 1);
	// Create a vector of the remaining bytes. This represents the actual data in the packet
	std::vector<uint8_t> messageBuffer;
	for (int i = 4; i < messageLength; i++) {
		messageBuffer.push_back(buffer[i]);
	}
	// Create the message and return a pointer to the object
	BLEMessagePacket *messagePacket = new BLEMessagePacket(messageId, packetId, 
		isPacketStart, isPacketEnd, &messageBuffer); 
	
	return messagePacket;
}

