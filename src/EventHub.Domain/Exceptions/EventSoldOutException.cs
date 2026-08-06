namespace EventHub.Domain.Exceptions;

public sealed class EventSoldOutException : EventHubException
{
    public EventSoldOutException(string message)
        : base(message)
    {
    }
}