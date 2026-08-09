using EventHub.Application.Interfaces;
using EventHub.Domain.Entities;
using EventHub.Domain.Exceptions;
using EventHub.Domain.Enums;

namespace EventHub.Application.Services;

public sealed class BookingService
{
    private readonly IUserRepository _userRepository;
    private readonly IEventRepository _eventRepository;
    private readonly IBookingRepository _bookingRepository;
    private readonly IPricingStrategyResolver _pricingStrategyResolver;

    public BookingService(
        IUserRepository userRepository,
        IEventRepository eventRepository,
        IBookingRepository bookingRepository,
        IPricingStrategyResolver pricingStrategyResolver)
    {
        _userRepository = userRepository;
        _eventRepository = eventRepository;
        _bookingRepository = bookingRepository;
        _pricingStrategyResolver = pricingStrategyResolver;
    }

    public Booking CreateBooking(
        Guid customerId,
        Guid eventId,
        Domain.Enums.TicketType ticketType,
        int quantity)
    {
        var user = _userRepository.GetById(customerId)
            ?? throw new EntityNotFoundException(
                "Customer was not found.");

        if (user is not Customer)
        {
            throw new UnauthorizedOperationException(
                "Only customers can create bookings.");
        }

        var eventItem = _eventRepository.GetById(eventId)
            ?? throw new EntityNotFoundException(
                "Event was not found.");

        if (_bookingRepository.HasActiveBooking(
                customerId,
                eventId))
        {
            throw new DuplicateBookingException(
                "Customer already has an active booking for this event.");
        }

        if (!eventItem.SupportsTicketType(ticketType))
        {
            throw new UnsupportedTicketTypeException(
                $"Ticket type '{ticketType}' is not supported for this event.");
        }

        var pricingStrategy =
            _pricingStrategyResolver.Resolve(ticketType);

        var unitPrice =
            pricingStrategy.CalculateUnitPrice(eventItem);

        eventItem.ReserveSeats(quantity);

        try
        {
            var ticket = new Ticket(
                ticketType,
                unitPrice,
                quantity);

            var booking = new Booking(
                customerId,
                eventId,
                ticket);

            _bookingRepository.Add(booking);
            _eventRepository.Update(eventItem);

            return booking;
        }
        catch
        {
            eventItem.ReleaseSeats(quantity);
            throw;
        }
    }

    public void CancelBooking(
        Guid customerId,
        Guid bookingId)
    {
        var booking = _bookingRepository.GetById(bookingId)
            ?? throw new EntityNotFoundException(
                "Booking was not found.");

        if (booking.CustomerId != customerId)
        {
            throw new UnauthorizedOperationException(
                "You are not authorized to cancel this booking.");
        }

        var eventItem = _eventRepository.GetById(
                booking.EventId)
            ?? throw new EntityNotFoundException(
                "Event was not found.");

        booking.Cancel();
        eventItem.ReleaseSeats(booking.Ticket.Quantity);

        _bookingRepository.Update(booking);
        _eventRepository.Update(eventItem);
    }

    public IReadOnlyCollection<Booking> GetCustomerBookings(
        Guid customerId)
    {
        return _bookingRepository
            .GetByCustomerId(customerId);
    }
}