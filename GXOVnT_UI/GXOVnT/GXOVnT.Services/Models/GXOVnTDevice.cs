﻿using GXOVnT.Services.Common;
using Plugin.BLE.Abstractions;

namespace GXOVnT.Services.Models;

public class GXOVnTDevice : NotifyChanged
{

    #region Members
    private string _id = Guid.NewGuid().ToString();
    private string _deviceName = string.Empty;
    private int _rssi;
    private bool _isConnectable;
    private DeviceState _deviceState = DeviceState.Disconnected;
    #endregion
    
    #region Properties

    public string Id
    {
        get => _id;
        set => SetField(ref _id, value);
    }

    public string DeviceName
    {
        get => _deviceName;
        set => SetField(ref _deviceName, value);
    }

    public int Rssi
    {
        get => _rssi;
        set => SetField(ref _rssi, value);
    }

    public bool IsConnectable
    {
        get => _isConnectable;
        set => SetField(ref _isConnectable, value);
    }

    public DeviceState DeviceState
    {
        get => _deviceState;
        set => SetField(ref _deviceState, value);
    }
    
    #endregion

}