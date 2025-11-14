using MassTransit;

namespace Notifications.Application.DTOs;

public record UserRegisteredCommand(string UserEmail, string Message);
