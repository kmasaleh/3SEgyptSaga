using Contracts.Events;
using MassTransit;
using Notifications.Application.DTOs;
using Notifications.Persistence;

namespace Notifications.Application.Handlers;

public class UserRegisteredCommandHandler(INotificationsService notificationsService) : IConsumer<UserRegisteredCommand>
{
    async public Task Consume(ConsumeContext<UserRegisteredCommand> context)
    {
        var eventId = await notificationsService.SaveEventAsync(new UserRegisteredDto
        (
           Message : context.Message.Message,
           UserEmail : context.Message.UserEmail
        ));
        

        await context.Publish(new UserRegisteredNotificationCreated
        (
            CorrelationId: eventId,
            UserEmail: context.Message.UserEmail,
            Message: context.Message.Message
        ));
    }
}
