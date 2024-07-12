using GXOVnT.Shared.Interfaces;

namespace GXOVnT.Services.Interfaces;

public interface IRequestPermissionService : IStateObject
{
    Task<bool> ApplicationHasBluetoothPermission();
    
    Task<bool> ApplicationHasPermission<TPermission>() where TPermission : Permissions.BasePermission, new();
    
    Task<bool> RequestApplicationPermission<TPermission>() where TPermission : Permissions.BasePermission, new();
    
    Task<bool> RequestBluetoothPermission();
    
}