using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Google.Cloud.AIPlatform.V1;
using Google.Cloud.TextToSpeech.V1;
using GrammarGuide.Services.Settings;
using GrammarGuide.Services.Tools;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace GrammarGuide.Services.Services;
using Value = Google.Protobuf.WellKnownTypes.Value;

public class BlobGenerator(IOptions<BlobSettings> settings) : IBlobGenerator
{
    public async Task<byte[]> GenerateImage(string prompt, string lang)
    {
        byte[] image = await GenerateImageVertexAi(prompt, lang);
        byte[] resized = ResizeImage(image);
        return resized;
    }

    private byte[] ResizeImage(byte[] inputImageBytes)
    {
        using (var inputStream = new MemoryStream(inputImageBytes))
        {
            using (var image = Image.Load<Rgba32>(inputStream))
            {
                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Size = new Size(settings.Value.ImageWidth, settings.Value.ImageHeight),
                    Mode = ResizeMode.Max,

                }));

                using (MemoryStream outputStream = new MemoryStream())
                {
                    image.Save(outputStream, new PngEncoder
                    {
                        CompressionLevel = PngCompressionLevel.BestCompression,
                        TransparentColorMode = PngTransparentColorMode.Preserve
                    });
                    outputStream.Position = 0;
                    return outputStream.ToArray();
                }
            }
        }
    }

    private async Task<byte[]> GenerateImageVertexAi(string prompt, string lang)
    {
        var predictionServiceClient = await new PredictionServiceClientBuilder
        {
            Endpoint = settings.Value.AiPlatformEndpoint
        }.BuildAsync();

        var predictRequest = new PredictRequest
        {
            EndpointAsEndpointName =
                EndpointName.FromProjectLocationPublisherModel(settings.Value.ProjectId, settings.Value.LocationId, settings.Value.PublisherId, settings.Value.ModelId),
            Instances =
            {
                Value.ForStruct(new()
                {
                    Fields =
                    {
                        ["prompt"] = Value.ForString(prompt)
                    }
                })
            },
            Parameters = Value.ForStruct(new()
            {
                Fields =
                {
                    ["sampleCount"] = Value.ForNumber(1)
                }
            })
        };

        PredictResponse response = await predictionServiceClient.PredictAsync(predictRequest);
        return Convert.FromBase64String(response.Predictions.First().StructValue.Fields["bytesBase64Encoded"]
            .StringValue);
    }

    public async Task<byte[]> GenerateAudio(string textToPronounce, string lang, string name = null)
    {
        var client = await TextToSpeechClient.CreateAsync();

        var input = new SynthesisInput
        {
            Text = textToPronounce.Cleaned()
        };

        var voiceSelection = new VoiceSelectionParams
        {
            LanguageCode = GetLanguageCode(lang),
            SsmlGender = SsmlVoiceGender.Female,
        };

        if (name != null)
        {
            voiceSelection.Name = name;
        }

        var audioConfig = new AudioConfig
        {
            AudioEncoding = AudioEncoding.Mp3
        };

        var response = await client.SynthesizeSpeechAsync(input, voiceSelection, audioConfig);

        var stream = new MemoryStream();
        response.AudioContent.WriteTo(stream);

        return stream.ToArray();
    }

    private string GetLanguageCode(string lang)
    {
        var codes = new Dictionary<string, string>
        {
            { "english", "en-US" },
            { "polish", "pl-PL" },
            { "spanish", "es-ES" },
            { "french", "fr-FR" },
            { "german", "de-DE" },
            { "italian", "it-IT" },
            { "portuguese", "pt-PT" },
            { "dutch", "nl-NL" },
            { "russian", "ru-RU" },
            { "turkish", "tr-TR" },
            { "chinese", "zh-CN" },
            { "japanese", "ja-JP" },
            { "latvian", "lv-LV" }
        };
        if (codes.TryGetValue(lang.ToLower(), out var code))
            return code;

        throw new NotSupportedException();
    }
}
