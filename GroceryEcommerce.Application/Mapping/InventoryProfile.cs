using AutoMapper;
using GroceryEcommerce.Application.Models.Inventory;
using GroceryEcommerce.Domain.Entities.Inventory;

namespace GroceryEcommerce.Application.Mapping;

public class InventoryProfile : Profile
{
    public InventoryProfile()
    {
        // Purchase Order mappings
        CreateMap<PurchaseOrder, PurchaseOrderDto>()
            .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => GetPurchaseOrderStatusName(src.Status)))
            .ForMember(dest => dest.CreatedByName, opt => opt.MapFrom(src => src.CreatedByUser != null ? $"{src.CreatedByUser.FirstName} {src.CreatedByUser.LastName}".Trim() : null))
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.PurchaseOrderItems));

        CreateMap<CreatePurchaseOrderRequest, PurchaseOrder>()
            .ForMember(dest => dest.PurchaseOrderId, opt => opt.MapFrom(src => Guid.NewGuid()))
            .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.PurchaseOrderItems, opt => opt.Ignore());

        CreateMap<UpdatePurchaseOrderRequest, PurchaseOrder>()
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        // Purchase Order Item mappings
        CreateMap<PurchaseOrderItem, PurchaseOrderItemDto>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
            .ForMember(dest => dest.ProductSku, opt => opt.MapFrom(src => src.Product.Sku))
            .ForMember(dest => dest.TotalCost, opt => opt.MapFrom(src => src.UnitCost * src.Quantity));

        CreateMap<CreatePurchaseOrderItemRequest, PurchaseOrderItem>()
            .ForMember(dest => dest.PurchaseOrderId, opt => opt.MapFrom(src => Guid.NewGuid()));
            // .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        CreateMap<UpdatePurchaseOrderItemRequest, PurchaseOrderItem>();
    }

    private static string GetPurchaseOrderStatusName(short status)
    {
        return status switch
        {
            1 => "Draft",
            2 => "Pending",
            3 => "Approved",
            4 => "Ordered",
            5 => "Received",
            6 => "Completed",
            7 => "Cancelled",
            _ => "Unknown"
        };
    }

}

