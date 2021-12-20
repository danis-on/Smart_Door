using SmartDoor.Server.Models.Database;

namespace SmartDoor.Server;


[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public sealed class AccessRoleAttribute : Attribute
{
    public UserRole Role { get; }
    public AccessRoleAttribute(UserRole role) => Role = role;
}