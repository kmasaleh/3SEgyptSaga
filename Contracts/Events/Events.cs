using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Events;


public record UserRegisteredNotificationCreated(Guid CorrelationId, string UserEmail,string Message);
public record WelcomeEmailSent(Guid CorrelationId,Guid EmailId);
public record WelcomeEmailFailed(Guid CorrelationId);
public record AuditLogged(Guid CorrelationId);
public record AuditFailed(Guid CorrelationId);