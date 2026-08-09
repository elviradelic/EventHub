using EventHub.Domain.Entities;
using EventHub.Domain.Enums;
using EventHub.Infrastructure.Repositories;

namespace EventHub.Tests.Repositories;

public sealed class RepositoryTests
{
    [Fact]
    public void UserRepository_AddAndGetById_ShouldReturnUser()
    {
        var repository = new InMemoryUserRepository();
        var customer = new Customer(
            "Test Customer",
            "customer@example.com");

        repository.Add(customer);

        var result = repository.GetById(customer.Id);

        Assert.Same(customer, result);
    }

    [Fact]
    public void UserRepository_GetByEmail_ShouldBeCaseInsensitive()
    {
        var repository = new InMemoryUserRepository();
        var customer = new Customer(
            "Test Customer",
            "customer@example.com");

        repository.Add(customer);

        var result = repository.GetByEmail(
            "CUSTOMER@EXAMPLE.COM");

        Assert.Same(customer, result);
    }

    [Fact]
    public void EventRepository_GetByOrganizerId_ShouldReturnOwnedEvents()
    {
        var repository = new InMemoryEventRepository();
        var organizerId = Guid.NewGuid();

        var concert = CreateConcert(organizerId);

        repository.Add(concert);

        var result = repository.GetByOrganizerId(
            organizerId);

        Assert.Single(result);
        Assert.Equal(concert.Id, result.Single().Id);
    }

    [Fact]
    public void BookingRepository_HasActiveBooking_ShouldReturnTrue()
    {
        var repository = new InMemoryBookingRepository();

        var customerId = Guid.NewGuid();
        var eventId = Guid.NewGuid();

        var ticket = new Ticket(
            TicketType.Standard,
            25m,
            1);

        var booking = new Booking(
            customerId,
            eventId,
            ticket);

        repository.Add(booking);

        Assert.True(
            repository.HasActiveBooking(
                customerId,
                eventId));
    }

    [Fact]
    public void BookingRepository_HasActiveBooking_ShouldReturnFalseAfterCancellation()
    {
        var repository = new InMemoryBookingRepository();

        var customerId = Guid.NewGuid();
        var eventId = Guid.NewGuid();

        var ticket = new Ticket(
            TicketType.Standard,
            25m,
            1);

        var booking = new Booking(
            customerId,
            eventId,
            ticket);

        repository.Add(booking);

        booking.Cancel();
        repository.Update(booking);

        Assert.False(
            repository.HasActiveBooking(
                customerId,
                eventId));
    }

    private static Concert CreateConcert(Guid organizerId)
    {
        var venue = new Venue(
            "Main Hall",
            "Test Street 1",
            "Sarajevo",
            500);

        return new Concert(
            organizerId,
            "Concert",
            "Description",
            DateTime.UtcNow.AddDays(10),
            venue,
            100,
            50m,
            "Performer",
            "Rock");
    }
}