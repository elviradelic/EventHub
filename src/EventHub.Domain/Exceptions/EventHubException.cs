namespace EventHub.Domain.Exceptions;

public abstract class EventHubException : Exception
{
    protected EventHubException(string message)
        : base(message)
    {
    }
}