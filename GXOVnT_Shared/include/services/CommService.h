/////////////////////////////////////////////////////////////////
#pragma once
#ifndef _GXOVNT_COMMSERVICE_H_
#define _GXOVNT_COMMSERVICE_H_

#include "BLECommService.h"

namespace GXOVnT {
    namespace services {

        class CommService
        {
            private:
                /* data */
                BleCommService *m_BleCommService = nullptr;


            public:
                CommService();
                ~CommService();

                void start();
                void stop();
        };
    }
}







#endif