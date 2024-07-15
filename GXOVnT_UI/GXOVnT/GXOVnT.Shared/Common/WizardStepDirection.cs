namespace GXOVnT.Shared.Common;

public class WizardStepDirection : Enumeration
{
    
    public static readonly WizardStepDirection None = new(0, nameof(None));
    public static readonly WizardStepDirection Back = new(1, nameof(Back));
    public static readonly WizardStepDirection Next = new(2, nameof(Next));

    private WizardStepDirection(int id, string name)
        : base(id, name)
    {
    }
    
}