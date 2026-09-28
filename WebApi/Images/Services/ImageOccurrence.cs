namespace Api.Images.Services;

using Api.Images.DTOs.Entry;
using Api.Images.DTOs.Return;
using System.Net.Http.Headers;

using Api.Images.DTOs.FastApiResponse;

public class ImageOccurrence
{
    string url = "http://localhost:8000";
    public async Task<ReturnOccurrenceDataDTO> CreateOccurrence(EntryOccurrenceDataDTO dto)
    {
        using var ms = new MemoryStream();
        await dto.Image.CopyToAsync(ms);
        byte[] imageBytes = ms.ToArray();          // serve pros DOIS

        string dataUri = $"data:{dto.Image.ContentType};base64,{Convert.ToBase64String(imageBytes)}";

        using var form = new MultipartFormDataContent();
        var imageContent = new ByteArrayContent(imageBytes);
        imageContent.Headers.ContentType = new MediaTypeHeaderValue(dto.Image.ContentType);
        form.Add(imageContent, "image", dto.Image.FileName);

        using var client = new HttpClient();
        var response = await client.PostAsync($"{url}/Api/YOLO/detect", form);
        var data = await response.Content.ReadFromJsonAsync<YoloDetectResponseDTO>();


        var result = new ReturnOccurrenceDataDTO
        {
            Id = Guid.NewGuid(),
            Have_hole = data.have_hole,
            Confidence = data.confidence_max.normalized * 100,
            Model_version = data.model_version,
            Image64 = dataUri,
            Image64_annotated = data.image_annotated

        };

        return result;
    }
}
