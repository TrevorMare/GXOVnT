// Root class to provide functionality to the calling library. 
// This provides the setup and run 
/////////////////////////////////////////////////////////////////
#pragma once
#ifndef _GXOVNT_H_
#define _GXOVNT_H_

#include "shared/Definitions.h"
#include "services/ServiceLocator.hpp"
//using namespace GXOVnT::services;

class GXOVnTSystem {
    public:
        static void Initialize() {
            //ServiceLocator::Initialize();
        }
};



#endif


// TODO: Figure out linking