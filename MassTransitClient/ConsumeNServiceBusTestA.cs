using MassTransit;
using MessageContracts;
using Microsoft.Extensions.Logging;

namespace MassTransitClient;

public class ConsumeNServiceBusTestA: IConsumer<NServiceBusTestMessageA>
{
    private ILogger<ConsumeNServiceBusTestA> _logger;

    public ConsumeNServiceBusTestA(ILogger<ConsumeNServiceBusTestA> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<NServiceBusTestMessageA> context)
    {
        _logger.LogInformation("Received NServiceBusTestMessageA: {MessageId}", context.Message.Bar);
        if (context.Message.Bar % 2 == 0) throw new Exception("Simulated unprocessable message");
        return Task.CompletedTask;
    }
}