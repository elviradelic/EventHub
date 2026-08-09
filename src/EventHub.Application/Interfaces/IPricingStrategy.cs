using EventHub.Domain.Entities;
using EventHub.Domain.Enums;

namespace EventHub.Application.Interfaces;

public interface IPricingStrategy
{
    TicketType SupportedTicketType { get; }

    decimal CalculateUnitPrice(Event eventItem);
}