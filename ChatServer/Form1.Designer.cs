namespace ChatServer;

partial class Form1
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    private System.Windows.Forms.TextBox txtIp;
    private System.Windows.Forms.TextBox txtPort;
    private System.Windows.Forms.Button btnStart;
    private System.Windows.Forms.Button btnStop;
    private System.Windows.Forms.TextBox txtLog;

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
        this.txtIp = new System.Windows.Forms.TextBox();
        this.txtPort = new System.Windows.Forms.TextBox();
        this.btnStart = new System.Windows.Forms.Button();
        this.btnStop = new System.Windows.Forms.Button();
        this.txtLog = new System.Windows.Forms.TextBox();
        this.SuspendLayout();
        // 
        // txtIp
        // 
        this.txtIp.Location = new System.Drawing.Point(12, 12);
        this.txtIp.Name = "txtIp";
        this.txtIp.PlaceholderText = "IP (например, 127.0.0.1)";
        this.txtIp.Size = new System.Drawing.Size(200, 23);
        this.txtIp.TabIndex = 0;
        // 
        // txtPort
        // 
        this.txtPort.Location = new System.Drawing.Point(218, 12);
        this.txtPort.Name = "txtPort";
        this.txtPort.PlaceholderText = "Порт (например, 9000)";
        this.txtPort.Size = new System.Drawing.Size(100, 23);
        this.txtPort.TabIndex = 1;
        // 
        // btnStart
        // 
        this.btnStart.Location = new System.Drawing.Point(324, 11);
        this.btnStart.Name = "btnStart";
        this.btnStart.Size = new System.Drawing.Size(75, 25);
        this.btnStart.TabIndex = 2;
        this.btnStart.Text = "Старт";
        this.btnStart.UseVisualStyleBackColor = true;
        this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
        // 
        // btnStop
        // 
        this.btnStop.Location = new System.Drawing.Point(405, 11);
        this.btnStop.Name = "btnStop";
        this.btnStop.Size = new System.Drawing.Size(75, 25);
        this.btnStop.TabIndex = 3;
        this.btnStop.Text = "Стоп";
        this.btnStop.UseVisualStyleBackColor = true;
        this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
        // 
        // txtLog
        // 
        this.txtLog.Location = new System.Drawing.Point(12, 43);
        this.txtLog.Multiline = true;
        this.txtLog.Name = "txtLog";
        this.txtLog.ReadOnly = true;
        this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
        this.txtLog.Size = new System.Drawing.Size(776, 395);
        this.txtLog.TabIndex = 4;
        // 
        // Form1
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(800, 450);
        this.Controls.Add(this.txtLog);
        this.Controls.Add(this.btnStop);
        this.Controls.Add(this.btnStart);
        this.Controls.Add(this.txtPort);
        this.Controls.Add(this.txtIp);
        this.Name = "Form1";
        this.Text = "Chat Server";
        this.ResumeLayout(false);
        this.PerformLayout();
    }

    #endregion
}

