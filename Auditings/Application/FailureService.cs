namespace Auditings.Application
{
    public class FailureService
    {
        public bool ShouldFail { get; private set; }
        public Task Fail(bool shouldFail)
        {
            ShouldFail = shouldFail;
            return Task.CompletedTask;
        }
    }
}
