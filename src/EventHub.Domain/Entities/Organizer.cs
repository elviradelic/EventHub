namespace EventHub.Domain.Entities;

public sealed class Organizer : User
{
    public Organizer(string fullName, string email)
        : base(fullName, email)
    {
    }
}