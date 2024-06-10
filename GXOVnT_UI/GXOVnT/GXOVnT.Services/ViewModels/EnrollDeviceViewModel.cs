using GXOVnT.Services.Common;
using GXOVnT.Services.Interfaces;

namespace GXOVnT.Services.ViewModels;

/// <summary>
/// A GXOVnT device should first be enrolled to do the initial firmware downloads
/// </summary>
public class EnrollDeviceViewModel : NotifyChanged
{

    #region Helpers
    public enum EnrollDeviceStep
    {
        ScanDevices = 1,
        ShowCurrentSettings = 2,
        ConfirmOverride = 3,
        SetSettings = 4,
        DownloadFirmware = 5,
        Finalize = 6
    }
    #endregion

    #region Members

    private EnrollDeviceStep _enrollStep = EnrollDeviceStep.ScanDevices;
    private readonly LogViewModel _logViewModel;
    private readonly IBluetoothService _bluetoothService;

    #endregion

    #region Properties

    public EnrollDeviceStep EnrollStep
    {
        get => _enrollStep;
        private set => SetField(ref _enrollStep, value);
    }
    #endregion

    #region ctor

    public EnrollDeviceViewModel(LogViewModel logViewModel)
    {
        _logViewModel = logViewModel;
    }

    #endregion

    #region Methods

    public void PreviousStep()
    {
        if (_enrollStep == EnrollDeviceStep.ScanDevices)
            return;

        var newStepValue = ((int)EnrollStep - 1);
        EnrollStep = (EnrollDeviceStep)newStepValue;
    }
    
    public void NextStep()
    {
        if (_enrollStep == EnrollDeviceStep.Finalize)
            return;
        
    }

    #endregion


}