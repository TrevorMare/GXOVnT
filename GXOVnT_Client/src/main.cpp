#define GXOVnT_DEBUG_ENABLE

#include <Arduino.h>
#include <GXOVnT_TPMS_BLE_Manager.h>
#include <GXOVnT_AutoUpdate.h>
#include <heltec.h>
//https://medium.com/@adityabangde/esp32-firmware-updates-from-github-a-simple-ota-solution-173a95f4a97b

// put function declarations here:
//int myFunction(int, int);

GXOVnT_TPMS_BLE_Manager *m_bleManager = new GXOVnT_TPMS_BLE_Manager();
GVOVnT_AutoUpdate *updater = new GVOVnT_AutoUpdate();
int scanTime = 10; //In seconds


void setup() {
  
  

  Heltec.begin(true /*DisplayEnable Enable*/, false /*LoRa Disable*/, true /*Serial Enable*/, true /*PABOOST Enable*/, 470E6 /**/);
  Serial.begin(115200);
  Serial.println("Type update in the console to flash latest firmware...");

  //m_bleManager->initializeManager();
  delay(500);

  String systemVer = "Version " + String(SoftwareVersion);

  //Heltec.display->printf("Version %s \n", SoftwareVersion);
  Heltec.display->clear();
  Heltec.display->drawString(0, 0, systemVer);
  Heltec.display->display();
  

}

void loop() {
  // put your main code here, to run repeatedly:
  if (Serial.available() && Serial.readString() == "update") {
    updater->checkForUpdatesAndInstall();
  }
  //m_bleManager->run();
  delay(2000);

}

