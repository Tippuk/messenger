using System;

namespace ChatServer;

/// <summary>
/// Простое представление сообщения в чате.
/// Этот же класс (с теми же полями) будет использоваться и на клиенте.
/// </summary>
public class Message
{
    /// <summary>
    /// От кого сообщение (имя пользователя).
    /// </summary>
    public string From { get; set; } = string.Empty;

    /// <summary>
    /// Кому сообщение. null или пустая строка — отправить всем.
    /// </summary>
    public string? To { get; set; }

    /// <summary>
    /// Текст сообщения.
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Временная метка на стороне сервера.
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Тип сообщения: обычное, системное, служебное.
    /// </summary>
    public MessageType Type { get; set; }

    /// <summary>
    /// Список пользователей. Используется, когда сервер рассылает обновлённый список клиентов.
    /// </summary>
    public List<string>? Users { get; set; }
}

public enum MessageType
{
    Chat,
    System,
    UserList
}

