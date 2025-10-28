using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Catalog.Product.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Interfaces.Services;
using GroceryEcommerce.Application.Models.Catalog;
using GroceryEcommerce.Domain.Entities.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Catalog.Product.Handlers;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<CreateProductResponse>>
{
    private readonly IProductRepository _productRepository;
    private readonly IProductImageRepository _productImageRepository;
    private readonly IProductVariantRepository _productVariantRepository;
    private readonly IProductAttributeValueRepository _productAttributeValueRepository;
    private readonly IProductTagAssignmentRepository _productTagAssignmentRepository;
    private readonly IAzureBlobStorageService _blobStorageService;
    private readonly ICacheService _cacheService;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateProductCommandHandler> _logger;

    public CreateProductCommandHandler(
        IProductRepository productRepository,
        IProductImageRepository productImageRepository,
        IProductVariantRepository productVariantRepository,
        IProductAttributeValueRepository productAttributeValueRepository,
        IProductTagAssignmentRepository productTagAssignmentRepository,
        IAzureBlobStorageService blobStorageService,
        ICacheService cacheService,
        IMapper mapper,
        ILogger<CreateProductCommandHandler> logger)
    {
        _productRepository = productRepository;
        _productImageRepository = productImageRepository;
        _productVariantRepository = productVariantRepository;
        _productAttributeValueRepository = productAttributeValueRepository;
        _productTagAssignmentRepository = productTagAssignmentRepository;
        _blobStorageService = blobStorageService;
        _cacheService = cacheService;
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

            // Upload và tạo product images
            if (request.Images != null && request.Images.Any())
            {
                foreach (var imageRequest in request.Images)
                {
                    try
                    {
                        // Upload image to Azure Blob Storage
                        string imageUrl;
                        if (!string.IsNullOrEmpty(imageRequest.ImageUrl))
                        {
                            // Nếu đã có URL (đã upload trước đó), sử dụng URL đó
                            imageUrl = imageRequest.ImageUrl;
                        }
                        else if (imageRequest.ImageFile != null)
                        {
                            // Upload file mới
                            using var stream = imageRequest.ImageFile.OpenReadStream();
                            imageUrl = await _blobStorageService.UploadImageAsync(
                                stream, 
                                imageRequest.ImageFile.FileName, 
                                imageRequest.ImageFile.ContentType, 
                                cancellationToken);
                        }
                        else
                        {
                            _logger.LogWarning("No image URL or file provided for product image");
                            continue;
                        }

                        var image = new Domain.Entities.Catalog.ProductImage
                        {
                            ImageId = Guid.NewGuid(),
                            ProductId = createdProduct.ProductId,
                            ImageUrl = imageUrl,
                            AltText = imageRequest.AltText,
                            IsPrimary = imageRequest.IsPrimary,
                            DisplayOrder = imageRequest.DisplayOrder,
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
                        _logger.LogError(ex, "Error processing product image");
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
                        ProductId = createdProduct.ProductId,
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
                        ProductId = createdProduct.ProductId,
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

            
            var response = _mapper.Map<CreateProductResponse>(createdProduct);
            
            await _cacheService.RemoveByPatternAsync("Product_*", cancellationToken);
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
