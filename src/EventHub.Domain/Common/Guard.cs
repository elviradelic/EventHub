using EventHub.Domain.Exceptions;

namespace EventHub.Domain.Common;

public static class Guard
{
    public static string AgainstNullOrWhiteSpace(
        string? value,
        string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ValidationException(
                $"{parameterName} cannot be empty.");
        }

        return value.Trim();
    }

    public static int AgainstNonPositive(
        int value,
        string parameterName)
    {
        if (value <= 0)
        {
            throw new ValidationException(
                $"{parameterName} must be greater than zero.");
        }

        return value;
    }

    public static decimal AgainstNegative(
        decimal value,
        string parameterName)
    {
        if (value < 0)
        {
            throw new ValidationException(
                $"{parameterName} cannot be negative.");
        }

        return value;
    }

    public static DateTime AgainstPastDate(
        DateTime value,
        string parameterName)
    {
        if (value <= DateTime.UtcNow)
        {
            throw new ValidationException(
                $"{parameterName} must be in the future.");
        }

        return value;
    }

    public static Guid AgainstEmptyGuid(
        Guid value,
        string parameterName)
    {
        if (value == Guid.Empty)
        {
            throw new ValidationException(
                $"{parameterName} cannot be empty.");
        }

        return value;
    }
}