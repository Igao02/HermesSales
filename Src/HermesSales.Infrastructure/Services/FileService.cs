using HermesSales.Application.Abstractions;
using Microsoft.AspNetCore.Hosting;

namespace HermesSales.Infrastructure.Services;

public class FileService : IFileService
{
    private readonly IWebHostEnvironment _environment;

    public FileService(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public async Task<string> SaveFileAsync(byte[] content, string fileName, string subDirectory, CancellationToken cancellationToken = default)
    {
        var uploadPath = Path.Combine(_environment.WebRootPath ?? "wwwroot", subDirectory);

        if (!Directory.Exists(uploadPath))
        {
            Directory.CreateDirectory(uploadPath);
        }

        var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(fileName)}";
        var filePath = Path.Combine(uploadPath, uniqueFileName);

        await File.WriteAllBytesAsync(filePath, content, cancellationToken);

        // Retorna o caminho relativo (a URL será montada no UseCase ou Endpoint se necessário)
        return $"/{subDirectory}/{uniqueFileName}".Replace("\\", "/");
    }
}
