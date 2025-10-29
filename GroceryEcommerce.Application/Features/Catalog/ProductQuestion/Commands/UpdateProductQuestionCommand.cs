using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;

namespace GroceryEcommerce.Application.Features.Catalog.ProductQuestion.Commands;

public record UpdateProductQuestionCommand(
    Guid QuestionId,
    string Question,
    short Status
) : IRequest<Result<UpdateProductQuestionResponse>>;
