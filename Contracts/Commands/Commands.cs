using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Commands;

public record SendWelcomeEmailCommand(Guid CorrelationId, string UserEmail, string Message);
public record SendAuditLogCommand(Guid CorrelationId, string Data);

public record CompensateWelcomeEmailCommand(Guid CorrelationId);
public record CompensateAuditLogCommand(Guid CorrelationId);