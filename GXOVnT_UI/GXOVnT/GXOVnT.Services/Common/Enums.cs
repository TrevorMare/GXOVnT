using System.ComponentModel.DataAnnotations;

namespace GXOVnT.Services.Common;

public enum LogMessageType
{
    
    Debug = 0,
    Information = 1,
    Warning = 2,
    Error = 3
}

public enum GXOVnTDeviceType
{
    [Display(Name = "Extension")]
    SystemTypeExtension = 1,
    [Display(Name = "Primary")]
    SystemTypePrimary = 2,
    
}