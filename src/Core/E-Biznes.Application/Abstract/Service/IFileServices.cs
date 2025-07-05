using Microsoft.AspNetCore.Http;

namespace E_Biznes.Application.Abstract.Service;

public interface IFileServices
{
    Task<string> UploadAsync(IFormFile file, string folderName);
    public Task DeleteFileAsync(string relativePath);

}
