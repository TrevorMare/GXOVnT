/////////////////////////////////////////////////////////////////
#pragma once
#ifndef _GXOVNT_COMMSERVICE_H_
#define _GXOVNT_COMMSERVICE_H_

#include "BLECommService.h"

namespace GXOVnT
{
	namespace services
	{

		class CommService : public BLECommServiceMessageCallback
		{
		private:
			/* data */
			BleCommService *m_BleCommService = nullptr;
			void handleBLEMessage(const uint8_t *buffer, size_t size) override;

		public:
			CommService();
			~CommService();

			void start();
			void stop();
		};
	}
}

#endif