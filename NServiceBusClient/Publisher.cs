using MessageContracts;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace NServiceBusClient;

public class Publisher : IHostedService
{
    private readonly IMessageSession _messageSession;
    private readonly ILogger<Publisher> _logger;

    public Publisher(IMessageSession messageSession, ILogger<Publisher>? logger)
    {
        _messageSession = messageSession ?? throw new ArgumentNullException(nameof(messageSession));
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Hosted service started.");
        var i = 0;
        while (true)
        {
            
            i += 1;
            var testAEvent = new NServiceBusTestMessageA()
            {
                Foo = "NServiceBus",
                Bar = i,
                Source = "NServiceBus"
            };
            await _messageSession.Publish(testAEvent, cancellationToken: cancellationToken);
            _logger.LogInformation("Message Sent: {MessageType} #{Count}", testAEvent.GetType().ToString(), i);
            await Task.Delay(5000, cancellationToken);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Hosted service stopping.");
        return Task.CompletedTask;
    }
}