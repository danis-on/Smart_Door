using SmartDoor.Server.Models.Database;

namespace SmartDoor.Server;
internal static class HttpContextHelpers
{
    private const string KEY_USER = "KEY_USER_1GIDXQ";
    internal static User? GetUser(this HttpContext ctx) => ctx.Items[KEY_USER] as User?;
    internal static void SetUser(this HttpContext ctx, User u) => ctx.Items[KEY_USER] = u;

}
