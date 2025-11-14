namespace Notifications.Domain;

public class Event
{
    public Guid Id { get; set; }
    public string Type { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public string Data { get; set; } = null!;
    public string UserEmail { get; set; } = null!;
}
