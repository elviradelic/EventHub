using EventHub.Domain.Common;
using EventHub.Domain.Enums;
using EventHub.Domain.Exceptions;

namespace EventHub.Domain.Entities;

public sealed class Booking
{
    public Guid Id { get; }

    public Guid CustomerId { get; }

    public Guid EventId { get; }

    public Ticket Ticket { get; }

    public BookingStatus Status { get; private set; }

    public DateTime CreatedAt { get; }

    public DateTime? CancelledAt { get; private set; }

    public decimal TotalPrice => Ticket.TotalPrice;

    public Booking(
        Guid customerId,
        Guid eventId,
        Ticket ticket)
    {
        CustomerId = Guard.AgainstEmptyGuid(
            customerId,
            nameof(customerId));

        EventId = Guard.AgainstEmptyGuid(
            eventId,
            nameof(eventId));

        Ticket = ticket
            ?? throw new ValidationException(
                "Ticket is required.");

        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
        Status = BookingStatus.Confirmed;
    }

    public void Cancel()
    {
        if (Status == BookingStatus.Cancelled)
        {
            throw new BookingAlreadyCancelledException(
                "The booking has already been cancelled.");
        }

        Status = BookingStatus.Cancelled;
        CancelledAt = DateTime.UtcNow;
    }

    public bool IsActive()
    {
        return Status == BookingStatus.Confirmed;
    }
}