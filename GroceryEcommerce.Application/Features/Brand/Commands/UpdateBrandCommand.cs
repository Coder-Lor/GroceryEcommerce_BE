using GroceryEcommerce.Application.Common;
using MediatR;

namespace GroceryEcommerce.Application.Features.Brand.Commands;

public record UpdateBrandCommand(
    string? Name,
    string? Slug,
    string? Description,
    string? LogoUrl,
    string? Website,
    short? Status
) : IRequest<Result<UpdateBrandResponse>>;


public sealed record UpdateBrandResponse
{
    public required Guid BrandId { get; set; }
    public required string Name { get; set; }
    public string? Slug { get; set; }
    public string? Description { get; set; }
    public string? LogoUrl { get; set; }
    public string? Website { get; set; }
    public short Status { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid UpdatedBy { get; set; }
}