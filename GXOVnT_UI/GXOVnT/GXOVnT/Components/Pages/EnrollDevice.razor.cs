using GXOVnT.Services.Common;
using Microsoft.AspNetCore.Components;
using GXOVnT.Services.Models;

namespace GXOVnT.Components.Pages;

public partial class EnrollDevice : ComponentBase
{

    #region Properties

    public List<ListItem<GXOVnTDeviceType>> DeviceTypes = ListItemExtensions.ConvertEnumToListItems<GXOVnTDeviceType>();
    
    public ListItem<GXOVnTDeviceType>? SelectedDeviceType { get; set; }

    #endregion

    #region ctor

    public EnrollDevice()
    {
       
    }

    #endregion
    
}