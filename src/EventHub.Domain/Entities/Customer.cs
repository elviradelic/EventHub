namespace EventHub.Domain.Entities;

public sealed class Customer : User
{
    public Customer(string fullName, string email)
        : base(fullName, email)
    {
    }
}