using System.ComponentModel;
using GXOVnT.Models;
using Microsoft.AspNetCore.Components;

namespace GXOVnT.Components.Shared.WizardComponent;

public partial class WizardComponent : ComponentBase
{

    #region Members

    private readonly List<WizardStepModel> _wizardSteps = new();
    private WizardStepModel? CurrentStepModel => _wizardSteps.Find(s => s.IsCurrentStep);

    #endregion
    
    #region Properties
    [Parameter]
    public string? WizardTitle { get; set; } = string.Empty;
    
    [Parameter]
    public EventCallback WizardCancelled { get; set; }
    
    [Parameter]
    public EventCallback WizardCompleted { get; set; }
    
    [Parameter]
    public EventCallback<WizardStepModel> WizardStepGotFocus { get; set; }

    [Parameter]
    public EventCallback<WizardStepModel> WizardStepLostFocus { get; set; }

    [Parameter]
    public int WizardStartStep { get; set; } = 1;
    
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    #endregion

    #region Methods
    public void AddWizardStep(WizardStepModel wizardStepModel)
    {
        var uuId = wizardStepModel.WizardStepUUId;
        
        var existingItem = _wizardSteps.Find(m =>
            m.WizardStepUUId.Equals(uuId));

        if (existingItem != null) return;

        AddAndAttachToModel(wizardStepModel);
       
        InvokeAsync(StateHasChanged);
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
        if (wizardStepModel.StepSequence == WizardStartStep)
            wizardStepModel.IsCurrentStep = true;
    }

    #endregion
    
    #region Event Callbacks

    private async Task CancelButtonClicked()
    {
        await WizardCancelled.InvokeAsync();
    }
    
    private async Task BackButtonClicked()
    {
        if (CurrentStepModel == null)
            return;
        var previousSequenceModel = _wizardSteps
            .OrderBy(x => x.StepSequence)
            .LastOrDefault(m => m.StepSequence < CurrentStepModel.StepSequence);

        if (previousSequenceModel == null)
            return;

        CurrentStepModel.IsCurrentStep = false;
        previousSequenceModel.IsCurrentStep = true;

        await WizardStepGotFocus.InvokeAsync(previousSequenceModel);
        await WizardStepLostFocus.InvokeAsync(CurrentStepModel);
    }
    
    private async Task NextButtonClicked()
    {
        if (CurrentStepModel == null)
            return;
        
        var nextSequenceModel = _wizardSteps
            .OrderBy(x => x.StepSequence)
            .FirstOrDefault(m => m.StepSequence > CurrentStepModel.StepSequence);

        if (nextSequenceModel == null)
            return;

        CurrentStepModel.IsCurrentStep = false;
        nextSequenceModel.IsCurrentStep = true;
        
        await WizardStepGotFocus.InvokeAsync(nextSequenceModel);
        await WizardStepLostFocus.InvokeAsync(CurrentStepModel);
    }
    

    private async void WizardStepModelOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        await InvokeAsync(StateHasChanged);
    }

    #endregion
    
}