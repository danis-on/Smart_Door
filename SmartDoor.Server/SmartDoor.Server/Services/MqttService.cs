using MQTTnet;
using MQTTnet.Server;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using MQTTnet.Protocol;

namespace SmartDoor.Server.Services;

public class MqttService : IHostedService
{
    private IMqttServer Server { get; set; } = null;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        using var cert = new X509Certificate2("Certificate/smartdoor.pfx", "smartdoor");

        var options = new MqttServerOptionsBuilder()
            .WithDefaultEndpoint()
            .WithEncryptedEndpoint()
            .WithEncryptionCertificate(cert)
            .WithConnectionValidator(OnNewConnection)
            .WithApplicationMessageInterceptor(OnNewMessage)
            .Build();

        Server = new MqttFactory().CreateMqttServer();
        return Server.StartAsync(options);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private void OnNewMessage(MqttApplicationMessageInterceptorContext ctx)
    {
        if (ctx.ClientId is not null)
            ctx.AcceptPublish = false;
    }

    private void OnNewConnection(MqttConnectionValidatorContext ctx)
    {
        if (ctx.Username != ctx.ClientId)
        {
            ctx.ReasonCode = MqttConnectReasonCode.ClientIdentifierNotValid;
            ctx.ReasonString = "ClientId and Username must match.";
            return;
        }

        if (!JwtHelpers.ValidateJWTForMqtt(ctx.Username, ctx.Password))
        {
            ctx.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
            ctx.ReasonString = "The AccessToken (password) is invalid or expired.";
            return;
        }

        ctx.ReasonCode = MqttConnectReasonCode.Success;
    }

    public Task Publish(string topic, string payload)
        => Server.PublishAsync(topic, payload);
}