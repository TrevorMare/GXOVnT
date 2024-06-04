using GXOVnT.Shared.Common;

namespace GXOVnT.Shared.JsonModels;

public class KeepAliveModel : BaseModel
{
    public KeepAliveModel()
    {
        MessageTypeId = (int)JsonModelType.KeepAlive;
    }
}