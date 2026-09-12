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

        // Validate subFolder to prevent directory traversal
        if (string.IsNullOrWhiteSpace(subFolder) ||
            subFolder.Contains("..", StringComparison.Ordinal) ||
            subFolder.Any(c => Path.GetInvalidFileNameChars().Contains(c) && c != '/' && c != '\\'))
        {
            errorMessage = "Geçersiz hedef klasör.";
            return false;
        }

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedImageExtensions.Contains(extension) ||
            !AllowedImageContentTypes.Contains(file.ContentType, StringComparer.OrdinalIgnoreCase))
        {
            errorMessage = "Sadece JPG, JPEG, PNG, GIF ve WEBP formatları desteklenmektedir.";
            return false;
        }

        // Magic bytes / Binary signature inspection
        if (!IsValidImageSignature(file, extension))
        {
            _logger.LogWarning("Geçersiz dosya imzası tespit edildi. Dosya adı: {FileName}, ContentType: {ContentType}", file.FileName, file.ContentType);
            errorMessage = "Dosya içeriği geçerli bir görsel formatı ile eşleşmiyor.";
            return false;
        }

        try
        {
            var webRoot = _environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var imagesBaseDir = Path.Combine(webRoot, "images");
            var folderPath = Path.Combine(imagesBaseDir, subFolder);

            // Canonical path validation
            var canonicalFolderPath = Path.GetFullPath(folderPath);
            var canonicalImagesBase = Path.GetFullPath(imagesBaseDir);
            if (!canonicalFolderPath.StartsWith(canonicalImagesBase, StringComparison.OrdinalIgnoreCase))
            {
                errorMessage = "Geçersiz hedef yolu.";
                return false;
            }

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

            var canonicalFullPath = Path.GetFullPath(fullPath);
            var canonicalImagesDir = Path.GetFullPath(Path.Combine(webRoot, "images"));

            // Path Traversal check: only allow deletion within the images folder
            if (!canonicalFullPath.StartsWith(canonicalImagesDir, StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning("Path traversal attempt blocked in DeleteImage: {FileUrl}", relativeFileUrl);
                return false;
            }

            if (File.Exists(canonicalFullPath))
            {
                File.Delete(canonicalFullPath);
                return true;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Görsel silinirken bir hata oluştu: {FileUrl}", relativeFileUrl);
        }

        return false;
    }

    private static bool IsValidImageSignature(IFormFile file, string extension)
    {
        try
        {
            using var stream = file.OpenReadStream();
            if (stream.Length < 12)
                return false;

            var header = new byte[12];
            var bytesRead = stream.Read(header, 0, 12);
            if (bytesRead < 12)
                return false;

            return extension switch
            {
                ".jpg" or ".jpeg" => header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF,
                ".png" => header[0] == 0x89 && header[1] == 0x50 && header[2] == 0x4E && header[3] == 0x47 &&
                          header[4] == 0x0D && header[5] == 0x0A && header[6] == 0x1A && header[7] == 0x0A,
                ".gif" => header[0] == 0x47 && header[1] == 0x49 && header[2] == 0x46 && header[3] == 0x38, // "GIF8"
                ".webp" => header[0] == 0x52 && header[1] == 0x49 && header[2] == 0x46 && header[3] == 0x46 && // "RIFF"
                           header[8] == 0x57 && header[9] == 0x45 && header[10] == 0x42 && header[11] == 0x50, // "WEBP"
                _ => false
            };
        }
        catch
        {
            return false;
        }
    }
}
