#include "settings/GXOVnT_Config.h"

/////////////////////////////////////////////////////////////////
// Public Methods
/////////////////////////////////////////////////////////////////

GXOVnT_Config::GXOVnT_Config() {}
GXOVnT_Config::~GXOVnT_Config() {
    closeSPIFFS();
}

bool GXOVnT_Config::configurationFileExists() {
    // Open the file system object
    if (!openSPIFFS()) 
        return false;
    bool result = SPIFFS.exists(config_file_name);
    // Close the file system object
    closeSPIFFS();
    return result;
}


bool GXOVnT_Config::readConfiguration() {
    // Try open the file system object
    if (!openSPIFFS()) return false;
    bool result = false;
   
    ESP_LOGI(LOG_TAG, "Reading configuration file");
    // Get the content of the file
    String configurationContent = readConfigurationFromFileSystem();
    // If there is a value
    if (configurationContent != "") {
        JsonDocument document;
        // Deserialize the json document
        deserializeJson(document, configurationContent);
        // If the document could be loaded, read the settings from the document
        if (!document.isNull()) {
            Settings.readSettingsFromJson(document);
            ESP_LOGI(LOG_TAG, "Settings were restored");
            result = true;
        } else {
            ESP_LOGE(LOG_TAG, "Unable to parse the json configuration");
        }
    }
    if (!result)
        ESP_LOGW(LOG_TAG, "The configuration file either didn't exist or did not contain any data");
    // Close the file system object
    closeSPIFFS();
    return result;
}

void GXOVnT_Config::saveConfiguration() {
    // Try open the file system object
    if (!openSPIFFS()) return;
    // Create a new document
    JsonDocument document;

    // Write all the settings to the document
    Settings.writeSettingsToJson(document);
    // Get the json content of the settings
    String jsonContent;

        

    serializeJsonPretty(document, jsonContent);
    // Write the file to the disk
    writeConfigurationToFileSystem(jsonContent);
    // Close file system object
    closeSPIFFS();
}

/////////////////////////////////////////////////////////////////
// Private Methods
/////////////////////////////////////////////////////////////////
bool GXOVnT_Config::openSPIFFS() {
    if (m_SPIFFS_open) return true;
    if(!SPIFFS.begin(FORMAT_SPIFFS_IF_FAILED)){
        ESP_LOGE(LOG_TAG, "SPIFFS mount failed. Unable to continue");
        m_SPIFFS_open = false;
        return false;
    }
    else{
        ESP_LOGI(LOG_TAG, "SPIFFS mount succeeded");
        m_SPIFFS_open = true;
        return true;
    }
}

bool GXOVnT_Config::closeSPIFFS() {
 if (!m_SPIFFS_open) return true;
    ESP_LOGI(LOG_TAG, "SPIFFS mount closed");
    // Close the SPIFFS
    SPIFFS.end();
    m_SPIFFS_open = false;
    return true;
}

String GXOVnT_Config::readConfigurationFromFileSystem() {
    // Check if the configuration file exists
    if (!configurationFileExists()) {
        ESP_LOGW(LOG_TAG, "Unable to read the configuration file from file system. The file does not exist");
        return "";
    } 
     if (!openSPIFFS()) 
        return "";
    // Open the file
    File file = SPIFFS.open(config_file_name, FILE_READ);
    // Check if the file is not null    
    if (!file) { 
        ESP_LOGE(LOG_TAG, "Unable to read the configuration file from file system. There was an error opening the file");
        return ""; 
    }
    // Read the content of the file
    String fileData = "";
    if (file.available()) {
        fileData = file.readString();
    } else {
        ESP_LOGW(LOG_TAG, "Unable to read the configuration file from file system. The file was empty");
    }
    // Close the file
    file.close();

    Serial.println("*********************");
    Serial.println(fileData);
    Serial.println("*********************");

    // Return the content
    return fileData;
}

void GXOVnT_Config::writeConfigurationToFileSystem(String content) {
    ESP_LOGI(LOG_TAG, "Opening the file to write configuration");
    // Open the file
    File file = SPIFFS.open(config_file_name, FILE_WRITE);

    Serial.println("*********************");
    Serial.println("Write content");
    Serial.println(content);

    Serial.println("*********************");

    // Check if the file is not null    
    if (!file) { 
        ESP_LOGI(LOG_TAG, "Could not write configuration, there was an error opening the file for write");
        return; 
    }

    if(file.print(content)) {
        ESP_LOGI(LOG_TAG, "Configuration file written to file system");
        
    } else {
        ESP_LOGE(LOG_TAG, "Configuration file writing failed");
    }

    // Close the file
    file.close();
}