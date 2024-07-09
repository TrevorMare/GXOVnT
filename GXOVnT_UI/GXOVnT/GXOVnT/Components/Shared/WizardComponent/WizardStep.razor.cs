using GXOVnT.ViewModels.Wizards;
using Microsoft.AspNetCore.Components;

namespace GXOVnT.Components.Shared.WizardComponent;

public partial class WizardStep<TWizardModelType> : ComponentBase  where TWizardModelType: WizardComponentModel, new()
{

    #region Properties

    public WizardStepModel WizardStepModel { get; private set; } = new();
    
    
    [Parameter]
    public string? WizardStepName 
    {
        get => WizardStepModel.WizardStepName;
        set => WizardStepModel.WizardStepName = value;
    }
    
    [Parameter]
    public bool IsBusy 
    {
        get => WizardStepModel.IsBusy;
        set => WizardStepModel.IsBusy = value;
    }
    
    [Parameter]
    public string? BusyText 
    {
        get => WizardStepModel.BusyText;
        set => WizardStepModel.BusyText = value;
    }
    
    [Parameter]
    public bool IsEnabled 
    {
        get => WizardStepModel.IsEnabled;
        set => WizardStepModel.IsEnabled = value;
    } 
    
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
    public bool CancelEnabled
    {
        get => WizardStepModel.CancelEnabled;
        set => WizardStepModel.CancelEnabled = value;
    } 
    
    [Parameter]
    public bool HasBackButton
    {
        get => WizardStepModel.HasBackButton;
        set => WizardStepModel.HasBackButton = value;
    } 
    
    [Parameter]
    public bool BackEnabled
    {
        get => WizardStepModel.BackEnabled;
        set => WizardStepModel.BackEnabled = value;
    }
    
    [Parameter]
    public bool HasForwardButton
    {
        get => WizardStepModel.HasForwardButton;
        set => WizardStepModel.HasForwardButton = value;
    }
    
    [Parameter]
    public bool ForwardEnabled
    {
        get => WizardStepModel.ForwardEnabled;
        set => WizardStepModel.ForwardEnabled = value;
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
    private WizardComponent<TWizardModelType>? WizardComponent { get; set; } 
    
    #endregion

    #region Overrides

    protected override void OnInitialized()
    {
        base.OnInitialized();
        
        WizardComponent?.AddWizardStep(WizardStepModel);
    }

    #endregion
    
}