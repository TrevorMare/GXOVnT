namespace GXOVnT.Services.Interfaces;

public interface IRequestPermissionService
{
    Task<bool> CheckBLEPermissionRequirement();
}