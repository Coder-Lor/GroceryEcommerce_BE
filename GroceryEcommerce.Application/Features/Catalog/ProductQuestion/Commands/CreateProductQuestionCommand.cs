using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;

namespace GroceryEcommerce.Application.Features.Catalog.ProductQuestion.Commands;

public record CreateProductQuestionCommand(
    Guid ProductId,
    Guid UserId,
    string Question,
    short Status = 1
) : IRequest<Result<CreateProductQuestionResponse>>;
