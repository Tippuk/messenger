using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace ChatClient;

/// <summary>
/// Класс, который отвечает только за сетевое подключение клиента.
/// Форма подписывается на события и вызывает методы Connect/Send.
/// </summary>
public class ChatClientConnection : IDisposable
{
    private TcpClient? _tcpClient;
    private StreamReader? _reader;
    private StreamWriter? _writer;
    private CancellationTokenSource? _cts;
    private Task? _receiveTask;

    public string UserName { get; private set; } = string.Empty;

    public bool IsConnected => _tcpClient != null && _tcpClient.Connected;

    public event Action<Message>? MessageReceived;
    public event Action? Disconnected;

    public async Task ConnectAsync(string ip, int port, string userName)
    {
        if (IsConnected)
            return;

        UserName = userName.Trim();

        _tcpClient = new TcpClient();
        await _tcpClient.ConnectAsync(ip, port).ConfigureAwait(false);

        var stream = _tcpClient.GetStream();
        _reader = new StreamReader(stream, Encoding.UTF8, leaveOpen: true);
        _writer = new StreamWriter(stream, Encoding.UTF8, bufferSize: 1024, leaveOpen: true)
        {
            AutoFlush = true
        };

        _cts = new CancellationTokenSource();

        // Первое сообщение — представляемся серверу.
        var hello = new Message
        {
            From = UserName,
            Text = "hello",
            Type = MessageType.System,
            Timestamp = DateTime.Now
        };

        await SendRawAsync(hello).ConfigureAwait(false);

        _receiveTask = Task.Run(() => ReceiveLoopAsync(_cts.Token));
    }

    public async Task SendChatAsync(string text, string? toUser)
    {
        if (!IsConnected || _writer == null)
            return;

        var msg = new Message
        {
            From = UserName,
            To = string.IsNullOrWhiteSpace(toUser) ? null : toUser,
            Text = text,
            Type = MessageType.Chat,
            Timestamp = DateTime.Now
        };

        await SendRawAsync(msg).ConfigureAwait(false);
    }

    private async Task SendRawAsync(Message message)
    {
        if (_writer == null)
            return;

        try
        {
            var json = JsonSerializer.Serialize(message);
            await _writer.WriteLineAsync(json).ConfigureAwait(false);
        }
        catch
        {
            // Если отправка сломалась, будем считать, что соединение разорвано.
            DisconnectInternal();
        }
    }

    private async Task ReceiveLoopAsync(CancellationToken token)
    {
        if (_reader == null)
            return;

        try
        {
            while (!token.IsCancellationRequested)
            {
                var line = await _reader.ReadLineAsync(token).ConfigureAwait(false);
                if (line == null)
                    break;

                Message? msg;
                try
                {
                    msg = JsonSerializer.Deserialize<Message>(line);
                }
                catch
                {
                    continue;
                }

                if (msg != null)
                {
                    MessageReceived?.Invoke(msg);
                }
            }
        }
        catch
        {
            // Ошибки чтения считаем разрывом соединения.
        }
        finally
        {
            DisconnectInternal();
        }
    }

    private void DisconnectInternal()
    {
        if (_cts != null && !_cts.IsCancellationRequested)
        {
            try
            {
                _cts.Cancel();
            }
            catch
            {
                // ignore
            }
        }

        try
        {
            _writer?.Dispose();
            _reader?.Dispose();
            _tcpClient?.Close();
        }
        catch
        {
            // ignore
        }

        _writer = null;
        _reader = null;
        _tcpClient = null;

        Disconnected?.Invoke();
    }

    public void Dispose()
    {
        DisconnectInternal();
        _cts?.Dispose();
    }
}

