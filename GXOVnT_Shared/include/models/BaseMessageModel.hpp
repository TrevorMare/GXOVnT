/////////////////////////////////////////////////////////////////
#pragma once
#ifndef _GXOVNT_MODELS_BASEMESSAGEMODEL_H_
#define _GXOVNT_MODELS_BASEMESSAGEMODEL_H_
#include <ArduinoJson.h>
#include "JsonMessageConstants.h"
#include "shared/Definitions.h"

using namespace GXOVnTLib::models::constants;

namespace GXOVnTLib::models {

class BaseMessageModel
{
    /// @brief Value for the type of model
    uint8_t m_messageTypeId = 0;
    /// @brief Value to indicate that this model is a response to another model
    uint16_t m_replyMessageId = 0;

protected:
    /// @brief A reference to the json document for this model
    JsonDocument m_jsonDocument; 
    void MessageTypeId(uint8_t value) { 
        m_messageTypeId = value; 
        m_jsonDocument[JsonFieldMessageTypeId] = m_messageTypeId;
    }
    void ReplyMessageId(uint16_t value) { 
        m_replyMessageId = value;
        m_jsonDocument[JsonFieldReplyMessageId] = m_replyMessageId;            
    }
public:
    /// @brief Default constructor
    BaseMessageModel() {};
    /// @brief Constructs a new model with the specified message type Id
    /// @param messageTypeId 
    BaseMessageModel(uint8_t messageTypeId, uint16_t replyMessageId = 0) { 
        MessageTypeId(messageTypeId);
        ReplyMessageId(replyMessageId);
    };
    BaseMessageModel(JsonDocument &doc, uint16_t replyMessageId = 0) { 
        if (doc != nullptr) {
            m_jsonDocument = doc;

            if (m_jsonDocument.containsKey(JsonFieldMessageTypeId)) {
                m_messageTypeId = m_jsonDocument[JsonFieldMessageTypeId];
            }

            if (m_jsonDocument.containsKey(JsonFieldReplyMessageId)) {
                m_replyMessageId = m_jsonDocument[JsonFieldReplyMessageId];
            }
        }
    };
    ~BaseMessageModel() {};
    JsonDocument *Json() { return &m_jsonDocument; }
    uint8_t MessageTypeId() { return m_messageTypeId; }
    uint16_t ReplyMessageId() { return m_replyMessageId; }
};

}

#endif