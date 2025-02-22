using System.Text;
using System.Text.Json;
using BenchmarkDotNet.Attributes;

[MemoryDiagnoser]
public class EndpointBenchmarks
{
    private HttpClient? _client;
    private Guid _deleteCategoryId;
    private Guid _testCategoryId;
    private string? _testCategoryName;

    [GlobalSetup]
    public async Task GlobalSetup()
    {
        _client = new HttpClient { BaseAddress = new Uri("https://localhost:7087") };

        // Use a unique category name to avoid conflict errors.
        _testCategoryName = "BenchmarkTestCategory_" + Guid.NewGuid();
        var requestBody = $"{{\"Name\":\"{_testCategoryName}\"}}";
        var content = new StringContent(requestBody, Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("/api/categories", content);
        response.EnsureSuccessStatusCode();
        var responseBody = await response.Content.ReadAsStringAsync();
        var category = JsonSerializer.Deserialize<CategoryResponse>(
            responseBody,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );
        _testCategoryId = category!.Id;
    }

    [Benchmark]
    public async Task GetAllCategories()
    {
        var response = await _client.GetAsync("/api/categories");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
    }

    [Benchmark]
    public async Task GetCategoryById()
    {
        var response = await _client.GetAsync($"/api/categories/{_testCategoryId}");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
    }

    [Benchmark]
    public async Task GetCategoryByName()
    {
        var response = await _client.GetAsync($"/api/categories/search?name={_testCategoryName}");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
    }

    [Benchmark]
    public async Task CreateCategory()
    {
        // Generate a unique name for each creation.
        var categoryName = "BenchmarkCreate_" + Guid.NewGuid();
        var requestBody = $"{{\"Name\":\"{categoryName}\"}}";
        var content = new StringContent(requestBody, Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("/api/categories", content);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadAsStringAsync();
    }

    [Benchmark]
    public async Task UpdateCategory()
    {
        var newName = "Updated_" + Guid.NewGuid();
        var requestBody = $"{{\"Name\":\"{newName}\"}}";
        var content = new StringContent(requestBody, Encoding.UTF8, "application/json");
        var response = await _client.PutAsync($"/api/categories/{_testCategoryId}", content);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadAsStringAsync();
    }

    // Synchronous iteration setup for the DeleteCategory benchmark.
    [IterationSetup(Target = nameof(DeleteCategory))]
    public void SetupDeleteCategory()
    {
        var categoryName = "BenchmarkDelete_" + Guid.NewGuid();
        var requestBody = $"{{\"Name\":\"{categoryName}\"}}";
        var content = new StringContent(requestBody, Encoding.UTF8, "application/json");

        var response = _client.PostAsync("/api/categories", content)
            .GetAwaiter().GetResult();
        response.EnsureSuccessStatusCode();
        var result = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        var category = JsonSerializer.Deserialize<CategoryResponse>(
            result, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );
        _deleteCategoryId = category!.Id;
    }

    [Benchmark]
    public async Task DeleteCategory()
    {
        var response = await _client.DeleteAsync($"/api/categories/{_deleteCategoryId}");
        response.EnsureSuccessStatusCode();
    }

    // DTO for deserializing API responses
    public class CategoryResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}