using MediatR;
using Microsoft.AspNetCore.Mvc;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Catalog.Brand.Queries;
using GroceryEcommerce.Application.Features.Catalog.Category.Commands;
using GroceryEcommerce.Application.Features.Catalog.Category.Queries;
using GroceryEcommerce.Application.Models.Catalog;
using GroceryEcommerce.Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;


namespace GroceryEcommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoryController(IMediator mediator, IAzureBlobStorageService blobStorageService) : ControllerBase
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

    [HttpPost("create-with-file")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<Result<CategoryDto>>> CreateCategoryWithFile(
        [FromForm] string categoryName,
        [FromForm] string? slug,
        [FromForm] string? description,
        [FromForm] string? metaTitle,
        [FromForm] string? metaDescription,
        [FromForm] Guid? parentCategoryId,
        [FromForm] short status,
        [FromForm] int displayOrder,
        [FromForm] IFormFile? imageFile,
        CancellationToken cancellationToken)
    {
        string? imageUrl = null;
        if (imageFile != null && imageFile.Length > 0)
        {
            using var stream = imageFile.OpenReadStream();
            imageUrl = await blobStorageService.UploadImageAsync(stream, imageFile.FileName, imageFile.ContentType, cancellationToken);
        }

        var command = new CreateCategoryCommand(categoryName, slug, description, imageUrl, metaTitle, metaDescription, parentCategoryId, status, displayOrder);
        var result = await mediator.Send(command);
        return Ok(result);
    }
    
    [HttpPut("{categoryId}")]
    public async Task<ActionResult<Result<CategoryDto>>> UpdateCategory([FromRoute] Guid categoryId, [FromBody] UpdateCategoryCommand request)
    {
        var result = await mediator.Send(request);
        return Ok(result);
    }

    [HttpPut("{categoryId}/upload-image")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<Result<string>>> UploadCategoryImage([FromRoute] Guid categoryId, [FromForm] IFormFile file, CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(Result<string>.Failure("No file uploaded"));
        }

        using var stream = file.OpenReadStream();
        var imageUrl = await blobStorageService.UploadImageAsync(stream, file.FileName, file.ContentType, cancellationToken);
        // Endpoint này chỉ upload và trả về URL; FE sẽ gọi UpdateCategory kèm ImageUrl.
        return Ok(Result<string>.Success(imageUrl));
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