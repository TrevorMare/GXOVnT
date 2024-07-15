namespace GXOVnT.Shared.Common;

public class WizardSchemaStepType : Enumeration
{
    
    public static readonly WizardSchemaStepType EnrollDeviceStartupInformation = new(0, nameof(EnrollDeviceStartupInformation));
    public static readonly WizardSchemaStepType CheckBluetoothPermissions = new(1, nameof(CheckBluetoothPermissions));
    public static readonly WizardSchemaStepType ScanBluetoothSystems = new(2, nameof(ScanBluetoothSystems));
    public static readonly WizardSchemaStepType ConfirmDeviceInformation = new(3, nameof(ConfirmDeviceInformation));
    public static readonly WizardSchemaStepType SetDeviceConfiguration = new(4, nameof(SetDeviceConfiguration));

    private WizardSchemaStepType(int id, string name)
        : base(id, name)
    {
    }
}