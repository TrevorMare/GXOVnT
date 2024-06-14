#define GXOVnT_DEBUG_ENABLE

#include <Arduino.h>

#include <GXOVnT_WebUpdate.h>
#include <heltec.h>
#include <GXOVnTRoot.h>


//https://medium.com/@adityabangde/esp32-firmware-updates-from-github-a-simple-ota-solution-173a95f4a97b

// GXOVnT_TPMS_BLE_Manager *m_bleManager = new GXOVnT_TPMS_BLE_Manager();
// GXOVnT_WebUpdate *updater = new GXOVnT_WebUpdate();
int scanTime = 10; //In seconds



void setup() {
  
  Heltec.begin(true /*DisplayEnable Enable*/, false /*LoRa Disable*/, true /*Serial Enable*/, true /*PABOOST Enable*/, 470E6 /**/);
  Serial.begin(115200);
  Serial.println("Type update in the console to flash latest firmware...");

  //m_bleManager->initializeManager();
  delay(500);

  String systemVer = "Version " + String(GXOVnT_FIRMWARE_VERSION);

  //Heltec.display->printf("Version %s \n", GXOVnT_FIRMWARE_VERSION);
  Heltec.display->clear();
  Heltec.display->drawString(0, 0, "I Love You");
  Heltec.display->display();
  
  
  delay(1500);

  GXOVnT.config->readConfiguration();

  Serial.printf("System name: %s \n",  GXOVnT.config->Settings.SystemSettings.SystemName().c_str());
  Serial.printf("Firmware version %s \n",  GXOVnT.config->Settings.SystemSettings.FirmwareVersion().c_str());
  Serial.printf("System Id: %s \n",  GXOVnT.config->Settings.SystemSettings.SystemId().c_str());
  Serial.printf("System Type: %d \n",  GXOVnT.config->Settings.SystemSettings.SystemType());
  Serial.printf("WiFi name: %s",  GXOVnT.config->Settings.WiFiSettings.SSID().c_str());
  
  Serial.printf("Test WiFi name: %s",  GXOVnT.config->Settings.TestWiFiSettings.SSID().c_str());
  Serial.printf("Should Test WiFi: %s", GXOVnT.config->Settings.TestWiFiSettings.TestOnNextBoot() ? "true" : "false");

  if (GXOVnT.config->Settings.TestWiFiSettings.TestOnNextBoot()) {
    const char *ssid = GXOVnT.config->Settings.TestWiFiSettings.SSID().c_str();
    const char *password = GXOVnT.config->Settings.TestWiFiSettings.Password().c_str();

    WiFi.begin(ssid, password);
    bool attemptingConnect = true;

    while (attemptingConnect) {
      if (WiFi.status() == WL_CONNECT_FAILED) {
        attemptingConnect = false;
        GXOVnT.config->Settings.TestWiFiSettings.TestResultCode(244);
        GXOVnT.config->Settings.TestWiFiSettings.TestResultMessage("Connection Failed");
        GXOVnT.config->Settings.TestWiFiSettings.Success(false);
      } 
      else if (WiFi.status() == WL_CONNECTED) { 
        attemptingConnect = false;
        GXOVnT.config->Settings.TestWiFiSettings.TestResultCode(200);
        GXOVnT.config->Settings.TestWiFiSettings.TestResultMessage("Connection Success");
        GXOVnT.config->Settings.TestWiFiSettings.Success(true);
      }

      if (!attemptingConnect) {
        GXOVnT.config->Settings.TestWiFiSettings.TestOnNextBoot(false);
        GXOVnT.config->Settings.TestWiFiSettings.Tested(true);
        GXOVnT.config->saveConfiguration();
      }
    }

    delay(1000);

    ESP.restart();

  } else {
    GXOVnT.commService->start();
  }
  
  

}

void loop() {

 
  delay(200);
  

}

