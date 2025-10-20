using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.ProductQuestion.Commands;
using GroceryEcommerce.Application.Features.ProductQuestion.Queries;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GroceryEcommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductQuestionController(IMediator mediator) : ControllerBase
{
    [HttpGet("by-id/{questionId}")]
    public async Task<ActionResult<Result<ProductQuestionDto>>> GetById([FromRoute] Guid questionId)
    {
        var query = new GetProductQuestionByIdQuery(questionId);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("by-product/{productId}")]
    public async Task<ActionResult<Result<PagedResult<ProductQuestionDto>>>> GetByProduct([FromRoute] Guid productId, [FromQuery] PagedRequest request)
    {
        var sortDirStr = request.SortDirection == SortDirection.Descending ? "Desc" : "Asc";
        var query = new GetProductQuestionsByProductQuery(productId, request.Page, request.PageSize, request.SortBy, sortDirStr);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("unanswered")]
    public async Task<ActionResult<Result<PagedResult<ProductQuestionDto>>>> GetUnanswered([FromQuery] PagedRequest request)
    {
        var sortDirStr = request.SortDirection == SortDirection.Descending ? "Desc" : "Asc";
        var query = new GetUnansweredQuestionsQuery(request.Page, request.PageSize, request.SortBy, sortDirStr);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpPost("create")]
    public async Task<ActionResult<Result<CreateProductQuestionResponse>>> Create([FromBody] CreateProductQuestionCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("update")]
    public async Task<ActionResult<Result<UpdateProductQuestionResponse>>> Update([FromBody] UpdateProductQuestionCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("answer")]
    public async Task<ActionResult<Result<bool>>> Answer([FromBody] AnswerProductQuestionCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("delete/{questionId}")]
    public async Task<ActionResult<Result<bool>>> Delete([FromRoute] Guid questionId)
    {
        var command = new DeleteProductQuestionCommand(questionId);
        var result = await mediator.Send(command);
        return Ok(result);
    }
}

