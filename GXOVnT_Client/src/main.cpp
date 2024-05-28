#define GXOVnT_DEBUG_ENABLE

#include <Arduino.h>

#include <GXOVnT_WebUpdate.h>
#include <heltec.h>
#include <GXOVnT.h>
// This is required to get the nanopb library into the dependency graph

#include <pb.h>


// #include <pb_decode.h>
// #include <pb_common.h>

//https://medium.com/@adityabangde/esp32-firmware-updates-from-github-a-simple-ota-solution-173a95f4a97b

// put function declarations here:
//int myFunction(int, int);

// GXOVnT_TPMS_BLE_Manager *m_bleManager = new GXOVnT_TPMS_BLE_Manager();
// GXOVnT_WebUpdate *updater = new GXOVnT_WebUpdate();
int scanTime = 10; //In seconds


bool decode_pb_string(pb_istream_t *stream, const pb_field_t *field, void **arg)
{
    //https://github.com/nanopb/nanopb/blob/master/tests/callbacks/decode_callbacks.c#L10
    uint8_t buffer[1024] = {0};

    /* We could read block-by-block to avoid the large buffer... */
    if (stream->bytes_left > sizeof(buffer) - 1)
        return false;
    
    

    ESP_LOGW(LOG_TAG, "Reading from the stream into buffer");
    if (!pb_read(stream, buffer, stream->bytes_left))
        return false;
    /* Print the string, in format comparable with protoc --decode.
     * Format comes from the arg defined in main().
     */
    ESP_LOGW(LOG_TAG, "Printing the value");
    printf((char*)*arg, buffer);
    return true;
}

bool encode_string(pb_ostream_t *stream, const pb_field_t *field, void * const *arg)
{
    std::string dataToEncode = "Ensure that the device is on before running the scan. Once a device has been detected, ";
    const char *str = dataToEncode.c_str();
    
    if (!pb_encode_tag_for_field(stream, field))
        return false;
    
    return pb_encode_string(stream, (uint8_t*)str, strlen(str));
}


void testProto() {



}


void setup() {
  
   uint8_t buffer[512];
    size_t count = fread(buffer, 1, sizeof(buffer), stdin);
    pb_istream_t stream = pb_istream_from_buffer(buffer, count);

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
  Serial.printf("System Type: %d \n", GXOVnTConfig.Settings.SystemSettings.SystemType());
  Serial.printf("WiFi name: %s", GXOVnTConfig.Settings.WiFiSettings.SSID().c_str());
  
 
  
  GXOVnTCommService.start();
  



}

void loop() {

    //GXOVnTCommService.run();

  // put your main code here, to run repeatedly:
  if (Serial.available() && Serial.readString() == "update") {

    // Serial.println("Starting updates");
//testProto();
    //updater->checkForUpdatesAndInstall();
  }
  //m_bleManager->run();
  

}

