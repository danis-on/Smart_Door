namespace SmartDoor.Server.Models.Api
{
    public record class LoginRequest (string login, string password, long? validityInSeconds);
    public record class LoginResponse (string token);
}
