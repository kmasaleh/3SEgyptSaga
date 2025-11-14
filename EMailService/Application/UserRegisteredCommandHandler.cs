using Contracts.Commands;
using Contracts.Events;
using EMailService.Persistence;
using MassTransit;

namespace EMailService.Application;



public class SendWelcomeEmailCommandHandler(EMailDbContext db, FailureService failureService) 
    : IConsumer<SendWelcomeEmailCommand>
{
    async public Task Consume(ConsumeContext<SendWelcomeEmailCommand> context)
    {
        if(failureService.ShouldFail)
        {
            await context.Publish(new WelcomeEmailFailed
                (
                    CorrelationId: context.Message.CorrelationId
                ));

            return;
        }
        var entity = db.EMails.Add(new Domain.EMail
        {
            To = context.Message.UserEmail,
            Subject = "Welcome to Our Service!",
            Body = context.Message.Message,
            SentAt = DateTime.Now,
            CorrelationId = context.Message.CorrelationId   
        });
        await db.SaveChangesAsync();
        await context.Publish(new WelcomeEmailSent
        (
            CorrelationId: context.Message.CorrelationId,
            EmailId : entity.Entity.Id
        ));
    }
}
