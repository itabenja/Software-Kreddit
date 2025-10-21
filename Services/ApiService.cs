using System.Net.Http.Json;
using System.Text.Json;
using shared.Model;

namespace kreddit_app.Data;

public class ApiService
{
    private readonly HttpClient http;

    public ApiService(HttpClient http)
    {
        this.http = http;
        // HttpClient.BaseAddress sættes i Program.cs til backendens URL (fx https://localhost:5001).
        // Derfor bruger vi relative stier nedenfor.
    }

    // Matcher: GET /api/threads
    public async Task<shared.Model.Thread[]?> GetThreads(int? count = null)
    {
        var url = count is > 0 ? $"/api/threads?count={count}" : "/api/threads";
        return await http.GetFromJsonAsync<shared.Model.Thread[]>(url, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }

    // Matcher: GET /api/threads/{id:int}
    public async Task<shared.Model.Thread?> GetThread(int id)
    {
        var url = $"/api/threads/{id}";
        return await http.GetFromJsonAsync<shared.Model.Thread>(url, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }

    // Matcher: POST /api/threads/{id:int}/comments  body: { text, authorName } -> Comment
    public async Task<Comment?> CreateComment(int threadId, string text, string authorName)
    {
        var url = $"/api/threads/{threadId}/comments";
        var payload = new { text, authorName };

        var resp = await http.PostAsJsonAsync(url, payload);
        resp.EnsureSuccessStatusCode();

        return await resp.Content.ReadFromJsonAsync<Comment>(new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }

    // Matcher: POST /api/threads/{id:int}/upvote  body: { voterName } -> int (score)
    public async Task<int> UpvoteThread(int threadId, string voterName)
    {
        var url = $"/api/threads/{threadId}/upvote";
        var payload = new { voterName };

        var resp = await http.PostAsJsonAsync(url, payload);
        resp.EnsureSuccessStatusCode();

        return (await resp.Content.ReadFromJsonAsync<int>())!;
    }

    // Matcher: POST /api/threads/{id:int}/downvote  body: { voterName } -> int (score)
    public async Task<int> DownvoteThread(int threadId, string voterName)
    {
        var url = $"/api/threads/{threadId}/downvote";
        var payload = new { voterName };

        var resp = await http.PostAsJsonAsync(url, payload);
        resp.EnsureSuccessStatusCode();

        return (await resp.Content.ReadFromJsonAsync<int>())!;
    }

    // Matcher: POST /api/threads  body: { title, text, authorName } -> Thread
    public async Task<shared.Model.Thread?> CreateThread(string title, string text, string authorName)
    {
        var url = "/api/threads";
        var payload = new { title, text, authorName };

        var resp = await http.PostAsJsonAsync(url, payload);
        resp.EnsureSuccessStatusCode();

        return await resp.Content.ReadFromJsonAsync<shared.Model.Thread>(new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }

    // Overload for at matche eksisterende UI-kald: CreatePost(title, text, userId)
    // Mapper userId midlertidigt til en fast authorName.
    public Task<shared.Model.Thread?> CreatePost(string title, string text, int userId)
        => CreateThread(title, text, "Guest");
}