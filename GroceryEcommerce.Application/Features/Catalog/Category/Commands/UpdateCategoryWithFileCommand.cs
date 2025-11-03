using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace GroceryEcommerce.Application.Features.Catalog.Category.Commands
{
    public record UpdateCategoryWithFileCommand(
        Guid CategoryId,
        string Name,
        string? Slug,
        string? Description,
        IFormFile? Image,
        string? MetaTitle,
        string? MetaDescription,
        Guid? ParentCategoryId,
        short Status,
        int DisplayOrder
    ) : IRequest<Result<bool>>;
}
