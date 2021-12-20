namespace SmartDoor.Server.Models.Database;


[Flags]
public enum UserRole : int
{
    Anonymous = 0,
    Admin = 1,
    User = 2
}

public record struct User(int Id, string Login, UserRole Role, string PasswordHash);


