using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;

namespace GroceryEcommerce.Application.Features.ProductQuestion.Commands;

public record CreateProductQuestionCommand(
    Guid ProductId,
    Guid UserId,
    string Question,
    short Status = 1
) : IRequest<Result<CreateProductQuestionResponse>>;
