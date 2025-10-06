using AutoMapper;
using GroceryEcommerce.Domain.Entities.Auth;
using GroceryEcommerce.Domain.Entities.Cart;
using GroceryEcommerce.EntityClasses;

namespace GroceryEcommerce.Infrastructure.Mapping;

public class CartMappingProfile : Profile
{
    public CartMappingProfile()
    {
        // ShoppingCart mapping - AutoMapper tự động map các thuộc tính có tên giống nhau
        CreateMap<ShoppingCart, ShoppingCartEntity>()
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.ShoppingCartItems, opt => opt.Ignore())
            .ForMember(dest => dest.AbandonedCarts, opt => opt.Ignore());

        CreateMap<ShoppingCartEntity, ShoppingCart>()
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.ShoppingCartItems, opt => opt.Ignore());

        // ShoppingCartItem mapping - AutoMapper tự động map các thuộc tính có tên giống nhau
        CreateMap<ShoppingCartItem, ShoppingCartItemEntity>()
            .ForMember(dest => dest.ShoppingCart, opt => opt.Ignore())
            .ForMember(dest => dest.Product, opt => opt.Ignore())
            .ForMember(dest => dest.ProductVariant, opt => opt.Ignore());

        CreateMap<ShoppingCartItemEntity, ShoppingCartItem>()
            .ForMember(dest => dest.ShoppingCart, opt => opt.Ignore())
            .ForMember(dest => dest.Product, opt => opt.Ignore())
            .ForMember(dest => dest.ProductVariant, opt => opt.Ignore());

        // Wishlist mapping - AutoMapper tự động map các thuộc tính có tên giống nhau
        CreateMap<Wishlist, WishlistEntity>()
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.WishlistItems, opt => opt.Ignore());

        CreateMap<WishlistEntity, Wishlist>()
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.WishlistItems, opt => opt.Ignore());

        // WishlistItem mapping - AutoMapper tự động map các thuộc tính có tên giống nhau
        CreateMap<WishlistItem, WishlistItemEntity>()
            .ForMember(dest => dest.Wishlist, opt => opt.Ignore())
            .ForMember(dest => dest.Product, opt => opt.Ignore())
            .ForMember(dest => dest.ProductVariant, opt => opt.Ignore());

        CreateMap<WishlistItemEntity, WishlistItem>()
            .ForMember(dest => dest.Wishlist, opt => opt.Ignore())
            .ForMember(dest => dest.Product, opt => opt.Ignore())
            .ForMember(dest => dest.ProductVariant, opt => opt.Ignore());

        // AbandonedCart mapping - AutoMapper tự động map các thuộc tính có tên giống nhau
        CreateMap<AbandonedCart, AbandonedCartEntity>()
            .ForMember(dest => dest.ShoppingCart, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore());

        CreateMap<AbandonedCartEntity, AbandonedCart>()
            .ForMember<ShoppingCart>(dest => dest.ShoppingCart, opt => opt.Ignore())
            .ForMember<User>(dest => dest.User, opt => opt.Ignore());
    }
}
