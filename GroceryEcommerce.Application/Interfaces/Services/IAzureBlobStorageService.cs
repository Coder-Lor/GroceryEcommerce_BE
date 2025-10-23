namespace GroceryEcommerce.Application.Interfaces.Services;

public interface IAzureBlobStorageService
{
    // CREATE - Upload ảnh mới
    Task<string> UploadImageAsync(Stream imageStream, string fileName, string contentType, CancellationToken cancellationToken = default);
    
    // READ - Lấy URL ảnh (private)
    Task<string> GetImageUrlAsync(string blobName);
    
    // READ - Lấy URL ảnh với Stored Access Policy (public access)
    Task<string> GetImageUrlWithStoredPolicyAsync(string blobName, string policyName = "readonly-policy");
    
    // READ - Lấy URL container với Stored Access Policy
    Task<string> GetContainerUrlWithStoredPolicyAsync(string policyName = "readonly-policy");
    
    // UPDATE - Thay thế ảnh (xóa cũ, upload mới)
    Task<string> UpdateImageAsync(string existingBlobName, Stream newImageStream, string newFileName, string contentType, CancellationToken cancellationToken = default);
    
    // DELETE - Xóa ảnh
    Task<bool> DeleteImageAsync(string blobName, CancellationToken cancellationToken = default);
    
    // UTILITY - Kiểm tra ảnh có tồn tại không
    Task<bool> ImageExistsAsync(string blobName, CancellationToken cancellationToken = default);
    
    // UTILITY - Lấy metadata của ảnh
    Task<object> GetImageMetadataAsync(string blobName, CancellationToken cancellationToken = default);
    
    // UTILITY - Lấy danh sách ảnh trong container
    Task<List<string>> ListImagesAsync(string prefix = "", CancellationToken cancellationToken = default);
    
    // TEST - Test SAS URL
    string TestSasUrl(string blobName = "test-image.jpg");
}