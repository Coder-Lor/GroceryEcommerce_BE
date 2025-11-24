using GroceryEcommerce.Application.Common;
using MediatR;

namespace GroceryEcommerce.Application.Features.Marketing.GiftCard.Commands;

public record DeleteGiftCardCommand(
    Guid GiftCardId
) : IRequest<Result<bool>>;

