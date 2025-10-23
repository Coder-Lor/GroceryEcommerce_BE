using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Product.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Interfaces.Services;
using GroceryEcommerce.Application.Models.Catalog;
using GroceryEcommerce.Domain.Entities.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Product.Handlers;

public class CreateProductWithFilesCommandHandler : IRequestHandler<CreateProductWithFilesCommand, Result<CreateProductResponse>>
{
    private readonly IProductRepository _productRepository;
    private readonly IProductImageRepository _productImageRepository;
    private readonly IProductVariantRepository _productVariantRepository;
    private readonly IProductAttributeValueRepository _productAttributeValueRepository;
    private readonly IProductTagAssignmentRepository _productTagAssignmentRepository;
    private readonly IAzureBlobStorageService _blobStorageService;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateProductWithFilesCommandHandler> _logger;

    public CreateProductWithFilesCommandHandler(
        IProductRepository productRepository,
        IProductImageRepository productImageRepository,
        IProductVariantRepository productVariantRepository,
        IProductAttributeValueRepository productAttributeValueRepository,
        IProductTagAssignmentRepository productTagAssignmentRepository,
        IAzureBlobStorageService blobStorageService,
        IMapper mapper,
        ILogger<CreateProductWithFilesCommandHandler> logger)
    {
        _productRepository = productRepository;
        _productImageRepository = productImageRepository;
        _productVariantRepository = productVariantRepository;
        _productAttributeValueRepository = productAttributeValueRepository;
        _productTagAssignmentRepository = productTagAssignmentRepository;
        _blobStorageService = blobStorageService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<CreateProductResponse>> Handle(CreateProductWithFilesCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Creating product with files: {Name}", request.Name);

            // Check if product with same SKU already exists
            var existingProduct = await _productRepository.GetBySkuAsync(request.Sku, cancellationToken);
            if (existingProduct.IsSuccess && existingProduct.Data != null)
            {
                _logger.LogWarning("Product with SKU {Sku} already exists", request.Sku);
                return Result<CreateProductResponse>.Failure("Product with this SKU already exists.");
            }

            // Create new product entity
            var product = new Domain.Entities.Catalog.Product
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

            // Upload và tạo product images từ files
            if (request.ImageFiles != null && request.ImageFiles.Any())
            {
                for (int i = 0; i < request.ImageFiles.Count; i++)
                {
                    try
                    {
                        var file = request.ImageFiles[i];
                        if (file.Length == 0) continue;

                        // Validate file type
                        var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };
                        if (!allowedTypes.Contains(file.ContentType))
                        {
                            _logger.LogWarning("Invalid file type for {FileName}", file.FileName);
                            continue;
                        }

                        // Validate file size (5MB max)
                        if (file.Length > 5 * 1024 * 1024)
                        {
                            _logger.LogWarning("File too large for {FileName}", file.FileName);
                            continue;
                        }

                        // Upload file to Azure Blob Storage
                        using var stream = file.OpenReadStream();
                        var imageUrl = await _blobStorageService.UploadImageAsync(
                            stream, 
                            file.FileName, 
                            file.ContentType, 
                            cancellationToken);

                        var image = new Domain.Entities.Catalog.ProductImage
                        {
                            ImageId = Guid.NewGuid(),
                            ProductId = createdProduct!.ProductId,
                            ImageUrl = imageUrl,
                            AltText = request.ImageAltTexts?.ElementAtOrDefault(i) ?? $"Product image {i + 1}",
                            IsPrimary = request.ImageIsPrimary?.ElementAtOrDefault(i) ?? (i == 0),
                            DisplayOrder = request.ImageDisplayOrders?.ElementAtOrDefault(i) ?? i,
                            CreatedAt = DateTime.UtcNow
                        };

                        var imageResult = await _productImageRepository.CreateAsync(image, cancellationToken);
                        if (!imageResult.IsSuccess)
                        {
                            _logger.LogWarning("Failed to create product image: {ImageUrl}", imageUrl);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing product image {Index}", i);
                    }
                }
            }

            // Create product variants
            if (request.Variants != null && request.Variants.Any())
            {
                foreach (var variantRequest in request.Variants)
                {
                    var variant = new Domain.Entities.Catalog.ProductVariant
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
                    var attributeValue = new Domain.Entities.Catalog.ProductAttributeValue
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
