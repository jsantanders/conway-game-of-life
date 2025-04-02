using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace IntegrationTests.Endpoints;

public class BoardsEndpointsTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task CreateBoard_WithValidState_ReturnsBoardId()
    {
        var boardRequest = new
        {
            cells = new List<List<bool>>
            {
                new List<bool> { true, false, true },
                new List<bool> { false, true, false },
                new List<bool> { true, false, true }
            }
        };

        var response = await _client.PostAsJsonAsync("/boards", boardRequest);
        response.EnsureSuccessStatusCode();

        var boardIdResponse = await response.Content.ReadFromJsonAsync<BoardIdResponse>();
        Assert.NotNull(boardIdResponse);
        Assert.NotEqual(Guid.Empty, boardIdResponse!.Id);
    }

    [Fact]
    public async Task CreateRandomBoard_ReturnsBoardId()
    {
        var randomBoardRequest = new
        {
            rows = 5,
            columns = 5
        };

        var response = await _client.PostAsJsonAsync("/boards/random", randomBoardRequest);
        response.EnsureSuccessStatusCode();

        var boardIdResponse = await response.Content.ReadFromJsonAsync<BoardIdResponse>();
        Assert.NotNull(boardIdResponse);
        Assert.NotEqual(Guid.Empty, boardIdResponse!.Id);
    }

    [Fact]
    public async Task GetNextGeneration_ReturnsNewBoardState()
    {
        // Create a board first.
        var boardCreation = new
        {
            cells = new List<List<bool>>
            {
                new List<bool> { false, true, false },
                new List<bool> { false, true, false },
                new List<bool> { false, true, false }
            }
        };

        var createResponse = await _client.PostAsJsonAsync("/boards", boardCreation);
        createResponse.EnsureSuccessStatusCode();
        var boardIdResponse = await createResponse.Content.ReadFromJsonAsync<BoardIdResponse>();

        // Call /boards/{id}/next to get the next generation.
        var nextGenResponse = await _client.PostAsync($"/boards/{boardIdResponse!.Id}/next", null);
        nextGenResponse.EnsureSuccessStatusCode();

        var boardResponse = await nextGenResponse.Content.ReadFromJsonAsync<BoardResponse>();
        Assert.NotNull(boardResponse);
        Assert.NotNull(boardResponse!);
    }
    
    [Fact]
    public async Task GetStepGeneration_ReturnsNewBoardState()
    {
        // Create a board first.
        var boardCreation = new
        {
            cells = new List<List<bool>>
            {
                new List<bool> { false, true, false },
                new List<bool> { false, true, false },
                new List<bool> { false, true, false }
            }
        };

        var createResponse = await _client.PostAsJsonAsync("/boards", boardCreation);
        createResponse.EnsureSuccessStatusCode();
        var boardIdResponse = await createResponse.Content.ReadFromJsonAsync<BoardIdResponse>();

        // Call /boards/{id}/next to get the next generation.
        var nextGenResponse = await _client.PostAsync($"/boards/{boardIdResponse!.Id}/steps/{10}", null);
        nextGenResponse.EnsureSuccessStatusCode();

        var boardResponse = await nextGenResponse.Content.ReadFromJsonAsync<BoardResponse>();
        Assert.NotNull(boardResponse);
        Assert.NotNull(boardResponse!);
    }
    
    [Fact]
    public async Task GetFinalGeneration_ReturnsNewBoardState()
    {
        // Create a board first.
        var boardCreation = new
        {
            cells = new List<List<bool>>
            {
                new List<bool> { false, true, false },
                new List<bool> { false, true, false },
                new List<bool> { false, true, false }
            }
        };

        var createResponse = await _client.PostAsJsonAsync("/boards", boardCreation);
        createResponse.EnsureSuccessStatusCode();
        var boardIdResponse = await createResponse.Content.ReadFromJsonAsync<BoardIdResponse>();

        // Call /boards/{id}/next to get the next generation.
        var nextGenResponse = await _client.PostAsync($"/boards/{boardIdResponse!.Id}/final", null);
        nextGenResponse.EnsureSuccessStatusCode();

        var boardResponse = await nextGenResponse.Content.ReadFromJsonAsync<BoardResponse>();
        Assert.NotNull(boardResponse);
        Assert.NotNull(boardResponse!);
    }
}

public class BoardIdResponse
{
    public Guid Id { get; set; }
}

public class BoardResponse
{
    public List<List<bool>> Cells { get; set; } = new();
}