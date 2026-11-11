using MarketHub.BLL.Interfaces;

namespace MarketHub.BLL.Services;


public class FileStorageOptions
{
    /// <summary>Absolute path of the wwwroot folder.</summary>
    public string WebRootPath { get; set; } = string.Empty;

    /// <summary>Folder inside wwwroot where files are written. Relative, forward slashes.</summary>
    public string SubFolder { get; set; } = "uploads/products";

    /// <summary>Largest accepted file. Enforced while copying, because a declared length can not be trusted.</summary>
    public long MaxFileBytes { get; set; } = 2 * 1024 * 1024; 

    /// <summary>The only extensions that can be written. Keep in sync with ProductService.</summary>
    public static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
}


public class LocalFileStorage : IFileStorage
{
    private readonly FileStorageOptions _options;
    private readonly string _webRoot;
    private readonly string _folder;

    public LocalFileStorage(FileStorageOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.WebRootPath))
        {
            throw new ArgumentException("FileStorageOptions.WebRootPath must be set.", nameof(options));
        }

        if (string.IsNullOrWhiteSpace(options.SubFolder) || Path.IsPathRooted(options.SubFolder))
        {
            throw new ArgumentException("FileStorageOptions.SubFolder must be a relative path.", nameof(options));
        }

        if (options.MaxFileBytes <= 0)
        {
            throw new ArgumentException("FileStorageOptions.MaxFileBytes must be greater than 0.", nameof(options));
        }

        _options = options;
        _webRoot = EnsureTrailingSeparator(Path.GetFullPath(options.WebRootPath));
        _folder = EnsureTrailingSeparator(Path.GetFullPath(Path.Combine(options.WebRootPath, options.SubFolder)));

        // The target folder must stay inside wwwroot (blocks a SubFolder such as "..\..\x").
        if (!_folder.StartsWith(_webRoot, StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException("FileStorageOptions.SubFolder must be inside wwwroot.", nameof(options));
        }
    }

    public async Task<string> SaveAsync(Stream content, string extension, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(content);

        extension = NormalizeExtension(extension);
        if (!FileStorageOptions.AllowedExtensions.Contains(extension))
        {
            throw new InvalidOperationException("Only JPG, PNG or WEBP images are allowed.");
        }

        Directory.CreateDirectory(_folder);

        var fileName = Guid.NewGuid().ToString("N") + extension;
        var fullPath = Path.Combine(_folder, fileName);

        try
        {
            await using (var target = new FileStream(
                fullPath, FileMode.CreateNew, FileAccess.Write, FileShare.None, bufferSize: 81920, useAsync: true))
            {
                var buffer = new byte[81920];
                long total = 0;
                int read;

                while ((read = await content.ReadAsync(buffer.AsMemory(), ct)) > 0)
                {
                    total += read;
                    if (total > _options.MaxFileBytes)
                    {
                        throw new InvalidOperationException(
                            $"The image is too large. The maximum size is {FormatSize(_options.MaxFileBytes)}.");
                    }

                    await target.WriteAsync(buffer.AsMemory(0, read), ct);
                }

                if (total == 0)
                {
                    throw new InvalidOperationException("The image file is empty.");
                }
            }
        }
        catch
        {
            // The FileStream is already closed here, so the half written file can be removed.
            TryDelete(fullPath);
            throw;
        }

        return _options.SubFolder.Replace('\\', '/').Trim('/') + "/" + fileName;
    }

    public void Delete(string relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
        {
            return;
        }

        string fullPath;
        try
        {
            fullPath = Path.GetFullPath(Path.Combine(_webRoot, relativePath.TrimStart('/', '\\')));
        }
        catch (Exception ex) when (ex is ArgumentException or NotSupportedException or PathTooLongException)
        {
            return; // invalid characters or an impossible path: nothing we could have written
        }

        // Only files that this class could have written: inside the upload folder AND with an allowed extension.
        // A bad or malicious path in the database can never delete anything else on the server.
        if (!fullPath.StartsWith(_folder, StringComparison.OrdinalIgnoreCase) ||
            !FileStorageOptions.AllowedExtensions.Contains(Path.GetExtension(fullPath).ToLowerInvariant()))
        {
            return;
        }

        TryDelete(fullPath);
    }

    // helpers

    private static string NormalizeExtension(string? extension)
    {
        extension = (extension ?? string.Empty).Trim().ToLowerInvariant();
        return extension.StartsWith('.') ? extension : "." + extension;
    }

    private static string EnsureTrailingSeparator(string path) =>
        path.EndsWith(Path.DirectorySeparatorChar) ? path : path + Path.DirectorySeparatorChar;

    private static string FormatSize(long bytes) =>
        bytes >= 1024 * 1024 ? $"{bytes / (1024 * 1024)} MB" : $"{bytes / 1024} KB";

    /// <summary>Deleting a file must never break the business operation: an orphan file is the worst case.</summary>
    private static void TryDelete(string fullPath)
    {
        try
        {
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }
        catch (IOException)
        {
        }
        catch (UnauthorizedAccessException)
        {
        }
    }
}
