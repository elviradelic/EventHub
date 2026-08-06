namespace EventHub.Domain.Exceptions;

public sealed class BookingAlreadyCancelledException : EventHubException
{
    public BookingAlreadyCancelledException(string message)
        : base(message)
    {
    }
}