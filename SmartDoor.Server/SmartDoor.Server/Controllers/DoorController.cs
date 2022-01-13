using System.Text.RegularExpressions;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using OtpNet;
using SmartDoor.Server.Models.Api;
using SmartDoor.Server.Models.Database;
using SmartDoor.Server.Services;

namespace SmartDoor.Server.Controllers;

[Route("[controller]")]
[ApiController]
public class DoorController : Controller
{
    private readonly SqliteConnection _db;
    private readonly MqttService _mqtt;

    public DoorController(SqliteConnection db, MqttService mqtt)
    {
        _db = db;
        _mqtt = mqtt;
    }

    [AccessRole(UserRole.User)]
    [AccessRole(UserRole.Admin)]
    [HttpGet("Open")]
    public IActionResult Open()
    {
        async Task Inner()
        {
            await _mqtt.Publish("state", "1").ConfigureAwait(false);
            await Task.Delay(2000).ConfigureAwait(false);
            await _mqtt.Publish("state", "0").ConfigureAwait(false);
        }

        _ = Inner();
        return Ok();
    }
}