using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Product.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Interfaces.Services;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Product.Handlers;

public class CreateProductWithFilesCommandHandler(
    IMapper mapper,
    ICurrentUserService currentUserService,
    IUnitOfWorkService unitOfWorkService,
    IProductRepository productRepository,
    IProductImageRepository productImageRepository,
    IProductVariantRepository productVariantRepository,
    IProductAttributeValueRepository productAttributeValueRepository,
    IProductTagAssignmentRepository productTagAssignmentRepository,
    IAzureBlobStorageService blobStorageService,
    ILogger<CreateProductWithFilesCommandHandler> logger)
    : IRequestHandler<CreateProductWithFilesCommand, Result<CreateProductResponse>>
{
    public async Task<Result<CreateProductResponse>> Handle(CreateProductWithFilesCommand request, CancellationToken cancellationToken)
    {
        Domain.Entities.Catalog.Product? createdProduct = null;
        
        try
        {
            logger.LogInformation("Creating product with files: {Name}", request.Name);

            // Check if product with same SKU already exists
            var existingProduct = await productRepository.GetBySkuAsync(request.Sku, cancellationToken);
            if (existingProduct.IsSuccess && existingProduct.Data != null)
            {
                logger.LogWarning("Product with SKU {Sku} already exists", request.Sku);
                return Result<CreateProductResponse>.Failure("Product with this SKU already exists.");
            }
            
            await unitOfWorkService.BeginTransactionAsync(cancellationToken);

            try
            {
                var userId = currentUserService.GetCurrentUserId();
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
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = userId ?? Guid.Parse("2e6d6589-8790-46be-8c73-b582618ccada")
                };
                
                var productResult = await productRepository.CreateAsync(product, cancellationToken);
                if (!productResult.IsSuccess)
                {
                    logger.LogError("Failed to create product: {Name}", request.Name);
                    throw new InvalidOperationException(productResult.ErrorMessage ?? "Unknown error occurred while creating the product.");
                }

                createdProduct = productResult.Data;
                
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
                                logger.LogWarning("Invalid file type for {FileName}", file.FileName);
                                continue;
                            }

                            // Validate file size (5MB max)
                            if (file.Length > 5 * 1024 * 1024)
                            {
                                logger.LogWarning("File too large for {FileName}", file.FileName);
                                continue;
                            }

                            // Upload file to Azure Blob Storage
                            using var stream = file.OpenReadStream();
                            var imageUrl = await blobStorageService.UploadImageAsync(
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

                            var imageResult = await productImageRepository.CreateAsync(image, cancellationToken);
                            if (!imageResult.IsSuccess)
                            {
                                logger.LogWarning("Failed to create product image: {ImageUrl}", imageUrl);
                            }
                        }
                        catch (Exception ex)
                        {
                            logger.LogError(ex, "Error processing product image {Index}", i);
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
                            // Guard against nulls in incoming variant DTOs to prevent possible null reference issues
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

                        var variantResult = await productVariantRepository.CreateAsync(variant, cancellationToken);
                        if (!variantResult.IsSuccess)
                        {
                            logger.LogWarning("Failed to create product variant: {Name}", variantRequest.Name);
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

                        var attributeResult = await productAttributeValueRepository.CreateAsync(attributeValue, cancellationToken);
                        if (!attributeResult.IsSuccess)
                        {
                            logger.LogWarning("Failed to create product attribute value: {ProductAttributeId}", attributeRequest.ProductAttributeId);
                        }
                    }
                }

                // Assign tags to product
                if (request.TagIds != null && request.TagIds.Any())
                {
                    foreach (var tagId in request.TagIds)
                    {
                        var tagResult = await productTagAssignmentRepository.AssignTagToProductAsync(createdProduct!.ProductId, tagId, cancellationToken);
                        if (!tagResult.IsSuccess)
                        {
                            logger.LogWarning("Failed to assign tag to product: {TagId}", tagId);
                        }
                    }
                }

                // Commit transaction trước khi map response
                await unitOfWorkService.CommitAsync(cancellationToken);
                
                // Refresh entity từ database để đảm bảo đồng bộ
                var refreshedProductResult = await productRepository.GetByIdAsync(createdProduct.ProductId, cancellationToken);
                if (!refreshedProductResult.IsSuccess || refreshedProductResult.Data == null)
                {
                    logger.LogError("Failed to refresh product after creation: {ProductId}", createdProduct.ProductId);
                    return Result<CreateProductResponse>.Failure("Product created but failed to retrieve updated data.");
                }
                
                createdProduct = refreshedProductResult.Data;
            }
            catch
            {
                await unitOfWorkService.RollbackAsync(cancellationToken);
                throw;
            }

            // Map to response safely
            try
            {
                var response = mapper.Map<CreateProductResponse>(createdProduct);
                logger.LogInformation("Product created successfully: {ProductId}", createdProduct.ProductId);
                return Result<CreateProductResponse>.Success(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to map created product to response for ProductId: {ProductId}", createdProduct.ProductId);
                return Result<CreateProductResponse>.Failure("Product created but failed to prepare response.");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating product: {Name}", request.Name);
            return Result<CreateProductResponse>.Failure("An error occurred while creating the product.");
        }
    }
}
