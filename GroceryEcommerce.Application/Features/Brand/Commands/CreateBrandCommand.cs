using GroceryEcommerce.Application.Common;
using MediatR;

namespace GroceryEcommerce.Application.Features.Brand.Commands;

public record CreateBrandCommand(
		string Description,
		string LogoUrl,
		string Name,
		string Slug,
		string? Website,
		short Status = 1
) : IRequest<Result<CreateBrandResponse>>;

public sealed record CreateBrandResponse
{
    public required Guid BrandId { get; set; }
    public required string Name { get; set; }
    public string? Slug { get; set; }
    public string? Description { get; set; }
    public string? LogoUrl { get; set; }
    public string? Website { get; set; }
    public short Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
}