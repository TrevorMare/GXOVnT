namespace GXOVnT.Services.Interfaces;

public interface IRequestPermissionService
{
    Task<bool> ApplicationHasBluetoothPermission();
    
    Task<bool> ApplicationHasPermission<TPermission>() where TPermission : Permissions.BasePermission, new();
    
    Task<bool> RequestApplicationPermission<TPermission>() where TPermission : Permissions.BasePermission, new();
    
    Task<bool> RequestBluetoothPermission();
    
}