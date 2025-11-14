namespace Auditings.Domain;

public class Log
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Data { get; set; } = null!;
    public Guid CorrelationId { get; set; }
}
