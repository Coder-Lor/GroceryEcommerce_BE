using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Marketing;
using MediatR;

namespace GroceryEcommerce.Application.Features.Marketing.GiftCard.Queries;

public record GetGiftCardByIdQuery(
    Guid GiftCardId
) : IRequest<Result<GiftCardDto?>>;

