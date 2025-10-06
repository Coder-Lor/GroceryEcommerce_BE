using AutoMapper;
using GroceryEcommerce.Domain.Entities.Catalog;
using GroceryEcommerce.EntityClasses;

namespace GroceryEcommerce.Infrastructure.Mapping;

public class CatalogMappingProfile : Profile
{
    public CatalogMappingProfile()
    {
        // Product mapping - AutoMapper tự động map các thuộc tính có tên giống nhau
        CreateMap<Product, ProductEntity>()
            .ForMember(dest => dest.Category, opt => opt.Ignore())
            .ForMember(dest => dest.Brand, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.User1, opt => opt.Ignore())
            .ForMember(dest => dest.ShoppingCartItems, opt => opt.Ignore())
            .ForMember(dest => dest.WishlistItems, opt => opt.Ignore())
            .ForMember(dest => dest.ProductAttributeValues, opt => opt.Ignore())
            .ForMember(dest => dest.ProductImages, opt => opt.Ignore())
            .ForMember(dest => dest.ProductQuestions, opt => opt.Ignore())
            .ForMember(dest => dest.ProductTagAssignments, opt => opt.Ignore())
            .ForMember(dest => dest.ProductVariants, opt => opt.Ignore())
            .ForMember(dest => dest.PurchaseOrderItems, opt => opt.Ignore())
            .ForMember(dest => dest.StockMovements, opt => opt.Ignore())
            .ForMember(dest => dest.ProductReviews, opt => opt.Ignore())
            .ForMember(dest => dest.OrderItems, opt => opt.Ignore());

        CreateMap<ProductEntity, Product>()
            .ForMember(dest => dest.Category, opt => opt.Ignore())
            .ForMember(dest => dest.Brand, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedByUser, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedByUser, opt => opt.Ignore())
            .ForMember(dest => dest.ProductImages, opt => opt.Ignore())
            .ForMember(dest => dest.ProductAttributeValues, opt => opt.Ignore())
            .ForMember(dest => dest.ProductVariants, opt => opt.Ignore())
            .ForMember(dest => dest.ProductTagAssignments, opt => opt.Ignore())
            .ForMember(dest => dest.ProductQuestions, opt => opt.Ignore())
            .ForMember(dest => dest.Reviews, opt => opt.Ignore())
            .ForMember(dest => dest.StockMovements, opt => opt.Ignore());

        // Category mapping - AutoMapper tự động map các thuộc tính có tên giống nhau
        CreateMap<Category, CategoryEntity>()
            .ForMember(dest => dest.Category, opt => opt.Ignore())
            .ForMember(dest => dest.Categories, opt => opt.Ignore())
            .ForMember(dest => dest.Products, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.User1, opt => opt.Ignore());

        CreateMap<CategoryEntity, Category>()
            .ForMember(dest => dest.ParentCategory, opt => opt.Ignore())
            .ForMember(dest => dest.SubCategories, opt => opt.Ignore())
            .ForMember(dest => dest.Products, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedByUser, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedByUser, opt => opt.Ignore());

        // Brand mapping - AutoMapper tự động map các thuộc tính có tên giống nhau
        CreateMap<Brand, BrandEntity>()
            .ForMember(dest => dest.Products, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.User1, opt => opt.Ignore());

        CreateMap<BrandEntity, Brand>()
            .ForMember(dest => dest.Products, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedByUser, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedByUser, opt => opt.Ignore());

        // ProductImage mapping - AutoMapper tự động map các thuộc tính có tên giống nhau
        CreateMap<ProductImage, ProductImageEntity>()
            .ForMember(dest => dest.Product, opt => opt.Ignore());

        CreateMap<ProductImageEntity, ProductImage>()
            .ForMember(dest => dest.Product, opt => opt.Ignore());

        // ProductVariant mapping - AutoMapper tự động map các thuộc tính có tên giống nhau
        CreateMap<ProductVariant, ProductVariantEntity>()
            .ForMember(dest => dest.Product, opt => opt.Ignore())
            .ForMember(dest => dest.ShoppingCartItems, opt => opt.Ignore())
            .ForMember(dest => dest.WishlistItems, opt => opt.Ignore())
            .ForMember(dest => dest.PurchaseOrderItems, opt => opt.Ignore())
            .ForMember(dest => dest.StockMovements, opt => opt.Ignore())
            .ForMember(dest => dest.OrderItems, opt => opt.Ignore());

        CreateMap<ProductVariantEntity, ProductVariant>()
            .ForMember(dest => dest.Product, opt => opt.Ignore());

        // ProductAttribute mapping - AutoMapper tự động map các thuộc tính có tên giống nhau
        CreateMap<ProductAttribute, ProductAttributeEntity>()
            .ForMember(dest => dest.ProductAttributeValues, opt => opt.Ignore());

        CreateMap<ProductAttributeEntity, ProductAttribute>()
            .ForMember(dest => dest.ProductAttributeValues, opt => opt.Ignore());

        // ProductAttributeValue mapping - AutoMapper tự động map các thuộc tính có tên giống nhau
        CreateMap<ProductAttributeValue, ProductAttributeValueEntity>()
            .ForMember(dest => dest.Product, opt => opt.Ignore())
            .ForMember(dest => dest.ProductAttribute, opt => opt.Ignore());

        CreateMap<ProductAttributeValueEntity, ProductAttributeValue>()
            .ForMember(dest => dest.Product, opt => opt.Ignore())
            .ForMember(dest => dest.Attribute, opt => opt.Ignore());

        // ProductTag mapping - AutoMapper tự động map các thuộc tính có tên giống nhau
        CreateMap<ProductTag, ProductTagEntity>()
            .ForMember(dest => dest.ProductTagAssignments, opt => opt.Ignore());

        CreateMap<ProductTagEntity, ProductTag>()
            .ForMember(dest => dest.ProductTagAssignments, opt => opt.Ignore());

        // ProductTagAssignment mapping - AutoMapper tự động map các thuộc tính có tên giống nhau
        CreateMap<ProductTagAssignment, ProductTagAssignmentEntity>()
            .ForMember(dest => dest.Product, opt => opt.Ignore())
            .ForMember(dest => dest.ProductTag, opt => opt.Ignore());

        CreateMap<ProductTagAssignmentEntity, ProductTagAssignment>()
            .ForMember(dest => dest.Product, opt => opt.Ignore())
            .ForMember(dest => dest.ProductTag, opt => opt.Ignore());

        // ProductQuestion mapping - AutoMapper tự động map các thuộc tính có tên giống nhau
        CreateMap<ProductQuestion, ProductQuestionEntity>()
            .ForMember(dest => dest.Product, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.User1, opt => opt.Ignore());

        CreateMap<ProductQuestionEntity, ProductQuestion>()
            .ForMember(dest => dest.Product, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.AnsweredByUser, opt => opt.Ignore());
    }
}
