using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Category.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Category.Handlers;

public class DeleteCategoryCommandHandler(
    ICategoryRepository categoryRepository,
    ILogger<DeleteCategoryCommandHandler> logger
) : IRequestHandler<DeleteCategoryCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Deleting category: {CategoryId}", request.CategoryId);

            // Check if category exists
            var existingCategoryResult = await categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);
            if (!existingCategoryResult.IsSuccess || existingCategoryResult.Data == null)
            {
                logger.LogWarning("Category not found: {CategoryId}", request.CategoryId);
                return Result<bool>.Failure("Category not found.");
            }

            // Check if category has subcategories
            var hasSubCategoriesResult = await categoryRepository.HasSubCategoriesAsync(request.CategoryId, cancellationToken);
            if (hasSubCategoriesResult.IsSuccess && hasSubCategoriesResult.Data)
            {
                logger.LogWarning("Cannot delete category with subcategories: {CategoryId}", request.CategoryId);
                return Result<bool>.Failure("Cannot delete category that has subcategories.");
            }

            // Check if category is in use
            var inUseResult = await categoryRepository.IsCategoryInUseAsync(request.CategoryId, cancellationToken);
            if (inUseResult.IsSuccess && inUseResult.Data)
            {
                logger.LogWarning("Cannot delete category in use: {CategoryId}", request.CategoryId);
                return Result<bool>.Failure("Cannot delete category that is currently in use.");
            }

            // Delete category
            var result = await categoryRepository.DeleteAsync(request.CategoryId, cancellationToken);
            if (!result.IsSuccess)
            {
                logger.LogError("Failed to delete category: {CategoryId}", request.CategoryId);
                return Result<bool>.Failure("Failed to delete category.");
            }

            logger.LogInformation("Category deleted successfully: {CategoryId}", request.CategoryId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting category: {CategoryId}", request.CategoryId);
            return Result<bool>.Failure("An error occurred while deleting the category.");
        }
    }
}
