using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using OtpNet;
using SmartDoor.Server.Models.Api;
using SmartDoor.Server.Models.Database;

namespace SmartDoor.Server.Controllers;

[Route("[controller]")]
[ApiController]
public class UserController : Controller
{
    private readonly SqliteConnection _db;

    public UserController(SqliteConnection db)
    {
        _db = db;
    }

    [AccessRole(UserRole.User)]
    [AccessRole(UserRole.Admin)]
    [HttpGet("Get/{id}")]
    public async Task<ActionResult<UserResponse>> GetById(int id)
    {
        var u = await _db.QuerySingleOrDefaultAsync<User>("select * from user where id = @id", new { id }).ConfigureAwait(false);
        return u.Id <= 0 ? NotFound() : new UserResponse(u.Id, u.Login, u.Role);
    }

    
    [AccessRole(UserRole.Admin)]
    [HttpGet("Get/AllUsers")]
    public async Task<IEnumerable<User>> Users( )
    {
        IEnumerable<User> u = await _db.QueryAsync<User>("select * from user").ConfigureAwait(false);
        return u;
    }



    [AccessRole(UserRole.Admin)]
    [HttpPost("Create")]
    public async Task<ActionResult<UserResponse>> Create(UserCreateRequest req)
    {
        var u = new User(0, req.Login, req.Role, UserHelper.GetPasswordHash(req.Login, req.PlaintextPassword));

        if (!UserHelper.ValidateRole(req.Role))
            return ValidationProblem($"Requested role is not valid.");

        var inserted = 1 == (await _db.ExecuteAsync(@"
            insert into user (login, role, passwordHash) 
            select @Login, @Role, @PasswordHash
            where not exists(select 1 from user where login = @Login);
            ", u).ConfigureAwait(false));

        if (!inserted)
            return ValidationProblem($"Login '{u.Login}' already exists.");
        var newUser = await _db.QuerySingleOrDefaultAsync<User>("select * from user where login = @Login", u).ConfigureAwait(false);
        return newUser.Id <= 0 ? NotFound() : new UserResponse(newUser.Id, newUser.Login, newUser.Role);
    }

    [AccessRole(UserRole.Admin)]
    [HttpPost("ChangeRole")]
    public async Task<ActionResult<UserResponse>> ChangeRole(UserChangeRoleRequest req)
    {
        if (!UserHelper.ValidateRole(req.NewRole))
            return ValidationProblem($"Requested role is not valid.");

        var updated = 1 == (await _db.ExecuteAsync(@"update user set role = @NewRole where id = @Id;", req).ConfigureAwait(false));

        if (!updated)
            return ValidationProblem($"User id '{req.Id}' does not exist.");

        var newUser = await _db.QuerySingleOrDefaultAsync<User>("select * from user where id = @Id", req).ConfigureAwait(false);
        return newUser.Id <= 0 ? NotFound() : new UserResponse(newUser.Id, newUser.Login, newUser.Role);
    }

    [AccessRole(UserRole.Admin)]
    [AccessRole(UserRole.User)]
    [HttpPost("ChangePassword")]
    public async Task<ActionResult<UserResponse>> ChangePassword(ChangePasswordRequest req)
    {
        var me = HttpContext.GetUser()!.Value;
        var oldHash = UserHelper.GetPasswordHash(me.Login, req.Current);
        var newHash = UserHelper.GetPasswordHash(me.Login, req.New);


        var updated = 1 == (await _db.ExecuteAsync(@"
            update user set passwordHash = @newHash where id = @Id and passwordHash = @oldHash;",
            new { oldHash, newHash, me.Id }).ConfigureAwait(false));

        if (!updated)
            return ValidationProblem($"Failed to change password - is current password correct?");

        var newUser = await _db.QuerySingleOrDefaultAsync<User>("select * from user where id = @Id", me).ConfigureAwait(false);
        return newUser.Id <= 0 ? NotFound() : new UserResponse(newUser.Id, newUser.Login, newUser.Role);
    }

    [AccessRole(UserRole.Admin)]
    [HttpPost("Delete/{id}")]
    public async Task<ActionResult> Delete([FromRoute] int id)
    {
        var deleted = 1 == (await _db.ExecuteAsync(@"
            delete from user where id = @id;
            ", new { id }).ConfigureAwait(false));

        if (!deleted)
            return ValidationProblem($"Failed to delete - does the requested user id exist?");
        return Ok();
    }


    [AccessRole(UserRole.Admin)]
    [AccessRole(UserRole.User)]
    [HttpPost("SetPin")]
    public async Task<ActionResult> SetPin(SetPinRequest req)
    {
        var me = HttpContext.GetUser()!.Value;
        var passHash = UserHelper.GetPasswordHash(me.Login, req.CurrentPassword);
        var pinHash = UserHelper.GetPasswordHash(me.Login, req.NewPin);


        var updated = 1 == (await _db.ExecuteAsync(@"
            update user set pinHash = @pinHash where id = @Id and passwordHash = @passHash;",
            new { pinHash, passHash, me.Id }).ConfigureAwait(false));

        if (!updated)
            return ValidationProblem($"Failed to set PIN - is current password correct?");

        return Ok();
    }

    [AccessRole(UserRole.Admin)]
    [AccessRole(UserRole.User)]
    [HttpPost("ResetTotp")]
    public async Task<ActionResult<ResetTotpResponse>> ResetTotp(ResetTotpRequest req)
    {
        var me = HttpContext.GetUser()!.Value;
        var passHash = UserHelper.GetPasswordHash(me.Login, req.CurrentPassword);

        var key = KeyGeneration.GenerateRandomKey(20);

        var updated = 1 == (await _db.ExecuteAsync(@"
            update user set totp = @totp where id = @Id and passwordHash = @passHash;",
            new { totp = $"{Base32Encoding.ToString(key)}", passHash, me.Id }).ConfigureAwait(false));

        if (!updated)
            return ValidationProblem($"Failed to reset TOTP - is current password correct?");

        return new ResetTotpResponse($"otpauth://totp/SmartDoor:{me.Login}?secret={Base32Encoding.ToString(key)}&issuer=SmartDoor");
    }
}
