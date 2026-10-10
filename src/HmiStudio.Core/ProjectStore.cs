using System.Text.Json;
using System.Text.Json.Serialization;

namespace HmiStudio.Core;

public static class ProjectStore
{
    public const int MaximumDocumentBytes = 16 * 1024 * 1024;

    public static HmiProject Clone(HmiProject project)
    {
        ThrowIfInvalid(project);
        var data = JsonSerializer.SerializeToUtf8Bytes(project, ProjectJsonContext.Default.HmiProject);
        if (data.Length > MaximumDocumentBytes)
            throw new InvalidDataException($"Project document exceeds {MaximumDocumentBytes} bytes.");
        return DeserializeAndValidate(data);
    }

    public static async Task<HmiProject> LoadAsync(string path, CancellationToken cancellationToken = default)
    {
        await using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read,
            64 * 1024, FileOptions.Asynchronous | FileOptions.SequentialScan);
        if (stream.Length is <= 0 or > MaximumDocumentBytes)
            throw new InvalidDataException($"Project document must contain 1 to {MaximumDocumentBytes} bytes.");
        var data = new byte[(int)stream.Length];
        await stream.ReadExactlyAsync(data, cancellationToken);
        if (stream.ReadByte() != -1)
            throw new InvalidDataException("Project document changed while loading.");
        cancellationToken.ThrowIfCancellationRequested();
        return DeserializeAndValidate(data);
    }

    public static async Task SaveAsync(HmiProject project, string path, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfInvalid(project);
        var data = JsonSerializer.SerializeToUtf8Bytes(project, ProjectJsonContext.Default.HmiProject);
        if (data.Length > MaximumDocumentBytes)
            throw new InvalidDataException($"Project document exceeds {MaximumDocumentBytes} bytes.");
        // Validate the actual serialized snapshot before replacing any existing document.
        DeserializeAndValidate(data);
        var fullPath = Path.GetFullPath(path);
        var directory = Path.GetDirectoryName(fullPath)!;
        var temporaryPath = Path.Combine(directory, $".{Path.GetFileName(fullPath)}.{Guid.NewGuid():N}.tmp");
        var ownsTemporaryFile = false;
        try
        {
            await using (var stream = new FileStream(temporaryPath, FileMode.CreateNew, FileAccess.Write,
                FileShare.None, 64 * 1024, FileOptions.Asynchronous | FileOptions.WriteThrough))
            {
                ownsTemporaryFile = true;
                await stream.WriteAsync(data, cancellationToken);
                await stream.FlushAsync(cancellationToken);
                stream.Flush(flushToDisk: true);
            }
            cancellationToken.ThrowIfCancellationRequested();
            File.Move(temporaryPath, fullPath, overwrite: true);
            ownsTemporaryFile = false;
        }
        finally
        {
            if (ownsTemporaryFile)
                File.Delete(temporaryPath);
        }
    }

    private static HmiProject DeserializeAndValidate(ReadOnlySpan<byte> data)
    {
        HmiProject project;
        try
        {
            project = JsonSerializer.Deserialize(data, ProjectJsonContext.Default.HmiProject)
                ?? throw new InvalidDataException("Project document cannot be null.");
        }
        catch (JsonException exception)
        {
            throw new InvalidDataException("Project document is not valid HmiStudio JSON.", exception);
        }
        ThrowIfInvalid(project);
        return project;
    }

    private static void ThrowIfInvalid(HmiProject project)
    {
        var errors = ProjectValidator.Validate(project);
        if (errors.Count != 0)
            throw new InvalidDataException(string.Join(Environment.NewLine, errors));
    }
}

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    WriteIndented = true, UseStringEnumConverter = true,
    UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow)]
[JsonSerializable(typeof(HmiProject))]
internal partial class ProjectJsonContext : JsonSerializerContext;
