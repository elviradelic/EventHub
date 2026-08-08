using EventHub.Domain.Common;

namespace EventHub.Domain.Entities;

public sealed class Venue
{
    public Guid Id { get; }

    public string Name { get; private set; }

    public string Address { get; private set; }

    public string City { get; private set; }

    public int MaximumCapacity { get; private set; }

    public Venue(
        string name,
        string address,
        string city,
        int maximumCapacity)
    {
        Id = Guid.NewGuid();

        Name = Guard.AgainstNullOrWhiteSpace(
            name,
            nameof(name));

        Address = Guard.AgainstNullOrWhiteSpace(
            address,
            nameof(address));

        City = Guard.AgainstNullOrWhiteSpace(
            city,
            nameof(city));

        MaximumCapacity = Guard.AgainstNonPositive(
            maximumCapacity,
            nameof(maximumCapacity));
    }

    public bool CanHost(int eventCapacity)
    {
        return eventCapacity > 0 &&
               eventCapacity <= MaximumCapacity;
    }

    public void UpdateDetails(
        string name,
        string address,
        string city,
        int maximumCapacity)
    {
        Name = Guard.AgainstNullOrWhiteSpace(
            name,
            nameof(name));

        Address = Guard.AgainstNullOrWhiteSpace(
            address,
            nameof(address));

        City = Guard.AgainstNullOrWhiteSpace(
            city,
            nameof(city));

        MaximumCapacity = Guard.AgainstNonPositive(
            maximumCapacity,
            nameof(maximumCapacity));
    }
}