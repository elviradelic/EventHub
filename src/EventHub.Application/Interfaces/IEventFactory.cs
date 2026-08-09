using EventHub.Domain.Entities;
using EventHub.Domain.Enums;

namespace EventHub.Application.Interfaces;

public interface IEventFactory
{
    Event Create(
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
        SkillLevel? skillLevel = null);
}