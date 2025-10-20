using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;

namespace GroceryEcommerce.Application.Features.Category.Commands;

public record UpdateCategoryCommand(
    Guid CategoryId,
    string Name,
    string? Slug,
    string? Description,
    string? MetaTitle,
    string? MetaDescription,
    Guid? ParentCategoryId,
    short Status,
    int DisplayOrder
) : IRequest<Result<UpdateCategoryResponse>>;
