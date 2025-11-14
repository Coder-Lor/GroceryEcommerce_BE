using AutoMapper;
using GroceryEcommerce.Application.Models.Catalog;
using GroceryEcommerce.Domain.Entities.Catalog;

namespace GroceryEcommerce.Application.Mapping;

public class CatalogProfile : Profile
{
    public CatalogProfile()
    {
        // Category mappings
        CreateMap<Category, CategoryDto>()
            .ForMember(dest => dest.ParentCategoryName,
                opt => opt.MapFrom(src => src.ParentCategory != null ? src.ParentCategory.Name : null))
            .ForMember(dest => dest.ProductCount, opt => opt.Ignore())
            .AfterMap((src, dest) =>
            {
                dest.ProductCount = src.Products != null ? src.Products.Count : 0;
            });

        CreateMap<CreateCategoryRequest, Category>()
            .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => Guid.NewGuid()))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        CreateMap<UpdateCategoryRequest, Category>()
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        CreateMap<Category, CreateCategoryResponse>()
            .IncludeBase<Category, CategoryDto>();

        CreateMap<Category, UpdateCategoryResponse>()
                .IncludeBase<Category, CategoryDto>();

        // Brand mappings
        CreateMap<Brand, BrandDto>()
            .ForMember(dest => dest.ProductCount, opt => opt.MapFrom(src => src.Products.Count))
            .IncludeAllDerived();

        // Product mappings
        CreateMap<Product, ProductDto>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : string.Empty))
            .ForMember(dest => dest.BrandName, opt => opt.MapFrom(src => src.Brand != null ? src.Brand.Name : null))
            .ForMember(dest => dest.PrimaryImageUrl, opt => opt.MapFrom(src => 
                src.ProductImages != null && src.ProductImages.Any(i => i.IsPrimary) 
                    ? src.ProductImages.FirstOrDefault(i => i.IsPrimary)!.ImageUrl 
                    : null))
            .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.ProductImages ?? new List<ProductImage>()))
            .ForMember(dest => dest.Variants, opt => opt.MapFrom(src => src.ProductVariants ?? new List<ProductVariant>()))
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => 
                src.ProductTagAssignments != null 
                    ? src.ProductTagAssignments.Select(pta => new ProductTagDto 
                    { 
                        ProductTagId = pta.ProductTag != null ? pta.ProductTag.TagId : Guid.Empty,
                        Name = pta.ProductTag != null ? pta.ProductTag.Name : string.Empty,
                        Slug = pta.ProductTag != null ? pta.ProductTag.Slug : string.Empty,
                        Description = pta.ProductTag != null ? pta.ProductTag.Description : string.Empty,
                        ProductCount = pta.ProductTag != null ? pta.ProductTag.ProductTagAssignments.Count : 0
                    })
                    : new List<ProductTagDto>()))
            .ForMember(dest => dest.AverageRating, opt => opt.MapFrom(src => 
                src.Reviews != null && src.Reviews.Any() 
                    ? (decimal?)src.Reviews.Average(r => r.Rating) 
                    : null))
            .ForMember(dest => dest.ReviewCount, opt => opt.MapFrom(src => src.Reviews != null ? src.Reviews.Count : 0));

        CreateMap<Product, ProductDetailDto>()
            .IncludeBase<Product, ProductDto>()
            .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src.ProductAttributeValues ?? new List<ProductAttributeValue>()))
            .ForMember(dest => dest.Questions, opt => opt.MapFrom(src => src.ProductQuestions ?? new List<ProductQuestion>()))
            .ForMember(dest => dest.Reviews, opt => opt.MapFrom(src => src.Reviews ?? new List<Domain.Entities.Reviews.ProductReview>()));
        // Add mapping for CreateProductResponse (inherits from ProductBaseResponse)
        CreateMap<Product, ProductBaseResponse>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : string.Empty))
            .ForMember(dest => dest.BrandName, opt => opt.MapFrom(src => src.Brand != null ? src.Brand.Name : null))
            .ForMember(dest => dest.PrimaryImageUrl, opt => opt.MapFrom(src =>
                src.ProductImages != null && src.ProductImages.Any(i => i.IsPrimary)
                    ? src.ProductImages.FirstOrDefault(i => i.IsPrimary)!.ImageUrl
                    : null))
            .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.ProductImages ?? new List<ProductImage>()))
            .ForMember(dest => dest.Variants, opt => opt.MapFrom(src => src.ProductVariants ?? new List<ProductVariant>()))
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src =>
                src.ProductTagAssignments != null
                    ? src.ProductTagAssignments.Select(pta => new ProductTagDto
                    {
                        ProductTagId = pta.ProductTag != null ? pta.ProductTag.TagId : Guid.Empty,
                        Name = pta.ProductTag != null ? pta.ProductTag.Name : string.Empty,
                        Slug = pta.ProductTag != null ? pta.ProductTag.Slug : string.Empty,
                        Description = pta.ProductTag != null ? pta.ProductTag.Description : string.Empty,
                        ProductCount = pta.ProductTag != null ? pta.ProductTag.ProductTagAssignments.Count : 0
                    })
                    : new List<ProductTagDto>()))
            .ForMember(dest => dest.AverageRating, opt => opt.MapFrom(src =>
                src.Reviews != null && src.Reviews.Any()
                    ? (decimal?)src.Reviews.Average(r => r.Rating)
                    : null))
            .ForMember(dest => dest.ReviewCount, opt => opt.MapFrom(src => src.Reviews != null ? src.Reviews.Count : 0))
            .IncludeAllDerived();

        CreateMap<Product, CreateProductResponse>()
            .IncludeBase<Product, ProductBaseResponse>();

        CreateMap<Product, GetProductBySlugResponse>()
            .IncludeBase<Product, ProductBaseResponse>();

        CreateMap<Product, ProductDetailDto>()
            .IncludeBase<Product, ProductDto>()
            .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src.ProductAttributeValues ?? new List<ProductAttributeValue>()))
            .ForMember(dest => dest.Questions, opt => opt.MapFrom(src => src.ProductQuestions ?? new List<ProductQuestion>()))
            .ForMember(dest => dest.Reviews, opt => opt.MapFrom(src => src.Reviews ?? new List<Domain.Entities.Reviews.ProductReview>()));

        CreateMap<Product, GetProductByIdResponse>()
            .IncludeBase<Product, ProductBaseResponse>();


        CreateMap<Category, CreateCategoryResponse>()
            .IncludeBase<Category, CategoryDto>()
            .ForMember(dest => dest.ParentCategoryName, opt => opt.MapFrom(src => src.ParentCategory != null ? src.ParentCategory.Name : null))
            .ForMember(dest => dest.ProductCount, opt => opt.MapFrom(src => src.Products.Count));

        CreateMap<Category, CategoryDto>()
            .ForMember(d => d.SubCategories, o => o.MapFrom(s => s.SubCategories ?? new List<Category>()))
            .PreserveReferences();
        
        CreateMap<IEnumerable<Category>, GetCategoryTreeResponse>()
            .ForMember(d => d.Categories, o => o.MapFrom(s => s));
        
        CreateMap<CreateProductRequest, Product>()
            .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => Guid.NewGuid()))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.ProductImages, opt => opt.Ignore())
            .ForMember(dest => dest.ProductVariants, opt => opt.Ignore())
            .ForMember(dest => dest.ProductTagAssignments, opt => opt.Ignore());

        CreateMap<UpdateProductRequest, Product>()
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        // Product Image mappings
        CreateMap<ProductImage, ProductImageDto>()
            .ForMember(dest => dest.ProductImageId, opt => opt.MapFrom(src => src.ImageId));

        CreateMap<CreateProductImageRequest, ProductImage>()
            .ForMember(dest => dest.ImageId, opt => opt.MapFrom(src => Guid.NewGuid()))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        CreateMap<UpdateProductImageRequest, ProductImage>();

        // Product Variant mappings
        CreateMap<ProductVariant, ProductVariantDto>()
            .ForMember(dest => dest.ProductVariantId, opt => opt.MapFrom(src => src.VariantId))
            .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src.VariantAttributeValues));

        // Add these mappings for response types
        CreateMap<ProductVariant, CreateProductVariantResponse>()
            .IncludeBase<ProductVariant, ProductVariantDto>();

        CreateMap<ProductVariant, UpdateProductVariantResponse>()
            .IncludeBase<ProductVariant, ProductVariantDto>();

        CreateMap<CreateProductVariantRequest, ProductVariant>()
            .ForMember(dest => dest.VariantId, opt => opt.MapFrom(src => Guid.NewGuid()))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.VariantAttributeValues, opt => opt.Ignore());

        CreateMap<UpdateProductVariantRequest, ProductVariant>()
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        // Product Attribute mappings
        CreateMap<ProductAttribute, ProductAttributeDto>();

        CreateMap<CreateProductAttributeRequest, ProductAttribute>()
            .ForMember(dest => dest.AttributeId, opt => opt.MapFrom(src => Guid.NewGuid()))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        CreateMap<UpdateProductAttributeRequest, ProductAttribute>()
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        // Product Attribute Value mappings
        CreateMap<ProductAttributeValue, ProductAttributeValueDto>()
            .ForMember(dest => dest.ProductAttributeName, opt => opt.MapFrom(src => src.Attribute.Name));

        CreateMap<CreateProductAttributeValueRequest, ProductAttributeValue>()
            .ForMember(dest => dest.ValueId, opt => opt.MapFrom(src => Guid.NewGuid()))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        CreateMap<UpdateProductAttributeValueRequest, ProductAttributeValue>();

        // Product Tag mappings
        CreateMap<ProductTag, ProductTagDto>()
            .ForMember(dest => dest.ProductCount, opt => opt.MapFrom(src => src.ProductTagAssignments.Count));

        CreateMap<CreateProductTagRequest, ProductTag>()
            .ForMember(dest => dest.TagId, opt => opt.MapFrom(src => Guid.NewGuid()))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        CreateMap<UpdateProductTagRequest, ProductTag>();

        // Product Question mappings
        CreateMap<ProductQuestion, ProductQuestionDto>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}".Trim()))
            .ForMember(dest => dest.AnsweredByName, opt => opt.MapFrom(src => src.AnsweredByUser != null ? $"{src.AnsweredByUser.FirstName} {src.AnsweredByUser.LastName}".Trim() : null));

        CreateMap<CreateProductQuestionRequest, ProductQuestion>()
            .ForMember(dest => dest.QuestionId, opt => opt.MapFrom(src => Guid.NewGuid()))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        CreateMap<UpdateProductQuestionRequest, ProductQuestion>()
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
    }
}
