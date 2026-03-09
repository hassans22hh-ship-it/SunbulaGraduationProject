using Microsoft.AspNetCore.Http;

namespace Shared.Services
{
    public interface IFileService
    {
        Task<string> SaveFileAsync(IFormFile file, string folderName, string[] allowedExtensions, CancellationToken cancellationToken = default);
        void DeleteFile(string fileUrl);
    }
}
