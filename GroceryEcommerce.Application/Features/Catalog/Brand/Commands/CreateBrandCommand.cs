using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;

namespace GroceryEcommerce.Application.Features.Catalog.Brand.Commands;

public  record CreateBrandCommand(
		string Description,
		string LogoUrl,
		string Name,
		string Slug,
		string? Website,
		short Status = 1
) : IRequest<Result<CreateBrandResponse>>;
