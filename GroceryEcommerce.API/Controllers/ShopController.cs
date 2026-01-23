using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Catalog.Shop.Commands;
using GroceryEcommerce.Application.Features.Catalog.Shop.Queries;
using GroceryEcommerce.Application.Models.Catalog;
using GroceryEcommerce.Application.Interfaces.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using System.Drawing.Imaging;

namespace GroceryEcommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShopController(IMediator mediator, IAzureBlobStorageService blobStorageService) : ControllerBase
{
    [HttpGet("paging")]
    public async Task<ActionResult<Result<PagedResult<ShopDto>>>> GetShopsPaging([FromQuery] PagedRequest request)
    {
        var result = await mediator.Send(new GetShopsPagingQuery(request));
        return Ok(result);
    }

    [HttpGet("active")]
    public async Task<ActionResult<Result<PagedResult<ShopDto>>>> GetActiveShopsPaging([FromQuery] PagedRequest request)
    {
        var result = await mediator.Send(new GetActiveShopsPagingQuery(request));
        return Ok(result);
    }

    [HttpGet("pending")]
    public async Task<ActionResult<Result<PagedResult<ShopDto>>>> GetPendingShopsPaging([FromQuery] PagedRequest request)
    {
        var result = await mediator.Send(new GetPendingShopsPagingQuery(request));
        return Ok(result);
    }

    [HttpGet("{shopId:guid}")]
    public async Task<ActionResult<Result<GetShopByIdResponse>>> GetById([FromRoute] Guid shopId)
    {
        var result = await mediator.Send(new GetShopByIdQuery(shopId));
        return Ok(result);
    }

    [HttpGet("slug/{slug}")]
    public async Task<ActionResult<Result<GetShopBySlugResponse>>> GetBySlug([FromRoute] string slug)
    {
        var result = await mediator.Send(new GetShopBySlugQuery(slug));
        return Ok(result);
    }

    [HttpGet("owner/{ownerUserId:guid}")]
    public async Task<ActionResult<Result<PagedResult<ShopDto>>>> GetByOwner(
        [FromRoute] Guid ownerUserId,
        [FromQuery] PagedRequest request)
    {
        var result = await mediator.Send(new GetShopsByOwnerQuery(ownerUserId, request));
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<Result<CreateShopResponse>>> Create([FromBody] CreateShopCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("register")]
    public async Task<ActionResult<Result<CreateShopResponse>>> Register([FromBody] CreateShopCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("register-with-file")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<Result<CreateShopResponse>>> RegisterWithFile([FromForm] CreateShopWithFileRequest request)
    {
        string? logoUrl = null;

        if (request.LogoFile is not null && request.LogoFile.Length > 0)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var ext = Path.GetExtension(request.LogoFile.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(ext))
            {
                return BadRequest(Result<CreateShopResponse>.Failure("Định dạng file không hợp lệ. Chỉ cho phép JPG, JPEG, PNG, WEBP"));
            }

            if (request.LogoFile.Length > 10 * 1024 * 1024)
            {
                return BadRequest(Result<CreateShopResponse>.Failure("Kích thước file vượt quá 10MB"));
            }

            try
            {
                using var inputStream = request.LogoFile.OpenReadStream();
                using var image = Image.FromStream(inputStream);

                const int maxSize = 1200;
                var newWidth = image.Width;
                var newHeight = image.Height;
                if (image.Width > maxSize || image.Height > maxSize)
                {
                    var ratio = Math.Min((double)maxSize / image.Width, (double)maxSize / image.Height);
                    newWidth = (int)(image.Width * ratio);
                    newHeight = (int)(image.Height * ratio);
                }

                using var resized = new Bitmap(image, new Size(newWidth, newHeight));
                using var ms = new MemoryStream();

                var encoder = GetJpegEncoder();
                if (encoder is null)
                {
                    resized.Save(ms, ImageFormat.Jpeg);
                }
                else
                {
                    var encoderParams = new EncoderParameters(1);
                    encoderParams.Param[0] = new EncoderParameter(Encoder.Quality, 80L);
                    resized.Save(ms, encoder, encoderParams);
                }

                ms.Position = 0;
                logoUrl = await blobStorageService.UploadImageAsync(ms, $"{Path.GetFileNameWithoutExtension(request.LogoFile.FileName)}.jpg", "image/jpeg");
            }
            catch
            {
                using var stream = request.LogoFile.OpenReadStream();
                logoUrl = await blobStorageService.UploadImageAsync(stream, request.LogoFile.FileName, request.LogoFile.ContentType);
            }
        }

        var command = new CreateShopCommand(
            request.Name,
            request.Slug,
            request.Description,
            logoUrl,
            request.Status,
            request.OwnerUserId);

        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("{shopId:guid}")]
    public async Task<ActionResult<Result<UpdateShopResponse>>> Update(
        [FromRoute] Guid shopId,
        [FromBody] UpdateShopCommand command)
    {
        // đảm bảo id route và body khớp nhau
        if (shopId != command.ShopId)
        {
            return BadRequest(Result<UpdateShopResponse>.Failure("Shop ID in route and body do not match"));
        }

        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("status/{shopId:guid}")]
    public async Task<ActionResult<Result<bool>>> UpdateStatus(
        [FromRoute] Guid shopId,
        [FromBody] UpdateShopStatusCommand command)
    {
        if (shopId != command.ShopId)
        {
            return BadRequest(Result<bool>.Failure("Shop ID in route and body do not match"));
        }

        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("{shopId:guid}")]
    public async Task<ActionResult<Result<bool>>> Delete([FromRoute] Guid shopId)
    {
        var result = await mediator.Send(new DeleteShopCommand(shopId));
        return Ok(result);
    }

    [HttpPost("accept/{shopId:guid}")]
    public async Task<ActionResult<Result<bool>>> AcceptShop([FromRoute] Guid shopId)
    {
        var result = await mediator.Send(new AcceptShopCommand(shopId));
        return Ok(result);
    }

    [HttpPost("reject/{shopId:guid}")]
    public async Task<ActionResult<Result<bool>>> RejectShop([FromRoute] Guid shopId)
    {
        var result = await mediator.Send(new RejectShopCommand(shopId));
        return Ok(result);
    }

    private static ImageCodecInfo? GetJpegEncoder()
    {
        return ImageCodecInfo.GetImageEncoders().FirstOrDefault(c => c.FormatID == ImageFormat.Jpeg.Guid);
    }
}


public class CreateShopWithFileRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Slug { get; set; }
    public string? Description { get; set; }
    public short Status { get; set; } = 1;
    public Guid OwnerUserId { get; set; }
    public IFormFile? LogoFile { get; set; }
}

