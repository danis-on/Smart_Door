using MQTTnet;
using MQTTnet.Server;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace SmartDoor.Server.Services;
public class MqttService : IHostedService
{
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

        var mqttServer = new MqttFactory().CreateMqttServer();
        return mqttServer.StartAsync(options);
    }

    private void OnNewMessage(MqttApplicationMessageInterceptorContext ctx)
    {
        throw new NotImplementedException();
    }

    private void OnNewConnection(MqttConnectionValidatorContext ctx)
    {
        throw new NotImplementedException();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
