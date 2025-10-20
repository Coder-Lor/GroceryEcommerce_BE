using MediatR;
using Microsoft.AspNetCore.Mvc;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Category.Commands;
using GroceryEcommerce.Application.Features.Category.Queries;
using GroceryEcommerce.Application.Models.Catalog;


namespace GroceryEcommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoryController(IMediator mediator) : ControllerBase
{
    [HttpGet("paging")  ]
    public async Task<ActionResult<Result<PagedResult<CategoryDto>>>> GetCategoriesPaging([FromQuery] PagedRequest request)
    {
        var query = new GetCategoriesPagingQuery(request);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("product-count/{categoryId}")]
    public async Task<ActionResult<Result<int>>> GetProductCountByCategory([FromRoute] Guid categoryId)
    {
        var query = new GetProductCountByCategoryQuery(categoryId);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("sub-categories/{categoryId}")]
    public async Task<ActionResult<Result<List<CategoryDto>>>> GetSubCategories([FromRoute] Guid categoryId)
    {
        var query = new GetSubCategoriesQuery(categoryId);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("category-path/{categoryId}")]
    public async Task<ActionResult<Result<List<CategoryDto>>>> GetCategoryPath([FromRoute] Guid categoryId)
    {
        var query = new GetCategoryPathQuery(categoryId);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("category-tree")]
    public async Task<ActionResult<Result<List<CategoryDto>>>> GetCategoryTree()
    {
        var query = new GetCategoryTreeQuery();
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("root-categories")]
    public async Task<ActionResult<Result<List<CategoryDto>>>> GetRootCategories()
    {
        var query = new GetRootCategoriesQuery();
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("active-categories")]
    public async Task<ActionResult<Result<List<CategoryDto>>>> GetActiveCategories()
    {
        var query = new GetActiveCategoriesQuery();
        var result = await mediator.Send(query);
        return Ok(result);
    }


    [HttpGet("{categoryId}")]
    public async Task<ActionResult<Result<CategoryDto>>> GetCategoryById([FromRoute] Guid categoryId)
    {
        var query = new GetCategoryByIdQuery(categoryId);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("name/{name}")]
    public async Task<ActionResult<Result<CategoryDto>>> GetCategoryByName([FromRoute] string name)
    {
        var query = new GetCategoryByNameQuery(name);
        var result = await mediator.Send(query);
        return Ok(result);
    }
    
    [HttpGet("slug/{slug}")]
    public async Task<ActionResult<Result<CategoryDto>>> GetCategoryBySlug([FromRoute] string slug)
    {
        var query = new GetCategoryBySlugQuery(slug);
        var result = await mediator.Send(query);
        return Ok(result);
    }
    
    [HttpPost]
    public async Task<ActionResult<Result<CategoryDto>>> CreateCategory([FromBody] CreateCategoryCommand request)
    {
        var result = await mediator.Send(request);
        return Ok(result);
    }
    
    [HttpPut("{categoryId}")]
    public async Task<ActionResult<Result<CategoryDto>>> UpdateCategory([FromRoute] Guid categoryId, [FromBody] UpdateCategoryCommand request)
    {
        var result = await mediator.Send(request);
        return Ok(result);
    }
    
    [HttpDelete("{categoryId}")]
    public async Task<ActionResult<Result<CategoryDto>>> DeleteCategory([FromRoute] Guid categoryId)
    {
        var result = await mediator.Send(new DeleteCategoryCommand(categoryId));
        return Ok(result);
    }
    
    [HttpPut("status/{categoryId}")]
    public async Task<ActionResult<Result<CategoryDto>>> UpdateCategoryStatus([FromRoute] Guid categoryId, [FromBody] UpdateCategoryStatusCommand request)
    {
        var result = await mediator.Send(request);
        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<Result<bool>>> CheckCategoryExistsById([FromRoute] Guid categoryId)
    {
        var query = new CheckCategoryExistsByIdQuery(categoryId);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("exists-by-name/{name}")]
    public async Task<ActionResult<Result<bool>>> CheckCategoryExistsByName([FromRoute] string name)
    {
        var query = new CheckCategoryExistsByNameQuery(name);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("has-sub-categories/{categoryId}")]
    public async Task<ActionResult<Result<bool>>> CheckCategoryHasSubCategories([FromRoute] Guid categoryId)
    {
        var query = new CheckCategoryHasSubCategoriesQuery(categoryId);
        var result = await mediator.Send(query);
        return Ok(result);
    }
    
    [HttpGet("in-use/{categoryId}")]
    public async Task<ActionResult<Result<bool>>> CheckCategoryInUse([FromRoute] Guid categoryId)
    {
        var query = new CheckCategoryInUseQuery(categoryId);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("is-root/{categoryId}")]
    public async Task<ActionResult<Result<bool>>> CheckIsRootCategory([FromRoute] Guid categoryId)
    {
        var query = new CheckIsRootCategoryQuery(categoryId);
        var result = await mediator.Send(query);
        return Ok(result);
    }
}