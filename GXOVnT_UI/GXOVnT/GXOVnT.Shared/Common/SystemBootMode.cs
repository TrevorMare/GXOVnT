namespace GXOVnT.Shared.Common;

public class SystemBootMode : Enumeration
{
    public static readonly SystemBootMode SystemBleMode = new(0, nameof(SystemBleMode));
    public static readonly SystemBootMode TestWiFiMode = new(1, nameof(TestWiFiMode));
    public static readonly SystemBootMode CheckFirmwareMode = new(2, nameof(CheckFirmwareMode));

    private SystemBootMode(int id, string name)
        : base(id, name)
    {
    }
}