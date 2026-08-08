using EventHub.Domain.Common;
using EventHub.Domain.Enums;

namespace EventHub.Domain.Entities;

public sealed class Workshop : Event
{
    public string Instructor { get; private set; }

    public SkillLevel RequiredSkillLevel { get; private set; }

    public Workshop(
        Guid organizerId,
        string title,
        string description,
        DateTime startDate,
        Venue venue,
        int capacity,
        decimal basePrice,
        string instructor,
        SkillLevel requiredSkillLevel)
        : base(
            organizerId,
            title,
            description,
            startDate,
            venue,
            capacity,
            basePrice)
    {
        Instructor = Guard.AgainstNullOrWhiteSpace(
            instructor,
            nameof(instructor));

        RequiredSkillLevel = requiredSkillLevel;
    }

    public void UpdateWorkshopDetails(
        string instructor,
        SkillLevel requiredSkillLevel)
    {
        Instructor = Guard.AgainstNullOrWhiteSpace(
            instructor,
            nameof(instructor));

        RequiredSkillLevel = requiredSkillLevel;
    }

    public override bool SupportsTicketType(TicketType ticketType)
    {
        return ticketType == TicketType.Standard;
    }

    public override string GetEventDetails()
    {
        return $"{Title} - Instructor: {Instructor}, level: {RequiredSkillLevel}";
    }
}