using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Marketing;
using MediatR;

namespace GroceryEcommerce.Application.Features.Marketing.GiftCard.Commands;

public record UpdateGiftCardCommand(
    Guid GiftCardId,
    string? Name,
    string? Description,
    decimal InitialAmount,
    DateTime ValidFrom,
    DateTime ValidTo,
    short Status,
    Guid? AssignedTo,
    string? AssignedToEmail
) : IRequest<Result<GiftCardDto>>;

