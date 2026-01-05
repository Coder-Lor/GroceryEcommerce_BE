using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Catalog.Product.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Interfaces.Services;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Catalog.Product.Handlers;

public class UpdateProductWithFilesCommandHandler : IRequestHandler<UpdateProductWithFilesCommand, Result<UpdateProductResponse>>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IProductRepository _productRepository;
    private readonly IProductImageRepository _productImageRepository;
    private readonly IProductVariantRepository _productVariantRepository;
    private readonly IProductAttributeValueRepository _productAttributeValueRepository;
    private readonly IProductTagAssignmentRepository _productTagAssignmentRepository;
    private readonly IAzureBlobStorageService _blobStorageService;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateProductWithFilesCommandHandler> _logger;
    private readonly IUnitOfWorkService _unitOfWorkService;

    public UpdateProductWithFilesCommandHandler(
        IMapper mapper,
        ICurrentUserService currentUserService,
        IUnitOfWorkService unitOfWorkService,
        IProductRepository productRepository,
        IProductImageRepository productImageRepository,
        IProductVariantRepository productVariantRepository,
        IProductAttributeValueRepository productAttributeValueRepository,
        IProductTagAssignmentRepository productTagAssignmentRepository,
        IAzureBlobStorageService blobStorageService,
        ILogger<UpdateProductWithFilesCommandHandler> logger
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

    public async Task<Result<UpdateProductResponse>> Handle(UpdateProductWithFilesCommand request, CancellationToken cancellationToken)
    {
        Domain.Entities.Catalog.Product? updatedProduct = null;

        try
        {
            _logger.LogInformation("Updating product with files: {ProductId}", request.ProductId);

            var pagedRequest = new PagedRequest { Page = 1, PageSize = 1000 };

            // Get existing product
            var existingProductResult = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);
            if (!existingProductResult.IsSuccess || existingProductResult.Data is null)
            {
                _logger.LogWarning("Product not found: {ProductId}", request.ProductId);
                return Result<UpdateProductResponse>.Failure("Product not found");
            }

            var existingProduct = existingProductResult.Data;

            // Check if SKU is being changed and if it already exists for another product
            if (existingProduct.Sku != request.Sku)
            {
                var existingProductBySku = await _productRepository.GetBySkuAsync(request.Sku, cancellationToken);
                if (existingProductBySku.IsSuccess && existingProductBySku.Data != null 
                    && existingProductBySku.Data.ProductId != request.ProductId)
                {
                    _logger.LogWarning("Product with SKU {Sku} already exists", request.Sku);
                    return Result<UpdateProductResponse>.Failure("Product with this SKU already exists.");
                }
            }

            // Check if Slug is being changed and if it already exists for another product
            if (!string.IsNullOrEmpty(request.Slug) && existingProduct.Slug != request.Slug)
            {
                var existingProductBySlug = await _productRepository.GetBySlugAsync(request.Slug, cancellationToken);
                if (existingProductBySlug.IsSuccess && existingProductBySlug.Data != null 
                    && existingProductBySlug.Data.ProductId != request.ProductId)
                {
                    _logger.LogWarning("Product with Slug {Slug} already exists", request.Slug);
                    return Result<UpdateProductResponse>.Failure("Product with this Slug already exists.");
                }
            }

            await _unitOfWorkService.BeginTransactionAsync(cancellationToken);

            try
            {
                var userId = _currentUserService.GetCurrentUserId();

                // Update product properties
                existingProduct.Name = request.Name;
                existingProduct.Slug = request.Slug;
                existingProduct.Sku = request.Sku;
                existingProduct.Description = request.Description;
                existingProduct.ShortDescription = request.ShortDescription;
                existingProduct.Price = request.Price;
                existingProduct.DiscountPrice = request.DiscountPrice;
                existingProduct.Cost = request.Cost;
                existingProduct.StockQuantity = request.StockQuantity;
                existingProduct.MinStockLevel = request.MinStockLevel;
                existingProduct.Weight = request.Weight;
                existingProduct.Dimensions = request.Dimensions;
                existingProduct.CategoryId = request.CategoryId;
                existingProduct.BrandId = request.BrandId;
                existingProduct.ShopId = request.ShopId;
                existingProduct.Status = request.Status;
                existingProduct.IsFeatured = request.IsFeatured;
                existingProduct.IsDigital = request.IsDigital;
                existingProduct.MetaTitle = request.MetaTitle;
                existingProduct.MetaDescription = request.MetaDescription;
                existingProduct.UpdatedAt = DateTime.UtcNow;
                existingProduct.UpdatedBy = userId ?? Guid.Parse("2e6d6589-8790-46be-8c73-b582618ccada");

                // Update product
                var updateResult = await _productRepository.UpdateAsync(existingProduct, cancellationToken);
                if (!updateResult.IsSuccess)
                {
                    _logger.LogError("Failed to update product: {ProductId}", request.ProductId);
                    throw new InvalidOperationException(updateResult.ErrorMessage ?? "Unknown error occurred while updating the product.");
                }

                // Delete images
                if (request.ImageIdsToDelete != null && request.ImageIdsToDelete.Any())
                {
                    foreach (var imageId in request.ImageIdsToDelete)
                    {
                        try
                        {
                            var existingImageResult = await _productImageRepository.GetByIdAsync(imageId, cancellationToken);
                            if (existingImageResult.IsSuccess && existingImageResult.Data != null)
                            {
                                var image = existingImageResult.Data;
                                
                                // Extract blob name from URL (format: https://.../{blobName})
                                var blobName = ExtractBlobNameFromUrl(image.ImageUrl);
                                if (!string.IsNullOrEmpty(blobName))
                                {
                                    // Delete from blob storage
                                    await _blobStorageService.DeleteImageAsync(blobName, cancellationToken);
                                }

                                // Delete from database
                                await _productImageRepository.DeleteAsync(imageId, cancellationToken);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error deleting product image {ImageId}", imageId);
                        }
                    }
                }

                // Upload new images
                if (request.NewImageFiles != null && request.NewImageFiles.Any())
                {
                    for (int i = 0; i < request.NewImageFiles.Count; i++)
                    {
                        try
                        {
                            var file = request.NewImageFiles[i];
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
                                ProductId = existingProduct.ProductId,
                                ImageUrl = imageUrl,
                                AltText = request.NewImageAltTexts?.ElementAtOrDefault(i) ?? $"Product image {i + 1}",
                                IsPrimary = request.NewImageIsPrimary?.ElementAtOrDefault(i) ?? false,
                                DisplayOrder = request.NewImageDisplayOrders?.ElementAtOrDefault(i) ?? i,
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

                // Handle product variants (simple update - clear existing and add new)
                if (request.Variants != null && request.Variants.Any())
                {
                    // Get existing variants
                    var existingVariantsResult = await _productVariantRepository.GetByProductIdAsync(pagedRequest, existingProduct.ProductId, cancellationToken);
                    if (existingVariantsResult.IsSuccess && existingVariantsResult.Data != null)
                    {
                        // Delete existing variants
                        foreach (var variant in existingVariantsResult.Data.Items)
                        {
                            await _productVariantRepository.DeleteAsync(variant.VariantId, cancellationToken);
                        }
                    }

                    // Create new variants
                    foreach (var variantRequest in request.Variants)
                    {
                        var variant = new Domain.Entities.Catalog.ProductVariant
                        {
                            VariantId = Guid.NewGuid(),
                            ProductId = existingProduct.ProductId,
                            Name = variantRequest.Name ?? string.Empty,
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

                // Handle product attribute values (simple update - clear existing and add new)
                if (request.Attributes != null && request.Attributes.Any())
                {
                    // Get existing attributes
                    var existingAttributesResult = await _productAttributeValueRepository.GetByProductIdAsync(pagedRequest, existingProduct.ProductId, cancellationToken);
                    if (existingAttributesResult.IsSuccess && existingAttributesResult.Data != null)
                    {
                        // Delete existing attributes
                        foreach (var attribute in existingAttributesResult.Data.Items)
                        {
                            await _productAttributeValueRepository.DeleteAsync(attribute.ValueId, cancellationToken);
                        }
                    }

                    // Create new attributes
                    foreach (var attributeRequest in request.Attributes)
                    {
                        var attributeValue = new Domain.Entities.Catalog.ProductAttributeValue
                        {
                            ValueId = Guid.NewGuid(),
                            ProductId = existingProduct.ProductId,
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

                // Handle tags (simple update - clear existing and add new)
                if (request.TagIds != null && request.TagIds.Any())
                {
                    // Delete existing tag assignments
                    await _productTagAssignmentRepository.RemoveAllTagsFromProductAsync(existingProduct.ProductId, cancellationToken);

                    // Assign new tags
                    foreach (var tagId in request.TagIds)
                    {
                        var tagResult = await _productTagAssignmentRepository.AssignTagToProductAsync(existingProduct.ProductId, tagId, cancellationToken);
                        if (!tagResult.IsSuccess)
                        {
                            _logger.LogWarning("Failed to assign tag to product: {TagId}", tagId);
                        }
                    }
                }

                // Commit transaction
                await _unitOfWorkService.CommitAsync(cancellationToken);

                // Refresh entity from database
                var refreshedProductResult = await _productRepository.GetByIdAsync(existingProduct.ProductId, cancellationToken);
                if (!refreshedProductResult.IsSuccess || refreshedProductResult.Data == null)
                {
                    _logger.LogError("Failed to refresh product after update: {ProductId}", existingProduct.ProductId);
                    return Result<UpdateProductResponse>.Failure("Product updated but failed to retrieve updated data.");
                }

                updatedProduct = refreshedProductResult.Data;
            }
            catch
            {
                await _unitOfWorkService.RollbackAsync(cancellationToken);
                throw;
            }

            // Map to response safely
            try
            {
                var response = _mapper.Map<UpdateProductResponse>(updatedProduct);
                _logger.LogInformation("Product updated successfully: {ProductId}", updatedProduct.ProductId);
                return Result<UpdateProductResponse>.Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to map updated product to response for ProductId: {ProductId}", updatedProduct.ProductId);
                return Result<UpdateProductResponse>.Failure("Product updated but failed to prepare response.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating product: {ProductId}", request.ProductId);
            return Result<UpdateProductResponse>.Failure("An error occurred while updating the product.");
        }
    }

    private string? ExtractBlobNameFromUrl(string imageUrl)
    {
        if (string.IsNullOrEmpty(imageUrl))
            return null;

        try
        {
            var uri = new Uri(imageUrl);
            var path = uri.AbsolutePath;
            
            // Extract blob name from path (format: /container/{blobName} or /{blobName})
            var parts = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length > 0)
            {
                // Return the last part as blob name
                return parts[^1];
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to extract blob name from URL: {ImageUrl}", imageUrl);
        }

        return null;
    }
}

