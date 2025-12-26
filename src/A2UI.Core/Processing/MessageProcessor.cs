using A2UI.Core.Messages;
using A2UI.Core.Types;
using System.Text.Json;

namespace A2UI.Core.Processing;

/// <summary>
/// Processes A2UI messages and maintains the state of surfaces and their data models.
/// </summary>
public class MessageProcessor
{
    private readonly Dictionary<string, Surface> _surfaces = new();
    private readonly JsonSerializerOptions _jsonOptions;

    /// <summary>
    /// Event raised when a surface is updated.
    /// </summary>
    public event EventHandler<SurfaceUpdatedEventArgs>? SurfaceUpdated;

    public MessageProcessor()
    {
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    /// <summary>
    /// Gets all surfaces.
    /// </summary>
    public IReadOnlyDictionary<string, Surface> Surfaces => _surfaces;

    /// <summary>
    /// Triggers a SurfaceUpdated event for the given surface.
    /// Use this when the data model is mutated directly (e.g., via SetData)
    /// and you want bound components to refresh without applying a DataModelUpdate message.
    /// </summary>
    public void NotifySurfaceUpdated(string surfaceId)
    {
        OnSurfaceUpdated(surfaceId);
    }

    /// <summary>
    /// Processes a batch of messages.
    /// </summary>
    public void ProcessMessages(IEnumerable<ServerToClientMessage> messages)
    {
        foreach (var message in messages)
        {
            ProcessMessage(message);
        }
    }

    /// <summary>
    /// Processes a single message.
    /// </summary>
    public void ProcessMessage(ServerToClientMessage message)
    {
        if (message.BeginRendering != null)
        {
            HandleBeginRendering(message.BeginRendering);
        }
        else if (message.SurfaceUpdate != null)
        {
            HandleSurfaceUpdate(message.SurfaceUpdate);
        }
        else if (message.DataModelUpdate != null)
        {
            HandleDataModelUpdate(message.DataModelUpdate);
        }
        else if (message.DeleteSurface != null)
        {
            HandleDeleteSurface(message.DeleteSurface);
        }
    }

    /// <summary>
    /// Processes a JSONL stream of messages.
    /// </summary>
    public async Task ProcessJsonLinesAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        using var reader = new StreamReader(stream);
        
        while (!reader.EndOfStream && !cancellationToken.IsCancellationRequested)
        {
            var line = await reader.ReadLineAsync(cancellationToken);
            if (string.IsNullOrWhiteSpace(line))
                continue;

            try
            {
                var message = JsonSerializer.Deserialize<ServerToClientMessage>(line, _jsonOptions);
                if (message != null)
                {
                    ProcessMessage(message);
                }
            }
            catch (JsonException ex)
            {
                // Log or handle JSON parsing errors
                Console.Error.WriteLine($"Error parsing message: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Gets a surface by ID.
    /// </summary>
    public Surface? GetSurface(string surfaceId)
    {
        return _surfaces.TryGetValue(surfaceId, out var surface) ? surface : null;
    }

    /// <summary>
    /// Clears all surfaces.
    /// </summary>
    public void ClearSurfaces()
    {
        _surfaces.Clear();
    }

    private void HandleBeginRendering(BeginRenderingMessage message)
    {
        var surface = GetOrCreateSurface(message.SurfaceId);
        
        // 清除旧的组件，因为这是一个新的渲染周期
        surface.Components.Clear();
        
        surface.RootComponentId = message.Root;
        surface.CatalogId = message.CatalogId ?? A2UIConstants.StandardCatalogId;
        surface.Styles = message.Styles;
        surface.IsReadyToRender = true;
        
        Console.WriteLine($"[A2UI] BeginRendering: SurfaceId={message.SurfaceId}, Root={message.Root}, IsReady={surface.IsReadyToRender}");
        
        // Notify subscribers that the surface has been updated
        OnSurfaceUpdated(message.SurfaceId);
    }

    private void HandleSurfaceUpdate(SurfaceUpdateMessage message)
    {
        var surface = GetOrCreateSurface(message.SurfaceId);

        foreach (var componentDef in message.Components)
        {
            var componentNode = ParseComponentDefinition(componentDef);
            surface.Components[componentDef.Id] = componentNode;
        }
        
        Console.WriteLine($"[A2UI] SurfaceUpdate: SurfaceId={message.SurfaceId}, ComponentCount={message.Components.Count}, TotalComponents={surface.Components.Count}");
        
        // Notify subscribers that the surface has been updated
        OnSurfaceUpdated(message.SurfaceId);
    }

    private void HandleDataModelUpdate(DataModelUpdateMessage message)
    {
        var surface = GetOrCreateSurface(message.SurfaceId);
        var path = message.Path ?? "/";

        // Convert the adjacency list format to a nested dictionary
        var dataDict = ConvertDataEntriesToDict(message.Contents);
        
        surface.DataModel.SetValue(path, dataDict);
        
        // Notify subscribers that the surface has been updated
        OnSurfaceUpdated(message.SurfaceId);
    }

    private void HandleDeleteSurface(DeleteSurfaceMessage message)
    {
        _surfaces.Remove(message.SurfaceId);
    }

    private Surface GetOrCreateSurface(string surfaceId)
    {
        if (!_surfaces.TryGetValue(surfaceId, out var surface))
        {
            surface = new Surface { Id = surfaceId };
            _surfaces[surfaceId] = surface;
        }
        return surface;
    }

    private ComponentNode ParseComponentDefinition(ComponentDefinition def)
    {
        // The component object should have exactly one key (the component type)
        var componentType = def.Component.Keys.FirstOrDefault() 
            ?? throw new InvalidOperationException("Component definition must have a type");

        var properties = def.Component[componentType] as Dictionary<string, object> 
            ?? new Dictionary<string, object>();

        return new ComponentNode
        {
            Id = def.Id,
            Type = componentType,
            Properties = properties,
            Weight = def.Weight
        };
    }

    private Dictionary<string, object> ConvertDataEntriesToDict(List<DataEntry> entries)
    {
        var result = new Dictionary<string, object>();

        foreach (var entry in entries)
        {
            object? value = null;

            if (entry.ValueString != null)
                value = entry.ValueString;
            else if (entry.ValueNumber != null)
                value = entry.ValueNumber.Value;
            else if (entry.ValueBoolean != null)
                value = entry.ValueBoolean.Value;
            else if (entry.ValueMap != null)
                value = ConvertDataEntriesToDict(entry.ValueMap);

            if (value != null)
            {
                result[entry.Key] = value;
            }
        }

        return result;
    }

    /// <summary>
    /// Resolves a path relative to a data context path.
    /// </summary>
    public string ResolvePath(string path, string? dataContextPath = null)
    {
        // If the path is absolute, it overrides any context
        if (path.StartsWith('/'))
        {
            return path;
        }

        // Special case: "." means the current context
        if (path == "." || path == "")
        {
            return dataContextPath ?? "/";
        }

        if (!string.IsNullOrEmpty(dataContextPath) && dataContextPath != "/")
        {
            // Ensure there's exactly one slash between the context and the path
            return dataContextPath.EndsWith('/')
                ? $"{dataContextPath}{path}"
                : $"{dataContextPath}/{path}";
        }

        // Fallback for no context or root context: make it an absolute path
        return $"/{path}";
    }

    /// <summary>
    /// Gets data from a surface's data model using a path.
    /// </summary>
    public object? GetData(string surfaceId, string path, string? dataContextPath = null)
    {
        var surface = GetSurface(surfaceId);
        if (surface == null)
            return null;

        var resolvedPath = ResolvePath(path, dataContextPath);
        return surface.DataModel.GetValue(resolvedPath);
    }

    /// <summary>
    /// Sets data in a surface's data model using a path.
    /// </summary>
    public void SetData(string surfaceId, string path, object value, string? dataContextPath = null)
    {
        var surface = GetOrCreateSurface(surfaceId);
        var resolvedPath = ResolvePath(path, dataContextPath);
        surface.DataModel.SetValue(resolvedPath, value);
    }

    /// <summary>
    /// Raises the SurfaceUpdated event.
    /// </summary>
    protected virtual void OnSurfaceUpdated(string surfaceId)
    {
        SurfaceUpdated?.Invoke(this, new SurfaceUpdatedEventArgs(surfaceId));
    }
}

/// <summary>
/// Event args for the SurfaceUpdated event.
/// </summary>
public class SurfaceUpdatedEventArgs : EventArgs
{
    public string SurfaceId { get; }

    public SurfaceUpdatedEventArgs(string surfaceId)
    {
        SurfaceId = surfaceId;
    }
}

