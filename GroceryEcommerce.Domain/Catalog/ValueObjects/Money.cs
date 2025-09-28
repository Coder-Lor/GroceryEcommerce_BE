namespace GroceryEcommerce.Domain.Catalog.ValueObjects
{
    public record Money
    {
        public decimal Amount { get; }
        public string Currency { get; }

        public Money(decimal amount, string currency = "USD")
        {
            if (amount < 0)
                throw new ArgumentException("Amount cannot be negative", nameof(amount));
            
            if (string.IsNullOrWhiteSpace(currency))
                throw new ArgumentException("Currency cannot be empty", nameof(currency));

            Amount = Math.Round(amount, 2);
            Currency = currency.ToUpperInvariant();
        }

        public static Money Zero(string currency = "USD") => new(0, currency);

        public static Money operator +(Money left, Money right)
        {
            if (left.Currency != right.Currency)
                throw new InvalidOperationException("Cannot add different currencies");
            
            return new Money(left.Amount + right.Amount, left.Currency);
        }

        public static Money operator -(Money left, Money right)
        {
            if (left.Currency != right.Currency)
                throw new InvalidOperationException("Cannot subtract different currencies");
            
            return new Money(left.Amount - right.Amount, left.Currency);
        }

        public static Money operator *(Money money, decimal multiplier) => 
            new(money.Amount * multiplier, money.Currency);

        public static bool operator >(Money left, Money right)
        {
            if (left.Currency != right.Currency)
                throw new InvalidOperationException("Cannot compare different currencies");
            
            return left.Amount > right.Amount;
        }

        public static bool operator <(Money left, Money right)
        {
            if (left.Currency != right.Currency)
                throw new InvalidOperationException("Cannot compare different currencies");
            
            return left.Amount < right.Amount;
        }

        public static bool operator >=(Money left, Money right) => !(left < right);
        public static bool operator <=(Money left, Money right) => !(left > right);

        public override string ToString() => $"{Amount:C} {Currency}";
    }
}