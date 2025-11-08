using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Catalog.Category.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Catalog.Category.Handlers;

public class UpdateCategoryCommandHandler(
    ICategoryRepository categoryRepository,
    IMapper mapper,
    ILogger<UpdateCategoryCommandHandler> logger
) : IRequestHandler<UpdateCategoryCommand, Result<UpdateCategoryResponse>>
{
    public async Task<Result<UpdateCategoryResponse>> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Updating category: {CategoryId}", request.CategoryId);

            // Get existing category
            var existingCategoryResult = await categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);
            if (!existingCategoryResult.IsSuccess || existingCategoryResult.Data == null)
            {
                logger.LogWarning("Category not found: {CategoryId}", request.CategoryId);
                return Result<UpdateCategoryResponse>.Failure("Category not found.");
            }

            var existingCategory = existingCategoryResult.Data;

            // Update properties
            existingCategory.Name = request.Name;
            existingCategory.Slug = request.Slug;
            existingCategory.Description = request.Description;
            existingCategory.ImageUrl = request.ImageUrl;
            existingCategory.MetaTitle = request.MetaTitle;
            existingCategory.MetaDescription = request.MetaDescription;
            existingCategory.ParentCategoryId = request.ParentCategoryId;
            existingCategory.Status = request.Status;
            existingCategory.DisplayOrder = request.DisplayOrder;
            existingCategory.UpdatedAt = DateTime.UtcNow;

            // Save changes
            var result = await categoryRepository.UpdateAsync(existingCategory, cancellationToken);
            if (!result.IsSuccess)
            {
                logger.LogError("Failed to update category: {CategoryId}", request.CategoryId);
                return Result<UpdateCategoryResponse>.Failure(result.ErrorMessage ?? "Failed to update category.");
            }

            // Map to response
            var response = mapper.Map<UpdateCategoryResponse>(existingCategory);
            
            logger.LogInformation("Category updated successfully: {CategoryId}", request.CategoryId);
            return Result<UpdateCategoryResponse>.Success(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating category: {CategoryId}", request.CategoryId);
            return Result<UpdateCategoryResponse>.Failure("An error occurred while updating the category.");
        }
    }
}
