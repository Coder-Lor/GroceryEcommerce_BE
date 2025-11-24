using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Marketing;
using MediatR;

namespace GroceryEcommerce.Application.Features.Marketing.GiftCard.Queries;

public record GetGiftCardByCodeQuery(
    string Code
) : IRequest<Result<GiftCardDto?>>;

