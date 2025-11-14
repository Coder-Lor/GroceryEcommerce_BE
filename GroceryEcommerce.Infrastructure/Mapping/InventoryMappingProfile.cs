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
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.PurchaseOrderItems, opt => opt.Ignore());

        CreateMap<PurchaseOrderEntity, PurchaseOrder>()
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
    }
}
