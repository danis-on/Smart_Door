using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using SmartDoor.Server.Models.Api;
using SmartDoor.Server.Models.Database;
using System.Security.Cryptography;
using System.Text;

namespace SmartDoor.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : Controller
{
    private readonly SqliteConnection _db;

    public AuthController(SqliteConnection db)
    {
        _db = db;
    }

    [HttpPost("Login")]
    [AccessRole(UserRole.Anonymous)]
    public async Task<ActionResult<LoginResponse>> Login(LoginRequest req)
    {
        var hash = UserHelper.GetPasswordHash(req.login, req.password);
        var user = await _db.QuerySingleOrDefaultAsync<User>("select * from user where login = @l and passwordHash = @p", new { l = req.login, p = hash }).ConfigureAwait(false);
        if (user.Id == 0)
            return Unauthorized();

        return new LoginResponse(JwtHelpers.GetTokenForUser(user, req.validityInSeconds > 0 ? TimeSpan.FromSeconds((double)req.validityInSeconds) : null));
    }
    
    [HttpPost("CreateMqttAccessToken")]
    [AccessRole(UserRole.Admin)]
    public ActionResult<LoginResponse> CreateMqttAccessToken(CreateMqttAccessTokenRequest req)
    {
        if (req.validityInHours is < 1 or > 87600)
            return UnprocessableEntity("ValidityInHours needs to be between 1 and 87600 (10 years).");
        if (req.identifier.Length is < 1 or > 100)
            return UnprocessableEntity("Identifier needs to be between 1 and 100 characters.");
        return new LoginResponse(JwtHelpers.GetTokenForMqtt(req.identifier, DateTime.Now.AddHours(req.validityInHours)));
    }
}
