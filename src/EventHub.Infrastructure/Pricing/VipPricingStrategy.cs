using EventHub.Application.Interfaces;
using EventHub.Domain.Entities;
using EventHub.Domain.Enums;

namespace EventHub.Infrastructure.Pricing;

public sealed class VipPricingStrategy : IPricingStrategy
{
    private const decimal VipMultiplier = 1.50m;

    public TicketType SupportedTicketType => TicketType.Vip;

    public decimal CalculateUnitPrice(Event eventItem)
    {
        ArgumentNullException.ThrowIfNull(eventItem);

        if (!eventItem.SupportsTicketType(TicketType.Vip))
        {
            throw new InvalidOperationException(
                "VIP tickets are not supported for this event.");
        }

        return eventItem.BasePrice * VipMultiplier;
    }
}