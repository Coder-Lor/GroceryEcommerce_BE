using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;

namespace GroceryEcommerce.Application.Features.ProductQuestion.Commands;

public record UpdateProductQuestionCommand(
    Guid QuestionId,
    string Question,
    short Status
) : IRequest<Result<UpdateProductQuestionResponse>>;
