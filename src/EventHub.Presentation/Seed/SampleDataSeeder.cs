using EventHub.Application.Interfaces;
using EventHub.Application.Services;
using EventHub.Domain.Entities;
using EventHub.Domain.Enums;

namespace EventHub.Presentation.Seed;

public static class SampleDataSeeder
{
    public static (Customer Customer, Organizer Organizer) Seed(
        IUserRepository userRepository,
        EventService eventService)
    {
        var customer = new Customer(
            "Demo Customer",
            "customer@eventhub.com");

        var organizer = new Organizer(
            "Demo Organizer",
            "organizer@eventhub.com");

        userRepository.Add(customer);
        userRepository.Add(organizer);

        var venue = new Venue(
            "Sarajevo Event Center",
            "Zmaja od Bosne 1",
            "Sarajevo",
            500);

        var concert = eventService.CreateEvent(
            organizer.Id,
            EventType.Concert,
            "Summer Music Night",
            "Live summer concert in Sarajevo.",
            DateTime.UtcNow.AddDays(14),
            venue,
            250,
            40m,
            "The Skyline Band",
            "Pop");

        eventService.PublishEvent(
            organizer.Id,
            concert.Id);

        var conference = eventService.CreateEvent(
            organizer.Id,
            EventType.Conference,
            "Tech Future 2026",
            "Conference about modern software technologies.",
            DateTime.UtcNow.AddDays(21),
            venue,
            300,
            80m,
            "Software Engineering",
            "Jane Doe");

        eventService.PublishEvent(
            organizer.Id,
            conference.Id);

        return (customer, organizer);
    }
}