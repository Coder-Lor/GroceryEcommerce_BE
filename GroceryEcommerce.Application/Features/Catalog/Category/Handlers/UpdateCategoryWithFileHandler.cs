using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Catalog.Category.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Catalog.Category.Handlers
{
    public class UpdateCategoryWithFileHandler(
        ICategoryRepository categoryRepository,
        ILogger<UpdateCategoryWithFileHandler> logger,
        IAzureBlobStorageService azureBlobStorageService
    ) : IRequestHandler<UpdateCategoryWithFileCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(UpdateCategoryWithFileCommand request, CancellationToken cancellationToken)
        {
            try
            {
                logger.LogInformation("Updating category with file: {CategoryId}", request.CategoryId);

                // Get existing category
                var existingCategoryResult = await categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);
                if (!existingCategoryResult.IsSuccess || existingCategoryResult.Data == null)
                {
                    logger.LogWarning("Category not found: {CategoryId}", request.CategoryId);
                    return Result<bool>.Failure("Category not found.");
                }

                var existingCategory = existingCategoryResult.Data;
                string? oldImageUrl = existingCategory.ImageUrl;
                string? newImageUrl = oldImageUrl;

                // Upload new image to Azure Blob Storage if provided
                if (request.Image != null && request.Image.Length > 0)
                {
                    try
                    {
                        using var stream = request.Image.OpenReadStream();

                        // If category already has an image, update it (delete old, upload new)
                        if (!string.IsNullOrEmpty(oldImageUrl))
                        {
                            var oldBlobName = oldImageUrl.Split('/').Last();
                            newImageUrl = await azureBlobStorageService.UpdateImageAsync(
                                oldBlobName,
                                stream,
                                request.Image.FileName,
                                request.Image.ContentType,
                                cancellationToken);
                            logger.LogInformation("Image updated successfully for category: {CategoryId}", request.CategoryId);
                        }
                        else
                        {
                            // Upload new image
                            newImageUrl = await azureBlobStorageService.UploadImageAsync(
                                stream,
                                request.Image.FileName,
                                request.Image.ContentType,
                                cancellationToken);

                            logger.LogInformation("Image uploaded successfully for category: {CategoryId}", request.CategoryId);
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Failed to upload/update image for category: {CategoryId}", request.CategoryId);
                        return Result<bool>.Failure("Failed to upload category image.");
                    }
                }

                // Update category properties
                existingCategory.Name = request.Name;
                existingCategory.Slug = request.Slug;
                existingCategory.Description = request.Description;
                existingCategory.ImageUrl = newImageUrl;
                existingCategory.MetaTitle = request.MetaTitle;
                existingCategory.MetaDescription = request.MetaDescription;
                existingCategory.ParentCategoryId = request.ParentCategoryId;
                existingCategory.Status = request.Status;
                existingCategory.DisplayOrder = request.DisplayOrder;
                existingCategory.UpdatedAt = DateTime.UtcNow;

                // Save changes to repository
                var updateResult = await categoryRepository.UpdateAsync(existingCategory, cancellationToken);
                if (!updateResult.IsSuccess)
                {
                    logger.LogError("Failed to update category: {CategoryId}", request.CategoryId);

                    // Rollback: If new image was uploaded but update failed, delete it and restore old image
                    if (newImageUrl != oldImageUrl && !string.IsNullOrEmpty(newImageUrl))
                    {
                        try
                        {
                            var newBlobName = newImageUrl.Split('/').Last();
                            await azureBlobStorageService.DeleteImageAsync(newBlobName, cancellationToken);
                            logger.LogInformation("Rolled back new image upload for failed category update");
                        }
                        catch (Exception ex)
                        {
                            logger.LogWarning(ex, "Failed to delete new image during rollback");
                        }
                    }

                    return Result<bool>.Failure(updateResult.ErrorMessage ?? "Failed to update category.");
                }

                logger.LogInformation("Category updated successfully: {CategoryId}", request.CategoryId);
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating category: {CategoryId}", request.CategoryId);
                return Result<bool>.Failure("An error occurred while updating the category.");
            }
        }
    }
}
