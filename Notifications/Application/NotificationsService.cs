using Microsoft.EntityFrameworkCore;
using Notifications.Application.DTOs;
using Notifications.Domain;
using Notifications.Persistence;

namespace Notifications.Application;



public interface INotificationsService
{
    Task<Guid> SaveEventAsync(UserRegisteredDto @event);
    Task<List<Event>> ListEventsAsync();
    Task<List<Event>> ListEventsByUserAsync(string userEmail);
    Task ResetAsync();
}

public class NotificationsService : INotificationsService
{
    private readonly NotificationsDbContext _db;

    public NotificationsService(NotificationsDbContext db)
    {
        _db = db;
    }

    public async Task<List<Event>> ListEventsByUserAsync(string userEmail)
    {
        List<Event> events = await _db.Events
            .Where(e => e.UserEmail == userEmail || e.Data.Contains(userEmail))
            .ToListAsync();
        return events;
    }

    public async Task<List<Event>> ListEventsAsync()
    {
        List<Event> events = await _db.Events.ToListAsync();
        return events;
    }

    public async Task<Guid> SaveEventAsync(UserRegisteredDto @event)
    {
        var e = _db.Events.Add(new Event
        {
            Id = Guid.NewGuid(),
            Type = typeof(UserRegisteredDto).Name,
            CreatedAt = DateTime.UtcNow,
            Data = System.Text.Json.JsonSerializer.Serialize(@event),
            UserEmail = @event.UserEmail,
        });
        await _db.SaveChangesAsync();
        return e.Entity.Id;
    }

    public async Task ResetAsync()
    {
        var data = await _db.Events.ToListAsync();
        _db.Events.RemoveRange(data.ToArray());
        await _db.SaveChangesAsync();
    }
}
