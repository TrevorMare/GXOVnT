using Gxovnt.Messaging;

namespace GXOVnT.Shared.Helpers;

public static class ProtoMessageBuilder
{

    public static Gxovnt.Messaging.Container BuildEchoMessage(string echoMessage)
    {
        return new Container()
        {
            MessageTypeId = Container.Types.MessageType_Id.Echo,
            TextValue = echoMessage
        };
    }
    
    
}