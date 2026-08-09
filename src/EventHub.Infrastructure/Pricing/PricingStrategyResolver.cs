using EventHub.Application.Interfaces;
using EventHub.Domain.Enums;
using EventHub.Domain.Exceptions;

namespace EventHub.Infrastructure.Pricing;

public sealed class PricingStrategyResolver : IPricingStrategyResolver
{
    private readonly IReadOnlyDictionary<TicketType, IPricingStrategy> _strategies;

    public PricingStrategyResolver(
        IEnumerable<IPricingStrategy> strategies)
    {
        _strategies = strategies.ToDictionary(
            strategy => strategy.SupportedTicketType);
    }

    public IPricingStrategy Resolve(TicketType ticketType)
    {
        if (!_strategies.TryGetValue(ticketType, out var strategy))
        {
            throw new UnsupportedTicketTypeException(
                $"No pricing strategy is registered for ticket type '{ticketType}'.");
        }

        return strategy;
    }
}