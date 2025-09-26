using MassTransit;
using MassTransit.Middleware;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MassTransitClient;

public class Program
{
    public static async Task Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();
        await host.RunAsync();
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)

            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                config.AddEnvironmentVariables();
                if (args != null)
                {
                    config.AddCommandLine(args);
                }

            })
            .ConfigureServices((hostContext, services) =>
            {
                // register other services and NServiceBus setup
                services.AddLogging(configure => { configure.AddConsole(); });
                
                services.AddMassTransit(busRegistrationConfigurator =>
                {
                        
                    // configure queue settings and retry policy for all endpoints
                    busRegistrationConfigurator.AddConfigureEndpointsCallback((name, cfg) =>  
                    {
                        if (cfg is IRabbitMqReceiveEndpointConfigurator rmq)
                        {
                            rmq.SetQuorumQueue(3);
                            rmq.UseMessageRetry(r => r.Immediate(2));
                        }
                    });
                        
                    busRegistrationConfigurator.AddConsumers(typeof(Program).Assembly);
                    
                    busRegistrationConfigurator.UsingRabbitMq((context, cfg) =>
                    {
                        cfg.Host("localhost", "/", host =>
                        {
                            host.Username("guest");
                            host.Password("guest");
                        });
                            
                        cfg.UseNServiceBusJsonSerializer();
                        
                        // configure one queue for several consumers
                        cfg.ReceiveEndpoint("MassTransitClient", e =>
                        {
                            e.SetQuorumQueue(3);
                            e.ConfigureConsumer<ConsumeNServiceBusTestA>(context);
                        });
                        
                    });
                });
            });
    }
}