using EventHub.Domain.Entities;
using EventHub.Domain.Enums;
using EventHub.Domain.Exceptions;

namespace EventHub.Tests.Domain;

public sealed class EventTests
{
    private static Venue CreateVenue()
    {
        return new Venue(
            "Main Hall",
            "Test Street 1",
            "Sarajevo",
            500);
    }

    [Fact]
    public void Concert_WithValidData_ShouldStartAsDraft()
    {
        var concert = new Concert(
            Guid.NewGuid(),
            "Summer Concert",
            "Live music event",
            DateTime.UtcNow.AddDays(10),
            CreateVenue(),
            200,
            50m,
            "Test Performer",
            "Rock");

        Assert.Equal(EventStatus.Draft, concert.Status);
        Assert.Equal(200, concert.AvailableSeats);
        Assert.Equal(0, concert.ReservedSeats);
    }

    [Fact]
    public void Concert_WithCapacityGreaterThanVenue_ShouldThrowValidationException()
    {
        Action action = () => new Concert(
            Guid.NewGuid(),
            "Large Concert",
            "Test event",
            DateTime.UtcNow.AddDays(10),
            CreateVenue(),
            600,
            50m,
            "Performer",
            "Pop");

        Assert.Throws<ValidationException>(action);
    }

    [Fact]
    public void Publish_DraftEvent_ShouldChangeStatusToPublished()
    {
        var concert = new Concert(
            Guid.NewGuid(),
            "Summer Concert",
            "Live music event",
            DateTime.UtcNow.AddDays(10),
            CreateVenue(),
            200,
            50m,
            "Test Performer",
            "Rock");

        concert.Publish();

        Assert.Equal(EventStatus.Published, concert.Status);
    }

    [Fact]
    public void ReserveSeats_ShouldReduceAvailableSeats()
    {
        var concert = new Concert(
            Guid.NewGuid(),
            "Summer Concert",
            "Live music event",
            DateTime.UtcNow.AddDays(10),
            CreateVenue(),
            200,
            50m,
            "Test Performer",
            "Rock");

        concert.Publish();

        concert.ReserveSeats(5);

        Assert.Equal(195, concert.AvailableSeats);
        Assert.Equal(5, concert.ReservedSeats);
    }

    [Fact]
    public void ReserveLastSeats_ShouldChangeStatusToSoldOut()
    {
        var concert = new Concert(
            Guid.NewGuid(),
            "Small Concert",
            "Live music event",
            DateTime.UtcNow.AddDays(10),
            CreateVenue(),
            2,
            50m,
            "Test Performer",
            "Rock");

        concert.Publish();
        concert.ReserveSeats(2);

        Assert.Equal(0, concert.AvailableSeats);
        Assert.Equal(EventStatus.SoldOut, concert.Status);
    }

    [Fact]
    public void ReleaseSeats_FromSoldOutEvent_ShouldReturnStatusToPublished()
    {
        var concert = new Concert(
            Guid.NewGuid(),
            "Small Concert",
            "Live music event",
            DateTime.UtcNow.AddDays(10),
            CreateVenue(),
            2,
            50m,
            "Test Performer",
            "Rock");

        concert.Publish();
        concert.ReserveSeats(2);

        concert.ReleaseSeats(1);

        Assert.Equal(1, concert.AvailableSeats);
        Assert.Equal(EventStatus.Published, concert.Status);
    }

    [Fact]
    public void Concert_ShouldSupportStandardAndVipTickets()
    {
        Event concert = new Concert(
            Guid.NewGuid(),
            "Concert",
            "Description",
            DateTime.UtcNow.AddDays(10),
            CreateVenue(),
            100,
            50m,
            "Performer",
            "Rock");

        Assert.True(concert.SupportsTicketType(TicketType.Standard));
        Assert.True(concert.SupportsTicketType(TicketType.Vip));
        Assert.False(concert.SupportsTicketType(TicketType.Student));
    }

    [Fact]
    public void Conference_ShouldSupportStandardAndStudentTickets()
    {
        Event conference = new Conference(
            Guid.NewGuid(),
            "Tech Conference",
            "Technology conference",
            DateTime.UtcNow.AddDays(10),
            CreateVenue(),
            100,
            100m,
            "Artificial Intelligence",
            "Jane Doe");

        Assert.True(conference.SupportsTicketType(TicketType.Standard));
        Assert.True(conference.SupportsTicketType(TicketType.Student));
        Assert.False(conference.SupportsTicketType(TicketType.Vip));
    }

    [Fact]
    public void Workshop_ShouldSupportOnlyStandardTickets()
    {
        Event workshop = new Workshop(
            Guid.NewGuid(),
            "C# Workshop",
            "Programming workshop",
            DateTime.UtcNow.AddDays(10),
            CreateVenue(),
            25,
            30m,
            "John Doe",
            SkillLevel.Beginner);

        Assert.True(workshop.SupportsTicketType(TicketType.Standard));
        Assert.False(workshop.SupportsTicketType(TicketType.Vip));
        Assert.False(workshop.SupportsTicketType(TicketType.Student));
    }
}