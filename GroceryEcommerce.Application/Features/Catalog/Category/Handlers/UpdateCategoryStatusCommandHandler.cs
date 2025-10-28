using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Catalog.Brand.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Catalog.Category.Handlers;

public class UpdateCategoryStatusCommandHandler(
    ICategoryRepository categoryRepository,
    IMapper mapper,
    ILogger<UpdateCategoryStatusCommandHandler> logger
) : IRequestHandler<UpdateCategoryStatusCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdateCategoryStatusCommand request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Updating category status: {CategoryId}, Status: {Status}", request.CategoryId, request.Status);

            // Get existing category
            var existingCategoryResult = await categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);
            if (!existingCategoryResult.IsSuccess || existingCategoryResult.Data == null)
            {
                logger.LogWarning("Category not found: {CategoryId}", request.CategoryId);
                return Result<bool>.Failure("Category not found.");
            }

            var existingCategory = existingCategoryResult.Data;
            existingCategory.Status = request.Status;
            existingCategory.UpdatedAt = DateTime.UtcNow;

            // Save changes
            var result = await categoryRepository.UpdateAsync(existingCategory, cancellationToken);
            if (!result.IsSuccess)
            {
                logger.LogError("Failed to update category status: {CategoryId}", request.CategoryId);
                return Result<bool>.Failure(result.ErrorMessage ?? "Failed to update category status.");
            }

            logger.LogInformation("Category status updated successfully: {CategoryId}", request.CategoryId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating category status: {CategoryId}", request.CategoryId);
            return Result<bool>.Failure("An error occurred while updating the category status.");
        }
    }
}
