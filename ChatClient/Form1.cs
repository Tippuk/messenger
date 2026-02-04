using System.Text.Json;

namespace ChatClient;

public partial class Form1 : Form
{
    private ChatClientConnection? _connection;
    private readonly List<Message> _history = new();
    private readonly string _historyFilePath;

    public Form1()
    {
        InitializeComponent();

        txtIp.Text = "127.0.0.1";
        txtPort.Text = "9000";

        _historyFilePath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "chat-log.json");

        LoadHistory();
    }

    private async void btnConnect_Click(object sender, EventArgs e)
    {
        if (_connection != null && _connection.IsConnected)
            return;

        var ip = txtIp.Text.Trim();
        var portText = txtPort.Text.Trim();
        var userName = txtUserName.Text.Trim();

        if (string.IsNullOrWhiteSpace(ip) ||
            string.IsNullOrWhiteSpace(portText) ||
            string.IsNullOrWhiteSpace(userName))
        {
            MessageBox.Show("Заполните IP, порт и ник.");
            return;
        }

        if (!int.TryParse(portText, out var port))
        {
            MessageBox.Show("Некорректный порт.");
            return;
        }

        _connection = new ChatClientConnection();
        _connection.MessageReceived += OnMessageReceived;
        _connection.Disconnected += OnDisconnected;

        try
        {
            await _connection.ConnectAsync(ip, port, userName);
            AppendLineToHistory($"Вы подключились как {userName}.");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Не удалось подключиться: {ex.Message}");
            _connection.MessageReceived -= OnMessageReceived;
            _connection.Disconnected -= OnDisconnected;
            _connection = null;
        }
    }

    private async void btnSend_Click(object sender, EventArgs e)
    {
        if (_connection == null || !_connection.IsConnected)
        {
            MessageBox.Show("Сначала подключитесь к серверу.");
            return;
        }

        var text = txtMessage.Text.Trim();
        if (string.IsNullOrWhiteSpace(text))
            return;

        string? toUser = null;
        if (lstUsers.SelectedItem is string selectedUser && !string.IsNullOrWhiteSpace(selectedUser))
        {
            // Если выбран конкретный пользователь, отправляем личное сообщение.
            toUser = selectedUser;
        }

        try
        {
            await _connection.SendChatAsync(text, toUser);
            txtMessage.Clear();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка отправки: {ex.Message}");
        }
    }

    private void OnMessageReceived(Message message)
    {
        if (InvokeRequired)
        {
            BeginInvoke(new Action<Message>(OnMessageReceived), message);
            return;
        }

        if (message.Type == MessageType.UserList && message.Users != null)
        {
            lstUsers.BeginUpdate();
            lstUsers.Items.Clear();
            foreach (var user in message.Users)
            {
                lstUsers.Items.Add(user);
            }
            lstUsers.EndUpdate();
            return;
        }

        // Сохраняем только обычные и системные сообщения в истории.
        if (message.Type == MessageType.Chat || message.Type == MessageType.System)
        {
            _history.Add(message);
            AppendLineToHistory($"[{message.Timestamp:HH:mm:ss}] {message.From}: {message.Text}");
            SaveHistory();
        }
    }

    private void OnDisconnected()
    {
        if (InvokeRequired)
        {
            BeginInvoke(new Action(OnDisconnected));
            return;
        }

        AppendLineToHistory("Соединение с сервером потеряно.");
    }

    private void AppendLineToHistory(string line)
    {
        rtbHistory.AppendText(line + Environment.NewLine);
    }

    private void LoadHistory()
    {
        if (!File.Exists(_historyFilePath))
            return;

        try
        {
            var json = File.ReadAllText(_historyFilePath);
            var messages = JsonSerializer.Deserialize<List<Message>>(json);
            if (messages == null)
                return;

            _history.Clear();
            _history.AddRange(messages);

            foreach (var msg in _history)
            {
                AppendLineToHistory($"[{msg.Timestamp:HH:mm:ss}] {msg.From}: {msg.Text}");
            }
        }
        catch
        {
            // Если история битая, просто игнорируем.
        }
    }

    private void SaveHistory()
    {
        try
        {
            var json = JsonSerializer.Serialize(_history, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            File.WriteAllText(_historyFilePath, json);
        }
        catch
        {
            // Не критично, если лог не сохранился.
        }
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        base.OnFormClosed(e);
        _connection?.Dispose();
    }
}

