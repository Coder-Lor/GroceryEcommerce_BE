using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Product.Commands;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GroceryEcommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController(IMediator mediator) : ControllerBase
{
    [HttpPost("create")]
    public async Task<ActionResult<Result<CreateProductResponse>>> Create([FromForm] CreateProductWithFilesCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("create-with-urls")]
    public async Task<ActionResult<Result<CreateProductResponse>>> CreateWithUrls([FromBody] CreateProductCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }
}