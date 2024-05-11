#define GXOVnT_DEBUG_ENABLE

#include <Arduino.h>
#include <GXOVnT_TPMS_BLE_Manager.h>
#include <heltec.h>
//https://medium.com/@adityabangde/esp32-firmware-updates-from-github-a-simple-ota-solution-173a95f4a97b

// put function declarations here:
int myFunction(int, int);

GXOVnT_TPMS_BLE_Manager *m_bleManager = new GXOVnT_TPMS_BLE_Manager();
int scanTime = 10; //In seconds


void setup() {
  // put your setup code here, to run once:
  int result = myFunction(2, 3);

  Heltec.begin(true /*DisplayEnable Enable*/, false /*LoRa Disable*/, true /*Serial Enable*/, true /*PABOOST Enable*/, 470E6 /**/);
  Serial.begin(115200);
  Serial.println("Scanning...");

  m_bleManager->initializeManager();


  

}

void loop() {
  // put your main code here, to run repeatedly:
  
  m_bleManager->run();
  delay(2000);

}

// put function definitions here:
int myFunction(int x, int y) {
  return x + y;
}