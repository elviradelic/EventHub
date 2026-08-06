using EventHub.Domain.Entities;
using EventHub.Domain.Exceptions;

namespace EventHub.Tests.Domain;

public sealed class UserTests
{
    [Fact]
    public void Customer_WithValidData_ShouldBeCreated()
    {
        // Arrange
        const string fullName = "Elvira Delic";
        const string email = "elvira@example.com";

        // Act
        var customer = new Customer(fullName, email);

        // Assert
        Assert.NotEqual(Guid.Empty, customer.Id);
        Assert.Equal(fullName, customer.FullName);
        Assert.Equal(email, customer.Email);
    }

    [Fact]
    public void Organizer_WithValidData_ShouldBeCreated()
    {
        // Arrange
        const string fullName = "Event Organizer";
        const string email = "organizer@example.com";

        // Act
        var organizer = new Organizer(fullName, email);

        // Assert
        Assert.NotEqual(Guid.Empty, organizer.Id);
        Assert.Equal(fullName, organizer.FullName);
        Assert.Equal(email, organizer.Email);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Customer_WithEmptyName_ShouldThrowValidationException(
        string fullName)
    {
        // Act
        Action action = () =>
            new Customer(fullName, "customer@example.com");

        // Assert
        Assert.Throws<ValidationException>(action);
    }

    [Theory]
    [InlineData("")]
    [InlineData("invalid-email")]
    [InlineData("customer@")]
    [InlineData("@example.com")]
    public void Customer_WithInvalidEmail_ShouldThrowValidationException(
        string email)
    {
        // Act
        Action action = () =>
            new Customer("Test Customer", email);

        // Assert
        Assert.Throws<ValidationException>(action);
    }

    [Fact]
    public void Customer_ShouldTrimNameAndNormalizeEmail()
    {
        // Act
        var customer = new Customer(
            "  Elvira Delic  ",
            "ELVIRA@EXAMPLE.COM");

        // Assert
        Assert.Equal("Elvira Delic", customer.FullName);
        Assert.Equal("elvira@example.com", customer.Email);
    }

    [Fact]
    public void UpdateProfile_WithValidData_ShouldUpdateUser()
    {
        // Arrange
        var customer = new Customer(
            "Old Name",
            "old@example.com");

        // Act
        customer.UpdateProfile(
            "New Name",
            "new@example.com");

        // Assert
        Assert.Equal("New Name", customer.FullName);
        Assert.Equal("new@example.com", customer.Email);
    }

    [Fact]
    public void UpdateProfile_WithInvalidEmail_ShouldThrowValidationException()
    {
        // Arrange
        var customer = new Customer(
            "Elvira Delic",
            "elvira@example.com");

        // Act
        Action action = () =>
            customer.UpdateProfile(
                "Elvira Delic",
                "invalid-email");

        // Assert
        Assert.Throws<ValidationException>(action);
    }
}