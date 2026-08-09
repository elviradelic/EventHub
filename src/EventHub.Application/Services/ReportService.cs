using EventHub.Application.Interfaces;
using EventHub.Application.Reports;
using EventHub.Domain.Entities;
using EventHub.Domain.Enums;
using EventHub.Domain.Exceptions;

namespace EventHub.Application.Services;

public sealed class ReportService
{
    private readonly IUserRepository _userRepository;
    private readonly IEventRepository _eventRepository;
    private readonly IBookingRepository _bookingRepository;

    public ReportService(
        IUserRepository userRepository,
        IEventRepository eventRepository,
        IBookingRepository bookingRepository)
    {
        _userRepository = userRepository;
        _eventRepository = eventRepository;
        _bookingRepository = bookingRepository;
    }

    public EventReport GenerateEventReport(
        Guid organizerId,
        Guid eventId)
    {
        var user = _userRepository.GetById(organizerId)
            ?? throw new EntityNotFoundException(
                "Organizer was not found.");

        if (user is not Organizer)
        {
            throw new UnauthorizedOperationException(
                "Only organizers can generate event reports.");
        }

        var eventItem = _eventRepository.GetById(eventId)
            ?? throw new EntityNotFoundException(
                "Event was not found.");

        if (eventItem.OrganizerId != organizerId)
        {
            throw new UnauthorizedOperationException(
                "You are not authorized to view this event report.");
        }

        var bookings = _bookingRepository.GetByEventId(eventId);

        var confirmedBookings = bookings
            .Where(booking =>
                booking.Status == BookingStatus.Confirmed)
            .ToList();

        var cancelledBookings = bookings
            .Where(booking =>
                booking.Status == BookingStatus.Cancelled)
            .ToList();

        var confirmedTickets = confirmedBookings
            .Sum(booking => booking.Ticket.Quantity);

        var revenue = confirmedBookings
            .Sum(booking => booking.TotalPrice);

        var occupancyRate = eventItem.Capacity == 0
            ? 0m
            : (decimal)confirmedTickets /
              eventItem.Capacity * 100m;

        return new EventReport(
            eventItem.Id,
            eventItem.Title,
            eventItem.Capacity,
            confirmedTickets,
            cancelledBookings.Count,
            eventItem.AvailableSeats,
            occupancyRate,
            revenue);
    }
}