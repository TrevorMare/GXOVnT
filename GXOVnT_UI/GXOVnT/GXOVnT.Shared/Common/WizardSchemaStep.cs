namespace GXOVnT.Shared.Common;

public class WizardSchemaStep : StateObject
{

    #region Members
    private bool _isCurrentStep;
    private string _stepTitle = string.Empty;
    private string _stepName = string.Empty;
    
    private bool _isCancelButtonVisible = true;
    private bool _isBackButtonVisible = true;
    private bool _isNextButtonVisible = true;
    private bool _isCancelButtonEnabled = false;
    private bool _isBackButtonEnabled = false;
    private bool _isNextButtonEnabled = false;
    private bool _isEnabled = true;    

    private string _cancelButtonText = "Cancel";
    private string _backButtonText = "Previous";
    private string _nextButtonText = "Next";
    #endregion

    #region Properties

    public bool IsCurrentStep
    {
        get => _isCurrentStep;
        set => SetField(ref _isCurrentStep, value);
    }
    
    public string StepTitle
    {
        get => _stepTitle;
        set => SetField(ref _stepTitle, value);
    }

    public string StepName
    {
        get => _stepName;
        set => SetField(ref _stepName, value);
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
    
    public bool IsEnabled
    {
        get => _isEnabled;
        set => SetField(ref _isEnabled, value);
    }
    
    public bool IsCancelButtonVisible
    {
        get => _isCancelButtonVisible;
        set => SetField(ref _isCancelButtonVisible, value);
    }
    
    public bool IsBackButtonVisible
    {
        get => _isBackButtonVisible;
        set => SetField(ref _isBackButtonVisible, value);
    }
    
    public bool IsNextButtonVisible
    {
        get => _isNextButtonVisible;
        set => SetField(ref _isNextButtonVisible, value);
    }

    public bool IsCancelButtonEnabled
    {
        get => _isCancelButtonEnabled && _isEnabled;
        set => SetField(ref _isCancelButtonEnabled, value);
    }
    
    public bool IsBackButtonEnabled
    {
        get => _isBackButtonEnabled && _isEnabled;
        set => SetField(ref _isBackButtonEnabled, value);
    }
    
    public bool IsNextButtonEnabled
    {
        get => _isNextButtonEnabled && _isEnabled;
        set => SetField(ref _isNextButtonEnabled, value);
    }

    public WizardSchemaStepType? WizardSchemaStepType => Enumeration.FromName<WizardSchemaStepType>(StepName);
    #endregion

    #region Methods

    public void ApplyNewState(WizardSchemaStep newStep)
    {
        BackButtonText = newStep.BackButtonText;
        CancelButtonText = newStep.CancelButtonText;
        NextButtonText = newStep.NextButtonText;
        IsBackButtonEnabled = newStep.IsBackButtonEnabled;
        IsCancelButtonEnabled = newStep.IsCancelButtonEnabled;
        IsNextButtonEnabled = newStep.IsNextButtonEnabled;
        IsBackButtonVisible = newStep.IsBackButtonVisible;
        IsCancelButtonVisible = newStep.IsCancelButtonVisible;
        IsNextButtonVisible = newStep.IsNextButtonVisible;
    }

    #endregion
    
}