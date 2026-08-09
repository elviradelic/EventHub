using EventHub.Application.Interfaces;
using EventHub.Domain.Entities;

namespace EventHub.Infrastructure.Repositories;

public sealed class InMemoryBookingRepository : IBookingRepository
{
    private readonly List<Booking> _bookings = [];

    public Booking? GetById(Guid id)
    {
        return _bookings.FirstOrDefault(
            booking => booking.Id == id);
    }

    public IReadOnlyCollection<Booking> GetAll()
    {
        return _bookings.AsReadOnly();
    }

    public IReadOnlyCollection<Booking> GetByCustomerId(
        Guid customerId)
    {
        return _bookings
            .Where(booking => booking.CustomerId == customerId)
            .ToList()
            .AsReadOnly();
    }

    public IReadOnlyCollection<Booking> GetByEventId(
        Guid eventId)
    {
        return _bookings
            .Where(booking => booking.EventId == eventId)
            .ToList()
            .AsReadOnly();
    }

    public bool HasActiveBooking(
        Guid customerId,
        Guid eventId)
    {
        return _bookings.Any(booking =>
            booking.CustomerId == customerId &&
            booking.EventId == eventId &&
            booking.IsActive());
    }

    public void Add(Booking booking)
    {
        ArgumentNullException.ThrowIfNull(booking);

        _bookings.Add(booking);
    }

    public void Update(Booking booking)
    {
        ArgumentNullException.ThrowIfNull(booking);

        var existingIndex = _bookings.FindIndex(
            existing => existing.Id == booking.Id);

        if (existingIndex >= 0)
        {
            _bookings[existingIndex] = booking;
        }
    }
}