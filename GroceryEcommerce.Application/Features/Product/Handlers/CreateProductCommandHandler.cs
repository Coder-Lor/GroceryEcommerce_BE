using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Product.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Models.Catalog;
using GroceryEcommerce.Domain.Entities.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

// Add aliases for domain entity types because this namespace contains a `Product` namespace
using ProductEntity = GroceryEcommerce.Domain.Entities.Catalog.Product;
using ProductImageEntity = GroceryEcommerce.Domain.Entities.Catalog.ProductImage;
using ProductVariantEntity = GroceryEcommerce.Domain.Entities.Catalog.ProductVariant;
using ProductAttributeValueEntity = GroceryEcommerce.Domain.Entities.Catalog.ProductAttributeValue;
using ProductTagAssignmentEntity = GroceryEcommerce.Domain.Entities.Catalog.ProductTagAssignment;

namespace GroceryEcommerce.Application.Features.Product.Handlers;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<CreateProductResponse>>
{
    private readonly IProductRepository _productRepository;
    private readonly IProductImageRepository _productImageRepository;
    private readonly IProductVariantRepository _productVariantRepository;
    private readonly IProductAttributeValueRepository _productAttributeValueRepository;
    private readonly IProductTagAssignmentRepository _productTagAssignmentRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateProductCommandHandler> _logger;

    public CreateProductCommandHandler(
        IProductRepository productRepository,
        IProductImageRepository productImageRepository,
        IProductVariantRepository productVariantRepository,
        IProductAttributeValueRepository productAttributeValueRepository,
        IProductTagAssignmentRepository productTagAssignmentRepository,
        IMapper mapper,
        ILogger<CreateProductCommandHandler> logger)
    {
        _productRepository = productRepository;
        _productImageRepository = productImageRepository;
        _productVariantRepository = productVariantRepository;
        _productAttributeValueRepository = productAttributeValueRepository;
        _productTagAssignmentRepository = productTagAssignmentRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<CreateProductResponse>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Creating product: {Name}", request.Name);

            // Check if product with same SKU already exists
            var existingProduct = await _productRepository.GetBySkuAsync(request.Sku, cancellationToken);
            if (existingProduct.IsSuccess && existingProduct.Data != null)
            {
                _logger.LogWarning("Product with SKU {Sku} already exists", request.Sku);
                return Result<CreateProductResponse>.Failure("Product with this SKU already exists.");
            }

            // Create new product entity
            var product = new ProductEntity
            {
                ProductId = Guid.NewGuid(),
                Name = request.Name,
                Slug = request.Slug,
                Sku = request.Sku,
                Description = request.Description,
                ShortDescription = request.ShortDescription,
                Price = request.Price,
                DiscountPrice = request.DiscountPrice,
                Cost = request.Cost,
                StockQuantity = request.StockQuantity,
                MinStockLevel = request.MinStockLevel,
                Weight = request.Weight,
                Dimensions = request.Dimensions,
                CategoryId = request.CategoryId,
                BrandId = request.BrandId,
                Status = request.Status,
                IsFeatured = request.IsFeatured,
                IsDigital = request.IsDigital,
                MetaTitle = request.MetaTitle,
                MetaDescription = request.MetaDescription,
                CreatedAt = DateTime.UtcNow
            };

            // Save product first
            var productResult = await _productRepository.CreateAsync(product, cancellationToken);
            if (!productResult.IsSuccess)
            {
                _logger.LogError("Failed to create product: {Name}", request.Name);
                return Result<CreateProductResponse>.Failure(productResult.ErrorMessage ?? "Unknown error occurred while creating the product.");
            }

            var createdProduct = productResult.Data;

            // Create product images
            if (request.Images != null && request.Images.Any())
            {
                foreach (var imageRequest in request.Images)
                {
                    var image = new ProductImageEntity
                    {
                        ImageId = Guid.NewGuid(),
                        ProductId = createdProduct!.ProductId,
                        ImageUrl = imageRequest.ImageUrl,
                        AltText = imageRequest.AltText,
                        IsPrimary = imageRequest.IsPrimary,
                        DisplayOrder = imageRequest.DisplayOrder,
                        CreatedAt = DateTime.UtcNow
                    };

                    var imageResult = await _productImageRepository.CreateAsync(image, cancellationToken);
                    if (!imageResult.IsSuccess)
                    {
                        _logger.LogWarning("Failed to create product image: {ImageUrl}", imageRequest.ImageUrl);
                    }
                }
            }

            // Create product variants
            if (request.Variants != null && request.Variants.Any())
            {
                foreach (var variantRequest in request.Variants)
                {
                    var variant = new ProductVariantEntity
                    {
                        VariantId = Guid.NewGuid(),
                        ProductId = createdProduct!.ProductId,
                        Name = variantRequest.Name,
                        Sku = variantRequest.Sku,
                        Price = variantRequest.Price,
                        DiscountPrice = variantRequest.DiscountPrice,
                        StockQuantity = variantRequest.StockQuantity,
                        Weight = variantRequest.Weight,
                        ImageUrl = variantRequest.ImageUrl,
                        Status = variantRequest.Status,
                        CreatedAt = DateTime.UtcNow
                    };

                    var variantResult = await _productVariantRepository.CreateAsync(variant, cancellationToken);
                    if (!variantResult.IsSuccess)
                    {
                        _logger.LogWarning("Failed to create product variant: {Name}", variantRequest.Name);
                    }
                }
            }

            // Create product attribute values
            if (request.Attributes != null && request.Attributes.Any())
            {
                foreach (var attributeRequest in request.Attributes)
                {
                    var attributeValue = new ProductAttributeValueEntity
                    {
                        ValueId = Guid.NewGuid(),
                        ProductId = createdProduct!.ProductId,
                        AttributeId = attributeRequest.ProductAttributeId,
                        Value = attributeRequest.Value,
                        CreatedAt = DateTime.UtcNow
                    };

                    var attributeResult = await _productAttributeValueRepository.CreateAsync(attributeValue, cancellationToken);
                    if (!attributeResult.IsSuccess)
                    {
                        _logger.LogWarning("Failed to create product attribute value: {ProductAttributeId}", attributeRequest.ProductAttributeId);
                    }
                }
            }

            // Assign tags to product
            if (request.TagIds != null && request.TagIds.Any())
            {
                foreach (var tagId in request.TagIds)
                {
                    var tagResult = await _productTagAssignmentRepository.AssignTagToProductAsync(createdProduct!.ProductId, tagId, cancellationToken);
                    if (!tagResult.IsSuccess)
                    {
                        _logger.LogWarning("Failed to assign tag to product: {TagId}", tagId);
                    }
                }
            }

            // Map to response
            var response = _mapper.Map<CreateProductResponse>(createdProduct);
            
            _logger.LogInformation("Product created successfully: {ProductId}", createdProduct!.ProductId);
            return Result<CreateProductResponse>.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating product: {Name}", request.Name);
            return Result<CreateProductResponse>.Failure("An error occurred while creating the product.");
        }
    }
}
