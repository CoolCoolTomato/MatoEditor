using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MatoEditor.Services;

public class FileSystemService : IFileSystemService
{
    public async Task<IEnumerable<FileSystemInfo>> GetDirectoryContentsAsync(string path)
    {
        try
        {
            return await Task.Run(() =>
            {
                var directory = new DirectoryInfo(path);
                return directory.EnumerateFileSystemInfos();
            });
        }
        catch (Exception)
        {
            return Enumerable.Empty<FileSystemInfo>();
        }
    }
    public Task<bool> DirectoryExistsAsync(string path)
    {
        try
        {
            return Task.FromResult(Directory.Exists(path));
        }
        catch (Exception)
        {
            return Task.FromResult(false);
        }        
    }
    public Task<bool> CreateDirectoryAsync(string path)
    {
        try
        {
            Task.Run(() => Directory.CreateDirectory(path));
            return Task.FromResult(false);
        }
        catch (Exception)
        {
            return Task.FromResult(false);
        }
    }
    public Task<bool> FileExistsAsync(string path)
    {
        try
        {
            return Task.FromResult(File.Exists(path));
        }
        catch (Exception)
        {
            return Task.FromResult(false);
        }
    }
    public async Task<bool> CreateFileAsync(string path)
    {
        try
        {
            if (await FileExistsAsync(path))
            {
                return false;
            }
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            await using (File.Create(path)){}
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
    public async Task<string> ReadFileAsync(string path)
    {
        try
        {
            using var reader = new StreamReader(path);
            return await reader.ReadToEndAsync();
        }
        catch (Exception)
        {
            return string.Empty;
        }
    }
    public async Task<bool> WriteFileAsync(string path, string content)
    {
        try
        {
            await using var writer = new StreamWriter(path, false);
            await writer.WriteAsync(content);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}