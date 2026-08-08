using EventHub.Domain.Common;
using EventHub.Domain.Enums;
using EventHub.Domain.Exceptions;

namespace EventHub.Domain.Entities;

public abstract class Event
{
    public Guid Id { get; }

    public Guid OrganizerId { get; }

    public string Title { get; private set; }

    public string Description { get; private set; }

    public DateTime StartDate { get; private set; }

    public Venue Venue { get; private set; }

    public int Capacity { get; private set; }

    public int AvailableSeats { get; private set; }

    public int ReservedSeats => Capacity - AvailableSeats;

    public decimal BasePrice { get; private set; }

    public EventStatus Status { get; private set; }

    protected Event(
        Guid organizerId,
        string title,
        string description,
        DateTime startDate,
        Venue venue,
        int capacity,
        decimal basePrice)
    {
        OrganizerId = Guard.AgainstEmptyGuid(
            organizerId,
            nameof(organizerId));

        Title = Guard.AgainstNullOrWhiteSpace(
            title,
            nameof(title));

        Description = Guard.AgainstNullOrWhiteSpace(
            description,
            nameof(description));

        StartDate = Guard.AgainstPastDate(
            startDate,
            nameof(startDate));

        Venue = venue
            ?? throw new ValidationException(
                "Venue is required.");

        Capacity = Guard.AgainstNonPositive(
            capacity,
            nameof(capacity));

        if (!Venue.CanHost(Capacity))
        {
            throw new ValidationException(
                "Event capacity cannot exceed venue capacity.");
        }

        BasePrice = Guard.AgainstNegative(
            basePrice,
            nameof(basePrice));

        Id = Guid.NewGuid();
        AvailableSeats = Capacity;
        Status = EventStatus.Draft;
    }

    public void Publish()
    {
        if (Status != EventStatus.Draft)
        {
            throw new InvalidEventStateException(
                "Only draft events can be published.");
        }

        if (!IsUpcoming())
        {
            throw new InvalidEventStateException(
                "An event must be in the future before it can be published.");
        }

        Status = EventStatus.Published;
    }

    public void Cancel()
    {
        if (Status is EventStatus.Cancelled or EventStatus.Completed)
        {
            throw new InvalidEventStateException(
                "Cancelled or completed events cannot be cancelled.");
        }

        Status = EventStatus.Cancelled;
    }

    public void ReserveSeats(int quantity)
    {
        Guard.AgainstNonPositive(quantity, nameof(quantity));

        if (Status != EventStatus.Published)
        {
            if (Status == EventStatus.SoldOut)
            {
                throw new EventSoldOutException(
                    "The event is sold out.");
            }

            throw new InvalidEventStateException(
                "Only published events can accept reservations.");
        }

        if (!IsUpcoming())
        {
            throw new InvalidEventStateException(
                "Bookings cannot be created after the event has started.");
        }

        if (!HasAvailableSeats(quantity))
        {
            throw new EventSoldOutException(
                "There are not enough available seats.");
        }

        AvailableSeats -= quantity;

        if (AvailableSeats == 0)
        {
            Status = EventStatus.SoldOut;
        }
    }

    public void ReleaseSeats(int quantity)
    {
        Guard.AgainstNonPositive(quantity, nameof(quantity));

        if (quantity > ReservedSeats)
        {
            throw new ValidationException(
                "Cannot release more seats than are currently reserved.");
        }

        AvailableSeats += quantity;

        if (Status == EventStatus.SoldOut &&
            AvailableSeats > 0 &&
            IsUpcoming())
        {
            Status = EventStatus.Published;
        }
    }

    public void UpdateBasicInformation(
        string title,
        string description,
        DateTime startDate,
        Venue venue,
        int capacity,
        decimal basePrice)
    {
        if (Status is EventStatus.Cancelled or EventStatus.Completed)
        {
            throw new InvalidEventStateException(
                "Cancelled or completed events cannot be updated.");
        }

        var validatedTitle = Guard.AgainstNullOrWhiteSpace(
            title,
            nameof(title));

        var validatedDescription = Guard.AgainstNullOrWhiteSpace(
            description,
            nameof(description));

        var validatedStartDate = Guard.AgainstPastDate(
            startDate,
            nameof(startDate));

        var validatedVenue = venue
            ?? throw new ValidationException(
                "Venue is required.");

        var validatedCapacity = Guard.AgainstNonPositive(
            capacity,
            nameof(capacity));

        var validatedPrice = Guard.AgainstNegative(
            basePrice,
            nameof(basePrice));

        if (!validatedVenue.CanHost(validatedCapacity))
        {
            throw new ValidationException(
                "Event capacity cannot exceed venue capacity.");
        }

        if (validatedCapacity < ReservedSeats)
        {
            throw new ValidationException(
                "Event capacity cannot be lower than the number of reserved seats.");
        }

        var reservedSeats = ReservedSeats;

        Title = validatedTitle;
        Description = validatedDescription;
        StartDate = validatedStartDate;
        Venue = validatedVenue;
        Capacity = validatedCapacity;
        AvailableSeats = Capacity - reservedSeats;
        BasePrice = validatedPrice;

        if (Status == EventStatus.SoldOut &&
            AvailableSeats > 0)
        {
            Status = EventStatus.Published;
        }
        else if (Status == EventStatus.Published &&
                 AvailableSeats == 0)
        {
            Status = EventStatus.SoldOut;
        }
    }

    public bool HasAvailableSeats(int quantity)
    {
        return quantity > 0 &&
               AvailableSeats >= quantity;
    }

    public bool IsUpcoming()
    {
        return StartDate > DateTime.UtcNow;
    }

    public bool CanBePublished()
    {
        return Status == EventStatus.Draft &&
               IsUpcoming() &&
               Capacity > 0 &&
               Venue.CanHost(Capacity);
    }

    public abstract bool SupportsTicketType(
        TicketType ticketType);

    public abstract string GetEventDetails();
}