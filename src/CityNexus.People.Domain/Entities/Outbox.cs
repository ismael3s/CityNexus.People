namespace CityNexus.People.Domain.Entities;

public sealed class Outbox
{
    public Guid Id { get; private set; }

    public string EventName { get; private set; } = string.Empty;

    public string Payload { get; private set; } = string.Empty;

    public DateTime CreatedAt { get; private set; }

    public DateTime? ProcessedAt { get; private set; }

    // private Outbox()
    // {
    // }

    private Outbox() { }

    public Outbox(string name, string payload)
    {
        Id = Guid.NewGuid();
        EventName = name;
        Payload = payload;
        CreatedAt = DateTime.UtcNow;
    }

    public void MarkAsProcessed()
    {
        ProcessedAt = DateTime.UtcNow;
    }
}
