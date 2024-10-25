using System.IO;
using System.Threading.Tasks;

namespace GrammarGuide.Services.Services;

public interface IBlobService
{
    Task<Stream> GetImageStream(string blobId);
    Task<Stream> GetAudioStream(string blobId);
    Task<string> SaveImageBlob(byte[] bytes);
    Task<string> SaveAudioBlob(byte[] bytes);
}