#include "GXOVnTRoot.h"

using namespace GXOVnTLib::services;
using namespace GXOVnTLib::settings;

GXOVnTRoot::GXOVnTRoot() {
  if (commService == NULL) {
    commService = new GXOVnTLib::services::CommService();
  }    
	if (config == NULL) {
    config = new GXOVnTLib::settings::Config();
  }    
}

GXOVnTRoot GXOVnT;