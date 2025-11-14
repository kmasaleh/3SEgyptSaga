using Contracts.Commands;
using Contracts.Events;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Notifications.Application.DTOs;
using Notifications.Persistence;

namespace Notifications.Application.Sagas;

public class NewUserOnboardSagaData : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; } = null!;
    public string UserEmail{ get; set; } = null!;
    public string Message { get; set; } = null!;    
    public bool WelcomeEmailSent { get; set; }
    public bool AuditLogged { get; set; }  
    public bool NotificationCompleted { get; set; }
}

public class NewUserOnboardSaga  : MassTransitStateMachine<NewUserOnboardSagaData>
{
    public State NotificationRegiserationState { get; private set; } = null!;
    public State EmailingState { get; private set; } = null!;
    public State AuditingState { get; private set; } = null!;
    public State CompletedState { get; private set; } = null!;

    public Event<WelcomeEmailFailed> WelcomeEmailFailedEvent { get; private set; } = null!;
    public Event<UserRegisteredNotificationCreated> UserNotificationSavedEvent { get; private set; } = null!;
    public Event<WelcomeEmailSent> WelcomeEmailSentEvent { get; private set; } = null!;
    public Event<AuditLogged> AuditLogSentEvent { get; private set; } = null!;
    public Event<AuditFailed> AuditLogFailedEvent { get; private set; } = null!;
    public NewUserOnboardSaga()
    {
        //serialize the current state to the DB
        InstanceState(x => x.CurrentState);
        //Wire up events to the state machine   
        Event(() => UserNotificationSavedEvent, x => x.CorrelateById(context => context.Message.CorrelationId));
        Event(() => WelcomeEmailSentEvent, x => x.CorrelateById(context => context.Message.CorrelationId));
        Event(() => AuditLogSentEvent, x => x.CorrelateById(context => context.Message.CorrelationId));

        Event(() => WelcomeEmailFailedEvent, x => x.CorrelateById(context => context.Message.CorrelationId));
        Event(() => AuditLogFailedEvent, x => x.CorrelateById(context => context.Message.CorrelationId));
        Initially
        (
            When(UserNotificationSavedEvent)
           .Then(context =>
            {
                context.Saga.CorrelationId = context.Message.CorrelationId;
                context.Saga.UserEmail = context.Message.UserEmail;
                context.Saga.Message = context.Message.Message; 
            })
            .TransitionTo(EmailingState)
            .Publish(context => new SendWelcomeEmailCommand
            (
                CorrelationId : context.Saga.CorrelationId,
                UserEmail : context.Message.UserEmail,
                Message : context.Message.Message
                    
            ))
        );


        During(EmailingState,
            When(WelcomeEmailFailedEvent)
                .ThenAsync(HandleNotificationCompensation.Compensate)
                .Finalize(),

            When(WelcomeEmailSentEvent)
                .Then(context =>
                {
                    context.Saga.WelcomeEmailSent = true;
                })
                .TransitionTo(AuditingState)
                .Publish(context => new SendAuditLogCommand
                (
                    CorrelationId: context.Saga.CorrelationId,
                    Data: context.Saga.Message
                ))
        );
        During(AuditingState,
            When(AuditLogFailedEvent)
                .ThenAsync(HandleNotificationCompensation.Compensate)
                .Publish(context => new CompensateWelcomeEmailCommand
                (
                    CorrelationId:   context.Saga.CorrelationId
                ))
                .Finalize(),

            When(AuditLogSentEvent)
                .Then(context =>
                {
                    context.Saga.AuditLogged = true;
                })
                .Finalize()
        );
        SetCompletedWhenFinalized();
    }
}

internal sealed class HandleNotificationCompensation 
{
    static public async Task Compensate (BehaviorContext<NewUserOnboardSagaData, object /*WelcomeEmailFailed*/> context)
    {
        var db = context.GetServiceOrCreateInstance<NotificationsDbContext>();
        // handle failure logic here
        var entity = await db.Events.FirstOrDefaultAsync(e => e.Id == context.Saga.CorrelationId);
        if (entity != null)
        {
            db.Events.Remove(entity);
            await db.SaveChangesAsync();
        }

    }
}