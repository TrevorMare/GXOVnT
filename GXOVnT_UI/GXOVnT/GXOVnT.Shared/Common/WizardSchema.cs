using System.ComponentModel;
using GXOVnT.Shared.Interfaces;

namespace GXOVnT.Shared.Common;

public abstract class WizardSchema : StateObject
{

    #region Members

    public event EventHandler? OnWizardComplete;
    
    protected readonly List<WizardSchemaStep> WizardSchemaSteps = new();
    
    protected WizardStepDirection StepDirection { get; set; } = WizardStepDirection.None;

    #endregion
    
    #region Properties
    public WizardSchemaStep? CurrentStep => WizardSchemaSteps.Find(s => s.IsCurrentStep);
    #endregion

    #region Public Methods

    public virtual Task OnCancelWizard() { return Task.CompletedTask; }
    
    public async Task GoToPreviousStep()
    {
        try
        {
            StepDirection = WizardStepDirection.Back;
            await PreviousStep();
        }
        catch (Exception ex)
        {
            LogService.LogError($"An error occured on the wizard previous step function {ex.Message}");
        }
        finally
        {
            StepDirection = WizardStepDirection.None;
        }
    }

    public async Task GoToNextStep()
    {
        try
        {
            StepDirection = WizardStepDirection.Next;
            await NextStep();
        }
        catch (Exception ex)
        {
            LogService.LogError($"An error occured on the wizard previous step function {ex.Message}");
        }
        finally
        {
            StepDirection = WizardStepDirection.None;
        }
    }

    /// <summary>
    /// Adds a wizard step to the model. This is used to bind the view model and
    /// the wizard together
    /// </summary>
    /// <param name="wizardStepWizardSchemaStep"></param>
    public void AddWizardStep(WizardSchemaStep wizardStepWizardSchemaStep)
    {
        // If we don't already have this step added, add it now
        if (WizardSchemaSteps.Contains(wizardStepWizardSchemaStep))
            return;
        
        WizardSchemaSteps.Add(wizardStepWizardSchemaStep);
        // Add the properties changed so that we can raise them
        wizardStepWizardSchemaStep.PropertyChanged += WizardStepWizardSchemaStepOnPropertyChanged;
        // Call the wizard step added virtual method. This method should be used to set the initial properties
        // on the wizard step 
        OnWizardStepAdded(wizardStepWizardSchemaStep);
    }
    #endregion

    #region Protected Methods
    /// <summary>
    /// This method should be implemented to initialize the specifics of the wizard step e.g. first step and button visibility. 
    /// </summary>
    /// <param name="wizardStepWizardSchemaStep"></param>
    protected virtual void OnWizardStepAdded(WizardSchemaStep wizardStepWizardSchemaStep) { }
    
    /// <summary>
    /// Checks if the current step is of the requested type
    /// </summary>
    /// <param name="wizardSchemaStepType"></param>
    /// <returns></returns>
    protected bool CurrentStepIsType(WizardSchemaStepType wizardSchemaStepType) =>
        (CurrentStep != null && (CurrentStep.WizardSchemaStepType?.Id ?? -1) == wizardSchemaStepType.Id); 
    
    protected void SetWizardComplete()
    {
        OnWizardComplete?.Invoke(this, EventArgs.Empty);
    }

    protected abstract Task PreviousStep();
    
    protected abstract Task NextStep();

    protected virtual void SetStepAsCurrent(WizardSchemaStepType wizardSchemaStepType)
    {
        // Clear the current step
        WizardSchemaSteps.ForEach(s => s.IsCurrentStep = false);

        var stepToSetCurrent =
            WizardSchemaSteps.Find(s => (s.WizardSchemaStepType?.Id ?? -1) == wizardSchemaStepType.Id);

        if (stepToSetCurrent == null)
            return;
        
        stepToSetCurrent.IsCurrentStep = true;
    }
    #endregion
    
    #region Event Callback
    /// <summary>
    /// Common event handler for when a view model busy state has changed. If the view model is busy, we need to update the
    /// enabled indicator on the view model.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    protected void ViewModelOnBusyStateChanged(object sender, BusyStateChangedArgs args)
    {
        SetBusyState(args.IsBusy, args.BusyStatus);
        
        if (CurrentStep != null)
            CurrentStep.IsEnabled = !args.IsBusy;
    }

    
    private void WizardStepWizardSchemaStepOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        OnStateChanged();
    }

    #endregion

}