namespace MessageContracts;

public class NServiceBusTestMessageA : IEvent
{
    public string Foo { get; set; }
    public int Bar { get; set; }
    public string Source { get; set; }
}