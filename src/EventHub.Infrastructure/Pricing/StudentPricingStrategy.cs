using EventHub.Application.Interfaces;
using EventHub.Domain.Entities;
using EventHub.Domain.Enums;

namespace EventHub.Infrastructure.Pricing;

public sealed class StudentPricingStrategy : IPricingStrategy
{
    private const decimal StudentMultiplier = 0.80m;

    public TicketType SupportedTicketType => TicketType.Student;

    public decimal CalculateUnitPrice(Event eventItem)
    {
        ArgumentNullException.ThrowIfNull(eventItem);

        if (!eventItem.SupportsTicketType(TicketType.Student))
        {
            throw new InvalidOperationException(
                "Student tickets are not supported for this event.");
        }

        return eventItem.BasePrice * StudentMultiplier;
    }
}