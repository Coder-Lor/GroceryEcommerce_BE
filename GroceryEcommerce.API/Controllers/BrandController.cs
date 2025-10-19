using MediatR;
using Microsoft.AspNetCore.Mvc;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Brand.Commands;
using GroceryEcommerce.Application.Features.Brand.Queries;

namespace GroceryEcommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BrandController(IMediator mediator) : ControllerBase
{
    [HttpGet("paging")]
    public async Task<IActionResult> GetBrandsPaging([FromQuery] PagedRequest request)
    {
        var result = await mediator.Send(new GetBrandsPagingQuery(request));
        return Ok(result);
    }

    [HttpGet("product-count/{brandId}")]
    public async Task<IActionResult> GetProductCountByBrand([FromRoute] Guid brandId)
    {
        var result = await mediator.Send(new GetProductCountByBrandQuery(brandId));
        return Ok(result);
    }

    [HttpGet("{brandId}")]
    public async Task<IActionResult> GetBrandById([FromRoute] Guid brandId)
    {
        var result = await mediator.Send(new GetBrandByIdQuery(brandId));
        return Ok(result);
    }
    
    [HttpGet("name/{name}")]
    public async Task<IActionResult> GetBrandByName([FromRoute] string name)
    {
        var result = await mediator.Send(new GetBrandByNameQuery(name));
        return Ok(result);
    }
    
    [HttpGet("slug/{slug}")]
    public async Task<IActionResult> GetBrandBySlug([FromRoute] string slug)
    {
        var result = await mediator.Send(new GetBrandBySlugQuery(slug));
        return Ok(result);
    }   

    [HttpPost]
    public async Task<IActionResult> CreateBrand([FromBody] CreateBrandCommand request)
    {
        var result = await mediator.Send(request);
        return Ok(result);
    }

    [HttpPut("{brandId}")]
    public async Task<IActionResult> UpdateBrand([FromRoute] Guid brandId, [FromBody] UpdateBrandCommand request)
    {
        var result = await mediator.Send(request);
        return Ok(result);
    }

    [HttpDelete("{brandId}")]
    public async Task<IActionResult> DeleteBrand([FromRoute] Guid brandId)
    {
        var result = await mediator.Send(new DeleteBrandCommand(brandId));
        return Ok(result);
    }

    [HttpPut("status/{brandId}")]
    public async Task<IActionResult> UpdateBrandStatus([FromRoute] Guid brandId, [FromBody] UpdateBrandStatusCommand request)
    {
        var result = await mediator.Send(new UpdateBrandStatusCommand(brandId, request.Status));
        return Ok(result);
    }
}