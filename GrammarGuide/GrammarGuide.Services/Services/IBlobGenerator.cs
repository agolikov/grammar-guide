using System.Threading.Tasks;

namespace GrammarGuide.Services.Services;

public interface IBlobGenerator
{
    Task<byte[]> GenerateImage(string prompt, string lang);
    
    Task<byte[]> GenerateAudio(string textToPronounce, string lang, string name);
}