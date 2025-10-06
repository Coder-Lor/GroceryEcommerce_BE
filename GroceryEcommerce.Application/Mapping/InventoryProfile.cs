using AutoMapper;
using GroceryEcommerce.Application.Models.Inventory;
using GroceryEcommerce.Domain.Entities.Inventory;

namespace GroceryEcommerce.Application.Mapping;

public class InventoryProfile : Profile
{
    public InventoryProfile()
    {
        // Warehouse mappings
        CreateMap<Warehouse, WarehouseDto>()
            .ForMember(dest => dest.ProductCount, opt => opt.MapFrom(src => src.StockMovements.Select(sm => sm.ProductId).Distinct().Count()))
            .ForMember(dest => dest.TotalStockValue, opt => opt.MapFrom(src => 0m)); // Will be calculated in service

        CreateMap<CreateWarehouseRequest, Warehouse>()
            .ForMember(dest => dest.WarehouseId, opt => opt.MapFrom(src => Guid.NewGuid()))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        CreateMap<UpdateWarehouseRequest, Warehouse>();

        // Supplier mappings
        CreateMap<Supplier, SupplierDto>()
            .ForMember(dest => dest.PurchaseOrderCount, opt => opt.MapFrom(src => src.PurchaseOrders.Count))
            .ForMember(dest => dest.TotalPurchaseAmount, opt => opt.MapFrom(src => src.PurchaseOrders.Sum(po => po.TotalAmount)));

        CreateMap<CreateSupplierRequest, Supplier>()
            .ForMember(dest => dest.SupplierId, opt => opt.MapFrom(src => Guid.NewGuid()))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        CreateMap<UpdateSupplierRequest, Supplier>();
            // .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))?

        // Purchase Order mappings
        CreateMap<PurchaseOrder, PurchaseOrderDto>()
            .ForMember(dest => dest.SupplierName, opt => opt.MapFrom(src => src.Supplier.Name))
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

        // Stock Movement mappings
        CreateMap<StockMovement, StockMovementDto>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
            .ForMember(dest => dest.ProductSku, opt => opt.MapFrom(src => src.Product.Sku))
            .ForMember(dest => dest.WarehouseName, opt => opt.MapFrom(src => src.Warehouse != null ? src.Warehouse.Name : null))
            .ForMember(dest => dest.MovementTypeName, opt => opt.MapFrom(src => GetMovementTypeName(src.MovementType)))
            .ForMember(dest => dest.CreatedByName, opt => opt.MapFrom(src => src.CreatedByUser != null ? $"{src.CreatedByUser.FirstName} {src.CreatedByUser.LastName}".Trim() : null));

        CreateMap<CreateStockMovementRequest, StockMovement>()
            .ForMember(dest => dest.MovementId, opt => opt.MapFrom(src => Guid.NewGuid()))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
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

    private static string GetMovementTypeName(short movementType)
    {
        return movementType switch
        {
            1 => "Purchase",
            2 => "Sale",
            3 => "Adjustment",
            4 => "Transfer In",
            5 => "Transfer Out",
            6 => "Return",
            7 => "Damage",
            8 => "Expiry",
            _ => "Unknown"
        };
    }
}

