using EventHub.Domain.Entities;
using EventHub.Domain.Enums;
using EventHub.Domain.Exceptions;

namespace EventHub.Tests.Domain;

public sealed class BookingTests
{
    [Fact]
    public void Ticket_WithValidData_ShouldCalculateTotalPrice()
    {
        var ticket = new Ticket(
            TicketType.Standard,
            25m,
            4);

        Assert.Equal(25m, ticket.UnitPrice);
        Assert.Equal(4, ticket.Quantity);
        Assert.Equal(100m, ticket.TotalPrice);
    }

    [Fact]
    public void Ticket_WithNegativePrice_ShouldThrowValidationException()
    {
        Action action = () => new Ticket(
            TicketType.Standard,
            -10m,
            1);

        Assert.Throws<ValidationException>(action);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Ticket_WithInvalidQuantity_ShouldThrowValidationException(
        int quantity)
    {
        Action action = () => new Ticket(
            TicketType.Standard,
            20m,
            quantity);

        Assert.Throws<ValidationException>(action);
    }

    [Fact]
    public void Booking_WithValidData_ShouldStartAsConfirmed()
    {
        var ticket = new Ticket(
            TicketType.Vip,
            75m,
            2);

        var booking = new Booking(
            Guid.NewGuid(),
            Guid.NewGuid(),
            ticket);

        Assert.NotEqual(Guid.Empty, booking.Id);
        Assert.Equal(BookingStatus.Confirmed, booking.Status);
        Assert.True(booking.IsActive());
        Assert.Null(booking.CancelledAt);
        Assert.Equal(150m, booking.TotalPrice);
    }

    [Fact]
    public void Cancel_ConfirmedBooking_ShouldChangeStatusToCancelled()
    {
        var booking = CreateBooking();

        booking.Cancel();

        Assert.Equal(BookingStatus.Cancelled, booking.Status);
        Assert.False(booking.IsActive());
        Assert.NotNull(booking.CancelledAt);
    }

    [Fact]
    public void Cancel_AlreadyCancelledBooking_ShouldThrowException()
    {
        var booking = CreateBooking();
        booking.Cancel();

        Action action = booking.Cancel;

        Assert.Throws<BookingAlreadyCancelledException>(action);
    }

    [Fact]
    public void Booking_WithEmptyCustomerId_ShouldThrowValidationException()
    {
        var ticket = new Ticket(
            TicketType.Standard,
            20m,
            1);

        Action action = () => new Booking(
            Guid.Empty,
            Guid.NewGuid(),
            ticket);

        Assert.Throws<ValidationException>(action);
    }

    [Fact]
    public void Booking_WithEmptyEventId_ShouldThrowValidationException()
    {
        var ticket = new Ticket(
            TicketType.Standard,
            20m,
            1);

        Action action = () => new Booking(
            Guid.NewGuid(),
            Guid.Empty,
            ticket);

        Assert.Throws<ValidationException>(action);
    }

    private static Booking CreateBooking()
    {
        var ticket = new Ticket(
            TicketType.Standard,
            30m,
            2);

        return new Booking(
            Guid.NewGuid(),
            Guid.NewGuid(),
            ticket);
    }
}