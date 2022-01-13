using Microsoft.Data.Sqlite;
using Serilog;
using SmartDoor.Server;
using SmartDoor.Server.Middleware;
using SmartDoor.Server.Services;


var db = new SqliteConnection("Data Source=SmartDoor.db");
await db.OpenAsync().ConfigureAwait(false);

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(options =>
    options.Filters.Add<AuthorizationFilter>()
);
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "SmartDoor.Server", Version = "v1" });
    c.AddSecurityDefinition("Token header Authorization (JWT)", new Microsoft.OpenApi.Models.OpenApiSecurityScheme()
    {
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Name = "Token",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
    });
    c.OperationFilter<AuthenticationRequirementsOperationFilter>();
});
builder.Services.AddHostedService<DBInitService>();
var mqtt = new MqttService();
builder.Services.AddSingleton(mqtt);
builder.Services.AddHostedService(_ => mqtt);
builder.Services.AddSingleton(db);


builder.Host.UseSerilog((context, config) => { config.WriteTo.Console(); });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "SmartDoor.Server v1"));
}

app.UseHttpsRedirection();

app.UseMiddleware<AuthenticationMiddleware>();

app.MapControllers();

app.Run();