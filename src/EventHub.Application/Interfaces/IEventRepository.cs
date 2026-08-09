using EventHub.Domain.Entities;

namespace EventHub.Application.Interfaces;

public interface IEventRepository
{
    Event? GetById(Guid id);

    IReadOnlyCollection<Event> GetAll();

    IReadOnlyCollection<Event> GetByOrganizerId(Guid organizerId);

    void Add(Event eventItem);

    void Update(Event eventItem);
}