using GXOVnT.Shared.Common;

namespace GXOVnT.ViewModels.Wizards;

public class WizardStepModel : NotifyChanged
{

    #region Members
    private readonly string _wizardStepUUId = Guid.NewGuid().ToString();
    private string? _wizardStepName; 
    private bool _isCurrentStep;
    private string? _stepTitle = string.Empty;
    private int _stepSequence;
    private bool _hasCancelButton = true;
    private bool _cancelEnabled = true;
    private bool _hasBackButton = true;
    private bool _backEnabled = true;
    private bool _hasForwardButton = true;
    private bool _forwardEnabled = true;
    private string _cancelButtonText = "Cancel";
    private string _backButtonText = "Previous";
    private string _nextButtonText = "Next";
    private bool _isBusy;
    private string? _busyText;
    private bool _isEnabled = true;
    
    #endregion
    
    #region Properties

    public string WizardStepUUId => _wizardStepUUId;

    public string? WizardStepName
    {
        get => _wizardStepName;
        set => SetField(ref _wizardStepName, value);
    }

    public bool IsEnabled
    {
        get => _isEnabled;
        set => SetField(ref _isEnabled, value);
    }
    
    public bool IsBusy
    {
        get => _isBusy;
        set => SetField(ref _isBusy, value);
    }
    
    public string? BusyText
    {
        get => _busyText;
        set => SetField(ref _busyText, value);
    }
    
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
    
    public bool CancelEnabled 
    {
        get => _cancelEnabled;
        set => SetField(ref _cancelEnabled, value);
    }
    
    public bool HasBackButton 
    {
        get => _hasBackButton;
        set => SetField(ref _hasBackButton, value);
    }
    
    public bool BackEnabled
    {
        get => _backEnabled;
        set => SetField(ref _backEnabled, value);
    }
    
    public bool HasForwardButton
    {
        get => _hasForwardButton;
        set => SetField(ref _hasForwardButton, value);
    }
    
    public bool ForwardEnabled
    {
        get => _forwardEnabled;
        set => SetField(ref _forwardEnabled, value);
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

    #region Methods

    public void SetIsBusy(bool isBusy, string? busyText = null)
    {
        IsBusy = isBusy;
        BusyText = isBusy ? busyText : string.Empty;
    }

    #endregion
    
}