using System;

namespace ChatClient;

/// <summary>
/// То же представление сообщения, что и на сервере.
/// Важно, чтобы набор полей совпадал, иначе сериализация/десериализация JSON сломается.
/// </summary>
public class Message
{
    public string From { get; set; } = string.Empty;
    public string? To { get; set; }
    public string Text { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public MessageType Type { get; set; }
    public List<string>? Users { get; set; }
}

public enum MessageType
{
    Chat,
    System,
    UserList
}

