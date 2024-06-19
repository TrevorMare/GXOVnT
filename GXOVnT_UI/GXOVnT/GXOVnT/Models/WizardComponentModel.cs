using System.ComponentModel;
using GXOVnT.Shared.Common;

namespace GXOVnT.Models;

public class WizardComponentModel : NotifyChanged
{

    #region Members

    private List<WizardStepModel> _wizardSteps = new();
    private bool _isBusy;
    private int _currentWizardStepSequence;
    private WizardStepModel? _currentWizardStep;

    #endregion

    #region Properties
    /// <summary>
    /// Gets or sets the wizard start step
    /// </summary>
    public int WizardStartStep { get; set; }

    /// <summary>
    /// Gets the current wizard step
    /// </summary>
    public int CurrentWizardStepSequence
    {
        get => _currentWizardStepSequence;
        private set => SetField(ref _currentWizardStepSequence, value);
    }
    
    public WizardStepModel? CurrentWizardStep
    {
        get => _currentWizardStep;
        private set => SetField(ref _currentWizardStep, value);
    }
    
    /// <summary>
    /// Gets a value indicating if any step is busy
    /// </summary>
    public bool IsBusy
    {
        get => _isBusy;
        private set => SetField(ref _isBusy, value);
    }
    
    /// <summary>
    /// Gets a list of registered wizard step models
    /// </summary>
    public IReadOnlyList<WizardStepModel> WizardSteps
    {
        get => _wizardSteps.ToList();
        private set => SetField(ref _wizardSteps, value.ToList());
    }

    #endregion

    #region Methods

  
    
    public void AddWizardStep(WizardStepModel wizardStepModel)
    {
        var uuId = wizardStepModel.WizardStepUUId;
        
        var existingItem = _wizardSteps.Find(m =>
            m.WizardStepUUId.Equals(uuId));

        if (existingItem != null) return;

        AddAndAttachToModel(wizardStepModel);
       
        OnPropertyChanged(nameof(WizardSteps));
    }

    public WizardStepModel? GetWizardStep(string wizardStepName)
    {
        return _wizardSteps.Find(s =>
            !string.IsNullOrWhiteSpace(s.WizardStepName) && s.WizardStepName.Equals(wizardStepName));
    }
    
    public bool GotoStep(string wizardStepName)
    {
        var requestedWizardStep = GetWizardStep(wizardStepName);
        return requestedWizardStep != null && GotoStep(requestedWizardStep.StepSequence);
    }

    public bool GotoStep(int stepOrderSequence)
    {
        var requestedOrderStep = _wizardSteps.Find(s => s.StepSequence == stepOrderSequence);
        if (requestedOrderStep == null)
            return false;

        if (stepOrderSequence == _currentWizardStepSequence)
            return false;
        
        _wizardSteps.ForEach(s => s.IsCurrentStep = false);
        requestedOrderStep.IsCurrentStep = true;
        
        CurrentWizardStepSequence = stepOrderSequence;
        CurrentWizardStep = requestedOrderStep;

        return true;
    }

    public bool GotoPreviousStep()
    {
        var previousSequenceModel = _wizardSteps
            .OrderBy(x => x.StepSequence)
            .LastOrDefault(m => m.StepSequence < _currentWizardStepSequence && m.IsEnabled);

        if (previousSequenceModel == null)
            return false;

        return GotoStep(previousSequenceModel.StepSequence);
    }

    public bool GotoNextStep()
    {
        var nextSequenceModel = _wizardSteps
            .OrderBy(x => x.StepSequence)
            .FirstOrDefault(m => m.StepSequence > _currentWizardStepSequence && m.IsEnabled);

        if (nextSequenceModel == null)
            return false;
        
        return GotoStep(nextSequenceModel.StepSequence);
    }
    #endregion

    #region Private Methods

    private void AddAndAttachToModel(WizardStepModel wizardStepModel)
    {
        // Attach the event handler
        wizardStepModel.PropertyChanged += WizardStepModelOnPropertyChanged;
        
        // Find other models with the same sequence as the one passed
        var sameStepSequenceModel = _wizardSteps.Find(m => m.StepSequence == wizardStepModel.StepSequence);
        // If there is a model with the same sequence, we need to use the next sequence number
        if (sameStepSequenceModel != null)
        {
            var maxSequence = _wizardSteps.Max(m => m.StepSequence)! + 1;
            wizardStepModel.StepSequence = maxSequence;
        }
        
        // Now add the step
        _wizardSteps.Add(wizardStepModel);

        // If this is the start sequence of the wizard, set this model to display
        if (wizardStepModel.StepSequence != WizardStartStep) return;
        
        CurrentWizardStepSequence = WizardStartStep;
        CurrentWizardStep = wizardStepModel;
        wizardStepModel.IsCurrentStep = true;

    }

    private void WizardStepModelOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {

        IsBusy = _wizardSteps.Exists(s => s.IsBusy);

        OnPropertyChanged(nameof(WizardSteps));
    }

    #endregion

}