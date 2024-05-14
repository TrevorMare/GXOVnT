/////////////////////////////////////////////////////////////////
#pragma once
#ifndef _GXOVNT_STATE_MANAGER_H
#define _GXOVNT_STATE_MANAGER_H

/////////////////////////////////////////////////////////////////
#include "GXOVnT_Shared.h"
#include "GXOVnT_State.h"


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

#endif