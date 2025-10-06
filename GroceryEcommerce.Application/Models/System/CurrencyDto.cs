namespace GroceryEcommerce.Application.Models.System;

public class CurrencyDto
{
    public Guid CurrencyId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Symbol { get; set; }
    public string? SymbolPosition { get; set; }
    public int DecimalPlaces { get; set; }
    public decimal ExchangeRate { get; set; }
    public bool IsDefault { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? LastExchangeRateUpdate { get; set; }
}

public class CreateCurrencyRequest
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Symbol { get; set; }
    public string? SymbolPosition { get; set; }
    public int DecimalPlaces { get; set; }
    public decimal ExchangeRate { get; set; }
    public bool IsDefault { get; set; }
    public bool IsActive { get; set; } = true;
}

public class UpdateCurrencyRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Symbol { get; set; }
    public string? SymbolPosition { get; set; }
    public int DecimalPlaces { get; set; }
    public decimal ExchangeRate { get; set; }
    public bool IsDefault { get; set; }
    public bool IsActive { get; set; }
}

public class CurrencyConversionRequest
{
    public decimal Amount { get; set; }
    public string FromCurrency { get; set; } = string.Empty;
    public string ToCurrency { get; set; } = string.Empty;
}

public class CurrencyConversionResponse
{
    public decimal OriginalAmount { get; set; }
    public string FromCurrency { get; set; } = string.Empty;
    public decimal ConvertedAmount { get; set; }
    public string ToCurrency { get; set; } = string.Empty;
    public decimal ExchangeRate { get; set; }
    public DateTime ConversionDate { get; set; }
}
