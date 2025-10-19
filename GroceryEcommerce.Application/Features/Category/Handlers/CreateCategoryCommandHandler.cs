using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Category.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Category.Handlers;

public class CreateCategoryCommandHandler(
    ICategoryRepository categoryRepository,
    IMapper mapper,
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


            var category = new Domain.Entities.Catalog.Category
            {
                CategoryId = Guid.NewGuid(),
                Name = request.Name,
                Slug = request.Slug,
                Description = request.Description,
                MetaTitle = request.MetaTitle,
                MetaDescription = request.MetaDescription,
                ParentCategoryId = request.ParentCategoryId,
                Status = request.Status,
                DisplayOrder = request.DisplayOrder,
                CreatedAt = DateTime.UtcNow
            };

            // Save to repository
            var result = await categoryRepository.CreateAsync(category, cancellationToken);
            if (!result.IsSuccess)
            {
                logger.LogError("Failed to create category: {Name}", request.Name);
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
