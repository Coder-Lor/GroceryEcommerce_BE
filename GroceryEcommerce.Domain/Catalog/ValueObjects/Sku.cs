namespace GroceryEcommerce.Domain.Catalog.ValueObjects
{
    public record Sku
    {
        public string Value { get; }

        public Sku(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("SKU cannot be empty", nameof(value));
            
            if (value.Length > 100)
                throw new ArgumentException("SKU cannot exceed 100 characters", nameof(value));

            Value = value.ToUpperInvariant();
        }

        public static implicit operator string(Sku sku) => sku.Value;
        public static implicit operator Sku(string sku) => new(sku);

        public override string ToString() => Value;
    }
}