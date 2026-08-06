namespace EventHub.Domain.Exceptions;

public sealed class InvalidEventStateException : EventHubException
{
    public InvalidEventStateException(string message)
        : base(message)
    {
    }
}