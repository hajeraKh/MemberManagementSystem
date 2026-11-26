using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace MemberManagementSystem.Web.Services
{
    public interface IFileService
    {
        Task<(string relativePath, string originalFileName, long fileSize, string extension)> UploadFileAsync(
            IFormFile file, 
            string subFolder, 
            string[] allowedExtensions);

        void DeleteFile(string? relativePath);
    }
}

