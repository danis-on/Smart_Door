namespace SmartDoor.Server.Middleware;
public class AuthenticationMiddleware
{
    public RequestDelegate Next { get; }

    public AuthenticationMiddleware(RequestDelegate next)
    {
        Next = next;
    }


    public async Task Invoke(HttpContext context)
    {
        var token = context.Request.Headers["Token"].FirstOrDefault()?.Replace("Bearer ", "");
        token ??= context.Request.Query["Token"].FirstOrDefault();
        await JwtHelpers.ValidateJWT(token, context).ConfigureAwait(false);
        await Next(context).ConfigureAwait(false);

        if (context.Response.StatusCode == 401 || context.Response.StatusCode == 403)
            await Task.Delay(2000).ConfigureAwait(false);

    }
}
