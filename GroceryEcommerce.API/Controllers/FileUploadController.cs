using GroceryEcommerce.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace GroceryEcommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FileUploadController(IAzureBlobStorageService blobStorageService) : ControllerBase
{
    [HttpPost("upload-image")]
    public async Task<IActionResult> UploadImage(IFormFile file, CancellationToken cancellationToken)
    {
        if (file.Length == 0)
        {
            return BadRequest("No file uploaded.");
        }

        // Validate file type
        var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };
        if (!allowedTypes.Contains(file.ContentType))
        {
            return BadRequest("Invalid file type. Only images are allowed.");
        }

        // Validate file size (5MB max)
        if (file.Length > 5 * 1024 * 1024)
        {
            return BadRequest("File size too large. Maximum 5MB allowed.");
        }

        try
        {
            using var stream = file.OpenReadStream();
            var imageUrl = await blobStorageService.UploadImageAsync(
                stream, 
                file.FileName, 
                file.ContentType, 
                cancellationToken);

            return Ok(new { ImageUrl = imageUrl });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error uploading file: {ex.Message}");
        }
    }

    [HttpPost("upload-multiple-images")]
    public async Task<IActionResult> UploadMultipleImages(List<IFormFile> files, CancellationToken cancellationToken)
    {
        if (!files.Any())
        {
            return BadRequest("No files uploaded.");
        }

        var uploadedUrls = new List<string>();
        var errors = new List<string>();

        foreach (var file in files)
        {
            try
            {
                if (file.Length == 0) continue;

                var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };
                if (!allowedTypes.Contains(file.ContentType))
                {
                    errors.Add($"Invalid file type for {file.FileName}");
                    continue;
                }

                if (file.Length > 5 * 1024 * 1024)
                {
                    errors.Add($"File too large for {file.FileName}");
                    continue;
                }

                using var stream = file.OpenReadStream();
                var imageUrl = await blobStorageService.UploadImageAsync(
                    stream, 
                    file.FileName, 
                    file.ContentType, 
                    cancellationToken);

                uploadedUrls.Add(imageUrl);
            }
            catch (Exception ex)
            {
                errors.Add($"Error uploading {file.FileName}: {ex.Message}");
            }
        }

        return Ok(new 
        { 
            UploadedUrls = uploadedUrls, 
            Errors = errors 
        });
    }

    [HttpDelete("delete-image")]
    public async Task<IActionResult> DeleteImage([FromQuery] string blobName, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(blobName))
        {
            return BadRequest("Blob name is required.");
        }

        try
        {
            var result = await blobStorageService.DeleteImageAsync(blobName, cancellationToken);
            return Ok(new { Success = result });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error deleting file: {ex.Message}");
        }
    }
}
