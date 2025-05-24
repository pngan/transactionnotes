namespace transactionnotes.ApiService.Authorization;

[Flags]
public enum UserPermission
{
    PermissionNone = 0,
    PermissionRead = 1 << 0,
    PermissionWrite = 1 << 1,
    PermissionAdmin = 1 << 2,

    PermissionSubscriber = 1 << 31 // 31 is the largest allowable bit shift 
}