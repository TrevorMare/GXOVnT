#define GXOVnT_DEBUG_ENABLE

#include <Arduino.h>

#include <GXOVnT_WebUpdate.h>
#include <heltec.h>
#include <GXOVnTRoot.h>


//https://medium.com/@adityabangde/esp32-firmware-updates-from-github-a-simple-ota-solution-173a95f4a97b


int scanTime = 10; //In seconds

static void TestWiFiConnection() {
  const char *ssid = GXOVnT.config->Settings.TestWiFiSettings.WiFiSsid().c_str();
  const char *password = GXOVnT.config->Settings.TestWiFiSettings.WiFiPassword().c_str();

  WiFi.begin(ssid, password);
  bool attemptingConnect = true;

  while (attemptingConnect) {
    if (WiFi.status() == WL_CONNECT_FAILED) {
      attemptingConnect = false;
      GXOVnT.config->Settings.TestWiFiSettings.StatusCode(500);
      GXOVnT.config->Settings.TestWiFiSettings.StatusMessage("Connection Failed");
      GXOVnT.config->Settings.TestWiFiSettings.Success(false);
    } 
    else if (WiFi.status() == WL_CONNECTED) { 
      attemptingConnect = false;
      GXOVnT.config->Settings.TestWiFiSettings.StatusCode(200);
      GXOVnT.config->Settings.TestWiFiSettings.StatusMessage("Connection Success");
      GXOVnT.config->Settings.TestWiFiSettings.Success(true);
    }
  }

  GXOVnT.config->Settings.SystemSettings.SystemBootMode(BOOT_MODE_SYSTEM_BLE_MODE);
  GXOVnT.config->Settings.TestWiFiSettings.Tested(true);
  GXOVnT.config->saveConfiguration();

  delay(1000);

  ESP.restart();
}

static void CheckFirmwareUpdates() {
  const char *ssid = GXOVnT.config->Settings.CheckFirmwareSettings.WiFiSsid().c_str();
  const char *password = GXOVnT.config->Settings.CheckFirmwareSettings.WiFiPassword().c_str();

  WiFi.begin(ssid, password);
  bool connectedFailed = false;
  bool connectedSuccess = false;

  while (!connectedFailed && !connectedSuccess) {
    if (WiFi.status() == WL_CONNECT_FAILED) {
      connectedFailed = true;
    } 
    else if (WiFi.status() == WL_CONNECTED) { 
      connectedSuccess = true;
    }
  }

  if (connectedFailed) {
    GXOVnT.config->Settings.CheckFirmwareSettings.StatusCode(500);
    GXOVnT.config->Settings.CheckFirmwareSettings.StatusMessage("Could not connect to WiFi to check for updates");
    GXOVnT.config->Settings.CheckFirmwareSettings.Success(false);
  } else if (connectedSuccess) {
    GXOVnT.config->Settings.CheckFirmwareSettings.StatusCode(200);
    GXOVnT.config->Settings.CheckFirmwareSettings.StatusMessage("OK");
    GXOVnT.config->Settings.CheckFirmwareSettings.Success(true);
  }

  GXOVnT.config->Settings.SystemSettings.SystemBootMode(BOOT_MODE_SYSTEM_BLE_MODE);
  GXOVnT.config->saveConfiguration();

  delay(1000);

  ESP.restart();
}

void setup() {
  
  Heltec.begin(true /*DisplayEnable Enable*/, false /*LoRa Disable*/, true /*Serial Enable*/, true /*PABOOST Enable*/, 470E6 /**/);
  Serial.begin(115200);
  Serial.println("Type update in the console to flash latest firmware...");

  
  delay(500);

  String systemVer = "Version " + String(GXOVnT_FIRMWARE_VERSION);

  //Heltec.display->printf("Version %s \n", GXOVnT_FIRMWARE_VERSION);
  Heltec.display->clear();
  Heltec.display->drawString(0, 0, "I Love You");
  Heltec.display->display();
  
  
  delay(1500);

  GXOVnT.config->deleteConfigurationFile();
  GXOVnT.config->readConfiguration();

  Serial.printf("System name: %s \n",  GXOVnT.config->Settings.SystemSettings.SystemName().c_str());
  Serial.printf("Firmware version %s \n",  GXOVnT.config->Settings.SystemSettings.FirmwareVersion().c_str());
  Serial.printf("System Id: %s \n",  GXOVnT.config->Settings.SystemSettings.SystemId().c_str());
  Serial.printf("System Type: %d \n",  GXOVnT.config->Settings.SystemSettings.SystemType());
  Serial.printf("WiFi name: %s",  GXOVnT.config->Settings.WiFiSettings.WiFiSsid().c_str());
  
  Serial.printf("Test WiFi name: %s",  GXOVnT.config->Settings.TestWiFiSettings.WiFiSsid().c_str());
  

  GXOVnT_BOOT_MODE bootMode = GXOVnT.config->Settings.SystemSettings.SystemBootMode();

  if (bootMode == BOOT_MODE_TEST_WIFI_MODE) {
    TestWiFiConnection();
  } else if (bootMode == BOOT_MODE_CHECK_FIRMWARE) {
    CheckFirmwareUpdates();
  } else {
    GXOVnT.commService->Start();
  }
  
  

}

void loop() {

 
  delay(200);
  

}


