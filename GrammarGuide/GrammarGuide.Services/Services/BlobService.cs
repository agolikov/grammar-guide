using System;
using System.IO;
using System.Threading.Tasks;
using Google.Cloud.Storage.V1;
using GrammarGuide.Services.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace GrammarGuide.Services.Services;

public class BlobService(IOptions<BlobSettings> settings) : IBlobService
{
    public async Task<Stream> GetImageStream(string blobId)
    {
        var memStream = new MemoryStream();
        var storageClient = await StorageClient.CreateAsync();
        await storageClient.DownloadObjectAsync(settings.Value.BucketName, blobId, memStream);
        memStream.Seek(0, SeekOrigin.Begin);
        return memStream;
    }

    public async Task<Stream> GetAudioStream(string blobId)
    {
        var memStream = new MemoryStream();
        var storageClient = await StorageClient.CreateAsync();
        await storageClient.DownloadObjectAsync(settings.Value.BucketName, blobId, memStream);
        memStream.Seek(0, SeekOrigin.Begin);
        return memStream;
    }

    public async Task<string> SaveImageBlob(byte[] bytes)
    {
        var objectName = GetImageName();
        var storageClient = await StorageClient.CreateAsync();
        var memoryStream = new MemoryStream(bytes);
        await storageClient.UploadObjectAsync(settings.Value.BucketName, objectName, null, memoryStream);
        return objectName;
    }

    public async Task<string> SaveAudioBlob(byte[] bytes)
    {
        var objectName = GetAudioName();
        var storageClient = await StorageClient.CreateAsync();
        var memoryStream = new MemoryStream(bytes);
        await storageClient.UploadObjectAsync(settings.Value.BucketName, objectName, null, memoryStream);
        return objectName;
    }

    private string GetImageName()
    {
        return "IMG_" + Guid.NewGuid().ToString().Replace("-", "").Substring(10);
    }

    private string GetAudioName()
    {
        return "AUD_" + Guid.NewGuid().ToString().Replace("-", "").Substring(10);
    }
}
