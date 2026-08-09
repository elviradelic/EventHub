namespace EventHub.Application.Reports;

public sealed record EventReport(
    Guid EventId,
    string EventTitle,
    int Capacity,
    int ConfirmedTickets,
    int CancelledBookings,
    int AvailableSeats,
    decimal OccupancyRate,
    decimal Revenue);