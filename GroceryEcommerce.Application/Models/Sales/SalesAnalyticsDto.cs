namespace GroceryEcommerce.Application.Models.Sales;

public class SalesAnalyticsDto
{
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public decimal TotalSales { get; set; }
    public int TotalOrders { get; set; }
    public decimal AverageOrderValue { get; set; }
    public List<SalesByStatusDto> SalesByStatus { get; set; } = new();
    public List<TopSellingProductDto> TopSellingProducts { get; set; } = new();
    public List<SalesByDayDto> SalesByDay { get; set; } = new();
    public List<SalesByMonthDto> SalesByMonth { get; set; } = new();
    public List<PaymentMethodAnalyticsDto> PaymentMethodAnalytics { get; set; } = new();
    public List<ShippingAnalyticsDto> ShippingAnalytics { get; set; } = new();
}

public class SalesByStatusDto
{
    public short Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public int OrderCount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal Percentage { get; set; }
}

public class TopSellingProductDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductSku { get; set; } = string.Empty;
    public int TotalQuantitySold { get; set; }
    public decimal TotalRevenue { get; set; }
    public int OrderCount { get; set; }
    public decimal AveragePrice { get; set; }
}

public class SalesByDayDto
{
    public DateTime Date { get; set; }
    public int OrderCount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal AverageOrderValue { get; set; }
}

public class SalesByMonthDto
{
    public int Year { get; set; }
    public int Month { get; set; }
    public string MonthName { get; set; } = string.Empty;
    public int OrderCount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal AverageOrderValue { get; set; }
    public decimal GrowthRate { get; set; }
}

public class PaymentMethodAnalyticsDto
{
    public short PaymentMethod { get; set; }
    public string PaymentMethodName { get; set; } = string.Empty;
    public int OrderCount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal Percentage { get; set; }
    public decimal AverageTransactionValue { get; set; }
}

public class ShippingAnalyticsDto
{
    public Guid? CarrierId { get; set; }
    public string? CarrierName { get; set; }
    public int ShipmentCount { get; set; }
    public decimal TotalShippingCost { get; set; }
    public decimal AverageShippingCost { get; set; }
    public int AverageDeliveryDays { get; set; }
    public decimal OnTimeDeliveryRate { get; set; }
}
