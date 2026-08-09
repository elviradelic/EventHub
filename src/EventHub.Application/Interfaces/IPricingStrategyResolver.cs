using EventHub.Domain.Enums;

namespace EventHub.Application.Interfaces;

public interface IPricingStrategyResolver
{
    IPricingStrategy Resolve(TicketType ticketType);
}