using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.ProductAttribute.Commands;
using GroceryEcommerce.Application.Features.ProductAttribute.Queries;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GroceryEcommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductAttributeController(IMediator mediator) : ControllerBase
{
    [HttpGet("exists-by-id/{attributeId}")]
    public async Task<ActionResult<Result<bool>>> CheckAttributeExistsById([FromRoute] Guid attributeId)
    {
        var query = new CheckAttributeExistsByIdQuery(attributeId);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("exists-by-name/{name}")]
    public async Task<ActionResult<Result<bool>>> CheckAttributeExistsByName([FromRoute] string name)
    {
        var query = new CheckAttributeExistsQuery(name);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("in-use/{attributeId}")]
    public async Task<ActionResult<Result<bool>>> CheckAttributeInUse([FromRoute] Guid attributeId)
    {
        var query = new CheckAttributeInUseQuery(attributeId);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("all")]
    public async Task<ActionResult<Result<PagedResult<ProductAttributeDto>>>> GetAllAttributes()
    {
        var query = new GetAllProductAttributesQuery();
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("by-id/{attributeId}")]
    public async Task<ActionResult<Result<ProductAttributeDto>>> GetAttributeById([FromRoute] Guid attributeId)
    {
        var query = new GetProductAttributeByIdQuery(attributeId);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("by-name/{name}")]
    public async Task<ActionResult<Result<ProductAttributeDto>>> GetAttributeByName([FromRoute] string name)
    {
        var query = new GetProductAttributeByNameQuery(name);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("by-type")]
    public async Task<ActionResult<Result<PagedResult<ProductAttributeDto>>>> GetAttributesByType([FromQuery] int attributeType, [FromQuery] PagedRequest request)
    {
        // map SortDirection enum to the string expected by query handlers ("Desc" or "Asc")
        var sortDirStr = request.SortDirection == SortDirection.Descending ? "Desc" : "Asc";

        // cast attributeType to short since repository/query expects short
        var query = new GetAttributesByTypeQuery((short)attributeType, request.Page, request.PageSize, request.SortBy, sortDirStr);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("required")]
    public async Task<ActionResult<Result<PagedResult<ProductAttributeDto>>>> GetRequiredAttributes([FromQuery] PagedRequest request)
    {
        var sortDirStr = request.SortDirection == SortDirection.Descending ? "Desc" : "Asc";
        var query = new GetRequiredAttributesQuery(request.Page, request.PageSize, request.SortBy, sortDirStr);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpPost("create")]
    public async Task<ActionResult<Result<CreateProductAttributeResponse>>> CreateAttribute([FromBody] CreateProductAttributeCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("update")]
    public async Task<ActionResult<Result<UpdateProductAttributeResponse>>> UpdateAttribute([FromBody] UpdateProductAttributeCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("delete/{attributeId}")]
    public async Task<ActionResult<Result<bool>>> DeleteAttribute([FromRoute] Guid attributeId)
    {
        var command = new DeleteProductAttributeCommand(attributeId);
        var result = await mediator.Send(command);
        return Ok(result);
    }
}
