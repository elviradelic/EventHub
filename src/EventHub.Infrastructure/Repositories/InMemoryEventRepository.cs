using EventHub.Application.Interfaces;
using EventHub.Domain.Entities;

namespace EventHub.Infrastructure.Repositories;

public sealed class InMemoryEventRepository : IEventRepository
{
    private readonly List<Event> _events = [];

    public Event? GetById(Guid id)
    {
        return _events.FirstOrDefault(eventItem => eventItem.Id == id);
    }

    public IReadOnlyCollection<Event> GetAll()
    {
        return _events.AsReadOnly();
    }

    public IReadOnlyCollection<Event> GetByOrganizerId(Guid organizerId)
    {
        return _events
            .Where(eventItem => eventItem.OrganizerId == organizerId)
            .ToList()
            .AsReadOnly();
    }

    public void Add(Event eventItem)
    {
        ArgumentNullException.ThrowIfNull(eventItem);

        _events.Add(eventItem);
    }

    public void Update(Event eventItem)
    {
        ArgumentNullException.ThrowIfNull(eventItem);

        var existingIndex = _events.FindIndex(
            existing => existing.Id == eventItem.Id);

        if (existingIndex >= 0)
        {
            _events[existingIndex] = eventItem;
        }
    }
}