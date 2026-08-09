using EventHub.Domain.Entities;

namespace EventHub.Application.Interfaces;

public interface IBookingRepository
{
    Booking? GetById(Guid id);

    IReadOnlyCollection<Booking> GetAll();

    IReadOnlyCollection<Booking> GetByCustomerId(Guid customerId);

    IReadOnlyCollection<Booking> GetByEventId(Guid eventId);

    bool HasActiveBooking(
        Guid customerId,
        Guid eventId);

    void Add(Booking booking);

    void Update(Booking booking);
}