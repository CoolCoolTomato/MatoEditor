using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MatoEditor.Services;

public class FileSystemService : IFileSystemService
{
    public async Task<bool> DirectoryExistsAsync(string path)
    {
        try
        {
            return await Task.FromResult(Directory.Exists(path));
        }
        catch (Exception)
        {
            return false;
        }        
    }
    public async Task<IEnumerable<DirectoryInfo>> GetSubDirectories(string path)
    {
        try
        {
            return await Task.Run(() =>
            {
                var directory = new DirectoryInfo(path);
                return directory.EnumerateDirectories();
            });
        }
        catch (Exception)
        {
            return Enumerable.Empty<DirectoryInfo>();
        }
    }
    public async Task<IEnumerable<FileInfo>> GetFiles(string path)
    {
        try
        {
            return await Task.Run(() =>
            {
                var directory = new DirectoryInfo(path);
                return directory.EnumerateFiles();
            });
        }
        catch (Exception)
        {
            return Enumerable.Empty<FileInfo>();
        }
    }
    public async Task<bool> CreateDirectoryAsync(string path)
    {
        try
        {
            await Task.Run(() => Directory.CreateDirectory(path));
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
    public async Task<bool> RenameDirectoryAsync(string oldPath, string newPath)
    {
        try
        {
            await Task.Run(() => Directory.Move(oldPath, newPath));
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
    public async Task<bool> DeleteDirectoryAsync(string path)
    {
        try
        {
            await Task.Run(() => Directory.Delete(path, true));
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
    public async Task<bool> FileExistsAsync(string path)
    {
        try
        {
            return await Task.FromResult(File.Exists(path));
        }
        catch (Exception)
        {
            return false;
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
    public async Task<bool> RenameFileAsync(string oldPath, string newPath)
    {
        try
        {
            if (!(await FileExistsAsync(oldPath)) || await FileExistsAsync(newPath))
            {
                return false;
            }
            Directory.CreateDirectory(Path.GetDirectoryName(newPath));
            File.Move(oldPath, newPath);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
    public async Task<bool> DeleteFileAsync(string path)
    {
        try
        {
            if (!(await FileExistsAsync(path)))
            {
                return false;
            }
            File.Delete(path);
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