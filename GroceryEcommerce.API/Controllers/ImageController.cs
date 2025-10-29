using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace GroceryEcommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ImageController(IAzureBlobStorageService blobService) : ControllerBase
{
    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<Result<string>>> UploadImage(IFormFile file)
    {
        try
        {
            if (file.Length == 0)
            {
                return BadRequest(Result<string>.Failure("No file uploaded"));
            }

            // Kiểm tra định dạng file
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            
            if (!allowedExtensions.Contains(fileExtension))
            {
                return BadRequest(Result<string>.Failure("Invalid file format. Only JPG, JPEG, PNG, GIF, WEBP are allowed"));
            }

            // Kiểm tra kích thước file (max 10MB)
            if (file.Length > 10 * 1024 * 1024)
            {
                return BadRequest(Result<string>.Failure("File size too large. Maximum size is 10MB"));
            }

            using var stream = file.OpenReadStream();
            var imageUrl = await blobService.UploadImageAsync(stream, file.FileName, file.ContentType);
            
            return Ok(Result<string>.Success(imageUrl));
        }
        catch (Exception ex)
        {
            return StatusCode(500, Result<string>.Failure($"Failed to upload image: {ex.Message}"));
        }
    }
    
    [HttpGet("{blobName}/url")]
    public async Task<ActionResult<Result<string>>> GetImageUrl(string blobName, [FromQuery] string policyName = "readonly-policy")
    {
        try
        {
            var sasUrl = await blobService.GetImageUrlWithStoredPolicyAsync(blobName, policyName);
            return Ok(Result<string>.Success(sasUrl));
        }
        catch (FileNotFoundException)
        {
            return NotFound(Result<string>.Failure("Image not found"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, Result<string>.Failure($"Failed to get image URL: {ex.Message}"));
        }
    }

    [HttpGet("container/url")]
    public async Task<ActionResult<Result<string>>> GetContainerUrl([FromQuery] string policyName = "readonly-policy")
    {
        try
        {
            var containerUrl = await blobService.GetContainerUrlWithStoredPolicyAsync(policyName);
            return Ok(Result<string>.Success(containerUrl));
        }
        catch (Exception ex)
        {
            return StatusCode(500, Result<string>.Failure($"Failed to get container URL: {ex.Message}"));
        }
    }


    [HttpPut("{blobName}")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<Result<string>>> UpdateImage(string blobName, IFormFile file)
    {
        try
        {
            if (file.Length == 0)
            {
                return BadRequest(Result<string>.Failure("No file uploaded"));
            }
            
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            
            if (!allowedExtensions.Contains(fileExtension))
            {
                return BadRequest(Result<string>.Failure("Invalid file format. Only JPG, JPEG, PNG, GIF, WEBP are allowed"));
            }
            
            if (file.Length > 10 * 1024 * 1024)
            {
                return BadRequest(Result<string>.Failure("File size too large. Maximum size is 10MB"));
            }
            
            var exists = await blobService.ImageExistsAsync(blobName);
            if (!exists)
            {
                return NotFound(Result<string>.Failure("Image not found"));
            }

            using var stream = file.OpenReadStream();
            var newImageUrl = await blobService.UpdateImageAsync(blobName, stream, file.FileName, file.ContentType);
            
            return Ok(Result<string>.Success(newImageUrl));
        }
        catch (Exception ex)
        {
            return StatusCode(500, Result<string>.Failure($"Failed to update image: {ex.Message}"));
        }
    }
    
    [HttpDelete("{blobName}")]
    public async Task<ActionResult<Result<bool>>> DeleteImage(string blobName)
    {
        try
        {
            var result = await blobService.DeleteImageAsync(blobName);
            
            if (result)
            {
                return Ok(Result<bool>.Success(true));
            }
            else
            {
                return NotFound(Result<bool>.Failure("Image not found"));
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, Result<bool>.Failure($"Failed to delete image: {ex.Message}"));
        }
    }
    
    [HttpGet("{blobName}/exists")]
    public async Task<ActionResult<Result<bool>>> ImageExists(string blobName)
    {
        try
        {
            var exists = await blobService.ImageExistsAsync(blobName);
            return Ok(Result<bool>.Success(exists));
        }
        catch (Exception ex)
        {
            return StatusCode(500, Result<bool>.Failure($"Failed to check image existence: {ex.Message}"));
        }
    }
    
    [HttpGet("{blobName}/metadata")]
    public async Task<ActionResult<Result<object>>> GetImageMetadata(string blobName)
    {
        try
        {
            var metadata = await blobService.GetImageMetadataAsync(blobName);
            return Ok(Result<object>.Success(metadata));
        }
        catch (FileNotFoundException)
        {
            return NotFound(Result<object>.Failure("Image not found"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, Result<object>.Failure($"Failed to get image metadata: {ex.Message}"));
        }
    }
    
    [HttpGet("list")]
    public async Task<ActionResult<Result<List<string>>>> ListImages([FromQuery] string prefix = "")
    {
        try
        {
            var images = await blobService.ListImagesAsync(prefix);
            return Ok(Result<List<string>>.Success(images));
        }
        catch (Exception ex)
        {
            return StatusCode(500, Result<List<string>>.Failure($"Failed to list images: {ex.Message}"));
        }
    }

    [HttpPost("upload-multiple")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<Result<List<string>>>> UploadMultipleImages(List<IFormFile> files)
    {
        try
        {
            if (!files.Any())
            {
                return BadRequest(Result<List<string>>.Failure("No files uploaded"));
            }

            var uploadedUrls = new List<string>();
            var errors = new List<string>();

            foreach (var file in files)
            {
                try
                {
                    // Kiểm tra định dạng file
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                    var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                    
                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        errors.Add($"Invalid file format for {file.FileName}. Only JPG, JPEG, PNG, GIF, WEBP are allowed");
                        continue;
                    }

                    // Kiểm tra kích thước file (max 10MB)
                    if (file.Length > 10 * 1024 * 1024)
                    {
                        errors.Add($"File size too large for {file.FileName}. Maximum size is 10MB");
                        continue;
                    }

                    using var stream = file.OpenReadStream();
                    var imageUrl = await blobService.UploadImageAsync(stream, file.FileName, file.ContentType);
                    uploadedUrls.Add(imageUrl);
                }
                catch (Exception ex)
                {
                    errors.Add($"Failed to upload {file.FileName}: {ex.Message}");
                }
            }

            if (uploadedUrls.Any())
            {
                return Ok(Result<List<string>>.Success(uploadedUrls));
            }
            else
            {
                return BadRequest(Result<List<string>>.Failure($"Failed to upload any images. Errors: {string.Join("; ", errors)}"));
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, Result<List<string>>.Failure($"Failed to upload images: {ex.Message}"));
        }
    }
}
