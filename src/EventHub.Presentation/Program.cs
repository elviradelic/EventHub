using EventHub.Application.Interfaces;
using EventHub.Application.Services;
using EventHub.Infrastructure.Factories;
using EventHub.Infrastructure.Pricing;
using EventHub.Infrastructure.Repositories;
using EventHub.Presentation.Menus;
using EventHub.Presentation.Seed;

var userRepository =
    new InMemoryUserRepository();

var eventRepository =
    new InMemoryEventRepository();

var bookingRepository =
    new InMemoryBookingRepository();

var eventFactory =
    new EventFactory();

IPricingStrategy[] pricingStrategies =
[
    new StandardPricingStrategy(),
    new VipPricingStrategy(),
    new StudentPricingStrategy()
];

var pricingResolver =
    new PricingStrategyResolver(
        pricingStrategies);

var eventService =
    new EventService(
        userRepository,
        eventRepository,
        bookingRepository,
        eventFactory);

var bookingService =
    new BookingService(
        userRepository,
        eventRepository,
        bookingRepository,
        pricingResolver);

var reportService =
    new ReportService(
        userRepository,
        eventRepository,
        bookingRepository);

var demoUsers =
    SampleDataSeeder.Seed(
        userRepository,
        eventService);

var app =
    new EventHubConsole(
        eventService,
        bookingService,
        reportService,
        demoUsers.Customer,
        demoUsers.Organizer);

app.Run();