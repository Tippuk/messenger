namespace ChatClient;

partial class Form1
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    private System.Windows.Forms.RichTextBox rtbHistory;
    private System.Windows.Forms.TextBox txtMessage;
    private System.Windows.Forms.Button btnSend;
    private System.Windows.Forms.TextBox txtIp;
    private System.Windows.Forms.TextBox txtPort;
    private System.Windows.Forms.TextBox txtUserName;
    private System.Windows.Forms.Button btnConnect;
    private System.Windows.Forms.ListBox lstUsers;
    private System.Windows.Forms.Label lblIp;
    private System.Windows.Forms.Label lblPort;
    private System.Windows.Forms.Label lblUserName;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        this.rtbHistory = new System.Windows.Forms.RichTextBox();
        this.txtMessage = new System.Windows.Forms.TextBox();
        this.btnSend = new System.Windows.Forms.Button();
        this.txtIp = new System.Windows.Forms.TextBox();
        this.txtPort = new System.Windows.Forms.TextBox();
        this.txtUserName = new System.Windows.Forms.TextBox();
        this.btnConnect = new System.Windows.Forms.Button();
        this.lstUsers = new System.Windows.Forms.ListBox();
        this.lblIp = new System.Windows.Forms.Label();
        this.lblPort = new System.Windows.Forms.Label();
        this.lblUserName = new System.Windows.Forms.Label();
        this.SuspendLayout();
        // 
        // rtbHistory
        // 
        this.rtbHistory.Location = new System.Drawing.Point(12, 51);
        this.rtbHistory.Name = "rtbHistory";
        this.rtbHistory.ReadOnly = true;
        this.rtbHistory.Size = new System.Drawing.Size(560, 330);
        this.rtbHistory.TabIndex = 0;
        this.rtbHistory.Text = "";
        // 
        // txtMessage
        // 
        this.txtMessage.Location = new System.Drawing.Point(12, 387);
        this.txtMessage.Multiline = true;
        this.txtMessage.Name = "txtMessage";
        this.txtMessage.Size = new System.Drawing.Size(479, 51);
        this.txtMessage.TabIndex = 1;
        // 
        // btnSend
        // 
        this.btnSend.Location = new System.Drawing.Point(497, 387);
        this.btnSend.Name = "btnSend";
        this.btnSend.Size = new System.Drawing.Size(75, 51);
        this.btnSend.TabIndex = 2;
        this.btnSend.Text = "Отправить";
        this.btnSend.UseVisualStyleBackColor = true;
        this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
        // 
        // txtIp
        // 
        this.txtIp.Location = new System.Drawing.Point(52, 12);
        this.txtIp.Name = "txtIp";
        this.txtIp.Size = new System.Drawing.Size(120, 23);
        this.txtIp.TabIndex = 3;
        // 
        // txtPort
        // 
        this.txtPort.Location = new System.Drawing.Point(215, 12);
        this.txtPort.Name = "txtPort";
        this.txtPort.Size = new System.Drawing.Size(60, 23);
        this.txtPort.TabIndex = 4;
        // 
        // txtUserName
        // 
        this.txtUserName.Location = new System.Drawing.Point(355, 12);
        this.txtUserName.Name = "txtUserName";
        this.txtUserName.Size = new System.Drawing.Size(136, 23);
        this.txtUserName.TabIndex = 5;
        // 
        // btnConnect
        // 
        this.btnConnect.Location = new System.Drawing.Point(497, 11);
        this.btnConnect.Name = "btnConnect";
        this.btnConnect.Size = new System.Drawing.Size(75, 25);
        this.btnConnect.TabIndex = 6;
        this.btnConnect.Text = "Войти";
        this.btnConnect.UseVisualStyleBackColor = true;
        this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
        // 
        // lstUsers
        // 
        this.lstUsers.FormattingEnabled = true;
        this.lstUsers.ItemHeight = 15;
        this.lstUsers.Location = new System.Drawing.Point(578, 51);
        this.lstUsers.Name = "lstUsers";
        this.lstUsers.Size = new System.Drawing.Size(210, 387);
        this.lstUsers.TabIndex = 7;
        // 
        // lblIp
        // 
        this.lblIp.AutoSize = true;
        this.lblIp.Location = new System.Drawing.Point(12, 15);
        this.lblIp.Name = "lblIp";
        this.lblIp.Size = new System.Drawing.Size(20, 15);
        this.lblIp.TabIndex = 8;
        this.lblIp.Text = "IP:";
        // 
        // lblPort
        // 
        this.lblPort.AutoSize = true;
        this.lblPort.Location = new System.Drawing.Point(178, 15);
        this.lblPort.Name = "lblPort";
        this.lblPort.Size = new System.Drawing.Size(37, 15);
        this.lblPort.TabIndex = 9;
        this.lblPort.Text = "Порт:";
        // 
        // lblUserName
        // 
        this.lblUserName.AutoSize = true;
        this.lblUserName.Location = new System.Drawing.Point(281, 15);
        this.lblUserName.Name = "lblUserName";
        this.lblUserName.Size = new System.Drawing.Size(68, 15);
        this.lblUserName.TabIndex = 10;
        this.lblUserName.Text = "Ваш ник:";
        // 
        // Form1
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(800, 450);
        this.Controls.Add(this.lblUserName);
        this.Controls.Add(this.lblPort);
        this.Controls.Add(this.lblIp);
        this.Controls.Add(this.lstUsers);
        this.Controls.Add(this.btnConnect);
        this.Controls.Add(this.txtUserName);
        this.Controls.Add(this.txtPort);
        this.Controls.Add(this.txtIp);
        this.Controls.Add(this.btnSend);
        this.Controls.Add(this.txtMessage);
        this.Controls.Add(this.rtbHistory);
        this.Name = "Form1";
        this.Text = "Messenger Client";
        this.ResumeLayout(false);
        this.PerformLayout();
    }

    #endregion
}

