using EventHub.Application.Interfaces;
using EventHub.Application.Services;
using EventHub.Domain.Entities;
using EventHub.Domain.Enums;
using EventHub.Domain.Exceptions;
using EventHub.Infrastructure.Factories;
using EventHub.Infrastructure.Pricing;
using EventHub.Infrastructure.Repositories;

namespace EventHub.Tests.Services;

public sealed class ApplicationServiceTests
{
    private static Venue CreateVenue()
    {
        return new Venue(
            "Main Hall",
            "Test Street 1",
            "Sarajevo",
            500);
    }

    private static (
        InMemoryUserRepository users,
        InMemoryEventRepository events,
        InMemoryBookingRepository bookings,
        EventService eventService,
        BookingService bookingService,
        Customer customer,
        Organizer organizer)
        CreateServices()
    {
        var users = new InMemoryUserRepository();
        var events = new InMemoryEventRepository();
        var bookings = new InMemoryBookingRepository();

        var customer = new Customer(
            "Test Customer",
            "customer@example.com");

        var organizer = new Organizer(
            "Test Organizer",
            "organizer@example.com");

        users.Add(customer);
        users.Add(organizer);

        var factory = new EventFactory();

        IPricingStrategy[] strategies =
        [
            new StandardPricingStrategy(),
            new VipPricingStrategy(),
            new StudentPricingStrategy()
        ];

        var resolver = new PricingStrategyResolver(strategies);

        var eventService = new EventService(
            users,
            events,
            factory);

        var bookingService = new BookingService(
            users,
            events,
            bookings,
            resolver);

        return (
            users,
            events,
            bookings,
            eventService,
            bookingService,
            customer,
            organizer);
    }

    [Fact]
    public void Organizer_ShouldCreateAndPublishEvent()
    {
        var setup = CreateServices();

        var eventItem = setup.eventService.CreateEvent(
            setup.organizer.Id,
            EventType.Concert,
            "Summer Concert",
            "Live music event",
            DateTime.UtcNow.AddDays(10),
            CreateVenue(),
            100,
            50m,
            "Test Performer",
            "Rock");

        Assert.Equal(EventStatus.Draft, eventItem.Status);

        setup.eventService.PublishEvent(
            setup.organizer.Id,
            eventItem.Id);

        Assert.Equal(EventStatus.Published, eventItem.Status);
    }

    [Fact]
    public void Customer_ShouldCreateBookingAndReduceAvailableSeats()
    {
        var setup = CreateServices();

        var eventItem = setup.eventService.CreateEvent(
            setup.organizer.Id,
            EventType.Concert,
            "Summer Concert",
            "Live music event",
            DateTime.UtcNow.AddDays(10),
            CreateVenue(),
            100,
            50m,
            "Test Performer",
            "Rock");

        setup.eventService.PublishEvent(
            setup.organizer.Id,
            eventItem.Id);

        var booking = setup.bookingService.CreateBooking(
            setup.customer.Id,
            eventItem.Id,
            TicketType.Standard,
            2);

        Assert.Equal(BookingStatus.Confirmed, booking.Status);
        Assert.Equal(2, booking.Ticket.Quantity);
        Assert.Equal(98, eventItem.AvailableSeats);
        Assert.Equal(2, eventItem.ReservedSeats);
    }

    [Fact]
    public void Customer_ShouldNotCreateDuplicateActiveBooking()
    {
        var setup = CreateServices();

        var eventItem = setup.eventService.CreateEvent(
            setup.organizer.Id,
            EventType.Concert,
            "Summer Concert",
            "Live music event",
            DateTime.UtcNow.AddDays(10),
            CreateVenue(),
            100,
            50m,
            "Test Performer",
            "Rock");

        setup.eventService.PublishEvent(
            setup.organizer.Id,
            eventItem.Id);

        setup.bookingService.CreateBooking(
            setup.customer.Id,
            eventItem.Id,
            TicketType.Standard,
            1);

        Action action = () =>
            setup.bookingService.CreateBooking(
                setup.customer.Id,
                eventItem.Id,
                TicketType.Standard,
                1);

        Assert.Throws<DuplicateBookingException>(action);
    }

    [Fact]
    public void Customer_ShouldCancelBookingAndRestoreSeats()
    {
        var setup = CreateServices();

        var eventItem = setup.eventService.CreateEvent(
            setup.organizer.Id,
            EventType.Concert,
            "Summer Concert",
            "Live music event",
            DateTime.UtcNow.AddDays(10),
            CreateVenue(),
            100,
            50m,
            "Test Performer",
            "Rock");

        setup.eventService.PublishEvent(
            setup.organizer.Id,
            eventItem.Id);

        var booking = setup.bookingService.CreateBooking(
            setup.customer.Id,
            eventItem.Id,
            TicketType.Standard,
            3);

        Assert.Equal(97, eventItem.AvailableSeats);

        setup.bookingService.CancelBooking(
            setup.customer.Id,
            booking.Id);

        Assert.Equal(BookingStatus.Cancelled, booking.Status);
        Assert.Equal(100, eventItem.AvailableSeats);
        Assert.Equal(0, eventItem.ReservedSeats);
    }

    [Fact]
    public void Customer_ShouldNotBookUnsupportedTicketType()
    {
        var setup = CreateServices();

        var eventItem = setup.eventService.CreateEvent(
            setup.organizer.Id,
            EventType.Workshop,
            "C# Workshop",
            "Programming workshop",
            DateTime.UtcNow.AddDays(10),
            CreateVenue(),
            25,
            30m,
            "John Doe",
            string.Empty,
            SkillLevel.Beginner);

        setup.eventService.PublishEvent(
            setup.organizer.Id,
            eventItem.Id);

        Action action = () =>
            setup.bookingService.CreateBooking(
                setup.customer.Id,
                eventItem.Id,
                TicketType.Vip,
                1);

        Assert.Throws<UnsupportedTicketTypeException>(action);
    }

    [Fact]
    public void SearchEvents_ShouldFilterByCityAndType()
    {
        var setup = CreateServices();

        var concert = setup.eventService.CreateEvent(
            setup.organizer.Id,
            EventType.Concert,
            "Sarajevo Concert",
            "Live music event",
            DateTime.UtcNow.AddDays(10),
            CreateVenue(),
            100,
            50m,
            "Performer",
            "Rock");

        setup.eventService.PublishEvent(
            setup.organizer.Id,
            concert.Id);

        var result = setup.eventService.SearchEvents(
            eventType: EventType.Concert,
            city: "Sarajevo");

        Assert.Single(result);
        Assert.IsType<Concert>(result.Single());
    }

    [Fact]
    public void NonOrganizer_ShouldNotCreateEvent()
    {
        var setup = CreateServices();

        Action action = () =>
            setup.eventService.CreateEvent(
                setup.customer.Id,
                EventType.Concert,
                "Invalid Event",
                "Description",
                DateTime.UtcNow.AddDays(10),
                CreateVenue(),
                100,
                50m,
                "Performer",
                "Rock");

        Assert.Throws<UnauthorizedOperationException>(action);
    }
    [Fact]
public void ReportService_ShouldCalculateEventStatistics()
{
    var setup = CreateServices();

    var eventItem = setup.eventService.CreateEvent(
        setup.organizer.Id,
        EventType.Concert,
        "Summer Concert",
        "Live music event",
        DateTime.UtcNow.AddDays(10),
        CreateVenue(),
        100,
        50m,
        "Performer",
        "Rock");

    setup.eventService.PublishEvent(
        setup.organizer.Id,
        eventItem.Id);

    setup.bookingService.CreateBooking(
        setup.customer.Id,
        eventItem.Id,
        TicketType.Standard,
        4);

    var reportService = new ReportService(
        setup.users,
        setup.events,
        setup.bookings);

    var report = reportService.GenerateEventReport(
        setup.organizer.Id,
        eventItem.Id);

    Assert.Equal(eventItem.Id, report.EventId);
    Assert.Equal(100, report.Capacity);
    Assert.Equal(4, report.ConfirmedTickets);
    Assert.Equal(96, report.AvailableSeats);
    Assert.Equal(4m, report.OccupancyRate);
    Assert.Equal(200m, report.Revenue);
    Assert.Equal(0, report.CancelledBookings);
}
[Fact]
public void ReportService_ShouldRejectDifferentOrganizer()
{
    var setup = CreateServices();

    var eventItem = setup.eventService.CreateEvent(
        setup.organizer.Id,
        EventType.Concert,
        "Summer Concert",
        "Live music event",
        DateTime.UtcNow.AddDays(10),
        CreateVenue(),
        100,
        50m,
        "Performer",
        "Rock");

    var anotherOrganizer = new Organizer(
        "Another Organizer",
        "another@example.com");

    setup.users.Add(anotherOrganizer);

    var reportService = new ReportService(
        setup.users,
        setup.events,
        setup.bookings);

    Action action = () =>
        reportService.GenerateEventReport(
            anotherOrganizer.Id,
            eventItem.Id);

    Assert.Throws<UnauthorizedOperationException>(action);
}
}