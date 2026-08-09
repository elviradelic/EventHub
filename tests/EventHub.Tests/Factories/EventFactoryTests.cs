using EventHub.Domain.Entities;
using EventHub.Domain.Enums;
using EventHub.Domain.Exceptions;
using EventHub.Infrastructure.Factories;

namespace EventHub.Tests.Factories;

public sealed class EventFactoryTests
{
    private readonly EventFactory _factory = new();

    [Theory]
    [InlineData(EventType.Concert, typeof(Concert))]
    [InlineData(EventType.Conference, typeof(Conference))]
    [InlineData(EventType.Workshop, typeof(Workshop))]
    public void Create_ShouldReturnCorrectEventType(
        EventType eventType,
        Type expectedType)
    {
        var venue = CreateVenue();

        var result = _factory.Create(
            eventType,
            Guid.NewGuid(),
            "Test Event",
            "Description",
            DateTime.UtcNow.AddDays(10),
            venue,
            100,
            50m,
            "Primary detail",
            "Secondary detail",
            SkillLevel.Beginner);

        Assert.IsType(expectedType, result);
    }

    [Fact]
    public void Create_WorkshopWithoutSkillLevel_ShouldThrowValidationException()
    {
        var venue = CreateVenue();

        Action action = () => _factory.Create(
            EventType.Workshop,
            Guid.NewGuid(),
            "C# Workshop",
            "Programming workshop",
            DateTime.UtcNow.AddDays(10),
            venue,
            25,
            30m,
            "Instructor",
            string.Empty);

        Assert.Throws<ValidationException>(action);
    }

    private static Venue CreateVenue()
    {
        return new Venue(
            "Main Hall",
            "Test Street 1",
            "Sarajevo",
            500);
    }
}