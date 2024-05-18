// Root class to provide functionality to the calling library. 
// This provides the setup and run 
/////////////////////////////////////////////////////////////////
#pragma once
#ifndef _GXOVNT_H_
#define _GXOVNT_H_

#include "settings/Config.h"
#include "services/CommService.h"





GXOVnT::settings::Config GXOVnTConfig;
GXOVnT::services::CommService CommService;
#endif


