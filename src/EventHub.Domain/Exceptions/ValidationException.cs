namespace EventHub.Domain.Exceptions;

public sealed class ValidationException : EventHubException
{
    public ValidationException(string message)
        : base(message)
    {
    }
}