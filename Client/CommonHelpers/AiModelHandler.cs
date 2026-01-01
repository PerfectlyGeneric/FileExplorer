using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

//Change to IHttpClientFactory later

public record TagScore(string Tag, float Score);
//public record AiResponse(TagScore[] TagsRanked)
public record AiResponse
{
    [JsonPropertyName("tags_ranked")]
    public TagScore[]? TagsRanked { get; init; }
}

public class AiModelHandler
{
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "http://localhost:8000";
        List<string> baseTags = new List<string>(){ "cat", "dog", "person", "car", "tree", "building", "indoor", "outdoor", "food", "animal", "landscape" };

    /// <summary>
    /// Method to start the Python API server for AI model handling. Uses a batch file to launch the server.
    /// </summary>
    public static void GetModelUp()
    {
        string baseDir = AppDomain.CurrentDomain.BaseDirectory; 
        string scriptPath = Path.Combine(baseDir, "ModelHandling", "start_api.bat");

        if(!System.IO.File.Exists(scriptPath))
        {
            scriptPath = @"E:\Proj_enter\FileExplorer\Scripts\start_api.bat";
        }   

        ProcessStartInfo psi = new ProcessStartInfo
        {
            FileName = scriptPath,
            CreateNoWindow = true,
            UseShellExecute = false
        };

        try
        {
            Process.Start(psi);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to start Python API: {ex.Message}");
        }
    }

    public AiModelHandler(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

        _httpClient.BaseAddress = new Uri(_baseUrl);
    }

    public async Task<string> GetAIResponse(string path, IEnumerable<string>? candidateTags)
    {
        var combinedTags = (candidateTags ?? Enumerable.Empty<string>()).Union(baseTags).ToList();


        if (string.IsNullOrWhiteSpace(path) || !System.IO.File.Exists(path))
            throw new FileNotFoundException("Input file path is invalid or does not exist.");

        using var form = new MultipartFormDataContent();

        //File Content
        using var fileStream = System.IO.File.OpenRead(path);
        using var fileContent = new StreamContent(fileStream);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
        form.Add(fileContent, "file", System.IO.Path.GetFileName(path));

        // Dynamic Tags
        foreach (var tag in combinedTags)
        {
            form.Add(new StringContent(tag), "candidate_tags");
        }

        try
        {


            var response = await _httpClient.PostAsync("tag-image", form);

            // Throws an exception if HTTP status is not 2xx
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();

            Debug.WriteLine($"Raw AI Response: {responseContent}");

            // JSON Deserialization
            var apiResponse = JsonSerializer.Deserialize<AiResponse>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true // handles "tags_ranked"
            });

            if (apiResponse?.TagsRanked == null){return "No Tag Found (API Response Null)"; 
            }return OutputThreeTags(apiResponse.TagsRanked);
        }


        catch (HttpRequestException ex)
        {
            // Handle connection issues (API down, wrong port, etc.)
            throw new InvalidOperationException($"AI Service error: {ex.Message}", ex);
        }
        catch (JsonException ex)
        {
            // Handle parsing issues
            throw new InvalidDataException($"Failed to parse AI response: {ex.Message}", ex);
        }
    }

    public static string OutputThreeTags(TagScore[] tagsRanked)
    {
        if (tagsRanked == null || tagsRanked.Length == 0)
        {
            return "No Tag Found";
        }

        var topTagsWithScores = tagsRanked
            .Take(3) 
            //.Select(t => $"{t.Tag} ({t.Score:F2})")
            .Select(t => $"{t.Tag}")
            .ToList();

        return string.Join(", ", topTagsWithScores);
    }

}
