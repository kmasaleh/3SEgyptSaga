namespace EMailService.Domain;

public class EMail
{
    public Guid Id { get; set; }
    public string To { get; set; } = null!;
    public string Subject { get; set; } = null!;
    public string Body { get; set; } = null!;
    public DateTime SentAt { get; set; }

    public Guid CorrelationId { get; set; }
}
