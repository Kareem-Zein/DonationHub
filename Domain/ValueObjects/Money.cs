using Domain.Abstractions;
using Domain.Primitivies;

namespace Domain.ValueObjects;

public sealed class Money : ValueObject
{
    public const int CurrencyISOCodeLength = 3;

    public string Currency { get; private set; } = null!;
    public decimal Amount { get; private set; }

    private Money(string currency, decimal amount)
    {
        Currency = currency;
        Amount = amount;
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Currency;
        yield return Amount;
    }

    public static Result<Money> Create(string currency, decimal amount)
    {
        if (string.IsNullOrWhiteSpace(currency))
            return Result<Money>.Failure(MoneyErrors.InvalidCurrency);

        var normalized = currency.Trim().ToUpperInvariant();

        if (normalized.Length != CurrencyISOCodeLength || !normalized.All(char.IsLetter))
            return Result<Money>.Failure(MoneyErrors.InvalidCurrency);

        if (amount < 0)
            return Result<Money>.Failure(MoneyErrors.NegativeValue);

        return Result<Money>.Success(new Money(normalized, amount));
    }

    public static Money Zero(string currency)
    {
        var result = Create(currency, 0m);

        if (result.IsFailure)
            throw new InvalidOperationException(
                $"Cannot create Zero Money: {result.Errors[0].Message}");

        return result.Value!;
    }

    public Money Add(Money other)
    {
        ArgumentNullException.ThrowIfNull(other);
        EnsureMatchingCurrency(other.Currency);

        return new Money(Currency, Amount + other.Amount);
    }


    private void EnsureMatchingCurrency(string currency)
    {
        if (!IsCurrenciesMatching(Currency, currency))
            throw new InvalidOperationException(
                $"Currency mismatch. Expected: {Currency}, Provided: {currency}");
    }

    private static bool IsCurrenciesMatching(string currency1, string currency2)
        => string.Equals(currency1, currency2, StringComparison.Ordinal);

    public bool HasSameCurrencyAs(Money other)
    {
        ArgumentNullException.ThrowIfNull(other);
        return IsCurrenciesMatching(Currency, other.Currency);
    }

    public static bool operator ==(Money left, Money right)
    {
        if (!IsCurrenciesMatching(left.Currency, right.Currency))
            return false;

        return left.Amount == right.Amount;
    }

    public static bool operator !=(Money left, Money right)
        => !(left == right);

    public static bool operator >=(Money left, Money right)
    {
        ArgumentNullException.ThrowIfNull(left);
        ArgumentNullException.ThrowIfNull(right);
        left.EnsureMatchingCurrency(right.Currency);
        return left.Amount >= right.Amount;
    }

    public static bool operator <=(Money left, Money right)
    {
        ArgumentNullException.ThrowIfNull(left);
        ArgumentNullException.ThrowIfNull(right);
        left.EnsureMatchingCurrency(right.Currency);
        return left.Amount <= right.Amount;
    }

    public static bool operator >(Money left, Money right)
    {
        left.EnsureMatchingCurrency(right.Currency);

        return left.Amount > right.Amount;
    }
    
    public static bool operator <(Money left, Money right)
    {
        left.EnsureMatchingCurrency(right.Currency);

        return left.Amount < right.Amount;
    }
}

public static class MoneyErrors
{
    public static readonly Error InvalidCurrency = Error.Validation("Money.InvalidCurrency", "The currency must be a valid 3-letter ISO 4217 code.");

    public static readonly Error NegativeValue = Error.Validation("Money.NegativeValue", "The amount cannot be negative.");

    public static readonly Error CurrencyMismatch = Error.Validation("Money.CurrencyMismatch", "The currencies do not match.");
}