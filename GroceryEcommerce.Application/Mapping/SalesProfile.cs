using AutoMapper;
using GroceryEcommerce.Application.Models.Sales;
using GroceryEcommerce.Domain.Entities.Sales;

namespace GroceryEcommerce.Application.Mapping;

public class SalesProfile : Profile
{
    public SalesProfile()
    {
        // Order mappings
        CreateMap<Order, OrderDto>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}".Trim()))
            .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.User.Email))
            .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => GetOrderStatusName(src.Status)))
            .ForMember(dest => dest.PaymentStatusName, opt => opt.MapFrom(src => GetPaymentStatusName(src.PaymentStatus)))
            .ForMember(dest => dest.PaymentMethodName, opt => opt.MapFrom(src => GetPaymentMethodName(src.PaymentMethod)))
            .ForMember(dest => dest.CreatedByName, opt => opt.MapFrom(src => src.CreatedByUser != null ? $"{src.CreatedByUser.FirstName} {src.CreatedByUser.LastName}".Trim() : null))
            .ForMember(dest => dest.ShippingFullAddress, opt => opt.MapFrom(src => $"{src.ShippingAddress}, {src.ShippingCity}, {src.ShippingState} {src.ShippingZipCode}, {src.ShippingCountry}"))
            .ForMember(dest => dest.BillingFullAddress, opt => opt.MapFrom(src => $"{src.BillingAddress}, {src.BillingCity}, {src.BillingState} {src.BillingZipCode}, {src.BillingCountry}"));

        CreateMap<Order, OrderDetailDto>()
            .IncludeBase<Order, OrderDto>()
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.OrderItems))
            .ForMember(dest => dest.StatusHistory, opt => opt.MapFrom(src => src.OrderStatusHistories))
            .ForMember(dest => dest.Payments, opt => opt.MapFrom(src => src.OrderPayments))
            .ForMember(dest => dest.Shipments, opt => opt.MapFrom(src => src.OrderShipments))
            .ForMember(dest => dest.Refunds, opt => opt.MapFrom(src => src.OrderRefunds));

        CreateMap<CreateOrderRequest, Order>()
            .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => Guid.NewGuid()))
            .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        CreateMap<UpdateOrderRequest, Order>()
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        // Order Item mappings
        CreateMap<OrderItem, OrderItemDto>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
            .ForMember(dest => dest.ProductSku, opt => opt.MapFrom(src => src.Product.Sku))
            .ForMember(dest => dest.ProductImageUrl, opt => opt.MapFrom(src => src.Product.ProductImages.FirstOrDefault(i => i.IsPrimary) != null ? src.Product.ProductImages.FirstOrDefault(i => i.IsPrimary)!.ImageUrl : null))
            .ForMember(dest => dest.VariantName, opt => opt.MapFrom(src => src.ProductVariant != null ? src.ProductVariant.Name : null))
            .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.UnitPrice * src.Quantity));

        CreateMap<CreateOrderItemRequest, OrderItem>()
            .ForMember(dest => dest.OrderItemId, opt => opt.MapFrom(src => Guid.NewGuid()));

        // Order Status History mappings
        CreateMap<OrderStatusHistory, OrderStatusHistoryDto>()
            .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => GetOrderStatusName(src.ToStatus)))
            .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Comment))
            .ForMember(dest => dest.CreatedByName, opt => opt.MapFrom(src => src.CreatedByUser != null ? $"{src.CreatedByUser.FirstName} {src.CreatedByUser.LastName}".Trim() : null));

        CreateMap<CreateOrderStatusHistoryRequest, OrderStatusHistory>()
            .ForMember(dest => dest.HistoryId, opt => opt.MapFrom(src => Guid.NewGuid()))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        // Order Payment mappings
        CreateMap<OrderPayment, OrderPaymentDto>()
            .ForMember(dest => dest.OrderNumber, opt => opt.MapFrom(src => src.Order.OrderNumber))
            .ForMember(dest => dest.PaymentMethodName, opt => opt.MapFrom(src => GetPaymentMethodName(src.PaymentMethod)))
            .ForMember(dest => dest.PaymentStatusName, opt => opt.MapFrom(src => GetPaymentStatusName(src.Status)))
            .ForMember(dest => dest.ProcessedByName, opt => opt.MapFrom(src => src.ProcessedByUser != null ? $"{src.ProcessedByUser.FirstName} {src.ProcessedByUser.LastName}".Trim() : null));

        CreateMap<CreateOrderPaymentRequest, OrderPayment>()
            .ForMember(dest => dest.PaymentId, opt => opt.MapFrom(src => Guid.NewGuid()))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        CreateMap<UpdateOrderPaymentRequest, OrderPayment>()
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        // Order Shipment mappings
        CreateMap<OrderShipment, OrderShipmentDto>()
            .ForMember(dest => dest.OrderNumber, opt => opt.MapFrom(src => src.Order.OrderNumber))
            .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => GetShipmentStatusName(src.Status)))
            .ForMember(dest => dest.ShippedByName, opt => opt.MapFrom(src => src.ShippedByUser != null ? $"{src.ShippedByUser.FirstName} {src.ShippedByUser.LastName}".Trim() : null))
            .ForMember(dest => dest.ShippingAddress, opt => opt.MapFrom(src => $"{src.Order.ShippingAddress}, {src.Order.ShippingCity}, {src.Order.ShippingState} {src.Order.ShippingZipCode}, {src.Order.ShippingCountry}"));

        CreateMap<CreateOrderShipmentRequest, OrderShipment>()
            .ForMember(dest => dest.ShipmentId, opt => opt.MapFrom(src => Guid.NewGuid()))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        CreateMap<UpdateOrderShipmentRequest, OrderShipment>()
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        // Order Refund mappings
        CreateMap<OrderRefund, OrderRefundDto>()
            .ForMember(dest => dest.OrderNumber, opt => opt.MapFrom(src => src.Order.OrderNumber))
            .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => GetRefundStatusName(src.Status)))
            .ForMember(dest => dest.RequestedByName, opt => opt.MapFrom(src => src.RequestedByUser != null ? $"{src.RequestedByUser.FirstName} {src.RequestedByUser.LastName}".Trim() : null))
            .ForMember(dest => dest.ProcessedByName, opt => opt.MapFrom(src => src.ProcessedByUser != null ? $"{src.ProcessedByUser.FirstName} {src.ProcessedByUser.LastName}".Trim() : null));

        CreateMap<CreateOrderRefundRequest, OrderRefund>()
            .ForMember(dest => dest.RefundId, opt => opt.MapFrom(src => Guid.NewGuid()))
            .ForMember(dest => dest.RequestedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        CreateMap<UpdateOrderRefundRequest, OrderRefund>()
            .ForMember(dest => dest.ProcessedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
    }

    private static string GetOrderStatusName(short status)
    {
        return status switch
        {
            1 => "Pending",
            2 => "Processing",
            3 => "Shipped",
            4 => "Delivered",
            5 => "Cancelled",
            _ => "Unknown"
        };
    }

    private static string GetPaymentStatusName(short status)
    {
        return status switch
        {
            1 => "Pending",
            2 => "Paid",
            3 => "Failed",
            4 => "Refunded",
            _ => "Unknown"
        };
    }

    private static string GetPaymentMethodName(short method)
    {
        return method switch
        {
            1 => "Credit Card",
            2 => "PayPal",
            3 => "Bank Transfer",
            4 => "COD",
            _ => "Unknown"
        };
    }

    private static string GetShipmentStatusName(short status)
    {
        return status switch
        {
            1 => "Pending",
            2 => "Preparing",
            3 => "Shipped",
            4 => "In Transit",
            5 => "Delivered",
            6 => "Failed",
            _ => "Unknown"
        };
    }

    private static string GetRefundStatusName(short status)
    {
        return status switch
        {
            1 => "Requested",
            2 => "Approved",
            3 => "Processing",
            4 => "Completed",
            5 => "Rejected",
            _ => "Unknown"
        };
    }
}