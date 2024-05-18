/////////////////////////////////////////////////////////////////
#pragma once
#ifndef _GXOVNT_STATE_MANAGER_H
#define _GXOVNT_STATE_MANAGER_H

/////////////////////////////////////////////////////////////////
#include "shared/Shared.h"
#include "shared/GXOVnT_State.h"
#include "GXOVnT_State_InitialState.h"


/////////////////////////////////////////////////////////////////

/////////////////////////////////////////////////////////////////

class GXOVnT_StateManager {
    public:
        GXOVnT_StateManager();
        GXOVnT_StateManager(GXOVnT_State *initialState);
        ~GXOVnT_StateManager();
    private: 
        GXOVnT_State *m_initialState = NULL;
        GXOVnT_State *m_currentState = NULL;
        GXOVnT_State *m_prevState = NULL;

};

GXOVnT_State_InitialState State_InitialState;

GXOVnT_StateManager StateManager(&State_InitialState);

#endif