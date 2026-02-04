using System.Net;

namespace ChatServer;

public partial class Form1 : Form
{
    private readonly ChatServerInstance _server = new();

    public Form1()
    {
        InitializeComponent();

        txtIp.Text = "127.0.0.1";
        txtPort.Text = "9000";

        _server.Log += AppendLogSafe;
        _server.MessageReceived += msg =>
        {
            var line = $"[{msg.Timestamp:HH:mm:ss}] {msg.From}: {msg.Text}";
            AppendLogSafe(line);
        };
    }

    private void btnStart_Click(object sender, EventArgs e)
    {
        if (_server.IsRunning)
            return;

        if (!IPAddress.TryParse(txtIp.Text.Trim(), out var ip))
        {
            MessageBox.Show("Некорректный IP-адрес");
            return;
        }

        if (!int.TryParse(txtPort.Text.Trim(), out var port))
        {
            MessageBox.Show("Некорректный порт");
            return;
        }

        _ = Task.Run(() => _server.StartAsync(ip, port));
    }

    private void btnStop_Click(object sender, EventArgs e)
    {
        _server.Stop();
    }

    private void AppendLogSafe(string text)
    {
        if (InvokeRequired)
        {
            BeginInvoke(new Action<string>(AppendLogSafe), text);
            return;
        }

        txtLog.AppendText(text + Environment.NewLine);
    }
}

