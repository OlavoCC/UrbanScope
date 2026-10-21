namespace Api.Images.Services;

using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;
using System.IO;
using Api.Data.Settings;
public class CloudinaryService
{
    private readonly Cloudinary _cloudinary;

    public CloudinaryService(Cloudinary cloudinary, IOptions<CloudinarySettings> settings)
    {
        _cloudinary = cloudinary;
    }

    public async Task<string> UploadImagemAsync(byte[] imagemBytes, string nomeArquivo)
    {
        if (imagemBytes == null || imagemBytes.Length == 0)
            throw new ArgumentException("Imagem vazia", nameof(imagemBytes));

        using var ms = new MemoryStream(imagemBytes);

        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(nomeArquivo, ms),
            PublicId = $"whatsapp/{Path.GetFileNameWithoutExtension(nomeArquivo)}",
            Folder = "urban-scope/whatsapp",
        };

        var resultado = await _cloudinary.UploadAsync(uploadParams);

        if (resultado.StatusCode == System.Net.HttpStatusCode.OK)
            return resultado.SecureUrl.AbsoluteUri;

        throw new Exception($"Falha no upload: {resultado.Error?.Message}");
    }
}