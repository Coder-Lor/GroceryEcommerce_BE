namespace GroceryEcommerce.Application.Interfaces.Services;
public interface IAzureBlobStorageService
{
    Task<string> UploadImageAsync(Stream imageStream, string fileName, string contentType, CancellationToken cancellationToken = default);
    Task<bool> DeleteImageAsync(string blobName, CancellationToken cancellationToken = default);
    Task<string> GetImageUrlAsync(string blobName);
}