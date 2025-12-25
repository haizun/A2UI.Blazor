using System.Text.Json;
using A2UI.Core.Messages;

namespace A2UI.Sample.BlazorServer.Services;

/// <summary>
/// Mock A2A Agent that simulates an LLM returning A2UI JSON responses
/// In a real application, this would call an actual LLM/Agent service
/// </summary>
public class MockA2AAgent
{
    private readonly ILogger<MockA2AAgent> _logger;

    public MockA2AAgent(ILogger<MockA2AAgent> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Simulates an Agent processing a user query and returning A2UI JSON messages
    /// </summary>
    public Task<List<ServerToClientMessage>> ProcessQueryAsync(string query)
    {
        _logger.LogInformation($"[MockA2AAgent] Processing query: {query}");

        // In a real implementation, this would:
        // 1. Send the query to an LLM with A2UI schema in the prompt
        // 2. Parse the LLM's JSON response
        // 3. Return the A2UI messages

        // For now, we'll match specific keywords and return appropriate UIs
        var messages = query.ToLower() switch
        {
            var q when q.Contains("联系人") || q.Contains("contact") => GetContactListExample(),
            var q when q.Contains("餐厅") || q.Contains("restaurant") => GetRestaurantExample(),
            var q when q.Contains("按钮") || q.Contains("button") => GetButtonExample(),
            var q when q.Contains("卡片") || q.Contains("card") => GetSimpleCardExample(),
            var q when q.Contains("表单") || q.Contains("form") || q.Contains("输入") => GetFormExample(),
            var q when q.Contains("产品") || q.Contains("商品") || q.Contains("product") => GetProductListExample(),
            var q when q.Contains("仪表盘") || q.Contains("统计") || q.Contains("dashboard") => GetDashboardExample(),
            var q when q.Contains("通知") || q.Contains("消息") || q.Contains("notification") => GetNotificationExample(),
            var q when q.Contains("用户") || q.Contains("资料") || q.Contains("profile") => GetUserProfileExample(),
            _ => GetWelcomeExample()
        };

        _logger.LogInformation($"[MockA2AAgent] Returning {messages.Count} messages");
        return Task.FromResult(messages);
    }

    private List<ServerToClientMessage> GetWelcomeExample()
    {
        return new List<ServerToClientMessage>
        {
            new ServerToClientMessage
            {
                BeginRendering = new BeginRenderingMessage
                {
                    SurfaceId = "demo-surface",
                    Root = "root-card",
                    CatalogId = "org.a2ui.standard@0.8"
                }
            },
            new ServerToClientMessage
            {
                SurfaceUpdate = new SurfaceUpdateMessage
                {
                    SurfaceId = "demo-surface",
                    Components = new List<ComponentDefinition>
                    {
                        new ComponentDefinition
                        {
                            Id = "root-card",
                            Component = new Dictionary<string, object>
                            {
                                ["Card"] = new Dictionary<string, object>
                                {
                                    ["child"] = "content-column"
                                }
                            }
                        },
                        new ComponentDefinition
                        {
                            Id = "content-column",
                            Component = new Dictionary<string, object>
                            {
                                ["Column"] = new Dictionary<string, object>
                                {
                                    ["children"] = new Dictionary<string, object>
                                    {
                                        ["explicitList"] = new[] { "title", "description", "hint" }
                                    }
                                }
                            }
                        },
                        new ComponentDefinition
                        {
                            Id = "title",
                            Component = new Dictionary<string, object>
                            {
                                ["Text"] = new Dictionary<string, object>
                                {
                                    ["text"] = new Dictionary<string, object>
                                    {
                                        ["literalString"] = "欢迎使用 A2UI！"
                                    },
                                    ["usageHint"] = "h1"
                                }
                            }
                        },
                        new ComponentDefinition
                        {
                            Id = "description",
                            Component = new Dictionary<string, object>
                            {
                                ["Text"] = new Dictionary<string, object>
                                {
                                    ["text"] = new Dictionary<string, object>
                                    {
                                        ["literalString"] = "这是一个由 AI Agent 生成的动态界面。"
                                    }
                                }
                            }
                        },
                        new ComponentDefinition
                        {
                            Id = "hint",
                            Component = new Dictionary<string, object>
                            {
                                ["Text"] = new Dictionary<string, object>
                                {
                                    ["text"] = new Dictionary<string, object>
                                    {
                                        ["literalString"] = "💡 试试输入: 显示联系人、显示餐厅、显示表单、显示产品、显示仪表盘、显示通知、显示用户资料"
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };
    }

    private List<ServerToClientMessage> GetSimpleCardExample()
    {
        return new List<ServerToClientMessage>
        {
            new ServerToClientMessage
            {
                BeginRendering = new BeginRenderingMessage
                {
                    SurfaceId = "demo-surface",
                    Root = "root",
                    CatalogId = "org.a2ui.standard@0.8"
                }
            },
            new ServerToClientMessage
            {
                SurfaceUpdate = new SurfaceUpdateMessage
                {
                    SurfaceId = "demo-surface",
                    Components = new List<ComponentDefinition>
                    {
                        new ComponentDefinition
                        {
                            Id = "root",
                            Component = new Dictionary<string, object>
                            {
                                ["Card"] = new Dictionary<string, object>
                                {
                                    ["child"] = "content"
                                }
                            }
                        },
                        new ComponentDefinition
                        {
                            Id = "content",
                            Component = new Dictionary<string, object>
                            {
                                ["Column"] = new Dictionary<string, object>
                                {
                                    ["children"] = new Dictionary<string, object>
                                    {
                                        ["explicitList"] = new[] { "title", "body" }
                                    }
                                }
                            }
                        },
                        new ComponentDefinition
                        {
                            Id = "title",
                            Component = new Dictionary<string, object>
                            {
                                ["Text"] = new Dictionary<string, object>
                                {
                                    ["text"] = new Dictionary<string, object>
                                    {
                                        ["literalString"] = "简单卡片"
                                    },
                                    ["usageHint"] = "h2"
                                }
                            }
                        },
                        new ComponentDefinition
                        {
                            Id = "body",
                            Component = new Dictionary<string, object>
                            {
                                ["Text"] = new Dictionary<string, object>
                                {
                                    ["text"] = new Dictionary<string, object>
                                    {
                                        ["literalString"] = "这是一个由 Agent 返回的简单卡片。"
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };
    }

    private List<ServerToClientMessage> GetButtonExample()
    {
        return new List<ServerToClientMessage>
        {
            new ServerToClientMessage
            {
                BeginRendering = new BeginRenderingMessage
                {
                    SurfaceId = "demo-surface",
                    Root = "root",
                    CatalogId = "org.a2ui.standard@0.8"
                }
            },
            new ServerToClientMessage
            {
                SurfaceUpdate = new SurfaceUpdateMessage
                {
                    SurfaceId = "demo-surface",
                    Components = new List<ComponentDefinition>
                    {
                        new ComponentDefinition
                        {
                            Id = "root",
                            Component = new Dictionary<string, object>
                            {
                                ["Card"] = new Dictionary<string, object>
                                {
                                    ["child"] = "content"
                                }
                            }
                        },
                        new ComponentDefinition
                        {
                            Id = "content",
                            Component = new Dictionary<string, object>
                            {
                                ["Column"] = new Dictionary<string, object>
                                {
                                    ["children"] = new Dictionary<string, object>
                                    {
                                        ["explicitList"] = new[] { "title", "desc", "button-row" }
                                    }
                                }
                            }
                        },
                        new ComponentDefinition
                        {
                            Id = "title",
                            Component = new Dictionary<string, object>
                            {
                                ["Text"] = new Dictionary<string, object>
                                {
                                    ["text"] = new Dictionary<string, object>
                                    {
                                        ["literalString"] = "交互按钮演示"
                                    },
                                    ["usageHint"] = "h2"
                                }
                            }
                        },
                        new ComponentDefinition
                        {
                            Id = "desc",
                            Component = new Dictionary<string, object>
                            {
                                ["Text"] = new Dictionary<string, object>
                                {
                                    ["text"] = new Dictionary<string, object>
                                    {
                                        ["literalString"] = "点击按钮与 Agent 交互："
                                    }
                                }
                            }
                        },
                        new ComponentDefinition
                        {
                            Id = "button-row",
                            Component = new Dictionary<string, object>
                            {
                                ["Row"] = new Dictionary<string, object>
                                {
                                    ["children"] = new Dictionary<string, object>
                                    {
                                        ["explicitList"] = new[] { "btn1", "btn2" }
                                    }
                                }
                            }
                        },
                        new ComponentDefinition
                        {
                            Id = "btn1",
                            Component = new Dictionary<string, object>
                            {
                                ["Button"] = new Dictionary<string, object>
                                {
                                    ["child"] = "btn1-text",
                                    ["primary"] = true,
                                    ["action"] = new Dictionary<string, object>
                                    {
                                        ["name"] = "like_action"
                                    }
                                }
                            }
                        },
                        new ComponentDefinition
                        {
                            Id = "btn1-text",
                            Component = new Dictionary<string, object>
                            {
                                ["Text"] = new Dictionary<string, object>
                                {
                                    ["text"] = new Dictionary<string, object>
                                    {
                                        ["literalString"] = "👍 喜欢"
                                    }
                                }
                            }
                        },
                        new ComponentDefinition
                        {
                            Id = "btn2",
                            Component = new Dictionary<string, object>
                            {
                                ["Button"] = new Dictionary<string, object>
                                {
                                    ["child"] = "btn2-text",
                                    ["action"] = new Dictionary<string, object>
                                    {
                                        ["name"] = "share_action"
                                    }
                                }
                            }
                        },
                        new ComponentDefinition
                        {
                            Id = "btn2-text",
                            Component = new Dictionary<string, object>
                            {
                                ["Text"] = new Dictionary<string, object>
                                {
                                    ["text"] = new Dictionary<string, object>
                                    {
                                        ["literalString"] = "🔗 分享"
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };
    }

    private List<ServerToClientMessage> GetContactListExample()
    {
        return new List<ServerToClientMessage>
        {
            new ServerToClientMessage
            {
                BeginRendering = new BeginRenderingMessage
                {
                    SurfaceId = "demo-surface",
                    Root = "root-column",
                    CatalogId = "org.a2ui.standard@0.8"
                }
            },
            new ServerToClientMessage
            {
                SurfaceUpdate = new SurfaceUpdateMessage
                {
                    SurfaceId = "demo-surface",
                    Components = new List<ComponentDefinition>
                    {
                        new ComponentDefinition
                        {
                            Id = "root-column",
                            Component = new Dictionary<string, object>
                            {
                                ["Column"] = new Dictionary<string, object>
                                {
                                    ["children"] = new Dictionary<string, object>
                                    {
                                        ["explicitList"] = new[] { "title", "list" }
                                    }
                                }
                            }
                        },
                        new ComponentDefinition
                        {
                            Id = "title",
                            Component = new Dictionary<string, object>
                            {
                                ["Text"] = new Dictionary<string, object>
                                {
                                    ["text"] = new Dictionary<string, object>
                                    {
                                        ["literalString"] = "团队联系人"
                                    },
                                    ["usageHint"] = "h1"
                                }
                            }
                        },
                        new ComponentDefinition
                        {
                            Id = "list",
                            Component = new Dictionary<string, object>
                            {
                                ["List"] = new Dictionary<string, object>
                                {
                                    ["direction"] = "vertical",
                                    ["children"] = new Dictionary<string, object>
                                    {
                                        ["template"] = new Dictionary<string, object>
                                        {
                                            ["componentId"] = "card-template",
                                            ["dataBinding"] = "/contacts"
                                        }
                                    }
                                }
                            }
                        },
                        new ComponentDefinition
                        {
                            Id = "card-template",
                            Component = new Dictionary<string, object>
                            {
                                ["Card"] = new Dictionary<string, object>
                                {
                                    ["child"] = "card-row"
                                }
                            }
                        },
                        new ComponentDefinition
                        {
                            Id = "card-row",
                            Component = new Dictionary<string, object>
                            {
                                ["Row"] = new Dictionary<string, object>
                                {
                                    ["children"] = new Dictionary<string, object>
                                    {
                                        ["explicitList"] = new[] { "card-content", "view-btn" }
                                    }
                                }
                            }
                        },
                        new ComponentDefinition
                        {
                            Id = "card-content",
                            Component = new Dictionary<string, object>
                            {
                                ["Column"] = new Dictionary<string, object>
                                {
                                    ["children"] = new Dictionary<string, object>
                                    {
                                        ["explicitList"] = new[] { "name-text", "title-text" }
                                    }
                                }
                            }
                        },
                        new ComponentDefinition
                        {
                            Id = "name-text",
                            Component = new Dictionary<string, object>
                            {
                                ["Text"] = new Dictionary<string, object>
                                {
                                    ["text"] = new Dictionary<string, object>
                                    {
                                        ["path"] = "name"
                                    },
                                    ["usageHint"] = "h3"
                                }
                            }
                        },
                        new ComponentDefinition
                        {
                            Id = "title-text",
                            Component = new Dictionary<string, object>
                            {
                                ["Text"] = new Dictionary<string, object>
                                {
                                    ["text"] = new Dictionary<string, object>
                                    {
                                        ["path"] = "title"
                                    }
                                }
                            }
                        },
                        new ComponentDefinition
                        {
                            Id = "view-btn",
                            Component = new Dictionary<string, object>
                            {
                                ["Button"] = new Dictionary<string, object>
                                {
                                    ["child"] = "view-btn-text",
                                    ["primary"] = true,
                                    ["action"] = new Dictionary<string, object>
                                    {
                                        ["name"] = "view_contact",
                                        ["context"] = new[]
                                        {
                                            new Dictionary<string, object>
                                            {
                                                ["key"] = "contactName",
                                                ["value"] = new Dictionary<string, object>
                                                {
                                                    ["path"] = "name"
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        },
                        new ComponentDefinition
                        {
                            Id = "view-btn-text",
                            Component = new Dictionary<string, object>
                            {
                                ["Text"] = new Dictionary<string, object>
                                {
                                    ["text"] = new Dictionary<string, object>
                                    {
                                        ["literalString"] = "查看"
                                    }
                                }
                            }
                        }
                    }
                }
            },
            new ServerToClientMessage
            {
                DataModelUpdate = new DataModelUpdateMessage
                {
                    SurfaceId = "demo-surface",
                    Path = "/",
                    Contents = new List<DataEntry>
                    {
                        new DataEntry
                        {
                            Key = "contacts",
                            ValueMap = new List<DataEntry>
                            {
                                new DataEntry
                                {
                                    Key = "contact1",
                                    ValueMap = new List<DataEntry>
                                    {
                                        new DataEntry { Key = "name", ValueString = "张三" },
                                        new DataEntry { Key = "title", ValueString = "高级工程师" },
                                        new DataEntry { Key = "email", ValueString = "zhangsan@example.com" }
                                    }
                                },
                                new DataEntry
                                {
                                    Key = "contact2",
                                    ValueMap = new List<DataEntry>
                                    {
                                        new DataEntry { Key = "name", ValueString = "李四" },
                                        new DataEntry { Key = "title", ValueString = "产品经理" },
                                        new DataEntry { Key = "email", ValueString = "lisi@example.com" }
                                    }
                                },
                                new DataEntry
                                {
                                    Key = "contact3",
                                    ValueMap = new List<DataEntry>
                                    {
                                        new DataEntry { Key = "name", ValueString = "王五" },
                                        new DataEntry { Key = "title", ValueString = "UI设计师" },
                                        new DataEntry { Key = "email", ValueString = "wangwu@example.com" }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };
    }

    private List<ServerToClientMessage> GetRestaurantExample()
    {
        return new List<ServerToClientMessage>
        {
            new ServerToClientMessage
            {
                BeginRendering = new BeginRenderingMessage
                {
                    SurfaceId = "demo-surface",
                    Root = "root",
                    CatalogId = "org.a2ui.standard@0.8"
                }
            },
            new ServerToClientMessage
            {
                SurfaceUpdate = new SurfaceUpdateMessage
                {
                    SurfaceId = "demo-surface",
                    Components = new List<ComponentDefinition>
                    {
                        new ComponentDefinition
                        {
                            Id = "root",
                            Component = new Dictionary<string, object>
                            {
                                ["Card"] = new Dictionary<string, object>
                                {
                                    ["child"] = "content"
                                }
                            }
                        },
                        new ComponentDefinition
                        {
                            Id = "content",
                            Component = new Dictionary<string, object>
                            {
                                ["Column"] = new Dictionary<string, object>
                                {
                                    ["children"] = new Dictionary<string, object>
                                    {
                                        ["explicitList"] = new[] { "title", "address", "button" }
                                    }
                                }
                            }
                        },
                        new ComponentDefinition
                        {
                            Id = "title",
                            Component = new Dictionary<string, object>
                            {
                                ["Text"] = new Dictionary<string, object>
                                {
                                    ["text"] = new Dictionary<string, object>
                                    {
                                        ["path"] = "name"
                                    },
                                    ["usageHint"] = "h2"
                                }
                            }
                        },
                        new ComponentDefinition
                        {
                            Id = "address",
                            Component = new Dictionary<string, object>
                            {
                                ["Text"] = new Dictionary<string, object>
                                {
                                    ["text"] = new Dictionary<string, object>
                                    {
                                        ["path"] = "address"
                                    }
                                }
                            }
                        },
                        new ComponentDefinition
                        {
                            Id = "button",
                            Component = new Dictionary<string, object>
                            {
                                ["Button"] = new Dictionary<string, object>
                                {
                                    ["child"] = "button-text",
                                    ["primary"] = true,
                                    ["action"] = new Dictionary<string, object>
                                    {
                                        ["name"] = "book_restaurant"
                                    }
                                }
                            }
                        },
                        new ComponentDefinition
                        {
                            Id = "button-text",
                            Component = new Dictionary<string, object>
                            {
                                ["Text"] = new Dictionary<string, object>
                                {
                                    ["text"] = new Dictionary<string, object>
                                    {
                                        ["literalString"] = "立即预订"
                                    }
                                }
                            }
                        }
                    }
                }
            },
            new ServerToClientMessage
            {
                DataModelUpdate = new DataModelUpdateMessage
                {
                    SurfaceId = "demo-surface",
                    Path = "/",
                    Contents = new List<DataEntry>
                    {
                        new DataEntry { Key = "name", ValueString = "意大利餐厅" },
                        new DataEntry { Key = "address", ValueString = "北京市朝阳区建国路88号" },
                        new DataEntry { Key = "rating", ValueNumber = 4.5 }
                    }
                }
            }
        };
    }

    private List<ServerToClientMessage> GetFormExample()
    {
        return new List<ServerToClientMessage>
        {
            new ServerToClientMessage
            {
                BeginRendering = new BeginRenderingMessage
                {
                    SurfaceId = "demo-surface",
                    Root = "root",
                    CatalogId = "org.a2ui.standard@0.8"
                }
            },
            new ServerToClientMessage
            {
                SurfaceUpdate = new SurfaceUpdateMessage
                {
                    SurfaceId = "demo-surface",
                    Components = new List<ComponentDefinition>
                    {
                        new ComponentDefinition
                        {
                            Id = "root",
                            Component = new Dictionary<string, object>
                            {
                                ["Card"] = new Dictionary<string, object>
                                {
                                    ["child"] = "form-content"
                                }
                            }
                        },
                        new ComponentDefinition
                        {
                            Id = "form-content",
                            Component = new Dictionary<string, object>
                            {
                                ["Column"] = new Dictionary<string, object>
                                {
                                    ["children"] = new Dictionary<string, object>
                                    {
                                        ["explicitList"] = new[] { "form-title", "name-input", "email-input", "message-input", "submit-btn" }
                                    }
                                }
                            }
                        },
                        new ComponentDefinition
                        {
                            Id = "form-title",
                            Component = new Dictionary<string, object>
                            {
                                ["Text"] = new Dictionary<string, object>
                                {
                                    ["text"] = new Dictionary<string, object>
                                    {
                                        ["literalString"] = "📝 联系表单"
                                    },
                                    ["usageHint"] = "h2"
                                }
                            }
                        },
                        new ComponentDefinition
                        {
                            Id = "name-input",
                            Component = new Dictionary<string, object>
                            {
                                ["TextInput"] = new Dictionary<string, object>
                                {
                                    ["label"] = "姓名",
                                    ["placeholder"] = "请输入您的姓名",
                                    ["dataBinding"] = "/formData/name"
                                }
                            }
                        },
                        new ComponentDefinition
                        {
                            Id = "email-input",
                            Component = new Dictionary<string, object>
                            {
                                ["TextInput"] = new Dictionary<string, object>
                                {
                                    ["label"] = "邮箱",
                                    ["placeholder"] = "请输入您的邮箱地址",
                                    ["dataBinding"] = "/formData/email"
                                }
                            }
                        },
                        new ComponentDefinition
                        {
                            Id = "message-input",
                            Component = new Dictionary<string, object>
                            {
                                ["TextInput"] = new Dictionary<string, object>
                                {
                                    ["label"] = "留言",
                                    ["placeholder"] = "请输入您的留言",
                                    ["multiline"] = true,
                                    ["dataBinding"] = "/formData/message"
                                }
                            }
                        },
                        new ComponentDefinition
                        {
                            Id = "submit-btn",
                            Component = new Dictionary<string, object>
                            {
                                ["Button"] = new Dictionary<string, object>
                                {
                                    ["child"] = "submit-text",
                                    ["primary"] = true,
                                    ["action"] = new Dictionary<string, object>
                                    {
                                        ["name"] = "submit_form"
                                    }
                                }
                            }
                        },
                        new ComponentDefinition
                        {
                            Id = "submit-text",
                            Component = new Dictionary<string, object>
                            {
                                ["Text"] = new Dictionary<string, object>
                                {
                                    ["text"] = new Dictionary<string, object>
                                    {
                                        ["literalString"] = "提交"
                                    }
                                }
                            }
                        }
                    }
                }
            },
            new ServerToClientMessage
            {
                DataModelUpdate = new DataModelUpdateMessage
                {
                    SurfaceId = "demo-surface",
                    Path = "/",
                    Contents = new List<DataEntry>
                    {
                        new DataEntry
                        {
                            Key = "formData",
                            ValueMap = new List<DataEntry>
                            {
                                new DataEntry { Key = "name", ValueString = "" },
                                new DataEntry { Key = "email", ValueString = "" },
                                new DataEntry { Key = "message", ValueString = "" }
                            }
                        }
                    }
                }
            }
        };
    }

    private List<ServerToClientMessage> GetProductListExample()
    {
        return new List<ServerToClientMessage>
        {
            new ServerToClientMessage
            {
                BeginRendering = new BeginRenderingMessage
                {
                    SurfaceId = "demo-surface",
                    Root = "root",
                    CatalogId = "org.a2ui.standard@0.8"
                }
            },
            new ServerToClientMessage
            {
                SurfaceUpdate = new SurfaceUpdateMessage
                {
                    SurfaceId = "demo-surface",
                    Components = new List<ComponentDefinition>
                    {
                        new ComponentDefinition
                        {
                            Id = "root",
                            Component = new Dictionary<string, object>
                            {
                                ["Column"] = new Dictionary<string, object>
                                {
                                    ["children"] = new Dictionary<string, object>
                                    {
                                        ["explicitList"] = new[] { "header", "product-list" }
                                    }
                                }
                            }
                        },
                        new ComponentDefinition
                        {
                            Id = "header",
                            Component = new Dictionary<string, object>
                            {
                                ["Text"] = new Dictionary<string, object>
                                {
                                    ["text"] = new Dictionary<string, object>
                                    {
                                        ["literalString"] = "🛍️ 热门商品"
                                    },
                                    ["usageHint"] = "h1"
                                }
                            }
                        },
                        new ComponentDefinition
                        {
                            Id = "product-list",
                            Component = new Dictionary<string, object>
                            {
                                ["List"] = new Dictionary<string, object>
                                {
                                    ["direction"] = "vertical",
                                    ["children"] = new Dictionary<string, object>
                                    {
                                        ["template"] = new Dictionary<string, object>
                                        {
                                            ["componentId"] = "product-card",
                                            ["dataBinding"] = "/products"
                                        }
                                    }
                                }
                            }
                        },
                        new ComponentDefinition
                        {
                            Id = "product-card",
                            Component = new Dictionary<string, object>
                            {
                                ["Card"] = new Dictionary<string, object>
                                {
                                    ["child"] = "product-layout"
                                }
                            }
                        },
                        new ComponentDefinition
                        {
                            Id = "product-layout",
                            Component = new Dictionary<string, object>
                            {
                                ["Row"] = new Dictionary<string, object>
                                {
                                    ["children"] = new Dictionary<string, object>
                                    {
                                        ["explicitList"] = new[] { "product-info", "buy-btn" }
                                    }
                                }
                            }
                        },
                        new ComponentDefinition
                        {
                            Id = "product-info",
                            Component = new Dictionary<string, object>
                            {
                                ["Column"] = new Dictionary<string, object>
                                {
                                    ["children"] = new Dictionary<string, object>
                                    {
                                        ["explicitList"] = new[] { "product-name", "product-price", "product-desc" }
                                    }
                                }
                            }
                        },
                        new ComponentDefinition
                        {
                            Id = "product-name",
                            Component = new Dictionary<string, object>
                            {
                                ["Text"] = new Dictionary<string, object>
                                {
                                    ["text"] = new Dictionary<string, object>
                                    {
                                        ["path"] = "name"
                                    },
                                    ["usageHint"] = "h3"
                                }
                            }
                        },
                        new ComponentDefinition
                        {
                            Id = "product-price",
                            Component = new Dictionary<string, object>
                            {
                                ["Text"] = new Dictionary<string, object>
                                {
                                    ["text"] = new Dictionary<string, object>
                                    {
                                        ["path"] = "price"
                                    }
                                }
                            }
                        },
                        new ComponentDefinition
                        {
                            Id = "product-desc",
                            Component = new Dictionary<string, object>
                            {
                                ["Text"] = new Dictionary<string, object>
                                {
                                    ["text"] = new Dictionary<string, object>
                                    {
                                        ["path"] = "description"
                                    }
                                }
                            }
                        },
                        new ComponentDefinition
                        {
                            Id = "buy-btn",
                            Component = new Dictionary<string, object>
                            {
                                ["Button"] = new Dictionary<string, object>
                                {
                                    ["child"] = "buy-text",
                                    ["primary"] = true,
                                    ["action"] = new Dictionary<string, object>
                                    {
                                        ["name"] = "buy_product",
                                        ["context"] = new[]
                                        {
                                            new Dictionary<string, object>
                                            {
                                                ["key"] = "productId",
                                                ["value"] = new Dictionary<string, object>
                                                {
                                                    ["path"] = "id"
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        },
                        new ComponentDefinition
                        {
                            Id = "buy-text",
                            Component = new Dictionary<string, object>
                            {
                                ["Text"] = new Dictionary<string, object>
                                {
                                    ["text"] = new Dictionary<string, object>
                                    {
                                        ["literalString"] = "购买"
                                    }
                                }
                            }
                        }
                    }
                }
            },
            new ServerToClientMessage
            {
                DataModelUpdate = new DataModelUpdateMessage
                {
                    SurfaceId = "demo-surface",
                    Path = "/",
                    Contents = new List<DataEntry>
                    {
                        new DataEntry
                        {
                            Key = "products",
                            ValueMap = new List<DataEntry>
                            {
                                new DataEntry
                                {
                                    Key = "prod1",
                                    ValueMap = new List<DataEntry>
                                    {
                                        new DataEntry { Key = "id", ValueString = "prod1" },
                                        new DataEntry { Key = "name", ValueString = "无线蓝牙耳机" },
                                        new DataEntry { Key = "price", ValueString = "¥299" },
                                        new DataEntry { Key = "description", ValueString = "高品质音效,长续航" }
                                    }
                                },
                                new DataEntry
                                {
                                    Key = "prod2",
                                    ValueMap = new List<DataEntry>
                                    {
                                        new DataEntry { Key = "id", ValueString = "prod2" },
                                        new DataEntry { Key = "name", ValueString = "智能手环" },
                                        new DataEntry { Key = "price", ValueString = "¥199" },
                                        new DataEntry { Key = "description", ValueString = "健康监测,运动追踪" }
                                    }
                                },
                                new DataEntry
                                {
                                    Key = "prod3",
                                    ValueMap = new List<DataEntry>
                                    {
                                        new DataEntry { Key = "id", ValueString = "prod3" },
                                        new DataEntry { Key = "name", ValueString = "便携充电宝" },
                                        new DataEntry { Key = "price", ValueString = "¥129" },
                                        new DataEntry { Key = "description", ValueString = "20000mAh大容量" }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };
    }

    private List<ServerToClientMessage> GetDashboardExample()
    {
        return new List<ServerToClientMessage>
        {
            new ServerToClientMessage
            {
                BeginRendering = new BeginRenderingMessage
                {
                    SurfaceId = "demo-surface",
                    Root = "root",
                    CatalogId = "org.a2ui.standard@0.8"
                }
            },
            new ServerToClientMessage
            {
                SurfaceUpdate = new SurfaceUpdateMessage
                {
                    SurfaceId = "demo-surface",
                    Components = new List<ComponentDefinition>
                    {
                        new ComponentDefinition
                        {
                            Id = "root",
                            Component = new Dictionary<string, object>
                            {
                                ["Column"] = new Dictionary<string, object>
                                {
                                    ["children"] = new Dictionary<string, object>
                                    {
                                        ["explicitList"] = new[] { "dashboard-title", "stats-row" }
                                    }
                                }
                            }
                        },
                        new ComponentDefinition
                        {
                            Id = "dashboard-title",
                            Component = new Dictionary<string, object>
                            {
                                ["Text"] = new Dictionary<string, object>
                                {
                                    ["text"] = new Dictionary<string, object>
                                    {
                                        ["literalString"] = "📊 数据统计"
                                    },
                                    ["usageHint"] = "h1"
                                }
                            }
                        },
                        new ComponentDefinition
                        {
                            Id = "stats-row",
                            Component = new Dictionary<string, object>
                            {
                                ["Row"] = new Dictionary<string, object>
                                {
                                    ["children"] = new Dictionary<string, object>
                                    {
                                        ["explicitList"] = new[] { "stat-card1", "stat-card2", "stat-card3" }
                                    }
                                }
                            }
                        },
                        new ComponentDefinition
                        {
                            Id = "stat-card1",
                            Component = new Dictionary<string, object>
                            {
                                ["Card"] = new Dictionary<string, object>
                                {
                                    ["child"] = "stat-content1"
                                }
                            }
                        },
                        new ComponentDefinition
                        {
                            Id = "stat-content1",
                            Component = new Dictionary<string, object>
                            {
                                ["Column"] = new Dictionary<string, object>
                                {
                                    ["children"] = new Dictionary<string, object>
                                    {
                                        ["explicitList"] = new[] { "stat1-label", "stat1-value" }
                                    }
                                }
                            }
                        },
                        new ComponentDefinition
                        {
                            Id = "stat1-label",
                            Component = new Dictionary<string, object>
                            {
                                ["Text"] = new Dictionary<string, object>
                                {
                                    ["text"] = new Dictionary<string, object>
                                    {
                                        ["literalString"] = "总用户数"
                                    }
                                }
                            }
                        },
                        new ComponentDefinition
                        {
                            Id = "stat1-value",
                            Component = new Dictionary<string, object>
                            {
                                ["Text"] = new Dictionary<string, object>
                                {
                                    ["text"] = new Dictionary<string, object>
                                    {
                                        ["path"] = "stats/totalUsers"
                                    },
                                    ["usageHint"] = "h2"
                                }
                            }
                        },
                        new ComponentDefinition
                        {
                            Id = "stat-card2",
                            Component = new Dictionary<string, object>
                            {
                                ["Card"] = new Dictionary<string, object>
                                {
                                    ["child"] = "stat-content2"
                                }
                            }
                        },
                        new ComponentDefinition
                        {
                            Id = "stat-content2",
                            Component = new Dictionary<string, object>
                            {
                                ["Column"] = new Dictionary<string, object>
                                {
                                    ["children"] = new Dictionary<string, object>
                                    {
                                        ["explicitList"] = new[] { "stat2-label", "stat2-value" }
                                    }
                                }
                            }
                        },
                        new ComponentDefinition
                        {
                            Id = "stat2-label",
                            Component = new Dictionary<string, object>
                            {
                                ["Text"] = new Dictionary<string, object>
                                {
                                    ["text"] = new Dictionary<string, object>
                                    {
                                        ["literalString"] = "活跃用户"
                                    }
                                }
                            }
                        },
                        new ComponentDefinition
                        {
                            Id = "stat2-value",
                            Component = new Dictionary<string, object>
                            {
                                ["Text"] = new Dictionary<string, object>
                                {
                                    ["text"] = new Dictionary<string, object>
                                    {
                                        ["path"] = "stats/activeUsers"
                                    },
                                    ["usageHint"] = "h2"
                                }
                            }
                        },
                        new ComponentDefinition
                        {
                            Id = "stat-card3",
                            Component = new Dictionary<string, object>
                            {
                                ["Card"] = new Dictionary<string, object>
                                {
                                    ["child"] = "stat-content3"
                                }
                            }
                        },
                        new ComponentDefinition
                        {
                            Id = "stat-content3",
                            Component = new Dictionary<string, object>
                            {
                                ["Column"] = new Dictionary<string, object>
                                {
                                    ["children"] = new Dictionary<string, object>
                                    {
                                        ["explicitList"] = new[] { "stat3-label", "stat3-value" }
                                    }
                                }
                            }
                        },
                        new ComponentDefinition
                        {
                            Id = "stat3-label",
                            Component = new Dictionary<string, object>
                            {
                                ["Text"] = new Dictionary<string, object>
                                {
                                    ["text"] = new Dictionary<string, object>
                                    {
                                        ["literalString"] = "今日访问"
                                    }
                                }
                            }
                        },
                        new ComponentDefinition
                        {
                            Id = "stat3-value",
                            Component = new Dictionary<string, object>
                            {
                                ["Text"] = new Dictionary<string, object>
                                {
                                    ["text"] = new Dictionary<string, object>
                                    {
                                        ["path"] = "stats/todayVisits"
                                    },
                                    ["usageHint"] = "h2"
                                }
                            }
                        }
                    }
                }
            },
            new ServerToClientMessage
            {
                DataModelUpdate = new DataModelUpdateMessage
                {
                    SurfaceId = "demo-surface",
                    Path = "/",
                    Contents = new List<DataEntry>
                    {
                        new DataEntry
                        {
                            Key = "stats",
                            ValueMap = new List<DataEntry>
                            {
                                new DataEntry { Key = "totalUsers", ValueString = "12,548" },
                                new DataEntry { Key = "activeUsers", ValueString = "8,234" },
                                new DataEntry { Key = "todayVisits", ValueString = "1,890" }
                            }
                        }
                    }
                }
            }
        };
    }

    private List<ServerToClientMessage> GetNotificationExample()
    {
        return new List<ServerToClientMessage>
        {
            new ServerToClientMessage
            {
                BeginRendering = new BeginRenderingMessage
                {
                    SurfaceId = "demo-surface",
                    Root = "root",
                    CatalogId = "org.a2ui.standard@0.8"
                }
            },
            new ServerToClientMessage
            {
                SurfaceUpdate = new SurfaceUpdateMessage
                {
                    SurfaceId = "demo-surface",
                    Components = new List<ComponentDefinition>
                    {
                        new ComponentDefinition
                        {
                            Id = "root",
                            Component = new Dictionary<string, object>
                            {
                                ["Column"] = new Dictionary<string, object>
                                {
                                    ["children"] = new Dictionary<string, object>
                                    {
                                        ["explicitList"] = new[] { "notif-title", "notif-list" }
                                    }
                                }
                            }
                        },
                        new ComponentDefinition
                        {
                            Id = "notif-title",
                            Component = new Dictionary<string, object>
                            {
                                ["Text"] = new Dictionary<string, object>
                                {
                                    ["text"] = new Dictionary<string, object>
                                    {
                                        ["literalString"] = "🔔 通知中心"
                                    },
                                    ["usageHint"] = "h1"
                                }
                            }
                        },
                        new ComponentDefinition
                        {
                            Id = "notif-list",
                            Component = new Dictionary<string, object>
                            {
                                ["List"] = new Dictionary<string, object>
                                {
                                    ["direction"] = "vertical",
                                    ["children"] = new Dictionary<string, object>
                                    {
                                        ["template"] = new Dictionary<string, object>
                                        {
                                            ["componentId"] = "notif-card",
                                            ["dataBinding"] = "/notifications"
                                        }
                                    }
                                }
                            }
                        },
                        new ComponentDefinition
                        {
                            Id = "notif-card",
                            Component = new Dictionary<string, object>
                            {
                                ["Card"] = new Dictionary<string, object>
                                {
                                    ["child"] = "notif-content"
                                }
                            }
                        },
                        new ComponentDefinition
                        {
                            Id = "notif-content",
                            Component = new Dictionary<string, object>
                            {
                                ["Column"] = new Dictionary<string, object>
                                {
                                    ["children"] = new Dictionary<string, object>
                                    {
                                        ["explicitList"] = new[] { "notif-header", "notif-message", "notif-time" }
                                    }
                                }
                            }
                        },
                        new ComponentDefinition
                        {
                            Id = "notif-header",
                            Component = new Dictionary<string, object>
                            {
                                ["Text"] = new Dictionary<string, object>
                                {
                                    ["text"] = new Dictionary<string, object>
                                    {
                                        ["path"] = "title"
                                    },
                                    ["usageHint"] = "h3"
                                }
                            }
                        },
                        new ComponentDefinition
                        {
                            Id = "notif-message",
                            Component = new Dictionary<string, object>
                            {
                                ["Text"] = new Dictionary<string, object>
                                {
                                    ["text"] = new Dictionary<string, object>
                                    {
                                        ["path"] = "message"
                                    }
                                }
                            }
                        },
                        new ComponentDefinition
                        {
                            Id = "notif-time",
                            Component = new Dictionary<string, object>
                            {
                                ["Text"] = new Dictionary<string, object>
                                {
                                    ["text"] = new Dictionary<string, object>
                                    {
                                        ["path"] = "time"
                                    }
                                }
                            }
                        }
                    }
                }
            },
            new ServerToClientMessage
            {
                DataModelUpdate = new DataModelUpdateMessage
                {
                    SurfaceId = "demo-surface",
                    Path = "/",
                    Contents = new List<DataEntry>
                    {
                        new DataEntry
                        {
                            Key = "notifications",
                            ValueMap = new List<DataEntry>
                            {
                                new DataEntry
                                {
                                    Key = "notif1",
                                    ValueMap = new List<DataEntry>
                                    {
                                        new DataEntry { Key = "title", ValueString = "✅ 系统更新" },
                                        new DataEntry { Key = "message", ValueString = "系统已成功更新到最新版本" },
                                        new DataEntry { Key = "time", ValueString = "5分钟前" }
                                    }
                                },
                                new DataEntry
                                {
                                    Key = "notif2",
                                    ValueMap = new List<DataEntry>
                                    {
                                        new DataEntry { Key = "title", ValueString = "📧 新消息" },
                                        new DataEntry { Key = "message", ValueString = "您收到了来自管理员的新消息" },
                                        new DataEntry { Key = "time", ValueString = "1小时前" }
                                    }
                                },
                                new DataEntry
                                {
                                    Key = "notif3",
                                    ValueMap = new List<DataEntry>
                                    {
                                        new DataEntry { Key = "title", ValueString = "⚠️ 安全提醒" },
                                        new DataEntry { Key = "message", ValueString = "检测到您的账号在新设备登录" },
                                        new DataEntry { Key = "time", ValueString = "2小时前" }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };
    }

    private List<ServerToClientMessage> GetUserProfileExample()
    {
        return new List<ServerToClientMessage>
        {
            new ServerToClientMessage
            {
                BeginRendering = new BeginRenderingMessage
                {
                    SurfaceId = "demo-surface",
                    Root = "root",
                    CatalogId = "org.a2ui.standard@0.8"
                }
            },
            new ServerToClientMessage
            {
                SurfaceUpdate = new SurfaceUpdateMessage
                {
                    SurfaceId = "demo-surface",
                    Components = new List<ComponentDefinition>
                    {
                        new ComponentDefinition
                        {
                            Id = "root",
                            Component = new Dictionary<string, object>
                            {
                                ["Card"] = new Dictionary<string, object>
                                {
                                    ["child"] = "profile-content"
                                }
                            }
                        },
                        new ComponentDefinition
                        {
                            Id = "profile-content",
                            Component = new Dictionary<string, object>
                            {
                                ["Column"] = new Dictionary<string, object>
                                {
                                    ["children"] = new Dictionary<string, object>
                                    {
                                        ["explicitList"] = new[] { "profile-header", "profile-info", "profile-stats", "action-buttons" }
                                    }
                                }
                            }
                        },
                        new ComponentDefinition
                        {
                            Id = "profile-header",
                            Component = new Dictionary<string, object>
                            {
                                ["Text"] = new Dictionary<string, object>
                                {
                                    ["text"] = new Dictionary<string, object>
                                    {
                                        ["literalString"] = "👤 用户资料"
                                    },
                                    ["usageHint"] = "h1"
                                }
                            }
                        },
                        new ComponentDefinition
                        {
                            Id = "profile-info",
                            Component = new Dictionary<string, object>
                            {
                                ["Column"] = new Dictionary<string, object>
                                {
                                    ["children"] = new Dictionary<string, object>
                                    {
                                        ["explicitList"] = new[] { "user-name", "user-email", "user-role", "user-bio" }
                                    }
                                }
                            }
                        },
                        new ComponentDefinition
                        {
                            Id = "user-name",
                            Component = new Dictionary<string, object>
                            {
                                ["Text"] = new Dictionary<string, object>
                                {
                                    ["text"] = new Dictionary<string, object>
                                    {
                                        ["path"] = "user/name"
                                    },
                                    ["usageHint"] = "h2"
                                }
                            }
                        },
                        new ComponentDefinition
                        {
                            Id = "user-email",
                            Component = new Dictionary<string, object>
                            {
                                ["Text"] = new Dictionary<string, object>
                                {
                                    ["text"] = new Dictionary<string, object>
                                    {
                                        ["path"] = "user/email"
                                    }
                                }
                            }
                        },
                        new ComponentDefinition
                        {
                            Id = "user-role",
                            Component = new Dictionary<string, object>
                            {
                                ["Text"] = new Dictionary<string, object>
                                {
                                    ["text"] = new Dictionary<string, object>
                                    {
                                        ["path"] = "user/role"
                                    }
                                }
                            }
                        },
                        new ComponentDefinition
                        {
                            Id = "user-bio",
                            Component = new Dictionary<string, object>
                            {
                                ["Text"] = new Dictionary<string, object>
                                {
                                    ["text"] = new Dictionary<string, object>
                                    {
                                        ["path"] = "user/bio"
                                    }
                                }
                            }
                        },
                        new ComponentDefinition
                        {
                            Id = "profile-stats",
                            Component = new Dictionary<string, object>
                            {
                                ["Row"] = new Dictionary<string, object>
                                {
                                    ["children"] = new Dictionary<string, object>
                                    {
                                        ["explicitList"] = new[] { "stat-followers", "stat-following" }
                                    }
                                }
                            }
                        },
                        new ComponentDefinition
                        {
                            Id = "stat-followers",
                            Component = new Dictionary<string, object>
                            {
                                ["Column"] = new Dictionary<string, object>
                                {
                                    ["children"] = new Dictionary<string, object>
                                    {
                                        ["explicitList"] = new[] { "followers-count", "followers-label" }
                                    }
                                }
                            }
                        },
                        new ComponentDefinition
                        {
                            Id = "followers-count",
                            Component = new Dictionary<string, object>
                            {
                                ["Text"] = new Dictionary<string, object>
                                {
                                    ["text"] = new Dictionary<string, object>
                                    {
                                        ["path"] = "user/followers"
                                    },
                                    ["usageHint"] = "h3"
                                }
                            }
                        },
                        new ComponentDefinition
                        {
                            Id = "followers-label",
                            Component = new Dictionary<string, object>
                            {
                                ["Text"] = new Dictionary<string, object>
                                {
                                    ["text"] = new Dictionary<string, object>
                                    {
                                        ["literalString"] = "关注者"
                                    }
                                }
                            }
                        },
                        new ComponentDefinition
                        {
                            Id = "stat-following",
                            Component = new Dictionary<string, object>
                            {
                                ["Column"] = new Dictionary<string, object>
                                {
                                    ["children"] = new Dictionary<string, object>
                                    {
                                        ["explicitList"] = new[] { "following-count", "following-label" }
                                    }
                                }
                            }
                        },
                        new ComponentDefinition
                        {
                            Id = "following-count",
                            Component = new Dictionary<string, object>
                            {
                                ["Text"] = new Dictionary<string, object>
                                {
                                    ["text"] = new Dictionary<string, object>
                                    {
                                        ["path"] = "user/following"
                                    },
                                    ["usageHint"] = "h3"
                                }
                            }
                        },
                        new ComponentDefinition
                        {
                            Id = "following-label",
                            Component = new Dictionary<string, object>
                            {
                                ["Text"] = new Dictionary<string, object>
                                {
                                    ["text"] = new Dictionary<string, object>
                                    {
                                        ["literalString"] = "正在关注"
                                    }
                                }
                            }
                        },
                        new ComponentDefinition
                        {
                            Id = "action-buttons",
                            Component = new Dictionary<string, object>
                            {
                                ["Row"] = new Dictionary<string, object>
                                {
                                    ["children"] = new Dictionary<string, object>
                                    {
                                        ["explicitList"] = new[] { "edit-btn", "settings-btn" }
                                    }
                                }
                            }
                        },
                        new ComponentDefinition
                        {
                            Id = "edit-btn",
                            Component = new Dictionary<string, object>
                            {
                                ["Button"] = new Dictionary<string, object>
                                {
                                    ["child"] = "edit-text",
                                    ["primary"] = true,
                                    ["action"] = new Dictionary<string, object>
                                    {
                                        ["name"] = "edit_profile"
                                    }
                                }
                            }
                        },
                        new ComponentDefinition
                        {
                            Id = "edit-text",
                            Component = new Dictionary<string, object>
                            {
                                ["Text"] = new Dictionary<string, object>
                                {
                                    ["text"] = new Dictionary<string, object>
                                    {
                                        ["literalString"] = "编辑资料"
                                    }
                                }
                            }
                        },
                        new ComponentDefinition
                        {
                            Id = "settings-btn",
                            Component = new Dictionary<string, object>
                            {
                                ["Button"] = new Dictionary<string, object>
                                {
                                    ["child"] = "settings-text",
                                    ["action"] = new Dictionary<string, object>
                                    {
                                        ["name"] = "open_settings"
                                    }
                                }
                            }
                        },
                        new ComponentDefinition
                        {
                            Id = "settings-text",
                            Component = new Dictionary<string, object>
                            {
                                ["Text"] = new Dictionary<string, object>
                                {
                                    ["text"] = new Dictionary<string, object>
                                    {
                                        ["literalString"] = "设置"
                                    }
                                }
                            }
                        }
                    }
                }
            },
            new ServerToClientMessage
            {
                DataModelUpdate = new DataModelUpdateMessage
                {
                    SurfaceId = "demo-surface",
                    Path = "/",
                    Contents = new List<DataEntry>
                    {
                        new DataEntry
                        {
                            Key = "user",
                            ValueMap = new List<DataEntry>
                            {
                                new DataEntry { Key = "name", ValueString = "许泽宇" },
                                new DataEntry { Key = "email", ValueString = "xuzeyu91@gmail.com" },
                                new DataEntry { Key = "role", ValueString = "架构师" },
                                new DataEntry { Key = "bio", ValueString = "热爱技术,专注于全栈开发和AI应用" },
                                new DataEntry { Key = "followers", ValueString = "1,234" },
                                new DataEntry { Key = "following", ValueString = "567" }
                            }
                        }
                    }
                }
            }
        };
    }
}

