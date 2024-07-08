#include "GXOVnTRoot.h"

using namespace GXOVnTLib::services;
using namespace GXOVnTLib::settings;


GXOVnTRoot::GXOVnTRoot() {
  if (commService == NULL) {
    commService = new GXOVnTLib::services::CommService();
  }  
  if (firmwareUpdateService == NULL) {
    firmwareUpdateService = new GXOVnTLib::services::FirmwareUpdateService();
  }  
	if (config == NULL) {
    config = new GXOVnTLib::settings::Config();
    config->readConfiguration();
  }    
}

void GXOVnTRoot::Initialize() {

  GXOVnT_BOOT_MODE bootMode = config->Settings.SystemSettings.SystemBootMode();

  if (bootMode == BOOT_MODE_TEST_WIFI_MODE) {
    testWiFiConnection();
  } else if (bootMode == BOOT_MODE_CHECK_FIRMWARE) {
    downloadLatestFirmwareList();
  } else {
    commService->Start();
  }
}

void GXOVnTRoot::testWiFiConnection() {
  const char *ssid = config->Settings.TestWiFiSettings.WiFiSsid().c_str();
  const char *password = config->Settings.TestWiFiSettings.WiFiPassword().c_str();

  WiFi.begin(ssid, password);
  bool attemptingConnect = true;

  while (attemptingConnect) {
    if (WiFi.status() == WL_CONNECT_FAILED) {
      attemptingConnect = false;
      config->Settings.TestWiFiSettings.StatusCode(500);
      config->Settings.TestWiFiSettings.StatusMessage("Connection Failed");
      config->Settings.TestWiFiSettings.Success(false);
    } 
    else if (WiFi.status() == WL_CONNECTED) { 
      attemptingConnect = false;
      config->Settings.TestWiFiSettings.StatusCode(200);
      config->Settings.TestWiFiSettings.StatusMessage("Connection Success");
      config->Settings.TestWiFiSettings.Success(true);
    }
  }

  config->Settings.SystemSettings.SystemBootMode(BOOT_MODE_SYSTEM_BLE_MODE);
  config->Settings.TestWiFiSettings.Tested(true);
  config->saveConfiguration();

  delay(1000);

  ESP.restart();
}

void GXOVnTRoot::downloadLatestFirmwareList() {
  std::string ssid = config->Settings.CheckFirmwareSettings.WiFiSsid();
  std::string password = config->Settings.CheckFirmwareSettings.WiFiPassword();

  if (ssid.compare("") == 0) {
    std::string ssid = config->Settings.WiFiSettings.WiFiSsid();
    std::string password = config->Settings.WiFiSettings.WiFiPassword();
  }

  firmwareUpdateService->Setup(ssid, password);
  FirmwareServiceDownloadListResult *result = firmwareUpdateService->DownloadLatestFirmwareList();

  config->Settings.CheckFirmwareSettings.Success(result->Success);
  config->Settings.CheckFirmwareSettings.StatusCode(result->StatusCode);
  config->Settings.CheckFirmwareSettings.StatusMessage(result->StatusMessage);
  config->Settings.SystemSettings.SystemBootMode(BOOT_MODE_SYSTEM_BLE_MODE);
  config->saveConfiguration();

  delay(1000);

  ESP.restart();
}

GXOVnTRoot GXOVnT;