using AutoMapper;
using GroceryEcommerce.Domain.Entities.Inventory;
using GroceryEcommerce.EntityClasses;

namespace GroceryEcommerce.Infrastructure.Mapping;

public class InventoryMappingProfile : Profile
{
    public InventoryMappingProfile()
    {
        // PurchaseOrder mapping - AutoMapper tự động map các thuộc tính có tên giống nhau
        CreateMap<PurchaseOrder, PurchaseOrderEntity>()
            .ForMember(dest => dest.Supplier, opt => opt.Ignore())
            .ForMember(dest => dest.Warehouse, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.PurchaseOrderItems, opt => opt.Ignore());

        CreateMap<PurchaseOrderEntity, PurchaseOrder>()
            .ForMember(dest => dest.Supplier, opt => opt.Ignore())
            .ForMember(dest => dest.Warehouse, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedByUser, opt => opt.Ignore())
            .ForMember(dest => dest.PurchaseOrderItems, opt => opt.Ignore());

        // PurchaseOrderItem mapping - AutoMapper tự động map các thuộc tính có tên giống nhau
        CreateMap<PurchaseOrderItem, PurchaseOrderItemEntity>()
            .ForMember(dest => dest.PurchaseOrder, opt => opt.Ignore())
            .ForMember(dest => dest.Product, opt => opt.Ignore())
            .ForMember(dest => dest.ProductVariant, opt => opt.Ignore());

        CreateMap<PurchaseOrderItemEntity, PurchaseOrderItem>()
            .ForMember(dest => dest.PurchaseOrder, opt => opt.Ignore())
            .ForMember(dest => dest.Product, opt => opt.Ignore())
            .ForMember(dest => dest.ProductVariant, opt => opt.Ignore());

        // StockMovement mapping - AutoMapper tự động map các thuộc tính có tên giống nhau
        CreateMap<StockMovement, StockMovementEntity>()
            .ForMember(dest => dest.Product, opt => opt.Ignore())
            .ForMember(dest => dest.ProductVariant, opt => opt.Ignore())
            .ForMember(dest => dest.Warehouse, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore());

        CreateMap<StockMovementEntity, StockMovement>()
            .ForMember(dest => dest.Product, opt => opt.Ignore())
            .ForMember(dest => dest.ProductVariant, opt => opt.Ignore())
            .ForMember(dest => dest.Warehouse, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedByUser, opt => opt.Ignore());

        // Supplier mapping - AutoMapper tự động map các thuộc tính có tên giống nhau
        CreateMap<Supplier, SupplierEntity>()
            .ForMember(dest => dest.PurchaseOrders, opt => opt.Ignore());

        CreateMap<SupplierEntity, Supplier>()
            .ForMember(dest => dest.PurchaseOrders, opt => opt.Ignore());

        // Warehouse mapping - AutoMapper tự động map các thuộc tính có tên giống nhau
        CreateMap<Warehouse, WarehouseEntity>()
            .ForMember(dest => dest.PurchaseOrders, opt => opt.Ignore())
            .ForMember(dest => dest.StockMovements, opt => opt.Ignore())
            .ForMember(dest => dest.OrderShipments, opt => opt.Ignore());

        CreateMap<WarehouseEntity, Warehouse>()
            .ForMember(dest => dest.PurchaseOrders, opt => opt.Ignore())
            .ForMember(dest => dest.StockMovements, opt => opt.Ignore())
            .ForMember(dest => dest.OrderShipments, opt => opt.Ignore());
    }
}
