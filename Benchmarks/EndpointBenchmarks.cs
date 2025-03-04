using System.Text;
using System.Text.Json;
using BenchmarkDotNet.Attributes;
using OrgaNestApi.Common.Domain;

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
        string baseUrl = Environment.GetEnvironmentVariable("API_BASE_URL") ?? "https://localhost:7087";
        _client = new HttpClient { BaseAddress = new Uri(baseUrl) };
        _client.DefaultRequestHeaders.ConnectionClose = false; // Keep connections open

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
        int pageNumber = 1;  // First page
        int pageSize = 1000;   // Default page size

        var response = await _client.GetAsync($"/api/categories?pageNumber={pageNumber}&pageSize={pageSize}");
        response.EnsureSuccessStatusCode();
    
        using var stream = await response.Content.ReadAsStreamAsync();
            
        var result = await JsonSerializer.DeserializeAsync<CursorPagedResult<CategoryResponse>>(stream, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }
    
    [Benchmark]
    public async Task GetAllPagesCategories()
    {
        int pageNumber = 1;
        int pageSize = 1000;
        int totalFetched = 0;

        while (true)
        {
            var response = await _client.GetAsync($"/api/categories?pageNumber={pageNumber}&pageSize={pageSize}");
            response.EnsureSuccessStatusCode();

            using var stream = await response.Content.ReadAsStreamAsync();
            
            var result = await JsonSerializer.DeserializeAsync<CursorPagedResult<CategoryResponse>>(stream, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            var count = result.Data.Count();
            
            if (result == null || count == 0)
                break; // No more pages to fetch

            totalFetched += count;
            pageNumber++; // Move to the next page
        }

        Console.WriteLine($"Total Categories Fetched: {totalFetched}");
    }

    [Benchmark]
    public async Task GetAllCategoriesWithCursor()
    {
        string? cursor = null;
        int pageSize = 1000;
        int totalFetched = 0;

        do
        {
            var url = $"/api/categories/cursor?pageSize={pageSize}&cursor={cursor}";

            var response = await _client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            using var stream = await response.Content.ReadAsStreamAsync();
            
            var result = await JsonSerializer.DeserializeAsync<CursorPagedResult<CategoryResponse>>(stream, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (result == null || result.Data.Count() == 0)
                break; // No more data, exit loop

            totalFetched += result.Data.Count();
            cursor = result.NextCursor; // Get the next cursor for pagination

        } while (!string.IsNullOrEmpty(cursor));

        Console.WriteLine($"Total Categories Fetched: {totalFetched}");
    }
    
    [Benchmark]
    public async Task GetAllCategoriesPagesWithCursor()
    {
        string? cursor = null;
        int pageSize = 1000;
        int totalFetched = 0;

        do
        {
            var url = $"/api/categories/cursor?pageSize={pageSize}&cursor={cursor}";

            var response = await _client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            using var stream = await response.Content.ReadAsStreamAsync();
            
            var result = await JsonSerializer.DeserializeAsync<CursorPagedResult<CategoryResponse>>(stream, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (result == null || result.Data.Count == 0)
                break; // No more pages

            totalFetched += result.Data.Count;
            cursor = result.NextCursor; // Move to next set of results
        } while (!string.IsNullOrEmpty(cursor));

        Console.WriteLine($"Total Categories Fetched: {totalFetched}");
    }
    
    /*[Benchmark]
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
    }*/

    /*[Benchmark]
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
    }*/

    // DTO for deserializing API responses
    public class CategoryResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
    
    public class CursorPagedResult<T>
    {
        public List<T> Data { get; set; } = new();
        public string? NextCursor { get; set; }
    }
}