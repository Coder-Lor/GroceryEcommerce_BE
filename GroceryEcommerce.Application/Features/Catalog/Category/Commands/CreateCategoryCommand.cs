using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace GroceryEcommerce.Application.Features.Catalog.Category.Commands;

public record CreateCategoryCommand(
    string Name,
    string? Slug,
    string? Description,
    IFormFile? Image,
    string? MetaTitle,
    string? MetaDescription,
    Guid? ParentCategoryId,
    short Status = 1,
    int DisplayOrder = 0
) : IRequest<Result<CreateCategoryResponse>>;
