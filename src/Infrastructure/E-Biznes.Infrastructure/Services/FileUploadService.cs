using E_Biznes.Application.Abstract.Service;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace E_Biznes.Infrastructure.Services;

public class FileUploadService : IFileServices
{
    private readonly IWebHostEnvironment _env;

    public FileUploadService(IWebHostEnvironment env)
    {
        _env = env;
    }

    public async Task<string> UploadAsync(IFormFile file, string folder = "uploads")
    {
        if (file == null)
            throw new ArgumentNullException(nameof(file), "File is null");

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
        var extension = Path.GetExtension(file.FileName).ToLower();

        if (!allowedExtensions.Contains(extension))
            throw new InvalidOperationException("Invalid file type");

        var webRootPath = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        var uploadFolder = Path.Combine(webRootPath, folder);

        if (!Directory.Exists(uploadFolder))
            Directory.CreateDirectory(uploadFolder);

        var fileName = $"{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(uploadFolder, fileName);

        using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        return $"/{folder}/{fileName}";
    }

    public async Task DeleteFileAsync(string relativePath)
    {
        var webRootPath = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        var filePath = Path.Combine(webRootPath, relativePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
        await Task.CompletedTask;
    }


}
