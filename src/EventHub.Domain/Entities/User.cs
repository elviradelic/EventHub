using EventHub.Domain.Common;

namespace EventHub.Domain.Entities;

public abstract class User
{
    public Guid Id { get; }

    public string FullName { get; private set; }

    public string Email { get; private set; }

    protected User(string fullName, string email)
    {
        Id = Guid.NewGuid();
        FullName = Guard.AgainstNullOrWhiteSpace(
            fullName,
            nameof(fullName));

        Email = Guard.AgainstInvalidEmail(
            email,
            nameof(email));
    }

    public void UpdateProfile(string fullName, string email)
    {
        FullName = Guard.AgainstNullOrWhiteSpace(
            fullName,
            nameof(fullName));

        Email = Guard.AgainstInvalidEmail(
            email,
            nameof(email));
    }
}