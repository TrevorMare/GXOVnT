using GXOVnT.Services.Common;

namespace GXOVnT.Models;

public class WizardStepModel : NotifyChanged
{

    #region Members
    private readonly string _wizardStepUUId = Guid.NewGuid().ToString();
    private bool _isCurrentStep;

    private string? _stepTitle = string.Empty;
    private int _stepSequence = 0;
    private bool _hasCancelButton = true;
    private bool _canCancelButton = true;
    private bool _hasBackButton = true;
    private bool _canBackButton = true;
    private bool _hasForwardButton = true;
    private bool _canForwardButton = true;
    private string _cancelButtonText = "Cancel";
    private string _backButtonText = "Previous";
    private string _nextButtonText = "Next";
    
    #endregion
    
    #region Properties

    public string WizardStepUUId => _wizardStepUUId;

    public int StepSequence
    {
        get => _stepSequence;
        set => SetField(ref _stepSequence, value);
    }
    
    public string? StepTitle
    {
        get => _stepTitle;
        set => SetField(ref _stepTitle, value);
    }
    
    public bool HasCancelButton
    {
        get => _hasCancelButton;
        set => SetField(ref _hasCancelButton, value);
    }
    
    public bool CanCancelButton 
    {
        get => _canCancelButton;
        set => SetField(ref _canCancelButton, value);
    }
    
    public bool HasBackButton 
    {
        get => _hasBackButton;
        set => SetField(ref _hasBackButton, value);
    }
    
    public bool CanBackButton
    {
        get => _canBackButton;
        set => SetField(ref _canBackButton, value);
    }
    
    public bool HasForwardButton
    {
        get => _hasForwardButton;
        set => SetField(ref _hasForwardButton, value);
    }
    
    public bool CanForwardButton
    {
        get => _canForwardButton;
        set => SetField(ref _canForwardButton, value);
    }

    public string CancelButtonText
    {
        get => _cancelButtonText;
        set => SetField(ref _cancelButtonText, value);
    }
    
    public string BackButtonText
    {
        get => _backButtonText;
        set => SetField(ref _backButtonText, value);
    }
    
    public string NextButtonText
    {
        get => _nextButtonText;
        set => SetField(ref _nextButtonText, value);
    }

    public bool IsCurrentStep
    {
        get => _isCurrentStep;
        set => SetField(ref _isCurrentStep, value);
    }
    #endregion
    
}