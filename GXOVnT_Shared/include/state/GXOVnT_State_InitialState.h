/////////////////////////////////////////////////////////////////
#pragma once
#ifndef _GXOVNT_STATE_INITIAL_STATE_H
#define _GXOVNT_STATE_INITIAL_STATE_H

#include "shared/GXOVnT_State.h"

class GXOVnT_State_InitialState : public GXOVnT_State {

    public:
        GXOVnT_State_InitialState() : GXOVnT_State::GXOVnT_State(GXOVNT_STATES_INITIAL_STATE) {} 

};



#endif