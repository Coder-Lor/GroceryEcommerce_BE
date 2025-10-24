using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using GroceryEcommerce.Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Infrastructure.Services;

public class AzureBlobStorageService : IAzureBlobStorageService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _containerName;
    private readonly ILogger<AzureBlobStorageService> _logger;
    private readonly string _sasUrl;
    private readonly string _baseUrl;
    public AzureBlobStorageService(IConfiguration configuration, ILogger<AzureBlobStorageService> logger)
    {
        _logger = logger;

        var connectionString = configuration.GetConnectionString("AzureStorage");
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Azure Storage connection string is not configured.");
        }

        _blobServiceClient = new BlobServiceClient(connectionString);
        _containerName = configuration["AzureStorage:ContainerName"] ?? "products";
        _sasUrl = configuration["AzureStorage:SasUrl"] ?? "";
        _baseUrl = configuration["AzureStorage:BaseUrl"] ?? "https://hauiimages2025.blob.core.windows.net";
    }

    // Helper method để extract SAS token từ URL
    private string ExtractSasToken(string sasUrl)
    {
        if (string.IsNullOrEmpty(sasUrl))
            return "";
            
        var questionMarkIndex = sasUrl.IndexOf('?');
        if (questionMarkIndex >= 0 && questionMarkIndex < sasUrl.Length - 1)
        {
            return sasUrl.Substring(questionMarkIndex + 1);
        }
        
        return "";
    }

    // Test method để kiểm tra SAS URL
    public string TestSasUrl(string blobName = "test-image.jpg")
    {
        if (!string.IsNullOrEmpty(_sasUrl))
        {
            var sasToken = ExtractSasToken(_sasUrl);
            var testUrl = $"{_baseUrl}/{_containerName}/{blobName}?{sasToken}";
            _logger.LogInformation("Test SAS URL: {TestUrl}", testUrl);
            return testUrl;
        }
        
        return "No SAS URL configured";
    }

    // CREATE - Upload ảnh mới
    public async Task<string> UploadImageAsync(Stream imageStream, string fileName, string contentType, CancellationToken cancellationToken = default)
    {
        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.None, cancellationToken: cancellationToken);

            var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
            var blobClient = containerClient.GetBlobClient(uniqueFileName);

            var blobHttpHeaders = new BlobHttpHeaders
            {
                ContentType = contentType
            };

            await blobClient.UploadAsync(imageStream, new BlobUploadOptions
            {
                HttpHeaders = blobHttpHeaders
            }, cancellationToken);

            _logger.LogInformation("Image uploaded successfully: {FileName}", uniqueFileName);
            return blobClient.Uri.ToString();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to upload image: {FileName}", fileName);
            throw;
        }
    }

    // READ - Lấy URL ảnh (private)
    public Task<string> GetImageUrlAsync(string blobName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        var blobClient = containerClient.GetBlobClient(blobName);
        return Task.FromResult(blobClient.Uri.ToString());
    }

    // READ - Lấy URL ảnh với Stored Access Policy (public access)
    public async Task<string> GetImageUrlWithStoredPolicyAsync(string blobName, string policyName = "readonly-policy")
    {
        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetBlobClient(blobName);
            
            // Kiểm tra blob có tồn tại không
            if (!await blobClient.ExistsAsync())
            {
                throw new FileNotFoundException($"Blob {blobName} not found");
            }
            
            // Sử dụng SAS URL có sẵn từ cấu hình
            if (!string.IsNullOrEmpty(_sasUrl))
            {
                // Extract SAS token từ URL
                var sasToken = ExtractSasToken(_sasUrl);
                return $"{_baseUrl}/{_containerName}/{blobName}?{sasToken}";
            }
            
            // Fallback: Sử dụng stored access policy nếu không có SAS URL
            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = _containerName,
                BlobName = blobName,
                Resource = "b", // blob
                Identifier = policyName
            };
            
            var sasUri = blobClient.GenerateSasUri(sasBuilder);
            return sasUri.ToString();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate SAS URL with stored policy: {BlobName}", blobName);
            throw;
        }
    }

    // READ - Lấy URL container với Stored Access Policy
    public Task<string> GetContainerUrlWithStoredPolicyAsync(string policyName = "readonly-policy")
    {
        try
        {
            // Sử dụng SAS URL có sẵn từ cấu hình
            if (!string.IsNullOrEmpty(_sasUrl))
            {
                return Task.FromResult(_sasUrl);
            }
            
            // Fallback: Sử dụng stored access policy nếu không có SAS URL
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            
            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = _containerName,
                Resource = "c", // container
                Identifier = policyName
            };
            
            var sasUri = containerClient.GenerateSasUri(sasBuilder);
            return Task.FromResult(sasUri.ToString());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate container SAS URL with stored policy");
            throw;
        }
    }

    // UPDATE - Thay thế ảnh (xóa cũ, upload mới)
    public async Task<string> UpdateImageAsync(string existingBlobName, Stream newImageStream, string newFileName, string contentType, CancellationToken cancellationToken = default)
    {
        try
        {
            // Xóa ảnh cũ
            await DeleteImageAsync(existingBlobName, cancellationToken);
            
            // Upload ảnh mới
            var newImageUrl = await UploadImageAsync(newImageStream, newFileName, contentType, cancellationToken);
            
            _logger.LogInformation("Image updated successfully: {OldBlobName} -> {NewFileName}", existingBlobName, newFileName);
            return newImageUrl;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update image: {BlobName}", existingBlobName);
            throw;
        }
    }

    // DELETE - Xóa ảnh
    public async Task<bool> DeleteImageAsync(string blobName, CancellationToken cancellationToken = default)
    {
        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetBlobClient(blobName);

            var response = await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);
            
            if (response.Value)
            {
                _logger.LogInformation("Image deleted successfully: {BlobName}", blobName);
            }
            else
            {
                _logger.LogWarning("Image not found for deletion: {BlobName}", blobName);
            }
            
            return response.Value;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete image: {BlobName}", blobName);
            return false;
        }
    }

    // UTILITY - Kiểm tra ảnh có tồn tại không
    public async Task<bool> ImageExistsAsync(string blobName, CancellationToken cancellationToken = default)
    {
        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetBlobClient(blobName);
            
            var response = await blobClient.ExistsAsync(cancellationToken);
            return response.Value;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to check if image exists: {BlobName}", blobName);
            return false;
        }
    }

    // UTILITY - Lấy metadata của ảnh
    public async Task<object> GetImageMetadataAsync(string blobName, CancellationToken cancellationToken = default)
    {
        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetBlobClient(blobName);
            
            var response = await blobClient.GetPropertiesAsync(cancellationToken: cancellationToken);
            return response.Value;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get image metadata: {BlobName}", blobName);
            throw;
        }
    }

    // UTILITY - Lấy danh sách ảnh trong container
    public async Task<List<string>> ListImagesAsync(string prefix = "", CancellationToken cancellationToken = default)
    {
        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobNames = new List<string>();
            
            await foreach (var blobItem in containerClient.GetBlobsAsync(prefix: prefix, cancellationToken: cancellationToken))
            {
                blobNames.Add(blobItem.Name);
            }
            
            _logger.LogInformation("Listed {Count} images with prefix: {Prefix}", blobNames.Count, prefix);
            return blobNames;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to list images with prefix: {Prefix}", prefix);
            throw;
        }
    }
}
