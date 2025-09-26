using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NServiceBusClient;

public class Program
{
    public static async Task Main(string[] args)
    {
        var hostBuilder = Host.CreateDefaultBuilder(args);

        hostBuilder.UseNServiceBus(context =>
        {
            // Create the endpoint configuration
            var endpointConfiguration = new EndpointConfiguration("SampleNServicebusConsumer.TestEvents");
            endpointConfiguration.SendFailedMessagesTo("SampleNServicebusConsumer.TestEvents.Error");

            // Configure transport (e.g., RabbitMQ)
            var transport = endpointConfiguration.UseTransport<RabbitMQTransport>();
            transport.ConnectionString("host=localhost;username=guest;password=guest");
            transport.UseConventionalRoutingTopology(QueueType.Quorum);
            endpointConfiguration.UseSerialization<NewtonsoftJsonSerializer>();
            
            // Installers
            endpointConfiguration.EnableInstallers();
            return endpointConfiguration;
        });

        // Add custom services
        hostBuilder.ConfigureServices((context, services) =>
        {
            // Register a hosted service that uses IMessageSession
            services.AddHostedService<Publisher>();
        });

        var host = hostBuilder.Build();
        await host.RunAsync();
    }
}