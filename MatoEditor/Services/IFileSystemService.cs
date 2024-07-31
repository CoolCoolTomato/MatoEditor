using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MatoEditor.Services;

public interface IFileSystemService
{
    Task<IEnumerable<FileSystemInfo>> GetDirectoryContentsAsync(string path);
    Task<bool> DirectoryExistsAsync(string path);
    Task<bool> CreateDirectoryAsync(string path);
    Task<bool> FileExistsAsync(string path);
    Task<bool> CreateFileAsync(string path);
    Task<string> ReadFileAsync(string path);
    Task<bool> WriteFileAsync(string path, string content);
}