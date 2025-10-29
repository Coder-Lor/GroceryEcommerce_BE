using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Catalog.ProductAttributeValue.Commands;
using GroceryEcommerce.Application.Features.Catalog.ProductAttributeValue.Queries;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GroceryEcommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductAttributeValueController(IMediator mediator) : ControllerBase
{
    [HttpGet("by-id/{valueId}")]
    public async Task<ActionResult<Result<ProductAttributeValueDto>>> GetById([FromRoute] Guid valueId)
    {
        var query = new GetProductAttributeValueByIdQuery(valueId);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("by-product/{productId}")]
    public async Task<ActionResult<Result<PagedResult<ProductAttributeValueDto>>>> GetByProduct([FromRoute] Guid productId, [FromQuery] PagedRequest request)
    {
        var sortDirStr = request.SortDirection == SortDirection.Descending ? "Desc" : "Asc";
        var query = new GetProductAttributeValuesByProductQuery(productId, request.Page, request.PageSize, request.SortBy, sortDirStr);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("by-attribute/{attributeId}")]
    public async Task<ActionResult<Result<PagedResult<ProductAttributeValueDto>>>> GetByAttribute([FromRoute] Guid attributeId, [FromQuery] PagedRequest request)
    {
        var sortDirStr = request.SortDirection == SortDirection.Descending ? "Desc" : "Asc";
        var query = new GetProductAttributeValuesByAttributeQuery(attributeId, request.Page, request.PageSize, request.SortBy, sortDirStr);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpPost("create")]
    public async Task<ActionResult<Result<CreateProductAttributeValueResponse>>> Create([FromBody] CreateProductAttributeValueCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("update")]
    public async Task<ActionResult<Result<UpdateProductAttributeValueResponse>>> Update([FromBody] UpdateProductAttributeValueCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("delete/{valueId}")]
    public async Task<ActionResult<Result<bool>>> Delete([FromRoute] Guid valueId)
    {
        var command = new DeleteProductAttributeValueCommand(valueId);
        var result = await mediator.Send(command);
        return Ok(result);
    }
}

