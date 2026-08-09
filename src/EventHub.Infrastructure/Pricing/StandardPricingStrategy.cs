using EventHub.Application.Interfaces;
using EventHub.Domain.Entities;
using EventHub.Domain.Enums;

namespace EventHub.Infrastructure.Pricing;

public sealed class StandardPricingStrategy : IPricingStrategy
{
    public TicketType SupportedTicketType => TicketType.Standard;

    public decimal CalculateUnitPrice(Event eventItem)
    {
        ArgumentNullException.ThrowIfNull(eventItem);

        return eventItem.BasePrice;
    }
}