namespace EventHub.Domain.Exceptions;

public sealed class UnsupportedTicketTypeException : EventHubException
{
    public UnsupportedTicketTypeException(string message)
        : base(message)
    {
    }
}