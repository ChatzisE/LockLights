namespace LockLights;

/// <summary>Application-wide identifiers.</summary>
internal static class AppInfo
{
    public const string Name = "LockLights";
    public const string RegistryPath = @"Software\" + Name;
    public const string SingleInstanceMutexName = @"Local\" + Name + "_SingleInstance";
}
