using System.Text.Json;
using System.Linq;
using System.IO;
using A2UI.Core.Messages;
using Microsoft.Extensions.Hosting;

namespace A2UI.Sample.BlazorServer.Services;

/// <summary>
/// Mock A2A Agent that loads mock responses from JSON files so expected LLM payloads are easy to inspect.
/// </summary>
public class MockA2AAgent
{
    private readonly ILogger<MockA2AAgent> _logger;
    private readonly IHostEnvironment _environment;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly Dictionary<string, string> _keywordToFile;

    public MockA2AAgent(ILogger<MockA2AAgent> logger, IHostEnvironment environment)
    {
        _logger = logger;
        _environment = environment;

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true
        };

        _keywordToFile = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["联系人"] = "contacts.json",
            ["contact"] = "contacts.json",
            ["餐厅"] = "restaurant.json",
            ["restaurant"] = "restaurant.json",
            ["按钮"] = "buttons.json",
            ["button"] = "buttons.json",
            ["卡片"] = "simple-card.json",
            ["card"] = "simple-card.json",
            ["表单"] = "form.json",
            ["form"] = "form.json",
            ["输入"] = "form.json",
            ["产品"] = "products.json",
            ["商品"] = "products.json",
            ["product"] = "products.json",
            ["仪表盘"] = "dashboard.json",
            ["统计"] = "dashboard.json",
            ["dashboard"] = "dashboard.json",
            ["通知"] = "notifications.json",
            ["消息"] = "notifications.json",
            ["notification"] = "notifications.json",
            ["用户"] = "user-profile.json",
            ["资料"] = "user-profile.json",
            ["profile"] = "user-profile.json"
        };
    }

    /// <summary>
    /// Simulates an Agent processing a user query and returning A2UI JSON messages.
    /// </summary>
    public async Task<List<ServerToClientMessage>> ProcessQueryAsync(string query)
    {
        _logger.LogInformation("[MockA2AAgent] Processing query: {Query}", query);

        var scenarioFile = ResolveScenarioFile(query);
        var messages = await LoadMessagesFromJsonAsync(scenarioFile);

        _logger.LogInformation("[MockA2AAgent] Returning {Count} messages from {File}", messages.Count, scenarioFile);
        return messages;
    }

    private string ResolveScenarioFile(string query)
    {
        foreach (var mapping in _keywordToFile)
        {
            if (query.Contains(mapping.Key, StringComparison.OrdinalIgnoreCase))
            {
                return mapping.Value;
            }
        }

        return "welcome.json";
    }

    private async Task<List<ServerToClientMessage>> LoadMessagesFromJsonAsync(string fileName)
    {
        var path = Path.Combine(_environment.ContentRootPath, "MockData", fileName);

        if (!File.Exists(path))
        {
            _logger.LogWarning("[MockA2AAgent] Mock JSON not found at {Path}. Falling back to welcome.json", path);
            if (!fileName.Equals("welcome.json", StringComparison.OrdinalIgnoreCase))
            {
                return await LoadMessagesFromJsonAsync("welcome.json");
            }

            return new List<ServerToClientMessage>();
        }

        await using var stream = File.OpenRead(path);
        var messages = await JsonSerializer.DeserializeAsync<List<ServerToClientMessage>>(stream, _jsonOptions)
            ?? new List<ServerToClientMessage>();

        NormalizeComponents(messages);
        return messages;
    }

    private void NormalizeComponents(IEnumerable<ServerToClientMessage> messages)
    {
        foreach (var message in messages)
        {
            var components = message.SurfaceUpdate?.Components;
            if (components == null)
            {
                continue;
            }

            foreach (var component in components)
            {
                component.Component = component.Component.ToDictionary(
                    kvp => kvp.Key,
                    kvp => NormalizeValue(kvp.Value));
            }
        }
    }

    // Convert JsonElement payloads back into plain dictionaries/lists so MessageProcessor can re-serialize cleanly.
    private object? NormalizeValue(object? value)
    {
        switch (value)
        {
            case JsonElement json:
                return json.ValueKind switch
                {
                    JsonValueKind.Object => json.EnumerateObject().ToDictionary(p => p.Name, p => NormalizeValue(p.Value)),
                    JsonValueKind.Array => json.EnumerateArray().Select(item => NormalizeValue(item)).ToList(),
                    JsonValueKind.String => json.GetString(),
                    JsonValueKind.Number => json.TryGetInt64(out var number) ? number : json.GetDouble(),
                    JsonValueKind.True => true,
                    JsonValueKind.False => false,
                    JsonValueKind.Null => null,
                    _ => json.GetRawText()
                };
            case Dictionary<string, object> dict:
                return dict.ToDictionary(kvp => kvp.Key, kvp => NormalizeValue(kvp.Value));
            case IEnumerable<object?> list:
                return list.Select(NormalizeValue).ToList();
            default:
                return value;
        }
    }
}
