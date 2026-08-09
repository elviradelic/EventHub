using EventHub.Application.Interfaces;
using EventHub.Domain.Entities;
using EventHub.Domain.Enums;
using EventHub.Domain.Exceptions;

namespace EventHub.Infrastructure.Factories;

public sealed class EventFactory : IEventFactory
{
    public Event Create(
        EventType eventType,
        Guid organizerId,
        string title,
        string description,
        DateTime startDate,
        Venue venue,
        int capacity,
        decimal basePrice,
        string primaryDetail,
        string secondaryDetail,
        SkillLevel? skillLevel = null)
    {
        return eventType switch
        {
            EventType.Concert => new Concert(
                organizerId,
                title,
                description,
                startDate,
                venue,
                capacity,
                basePrice,
                primaryDetail,
                secondaryDetail),

            EventType.Conference => new Conference(
                organizerId,
                title,
                description,
                startDate,
                venue,
                capacity,
                basePrice,
                primaryDetail,
                secondaryDetail),

            EventType.Workshop => new Workshop(
                organizerId,
                title,
                description,
                startDate,
                venue,
                capacity,
                basePrice,
                primaryDetail,
                skillLevel ?? throw new ValidationException(
                    "Skill level is required for workshops.")),

            _ => throw new ValidationException(
                $"Unsupported event type: {eventType}.")
        };
    }
}