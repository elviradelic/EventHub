using EventHub.Domain.Common;
using EventHub.Domain.Enums;

namespace EventHub.Domain.Entities;

public sealed class Conference : Event
{
    public string Topic { get; private set; }

    public string KeynoteSpeaker { get; private set; }

    public Conference(
        Guid organizerId,
        string title,
        string description,
        DateTime startDate,
        Venue venue,
        int capacity,
        decimal basePrice,
        string topic,
        string keynoteSpeaker)
        : base(
            organizerId,
            title,
            description,
            startDate,
            venue,
            capacity,
            basePrice)
    {
        Topic = Guard.AgainstNullOrWhiteSpace(
            topic,
            nameof(topic));

        KeynoteSpeaker = Guard.AgainstNullOrWhiteSpace(
            keynoteSpeaker,
            nameof(keynoteSpeaker));
    }

    public void UpdateConferenceDetails(
        string topic,
        string keynoteSpeaker)
    {
        Topic = Guard.AgainstNullOrWhiteSpace(
            topic,
            nameof(topic));

        KeynoteSpeaker = Guard.AgainstNullOrWhiteSpace(
            keynoteSpeaker,
            nameof(keynoteSpeaker));
    }

    public override bool SupportsTicketType(TicketType ticketType)
    {
        return ticketType is TicketType.Standard or TicketType.Student;
    }

    public override string GetEventDetails()
    {
        return $"{Title} - {Topic}, keynote: {KeynoteSpeaker}";
    }
}