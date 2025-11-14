using Auditings.Persistence;
using Contracts.Commands;
using MassTransit;

namespace Auditings.Application;


public class CompensateAuditLogCommandHandler(AuditingDbContext db) : IConsumer<CompensateAuditLogCommand>
{
    async public Task Consume(ConsumeContext<CompensateAuditLogCommand> context)
    {
        var entity = db.Logs.Where(e => e.CorrelationId == context.Message.CorrelationId).FirstOrDefault();
        if (entity != null)
        {
            db.Logs.Remove(entity);
            await db.SaveChangesAsync();
        }
    }
}
