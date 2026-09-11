using Microsoft.AspNetCore.Http;

namespace dijitalmenu.Services;

public interface IStorageService
{
    bool TrySaveImage(IFormFile? file, string subFolder, out string? fileUrl, out string? errorMessage);
    bool DeleteImage(string? relativeFileUrl);
}
