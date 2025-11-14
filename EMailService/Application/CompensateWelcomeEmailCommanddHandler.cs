using Contracts.Commands;
using EMailService.Persistence;
using MassTransit;

namespace EMailService.Application;

public class CompensateWelcomeEmailCommanddHandler(EMailDbContext db) : IConsumer<CompensateWelcomeEmailCommand>
{
    async public Task Consume(ConsumeContext<CompensateWelcomeEmailCommand> context)
    {
        var entity = db.EMails.Where(e => e.CorrelationId == context.Message.CorrelationId).FirstOrDefault();
        if (entity != null)
        {
            db.EMails.Remove(entity);
            await db.SaveChangesAsync();
        }
    }
}
