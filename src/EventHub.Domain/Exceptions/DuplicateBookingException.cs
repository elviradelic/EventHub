namespace EventHub.Domain.Exceptions;

public sealed class DuplicateBookingException : EventHubException
{
    public DuplicateBookingException(string message)
        : base(message)
    {
    }
}