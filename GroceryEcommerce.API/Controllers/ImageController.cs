using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace GroceryEcommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ImageController(IAzureBlobStorageService blobService) : ControllerBase
{
    /// <summary>
    /// Upload ảnh mới lên Azure Blob Storage
    /// </summary>
    /// <param name="file">File ảnh cần upload</param>
    /// <returns>URL của ảnh đã upload</returns>
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

    /// <summary>
    /// Lấy URL ảnh với Stored Access Policy (có thể truy cập public)
    /// </summary>
    /// <param name="blobName">Tên blob của ảnh</param>
    /// <param name="policyName">Tên policy (mặc định: readonly-policy)</param>
    /// <returns>URL ảnh với SAS token</returns>
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

    /// <summary>
    /// Lấy URL container với Stored Access Policy (có thể cache lâu dài)
    /// </summary>
    /// <param name="policyName">Tên policy (mặc định: readonly-policy)</param>
    /// <returns>URL container với SAS token</returns>
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

    /// <summary>
    /// Cập nhật ảnh (thay thế ảnh cũ bằng ảnh mới)
    /// </summary>
    /// <param name="blobName">Tên blob của ảnh cần thay thế</param>
    /// <param name="file">File ảnh mới</param>
    /// <returns>URL của ảnh mới</returns>
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

            // Kiểm tra ảnh cũ có tồn tại không
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

    /// <summary>
    /// Xóa ảnh
    /// </summary>
    /// <param name="blobName">Tên blob của ảnh cần xóa</param>
    /// <returns>Kết quả xóa ảnh</returns>
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

    /// <summary>
    /// Kiểm tra ảnh có tồn tại không
    /// </summary>
    /// <param name="blobName">Tên blob của ảnh</param>
    /// <returns>Trạng thái tồn tại của ảnh</returns>
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

    /// <summary>
    /// Lấy metadata của ảnh
    /// </summary>
    /// <param name="blobName">Tên blob của ảnh</param>
    /// <returns>Metadata của ảnh</returns>
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

    /// <summary>
    /// Lấy danh sách tất cả ảnh trong container
    /// </summary>
    /// <param name="prefix">Tiền tố để lọc ảnh (tùy chọn)</param>
    /// <returns>Danh sách tên các ảnh</returns>
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

    /// <summary>
    /// Upload nhiều ảnh cùng lúc
    /// </summary>
    /// <param name="files">Danh sách file ảnh</param>
    /// <returns>Danh sách URL của các ảnh đã upload</returns>
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
