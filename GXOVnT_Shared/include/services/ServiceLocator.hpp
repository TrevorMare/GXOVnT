/////////////////////////////////////////////////////////////////
#pragma once
#ifndef _GXOVNT_SERVICELOCATOR_H_
#define _GXOVNT_SERVICELOCATOR_H_

#include "shared/Definitions.h"
#include "settings/Config.h"
#include "services/CommService.h"
#include "services/BLECommService.h"
#include "services/JsonMessageService.hpp"

using namespace GXOVnT::settings;

static GXOVnT::services::CommService CommService;

namespace GXOVnT 
{
    namespace services
    {

        class ServiceLocator
        {
        private:
            static Config *m_config;
            static CommService *m_commService;            
            static JsonMessageService *m_jsonMessageService;
            static BleCommService *m_bleCommService;
        public:
            static void Initialize() {
                if (m_config == NULL) m_config = new Config();
                if (m_commService == NULL) m_commService = new CommService();
                if (m_jsonMessageService == NULL) m_jsonMessageService = new JsonMessageService();
                if (m_bleCommService == NULL) m_bleCommService = new BleCommService();
            }
            static Config *GXOVnTConfig() { return m_config; }
            static CommService *GXOVnTCommService() { return m_commService; }
            static JsonMessageService *GXOVnTJsonMessageService() { return m_jsonMessageService; }
            static BleCommService *GXOVnTBleCommService() { return m_bleCommService; }
            static void ProvideGXOVnTConfig(Config *config) { m_config = config; }
            static void ProvideGXOVnTCommService(CommService *commService) { m_commService = commService; }
            static void ProvideGXOVnTJsonMessageService(JsonMessageService *jsonMessageService) { m_jsonMessageService = jsonMessageService; }
            static void ProvideGXOVnTBleCommService(BleCommService *bleCommService) { m_bleCommService = bleCommService; }
        };
    }
}
#endif