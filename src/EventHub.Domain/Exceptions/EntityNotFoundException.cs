namespace EventHub.Domain.Exceptions;

public sealed class EntityNotFoundException : EventHubException
{
    public EntityNotFoundException(string message)
        : base(message)
    {
    }
}