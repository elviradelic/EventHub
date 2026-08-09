using EventHub.Application.Interfaces;
using EventHub.Domain.Entities;
using EventHub.Domain.Enums;
using EventHub.Domain.Exceptions;

namespace EventHub.Application.Services;

public sealed class EventService
{
    private readonly IUserRepository _userRepository;
    private readonly IEventRepository _eventRepository;
    private readonly IEventFactory _eventFactory;

    public EventService(
        IUserRepository userRepository,
        IEventRepository eventRepository,
        IEventFactory eventFactory)
    {
        _userRepository = userRepository;
        _eventRepository = eventRepository;
        _eventFactory = eventFactory;
    }

    public Event CreateEvent(
        Guid organizerId,
        EventType eventType,
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
        var user = _userRepository.GetById(organizerId)
            ?? throw new EntityNotFoundException(
                "Organizer was not found.");

        if (user is not Organizer)
        {
            throw new UnauthorizedOperationException(
                "Only organizers can create events.");
        }

        var eventItem = _eventFactory.Create(
            eventType,
            organizerId,
            title,
            description,
            startDate,
            venue,
            capacity,
            basePrice,
            primaryDetail,
            secondaryDetail,
            skillLevel);

        _eventRepository.Add(eventItem);

        return eventItem;
    }

    public void PublishEvent(
        Guid organizerId,
        Guid eventId)
    {
        var eventItem = GetEvent(eventId);

        EnsureOwnership(eventItem, organizerId);

        eventItem.Publish();
        _eventRepository.Update(eventItem);
    }

    public void CancelEvent(
        Guid organizerId,
        Guid eventId)
    {
        var eventItem = GetEvent(eventId);

        EnsureOwnership(eventItem, organizerId);

        eventItem.Cancel();
        _eventRepository.Update(eventItem);
    }

    public IReadOnlyCollection<Event> GetPublicEvents()
    {
        return _eventRepository
            .GetAll()
            .Where(eventItem =>
                eventItem.Status is EventStatus.Published
                    or EventStatus.SoldOut)
            .Where(eventItem => eventItem.IsUpcoming())
            .ToList()
            .AsReadOnly();
    }

    public IReadOnlyCollection<Event> SearchEvents(
        string? searchTerm = null,
        EventType? eventType = null,
        string? city = null)
    {
        IEnumerable<Event> query = GetPublicEvents();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(eventItem =>
                eventItem.Title.Contains(
                    searchTerm,
                    StringComparison.OrdinalIgnoreCase) ||
                eventItem.Description.Contains(
                    searchTerm,
                    StringComparison.OrdinalIgnoreCase) ||
                eventItem.GetEventDetails().Contains(
                    searchTerm,
                    StringComparison.OrdinalIgnoreCase));
        }

        if (eventType.HasValue)
        {
            query = query.Where(eventItem =>
                MatchesEventType(eventItem, eventType.Value));
        }

        if (!string.IsNullOrWhiteSpace(city))
        {
            query = query.Where(eventItem =>
                string.Equals(
                    eventItem.Venue.City,
                    city,
                    StringComparison.OrdinalIgnoreCase));
        }

        return query.ToList().AsReadOnly();
    }

    private Event GetEvent(Guid eventId)
    {
        return _eventRepository.GetById(eventId)
            ?? throw new EntityNotFoundException(
                "Event was not found.");
    }

    private static void EnsureOwnership(
        Event eventItem,
        Guid organizerId)
    {
        if (eventItem.OrganizerId != organizerId)
        {
            throw new UnauthorizedOperationException(
                "You are not authorized to manage this event.");
        }
    }

    private static bool MatchesEventType(
        Event eventItem,
        EventType eventType)
    {
        return eventType switch
        {
            EventType.Concert => eventItem is Concert,
            EventType.Conference => eventItem is Conference,
            EventType.Workshop => eventItem is Workshop,
            _ => false
        };
    }
}