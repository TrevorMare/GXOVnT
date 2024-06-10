using GXOVnT.Models;
using Microsoft.AspNetCore.Components;

namespace GXOVnT.Components.Shared.WizardComponent;

public partial class WizardStep : ComponentBase
{

    #region Properties

    public WizardStepModel WizardStepModel { get; private set; } = new();
    
    [Parameter]
    public string? WizardStepTitle 
    {
        get => WizardStepModel.StepTitle;
        set => WizardStepModel.StepTitle = value;
    } 

    [Parameter]
    public int StepSequence
    {
        get => WizardStepModel.StepSequence;
        set => WizardStepModel.StepSequence = value;
    } 
    
    [Parameter]
    public bool HasCancelButton
    {
        get => WizardStepModel.HasCancelButton;
        set => WizardStepModel.HasCancelButton = value;
    } 
    
    [Parameter]
    public bool CanCancelButton
    {
        get => WizardStepModel.CanCancelButton;
        set => WizardStepModel.CanCancelButton = value;
    } 
    
    [Parameter]
    public bool HasBackButton
    {
        get => WizardStepModel.HasBackButton;
        set => WizardStepModel.HasBackButton = value;
    } 
    
    [Parameter]
    public bool CanBackButton
    {
        get => WizardStepModel.CanBackButton;
        set => WizardStepModel.CanBackButton = value;
    }
    
    [Parameter]
    public bool HasForwardButton
    {
        get => WizardStepModel.HasForwardButton;
        set => WizardStepModel.HasForwardButton = value;
    }
    
    [Parameter]
    public bool CanForwardButton
    {
        get => WizardStepModel.CanForwardButton;
        set => WizardStepModel.CanForwardButton = value;
    }
    
    [Parameter]
    public string CancelButtonText
    {
        get => WizardStepModel.CancelButtonText;
        set => WizardStepModel.CancelButtonText = value;
    }
    
    [Parameter]
    public string BackButtonText
    {
        get => WizardStepModel.BackButtonText;
        set => WizardStepModel.BackButtonText = value;
    }
    
    [Parameter]
    public string NextButtonText
    {
        get => WizardStepModel.NextButtonText;
        set => WizardStepModel.NextButtonText = value;
    }
    
    [Parameter]
    public RenderFragment? ChildContent { get; set; }
    
    [CascadingParameter] 
    private WizardComponent? WizardComponent { get; set; } 
    
    #endregion

    #region Overrides

    protected override void OnInitialized()
    {
        base.OnInitialized();
        
        WizardComponent?.AddWizardStep(WizardStepModel);
    }

    #endregion
    
}