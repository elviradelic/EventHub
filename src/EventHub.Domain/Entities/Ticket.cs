using EventHub.Domain.Common;
using EventHub.Domain.Enums;

namespace EventHub.Domain.Entities;

public sealed class Ticket
{
    public TicketType Type { get; }

    public decimal UnitPrice { get; }

    public int Quantity { get; }

    public decimal TotalPrice { get; }

    public Ticket(
        TicketType type,
        decimal unitPrice,
        int quantity)
    {
        Type = type;

        UnitPrice = Guard.AgainstNegative(
            unitPrice,
            nameof(unitPrice));

        Quantity = Guard.AgainstNonPositive(
            quantity,
            nameof(quantity));

        TotalPrice = UnitPrice * Quantity;
    }
}