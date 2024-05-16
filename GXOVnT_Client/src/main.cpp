#define GXOVnT_DEBUG_ENABLE

#include <Arduino.h>
#include <GXOVnT_TPMS_BLE_Manager.h>
#include <GXOVnT_WebUpdate.h>
#include <heltec.h>
#include <GXOVnT.h>

//https://medium.com/@adityabangde/esp32-firmware-updates-from-github-a-simple-ota-solution-173a95f4a97b

// put function declarations here:
//int myFunction(int, int);

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

  //Heltec.display->printf("Version %s \n", SoftwareVersion);
  Heltec.display->clear();
  Heltec.display->drawString(0, 0, systemVer);
  Heltec.display->display();
  
  
  delay(1500);
  GXOVnTConfig.readConfiguration();

  Serial.printf("System name: %s \n", GXOVnTConfig.Settings.SystemSettings.SystemName().c_str());
  Serial.printf("Firmware version %s \n", GXOVnTConfig.Settings.SystemSettings.FirmwareVersion().c_str());
  Serial.printf("System Id: %s \n", GXOVnTConfig.Settings.SystemSettings.SystemId().c_str());
  Serial.printf("WiFi name: %s", GXOVnTConfig.Settings.WiFiSettings.SSID().c_str());
  
 
  

  



}

void loop() {
  // put your main code here, to run repeatedly:
  if (Serial.available() && Serial.readString() == "update") {

    Serial.println("Starting updates");

    //updater->checkForUpdatesAndInstall();
  }
  //m_bleManager->run();
  delay(2000);

}

