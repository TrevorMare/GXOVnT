#define GXOVnT_DEBUG_ENABLE

#include <Arduino.h>

#include <GXOVnT_WebUpdate.h>
#include <heltec.h>
#include <GXOVnTRoot.h>


//https://medium.com/@adityabangde/esp32-firmware-updates-from-github-a-simple-ota-solution-173a95f4a97b


int scanTime = 10; //In seconds

void setup() {
  
  Heltec.begin(true /*DisplayEnable Enable*/, false /*LoRa Disable*/, true /*Serial Enable*/, true /*PABOOST Enable*/, 470E6 /**/);
  Serial.begin(115200);
  Serial.println("Type update in the console to flash latest firmware...");

  
  delay(500);

  String systemVer = "Version " + String(GXOVnT_FIRMWARE_VERSION);

  //Heltec.display->printf("Version %s \n", GXOVnT_FIRMWARE_VERSION);
  Heltec.display->clear();
  Heltec.display->drawString(0, 0, "Initializing device");
  Heltec.display->display();
  
  GXOVnT.Initialize();
  

}

void loop() {

 
  delay(200);
  

}


