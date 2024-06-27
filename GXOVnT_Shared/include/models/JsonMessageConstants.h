/////////////////////////////////////////////////////////////////
#pragma once
#ifndef _GXOVNT_MODELS_JSONMESSAGECONSTANTS_H_
#define _GXOVNT_MODELS_JSONMESSAGECONSTANTS_H_
#include "shared/Definitions.h"

namespace GXOVnTLib::models::constants {

    // Field name constants for the message json documents
    static const std::string JsonFieldWiFiSsid = "wifiSsid";
    static const std::string JsonFieldWiFiPassword = "wifiPassword";
    static const std::string JsonFieldSuccess = "success";
    static const std::string JsonFieldTested = "tested";
    static const std::string JsonFieldStatusCode = "statusCode";
    static const std::string JsonFieldStatusMessage = "statusMessage";
    static const std::string JsonFieldEchoMessage = "echoMessage";
    static const std::string JsonFieldSystemPassword = "systemPassword";
    static const std::string JsonFieldSystemType = "systemType";
    static const std::string JsonFieldSystemConfigured = "systemConfigured";
    static const std::string JsonFieldSystemName = "systemName";
    static const std::string JsonFieldSystemId = "systemId";
    static const std::string JsonFieldFirmwareVersion = "firmwareVersion";
    static const std::string JsonFieldMessageTypeId = "messageTypeId";
    static const std::string JsonFieldReplyMessageId = "replyMessageId";
    static const std::string JsonFieldFirmwareVersions = "firmwareVersions";
    static const std::string JsonFieldDownloadLocation = "downloadLocation";
    static const std::string JsonFieldFirmwareVersionInstalled = "firmwareVersionInstalled";
    static const std::string JsonFieldVersionNumber = "versionNumber";
    


    // Message type constants
	static const uint8_t MsgType_EchoRequest = 3;
	static const uint8_t MsgType_KeepAliveRequest = 4;
	static const uint8_t MsgType_GetSystemSettingsRequest = 5;
	static const uint8_t MsgType_SetSystemSettingsRequest = 6;
	static const uint8_t MsgType_TestWiFiSettingsRequest = 7;
	static const uint8_t MsgType_GetTestWiFiSettingsResultRequest = 8;
    static const uint8_t MsgType_CheckFirmwareUpdateRequest = 9;
    static const uint8_t MsgType_GetFirmwareUpdateResultRequest = 10;
    static const uint8_t MsgType_DeleteSystemSettingsRequest = 97;
	static const uint8_t MsgType_RebootRequest = 98;
	static const uint8_t MsgType_SaveConfigurationRequest = 99;

	static const uint8_t MsgType_StatusResponse = 101;
	static const uint8_t MsgType_GetSystemSettingsResponse = 102;
	static const uint8_t MsgType_GetTestWiFiSettingsResultResponse = 103;
    static const uint8_t MsgType_EchoResponse = 104;
    static const uint8_t MsgType_GetFirmwareUpdateResultResponse = 105;

}

#endif