using SmartDoor.Server.Models.Database;

namespace SmartDoor.Server.Models.Api;

public record class AllUsers(IEnumerable<User> users);
public record class UserResponse(int Id, string Login, UserRole Role);
public record class UserCreateRequest(string Login, UserRole Role, string PlaintextPassword);
public record class UserChangeRoleRequest(int Id, UserRole NewRole);
public record class ChangePasswordRequest(string Current, string New);
public record class SetPinRequest(string CurrentPassword, string NewPin);
public record class ResetTotpRequest(string CurrentPassword);
public record class ResetTotpResponse(string data);