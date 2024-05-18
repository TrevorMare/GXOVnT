// Root class to provide functionality to the calling library. 
// This provides the setup and run 
/////////////////////////////////////////////////////////////////
#pragma once
#ifndef _GXOVNT_H_
#define _GXOVNT_H_

#include "settings/Config.h"
#include "services/CommService.h"
using namespace GXOVnT::services;
using namespace GXOVnT::settings;



extern Config GXOVnTConfig;
extern CommService GXOVnTCommService;


#endif


