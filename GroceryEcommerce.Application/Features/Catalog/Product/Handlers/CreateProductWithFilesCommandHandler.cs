using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Catalog.Product.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Interfaces.Services;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Catalog.Product.Handlers;

public class CreateProductWithFilesCommandHandler : IRequestHandler<CreateProductWithFilesCommand, Result<CreateProductResponse>>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IProductRepository _productRepository;
    private readonly IProductImageRepository _productImageRepository;
    private readonly IProductVariantRepository _productVariantRepository;
    private readonly IProductAttributeValueRepository _productAttributeValueRepository;
    private readonly IProductTagAssignmentRepository _productTagAssignmentRepository;
    private readonly IAzureBlobStorageService _blobStorageService;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateProductWithFilesCommandHandler> _logger;
    private readonly IUnitOfWorkService _unitOfWorkService;
    public CreateProductWithFilesCommandHandler(
        IMapper mapper,
        ICurrentUserService currentUserService,
        IUnitOfWorkService unitOfWorkService,
        IProductRepository productRepository,
        IProductImageRepository productImageRepository,
        IProductVariantRepository productVariantRepository,
        IProductAttributeValueRepository productAttributeValueRepository,
        IProductTagAssignmentRepository productTagAssignmentRepository,
        IAzureBlobStorageService blobStorageService,
        ILogger<CreateProductWithFilesCommandHandler> logger
    )
    {
        _productRepository = productRepository;
        _productImageRepository = productImageRepository;
        _productVariantRepository = productVariantRepository;
        _productAttributeValueRepository = productAttributeValueRepository;
        _productTagAssignmentRepository = productTagAssignmentRepository;
        _blobStorageService = blobStorageService;
        _currentUserService = currentUserService;       
        _unitOfWorkService = unitOfWorkService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<CreateProductResponse>> Handle(CreateProductWithFilesCommand request, CancellationToken cancellationToken)
    {
        Domain.Entities.Catalog.Product? createdProduct = null;
        
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

            var existingProductBySlug = await _productRepository.GetBySlugAsync(request.Slug, cancellationToken);
            if (existingProductBySlug.IsSuccess && existingProductBySlug.Data != null)
            {
                _logger.LogWarning("Product with Slug {Slug} already exists", request.Slug);
                return Result<CreateProductResponse>.Failure("Product with this Slug already exists.");
            }

            await _unitOfWorkService.BeginTransactionAsync(cancellationToken);

            try
            {
                var userId = _currentUserService.GetCurrentUserId();
                var shopId = request.ShopId ?? _currentUserService.GetCurrentUserShopId();
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
                    ShopId = shopId,
                    Status = request.Status,
                    IsFeatured = request.IsFeatured,
                    IsDigital = request.IsDigital,
                    MetaTitle = request.MetaTitle,
                    MetaDescription = request.MetaDescription,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = userId ?? Guid.Parse("2e6d6589-8790-46be-8c73-b582618ccada")
                };
                
                var productResult = await _productRepository.CreateAsync(product, cancellationToken);
                if (!productResult.IsSuccess)
                {
                    _logger.LogError("Failed to create product: {Name}", request.Name);
                    throw new InvalidOperationException(productResult.ErrorMessage ?? "Unknown error occurred while creating the product.");
                }
                
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
                                ProductId = product.ProductId,
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
                    for (int i = 0; i < request.Variants.Count; i++)
                    {
                        var variantRequest = request.Variants[i];

                        try
                        {
                            // Bảo đảm SKU của variant không bị trống và không trùng
                            var baseSku = string.IsNullOrWhiteSpace(variantRequest.Sku)
                                ? $"{product.Sku}-VAR-{i + 1}"
                                : variantRequest.Sku.Trim();

                            var variantSku = baseSku;
                            var attempt = 0;
                            while (true)
                            {
                                var existingVariant = await _productVariantRepository.GetBySkuAsync(variantSku, cancellationToken);
                                if (!existingVariant.IsSuccess || existingVariant.Data is null)
                                {
                                    // Không trùng, dùng SKU này
                                    break;
                                }

                                attempt++;
                                variantSku = $"{baseSku}-{attempt}";
                            }

                            var variant = new Domain.Entities.Catalog.ProductVariant
                            {
                                VariantId = Guid.NewGuid(),
                                ProductId = product.ProductId,
                                // Guard against nulls in incoming variant DTOs to prevent possible null reference issues
                                Name = variantRequest.Name ?? string.Empty,
                                Sku = variantSku,
                                Price = variantRequest.Price,
                                DiscountPrice = variantRequest.DiscountPrice,
                                StockQuantity = variantRequest.StockQuantity,
                                MinStockLevel = variantRequest.MinStockLevel,
                                Weight = variantRequest.Weight,
                                ImageUrl = variantRequest.ImageUrl,
                                Status = variantRequest.Status,
                                CreatedAt = DateTime.UtcNow
                            };

                            var variantResult = await _productVariantRepository.CreateAsync(variant, cancellationToken);
                            if (!variantResult.IsSuccess)
                            {
                                _logger.LogWarning("Failed to create product variant: {Name} - {Error}", variantRequest.Name, variantResult.ErrorMessage);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error creating variant {Index} for product {ProductId}", i, product.ProductId);
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
                            ProductId = product.ProductId,
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
                        var tagResult = await _productTagAssignmentRepository.AssignTagToProductAsync(product.ProductId, tagId, cancellationToken);
                        if (!tagResult.IsSuccess)
                        {
                            _logger.LogWarning("Failed to assign tag to product: {TagId}", tagId);
                        }
                    }
                }

                // Commit transaction trước khi map response
                await _unitOfWorkService.CommitAsync(cancellationToken);
                
                // Refresh entity từ database để đảm bảo đồng bộ
                var refreshedProductResult = await _productRepository.GetByIdAsync(product.ProductId, cancellationToken);
                if (!refreshedProductResult.IsSuccess || refreshedProductResult.Data == null)
                {
                    _logger.LogError("Failed to refresh product after creation: {ProductId}", product.ProductId);
                    return Result<CreateProductResponse>.Failure("Product created but failed to retrieve updated data.");
                }
                
                createdProduct = refreshedProductResult.Data;
            }
            catch (Exception ex)
            {
                await _unitOfWorkService.RollbackAsync(cancellationToken);
                throw;
            }

            // Map to response safely
            try
            {
                var response = _mapper.Map<CreateProductResponse>(createdProduct);
                _logger.LogInformation("Product created successfully: {ProductId}", createdProduct.ProductId);
                return Result<CreateProductResponse>.Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to map created product to response for ProductId: {ProductId}", createdProduct.ProductId);
                return Result<CreateProductResponse>.Failure("Product created but failed to prepare response.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating product: {Name}", request.Name);
            return Result<CreateProductResponse>.Failure("An error occurred while creating the product.");
        }
    }
}
