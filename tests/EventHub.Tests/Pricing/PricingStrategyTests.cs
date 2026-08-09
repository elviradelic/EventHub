using EventHub.Application.Interfaces;
using EventHub.Domain.Entities;
using EventHub.Domain.Enums;
using EventHub.Infrastructure.Pricing;

namespace EventHub.Tests.Pricing;

public sealed class PricingStrategyTests
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
    public void StandardPricing_ShouldReturnBasePrice()
    {
        Event concert = CreateConcert(100m);
        var strategy = new StandardPricingStrategy();

        var price = strategy.CalculateUnitPrice(concert);

        Assert.Equal(100m, price);
    }

    [Fact]
    public void VipPricing_ShouldIncreasePriceByFiftyPercent()
    {
        Event concert = CreateConcert(100m);
        var strategy = new VipPricingStrategy();

        var price = strategy.CalculateUnitPrice(concert);

        Assert.Equal(150m, price);
    }

    [Fact]
    public void StudentPricing_ShouldApplyTwentyPercentDiscount()
    {
        Event conference = new Conference(
            Guid.NewGuid(),
            "Tech Conference",
            "Technology conference",
            DateTime.UtcNow.AddDays(10),
            CreateVenue(),
            100,
            100m,
            "AI",
            "Jane Doe");

        var strategy = new StudentPricingStrategy();

        var price = strategy.CalculateUnitPrice(conference);

        Assert.Equal(80m, price);
    }

    [Fact]
    public void Resolver_ShouldReturnStrategyForRequestedTicketType()
    {
        IPricingStrategy[] strategies =
        [
            new StandardPricingStrategy(),
            new VipPricingStrategy(),
            new StudentPricingStrategy()
        ];

        var resolver = new PricingStrategyResolver(strategies);

        var strategy = resolver.Resolve(TicketType.Vip);

        Assert.IsType<VipPricingStrategy>(strategy);
    }

    private static Concert CreateConcert(decimal basePrice)
    {
        return new Concert(
            Guid.NewGuid(),
            "Concert",
            "Description",
            DateTime.UtcNow.AddDays(10),
            CreateVenue(),
            100,
            basePrice,
            "Performer",
            "Rock");
    }
}