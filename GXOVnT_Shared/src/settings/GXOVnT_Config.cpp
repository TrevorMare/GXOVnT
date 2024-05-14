#include "settings/GXOVnT_Config.h"

GXOVnT_Config::GXOVnT_Config() {}

GXOVnT_Config::~GXOVnT_Config() {
    disposeState();
}

bool GXOVnT_Config::initializeState() {
    // Check if the state is already initialized
    if (m_initialized) return true;

    if(!SPIFFS.begin(FORMAT_SPIFFS_IF_FAILED)){
        ESP_LOGE(LOG_TAG, "SPIFFS mount failed. Unable to continue");
        m_initialized = false;
        return false;
    }
    else{
        ESP_LOGI(LOG_TAG, "SPIFFS mount succeeded");
        m_initialized = true;
        return true;
    }
}

bool GXOVnT_Config::disposeState() {
    // Not initialized, return false
    if (!m_initialized) return m_initialized;
    // Close the SPIFFS
    SPIFFS.end();
    m_initialized = false;
    return true;
}

bool GXOVnT_Config::configurationFileExists() {
    if (!m_initialized) {
        ESP_LOGE(LOG_TAG, "The state must first be initialized before calling any other methods");
        return false;
    }
    return SPIFFS.exists(config_file_name);
}

String GXOVnT_Config::readConfigurationContent() {
    // Check if the configuration file exists
    if (!configurationFileExists()) return "";
    
    // Open the file
    File file = SPIFFS.open(config_file_name, FILE_READ);

    // Check if the file is not null    
    if (!file) { return ""; }

    // Read the content of the file
    String fileData = "";
    if (file.available()) {
        fileData = file.readString();
    }
    // Close the file
    file.close();
    // Return the content
    return fileData;
}