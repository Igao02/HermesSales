namespace HermesSales.Domain.Repositories;

public interface IFileService
{
    Task<string> SaveFileAsync(byte[] content, string fileName, string subDirectory, CancellationToken cancellationToken = default);
}
