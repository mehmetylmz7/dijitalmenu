using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;

namespace dijitalmenu.Services;

public class LocalStorageService : IStorageService
{
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<LocalStorageService> _logger;

    private const long MaxImageFileSize = 5 * 1024 * 1024; // 5 MB
    private static readonly string[] AllowedImageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
    private static readonly string[] AllowedImageContentTypes =
    {
        "image/jpeg",
        "image/png",
        "image/gif",
        "image/webp"
    };

    public LocalStorageService(IWebHostEnvironment environment, ILogger<LocalStorageService> logger)
    {
        _environment = environment;
        _logger = logger;
    }

    public bool TrySaveImage(IFormFile? file, string subFolder, out string? fileUrl, out string? errorMessage)
    {
        fileUrl = null;
        errorMessage = null;

        if (file == null || file.Length == 0)
        {
            return true;
        }

        if (file.Length > MaxImageFileSize)
        {
            errorMessage = "Yüklenen görsel en fazla 5 MB olabilir.";
            return false;
        }

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedImageExtensions.Contains(extension) ||
            !AllowedImageContentTypes.Contains(file.ContentType, StringComparer.OrdinalIgnoreCase))
        {
            errorMessage = "Sadece JPG, JPEG, PNG, GIF ve WEBP formatları desteklenmektedir.";
            return false;
        }

        try
        {
            var webRoot = _environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var folderPath = Path.Combine(webRoot, "images", subFolder);
            Directory.CreateDirectory(folderPath);

            var fileName = $"{Guid.NewGuid():N}{extension}";
            var fullPath = Path.Combine(folderPath, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            fileUrl = $"/images/{subFolder}/{fileName}";
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Görsel kaydedilirken hata oluştu: {FileName}", file.FileName);
            errorMessage = "Görsel kaydedilirken bir hata oluştu. Lütfen tekrar deneyin.";
            return false;
        }
    }

    public bool DeleteImage(string? relativeFileUrl)
    {
        if (string.IsNullOrWhiteSpace(relativeFileUrl))
            return false;

        try
        {
            var normalizedPath = relativeFileUrl.TrimStart('/', '\\').Replace('/', Path.DirectorySeparatorChar);
            var webRoot = _environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var fullPath = Path.Combine(webRoot, normalizedPath);

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                return true;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Görsel silinirken bir hata oluştu: {FileUrl}", relativeFileUrl);
        }

        return false;
    }
}
