using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace ChatServer;

/// <summary>
/// Класс, который инкапсулирует всю сетевую логику сервера.
/// UI (форма) только подписывается на события и вызывает методы Start/Stop.
/// </summary>
public class ChatServerInstance
{
    private readonly object _syncRoot = new();
    private readonly List<ClientConnection> _clients = new();

    private TcpListener? _listener;
    private CancellationTokenSource? _cts;

    public bool IsRunning { get; private set; }

    /// <summary>
    /// Событие для логирования. Удобно выводить в текстовое поле на форме.
    /// </summary>
    public event Action<string>? Log;

    /// <summary>
    /// Событие, которое вызывается, когда приходит новое сообщение.
    /// </summary>
    public event Action<Message>? MessageReceived;

    /// <summary>
    /// Событие при изменении списка пользователей.
    /// </summary>
    public event Action<IReadOnlyList<string>>? UsersChanged;

    public async Task StartAsync(IPAddress ip, int port)
    {
        if (IsRunning)
            return;

        _cts = new CancellationTokenSource();
        _listener = new TcpListener(ip, port);
        _listener.Start();
        IsRunning = true;

        Log?.Invoke($"Сервер запущен на {ip}:{port}");

        try
        {
            while (!_cts.IsCancellationRequested)
            {
                var tcpClient = await _listener.AcceptTcpClientAsync(_cts.Token).ConfigureAwait(false);
                _ = HandleClientAsync(tcpClient, _cts.Token);
            }
        }
        catch (OperationCanceledException)
        {
            // Нормальное завершение.
        }
        catch (Exception ex)
        {
            Log?.Invoke($"Ошибка сервера: {ex.Message}");
        }
        finally
        {
            StopInternal();
        }
    }

    public void Stop()
    {
        if (!IsRunning)
            return;

        _cts?.Cancel();
        StopInternal();
        Log?.Invoke("Сервер остановлен");
    }

    private void StopInternal()
    {
        IsRunning = false;

        lock (_syncRoot)
        {
            foreach (var client in _clients.ToList())
            {
                client.Dispose();
            }
            _clients.Clear();
        }

        _listener?.Stop();
        _listener = null;
    }

    private async Task HandleClientAsync(TcpClient tcpClient, CancellationToken token)
    {
        var connection = new ClientConnection(tcpClient);

        lock (_syncRoot)
        {
            _clients.Add(connection);
        }

        Log?.Invoke("Новый клиент подключился, ждём имя пользователя...");

        try
        {
            // Первая полученная от клиента строка — служебное сообщение с именем пользователя.
            var firstLine = await connection.ReadLineAsync(token).ConfigureAwait(false);
            if (firstLine == null)
            {
                RemoveClient(connection, notifyOthers: false);
                return;
            }

            var hello = JsonSerializer.Deserialize<Message>(firstLine);
            if (hello == null || string.IsNullOrWhiteSpace(hello.From))
            {
                RemoveClient(connection, notifyOthers: false);
                return;
            }

            connection.UserName = hello.From.Trim();

            Log?.Invoke($"Пользователь {connection.UserName} присоединился.");

            BroadcastSystemMessage($"{connection.UserName} вошёл в чат.");
            RaiseUsersChanged();

            // Основной цикл чтения сообщений.
            while (!token.IsCancellationRequested)
            {
                var line = await connection.ReadLineAsync(token).ConfigureAwait(false);
                if (line == null)
                {
                    // Клиент отключился.
                    break;
                }

                Message? message;
                try
                {
                    message = JsonSerializer.Deserialize<Message>(line);
                }
                catch (Exception ex)
                {
                    Log?.Invoke($"Ошибка парсинга сообщения: {ex.Message}");
                    continue;
                }

                if (message == null)
                    continue;

                // Проставляем серверное время.
                message.Timestamp = DateTime.Now;

                HandleIncomingMessage(connection, message);
            }
        }
        catch (OperationCanceledException)
        {
            // Сервер останавливается.
        }
        catch (Exception ex)
        {
            Log?.Invoke($"Ошибка клиента {connection.UserName ?? "?"}: {ex.Message}");
        }
        finally
        {
            RemoveClient(connection, notifyOthers: true);
        }
    }

    private void HandleIncomingMessage(ClientConnection sender, Message message)
    {
        MessageReceived?.Invoke(message);

        if (message.Type == MessageType.Chat)
        {
            if (string.IsNullOrWhiteSpace(message.To))
            {
                // Обычное сообщение для всех.
                Broadcast(message);
            }
            else
            {
                // Личное сообщение.
                SendPrivate(message);
            }
        }
    }

    private void Broadcast(Message message)
    {
        string json = JsonSerializer.Serialize(message);

        lock (_syncRoot)
        {
            foreach (var client in _clients.ToList())
            {
                client.TrySendLine(json);
            }
        }
    }

    private void SendPrivate(Message message)
    {
        if (string.IsNullOrWhiteSpace(message.To))
        {
            Broadcast(message);
            return;
        }

        string json = JsonSerializer.Serialize(message);

        ClientConnection? target = null;
        ClientConnection? sender = null;

        lock (_syncRoot)
        {
            target = _clients.FirstOrDefault(c =>
                string.Equals(c.UserName, message.To, StringComparison.OrdinalIgnoreCase));

            sender = _clients.FirstOrDefault(c =>
                string.Equals(c.UserName, message.From, StringComparison.OrdinalIgnoreCase));
        }

        // Отправляем получателю.
        target?.TrySendLine(json);

        // А также отправителю, чтобы он видел своё сообщение.
        if (sender != null && !ReferenceEquals(sender, target))
        {
            sender.TrySendLine(json);
        }
    }

    private void BroadcastSystemMessage(string text)
    {
        var msg = new Message
        {
            From = "Система",
            Text = text,
            Timestamp = DateTime.Now,
            Type = MessageType.System
        };

        Broadcast(msg);
    }

    private void RaiseUsersChanged()
    {
        List<string> users;

        lock (_syncRoot)
        {
            users = _clients
                .Where(c => !string.IsNullOrWhiteSpace(c.UserName))
                .Select(c => c.UserName!)
                .OrderBy(n => n)
                .ToList();
        }

        UsersChanged?.Invoke(users);

        // Дополнительно отправляем список пользователей всем клиентам.
        var msg = new Message
        {
            Type = MessageType.UserList,
            Users = users,
            Timestamp = DateTime.Now,
            From = "Система"
        };

        Broadcast(msg);
    }

    private void RemoveClient(ClientConnection connection, bool notifyOthers)
    {
        bool removed;
        string? userName;

        lock (_syncRoot)
        {
            removed = _clients.Remove(connection);
            userName = connection.UserName;
        }

        connection.Dispose();

        if (!removed)
            return;

        if (!string.IsNullOrWhiteSpace(userName) && notifyOthers)
        {
            Log?.Invoke($"Пользователь {userName} отключился.");
            BroadcastSystemMessage($"{userName} покинул чат.");
        }

        RaiseUsersChanged();
    }
}

/// <summary>
/// Обёртка вокруг TcpClient, чтобы хранить имя пользователя и упрощённые методы чтения/записи.
/// </summary>
public sealed class ClientConnection : IDisposable
{
    private readonly TcpClient _tcpClient;
    private readonly StreamReader _reader;
    private readonly StreamWriter _writer;

    private bool _disposed;

    public string? UserName { get; set; }

    public ClientConnection(TcpClient tcpClient)
    {
        _tcpClient = tcpClient;
        var stream = tcpClient.GetStream();

        _reader = new StreamReader(stream, Encoding.UTF8, leaveOpen: true);
        _writer = new StreamWriter(stream, Encoding.UTF8, bufferSize: 1024, leaveOpen: true)
        {
            AutoFlush = true
        };
    }

    public async Task<string?> ReadLineAsync(CancellationToken token)
    {
        try
        {
            return await _reader.ReadLineAsync(token).ConfigureAwait(false);
        }
        catch
        {
            return null;
        }
    }

    public void TrySendLine(string line)
    {
        if (_disposed) return;

        try
        {
            _writer.WriteLine(line);
        }
        catch
        {
            // Игнорируем ошибки отправки: клиент, скорее всего, уже отключился.
        }
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        try
        {
            _writer.Dispose();
            _reader.Dispose();
            _tcpClient.Close();
        }
        catch
        {
            // Игнорируем ошибки при закрытии.
        }
    }
}

