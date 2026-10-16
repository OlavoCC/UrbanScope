namespace Api.Images.Services;

using Api.Images.DTOs.Entry;
using Api.Images.DTOs.Return;
using Api.Images.DTOs.FastApiResponse;
using System.Net.Http.Headers;

public class ImageOccurrence
{
    private readonly string url = "http://localhost:8000";

    public async Task<ReturnOccurrenceDataDTO> CreateOccurrence(byte[] imageBytes, string contentType, string fileName)
    {
        string dataUri = $"data:{contentType};base64,{Convert.ToBase64String(imageBytes)}";

        using var form = new MultipartFormDataContent();
        var imageContent = new ByteArrayContent(imageBytes);
        imageContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
        form.Add(imageContent, "image", fileName);

        using var client = new HttpClient();
        var response = await client.PostAsync($"{url}/Api/YOLO/detect", form);
        var data = await response.Content.ReadFromJsonAsync<YoloDetectResponseDTO>();

        return new ReturnOccurrenceDataDTO
        {
            Id = Guid.NewGuid(),
            Have_hole = data!.have_hole,
            Confidence = data.confidence_max.normalized * 100,
            Model_version = data.model_version,
            Image64 = dataUri,
            Image64_annotated = data.image_annotated
        };
    }
}
