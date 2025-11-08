using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Catalog.Category.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Interfaces.Services;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Catalog.Category.Handlers;

public class CreateCategoryCommandHandler(
    IMapper mapper,
    ICategoryRepository categoryRepository,
    ICurrentUserService currentUserService,
    IAzureBlobStorageService blobStorageService,
    ILogger<CreateCategoryCommandHandler> logger
) : IRequestHandler<CreateCategoryCommand, Result<CreateCategoryResponse>>
{
    public async Task<Result<CreateCategoryResponse>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Creating category: {Name}", request.Name);

            var existingCategory = await categoryRepository.GetByNameAsync(request.Name, cancellationToken);
            if (existingCategory.IsSuccess && existingCategory.Data != null)
            {
                logger.LogWarning("Category with name {Name} already exists", request.Name);
                return Result<CreateCategoryResponse>.Failure("Category with this name already exists.");
            }

            // Upload image to Azure Blob Storage if provided
            string? imageUrl = null;
            if (request.Image != null && request.Image.Length > 0)
            {
                try
                {
                    using var stream = request.Image.OpenReadStream();
                    imageUrl = await blobStorageService.UploadImageAsync(
                        stream,
                        request.Image.FileName,
                        request.Image.ContentType,
                        cancellationToken);

                    logger.LogInformation("Image uploaded successfully for category: {Name}", request.Name);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to upload image for category: {Name}", request.Name);
                    return Result<CreateCategoryResponse>.Failure("Failed to upload category image.");
                }
            }

            var category = new Domain.Entities.Catalog.Category
            {
                CategoryId = Guid.NewGuid(),
                Name = request.Name,
                Slug = request.Slug,
                Description = request.Description,
                ImageUrl = imageUrl,
                MetaTitle = request.MetaTitle,
                MetaDescription = request.MetaDescription,
                ParentCategoryId = request.ParentCategoryId,
                Status = request.Status,
                DisplayOrder = request.DisplayOrder,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = currentUserService.GetCurrentUserId() ?? Guid.Parse("2e6d6589-8790-46be-8c73-b582618ccada")
            };

            // Save to repository
            var result = await categoryRepository.CreateAsync(category, cancellationToken);
            if (!result.IsSuccess)
            {
                logger.LogError("Failed to create category: {Name}", request.Name);

                // Rollback: Delete uploaded image if category creation fails
                if (!string.IsNullOrEmpty(imageUrl))
                {
                    try
                    {
                        var blobName = imageUrl.Split('/').Last();
                        await blobStorageService.DeleteImageAsync(blobName, cancellationToken);
                        logger.LogInformation("Rolled back image upload for failed category creation");
                    }
                    catch (Exception ex)
                    {
                        logger.LogWarning(ex, "Failed to delete uploaded image during rollback");
                    }
                }

                return Result<CreateCategoryResponse>.Failure("Failed to create category.");
            }

            // Map to response
            var response = mapper.Map<CreateCategoryResponse>(result.Data);

            logger.LogInformation("Category created successfully: {CategoryId}", result.Data.CategoryId);
            return Result<CreateCategoryResponse>.Success(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating category: {Name}", request.Name);
            return Result<CreateCategoryResponse>.Failure("An error occurred while creating the category.");
        }
    }
}