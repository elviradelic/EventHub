using EventHub.Domain.Common;
using EventHub.Domain.Enums;

namespace EventHub.Domain.Entities;

public sealed class Concert : Event
{
    public string Performer { get; private set; }

    public string Genre { get; private set; }

    public Concert(
        Guid organizerId,
        string title,
        string description,
        DateTime startDate,
        Venue venue,
        int capacity,
        decimal basePrice,
        string performer,
        string genre)
        : base(
            organizerId,
            title,
            description,
            startDate,
            venue,
            capacity,
            basePrice)
    {
        Performer = Guard.AgainstNullOrWhiteSpace(
            performer,
            nameof(performer));

        Genre = Guard.AgainstNullOrWhiteSpace(
            genre,
            nameof(genre));
    }

    public void UpdateConcertDetails(
        string performer,
        string genre)
    {
        Performer = Guard.AgainstNullOrWhiteSpace(
            performer,
            nameof(performer));

        Genre = Guard.AgainstNullOrWhiteSpace(
            genre,
            nameof(genre));
    }

    public override bool SupportsTicketType(TicketType ticketType)
    {
        return ticketType is TicketType.Standard or TicketType.Vip;
    }

    public override string GetEventDetails()
    {
        return $"{Title} - {Performer} ({Genre})";
    }
}