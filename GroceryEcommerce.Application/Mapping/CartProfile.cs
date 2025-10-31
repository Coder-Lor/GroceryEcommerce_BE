using AutoMapper;
using GroceryEcommerce.Application.Models.Cart;
using GroceryEcommerce.Domain.Entities.Cart;
using GroceryEcommerce.Application.Features.Cart.ShoppingCart.Commands;
using GroceryEcommerce.Application.Features.Cart.Wishlist.Commands;

namespace GroceryEcommerce.Application.Mapping;

public class CartProfile : Profile
{
    public CartProfile()
    {
        // Request DTO to Command mappings
        CreateMap<AddToCartRequest, AddShoppingCartItemCommand>()
            .ConstructUsing(src => new AddShoppingCartItemCommand(src.ProductId, src.ProductVariantId, src.Quantity));

        CreateMap<UpdateQuantityRequest, UpdateShoppingCartItemQuantityCommand>()
            .ConstructUsing(src => new UpdateShoppingCartItemQuantityCommand(Guid.Empty, src.Quantity)); // itemId will be set from route

        CreateMap<AddToWishlistRequest, AddWishlistItemCommand>()
            .ConstructUsing(src => new AddWishlistItemCommand(Guid.Empty, src.ProductId, src.VariantId)); // userId will be set from route

        // Shopping Cart mappings
        CreateMap<ShoppingCart, ShoppingCartDto>()
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.ShoppingCartItems))
            .ForMember(dest => dest.SubTotal, opt => opt.MapFrom(src => src.ShoppingCartItems.Sum(item => item.UnitPrice * item.Quantity)))
            .ForMember(dest => dest.TaxAmount, opt => opt.Ignore()) // Will be calculated in service
            .ForMember(dest => dest.ShippingAmount, opt => opt.Ignore()) // Will be calculated in service
            .ForMember(dest => dest.DiscountAmount, opt => opt.Ignore()) // Will be calculated in service
            .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.ShoppingCartItems.Sum(item => item.UnitPrice * item.Quantity)))
            .ForMember(dest => dest.CouponCode, opt => opt.Ignore()); // Will be set in service

        // Shopping Cart Item mappings
        CreateMap<ShoppingCartItem, ShoppingCartItemDto>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
            .ForMember(dest => dest.ProductSku, opt => opt.MapFrom(src => src.Product.Sku))
            .ForMember(dest => dest.ProductImageUrl, opt => opt.MapFrom(src => src.Product.ProductImages.FirstOrDefault(i => i.IsPrimary) != null ? src.Product.ProductImages.FirstOrDefault(i => i.IsPrimary)!.ImageUrl : null))
            .ForMember(dest => dest.VariantName, opt => opt.MapFrom(src => src.ProductVariant != null ? src.ProductVariant.Name : null))
            .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.UnitPrice * src.Quantity));

        CreateMap<AddToCartRequest, ShoppingCartItem>()
            .ForMember(dest => dest.CartItemId, opt => opt.MapFrom(src => Guid.NewGuid()))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.Product, opt => opt.Ignore())
            .ForMember(dest => dest.ProductVariant, opt => opt.Ignore())
            .ForMember(dest => dest.ShoppingCart, opt => opt.Ignore());

        // Wishlist mappings
        CreateMap<Wishlist, WishlistDto>()
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.WishlistItems));

        // Wishlist Item mappings
        CreateMap<WishlistItem, WishlistItemDto>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
            .ForMember(dest => dest.ProductSku, opt => opt.MapFrom(src => src.Product.Sku))
            .ForMember(dest => dest.ProductImageUrl, opt => opt.MapFrom(src => src.Product.ProductImages.FirstOrDefault(i => i.IsPrimary) != null ? src.Product.ProductImages.FirstOrDefault(i => i.IsPrimary)!.ImageUrl : null))
            .ForMember(dest => dest.ProductPrice, opt => opt.MapFrom(src => src.Product.Price))
            .ForMember(dest => dest.ProductDiscountPrice, opt => opt.MapFrom(src => src.Product.DiscountPrice))
            .ForMember(dest => dest.ProductStockQuantity, opt => opt.MapFrom(src => src.Product.StockQuantity))
            .ForMember(dest => dest.VariantName, opt => opt.MapFrom(src => src.ProductVariant != null ? src.ProductVariant.Name : null));

        // Abandoned Cart mappings
        CreateMap<AbandonedCart, AbandonedCartDto>()
            .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.User!.Email))
            .ForMember(dest => dest.UserName,
                opt => opt.MapFrom(src => $"{src.User!.FirstName} {src.User.LastName}".Trim()))
            .ForMember(dest => dest.CartValue,
                opt => opt.MapFrom(src => src.ShoppingCart.ShoppingCartItems.Sum(item => item.UnitPrice * item.Quantity)))
            .ForMember(dest => dest.ItemCount, opt => opt.MapFrom(src => src.ShoppingCart.ShoppingCartItems.Count))
            .ForMember(dest => dest.IsRecovered, opt => opt.MapFrom(src => false))
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.ShoppingCart.ShoppingCartItems));
    }
}
