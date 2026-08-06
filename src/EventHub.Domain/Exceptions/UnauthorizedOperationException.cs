namespace EventHub.Domain.Exceptions;

public sealed class UnauthorizedOperationException : EventHubException
{
    public UnauthorizedOperationException(string message)
        : base(message)
    {
    }
}