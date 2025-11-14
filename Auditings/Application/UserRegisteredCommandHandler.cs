using Contracts.Commands;
using Contracts.Events;
using Auditings.Persistence;
using MassTransit;

namespace Auditings.Application;

public class AuditLogCommandHandler(AuditingDbContext db, FailureService failureService) 
    : IConsumer<SendAuditLogCommand>
{
    async public Task Consume(ConsumeContext<SendAuditLogCommand> context)
    {
        if(failureService.ShouldFail)
        {
            await context.Publish(new AuditFailed
                (
                    CorrelationId: context.Message.CorrelationId
                ));

            return;
        }
        var entity = db.Logs.Add(new Domain.Log
        {
            Data = context.Message.Data,
            CreatedAt = DateTime.Now,
            CorrelationId = context.Message.CorrelationId
        });
        await db.SaveChangesAsync();
        await context.Publish(new AuditLogged
        (
            CorrelationId: context.Message.CorrelationId
        ));
    }
}
