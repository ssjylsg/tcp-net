namespace SocketSubscriber
{
    partial class SubscriberForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label3 = new System.Windows.Forms.Label();
            this.txtTopicName = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.btnClearAstaListView = new System.Windows.Forms.Button();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.sendBtn = new System.Windows.Forms.Button();
            this.sendInfoRtb = new System.Windows.Forms.RichTextBox();
            this.sendFileBtn = new System.Windows.Forms.Button();
            this.TcpBtn = new System.Windows.Forms.Button();
            this.socketrbtn = new System.Windows.Forms.RadioButton();
            this.tcprbtn = new System.Windows.Forms.RadioButton();
            this.autoSendBtn = new System.Windows.Forms.Button();
            this.sendTxt = new System.Windows.Forms.TextBox();
            this.isServiceCkb = new System.Windows.Forms.CheckBox();
            this.watchInfoLbl = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(346, 27);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 12);
            this.label3.TabIndex = 37;
            this.label3.Text = "Topic Name";
            // 
            // txtTopicName
            // 
            this.txtTopicName.Location = new System.Drawing.Point(430, 25);
            this.txtTopicName.Name = "txtTopicName";
            this.txtTopicName.Size = new System.Drawing.Size(72, 21);
            this.txtTopicName.TabIndex = 36;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.Navy;
            this.label4.Location = new System.Drawing.Point(36, 60);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(147, 13);
            this.label4.TabIndex = 35;
            this.label4.Text = "Events Received  server";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(216, 23);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(85, 21);
            this.button2.TabIndex = 32;
            this.button2.Text = "UnSubscribe";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Visible = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(226, 22);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 21);
            this.button3.TabIndex = 31;
            this.button3.Text = "Subscribe";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // btnClearAstaListView
            // 
            this.btnClearAstaListView.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnClearAstaListView.Location = new System.Drawing.Point(322, 269);
            this.btnClearAstaListView.Name = "btnClearAstaListView";
            this.btnClearAstaListView.Size = new System.Drawing.Size(68, 21);
            this.btnClearAstaListView.TabIndex = 33;
            this.btnClearAstaListView.Text = "Clear List";
            this.btnClearAstaListView.UseVisualStyleBackColor = true;
            this.btnClearAstaListView.Click += new System.EventHandler(this.btnClearAstaListView_Click);
            // 
            // richTextBox1
            // 
            this.richTextBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox1.Location = new System.Drawing.Point(29, 85);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(409, 178);
            this.richTextBox1.TabIndex = 38;
            this.richTextBox1.Text = "";
            // 
            // sendBtn
            // 
            this.sendBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.sendBtn.Location = new System.Drawing.Point(530, 269);
            this.sendBtn.Name = "sendBtn";
            this.sendBtn.Size = new System.Drawing.Size(75, 23);
            this.sendBtn.TabIndex = 39;
            this.sendBtn.Text = "发送";
            this.sendBtn.UseVisualStyleBackColor = true;
            this.sendBtn.Click += new System.EventHandler(this.sendBtn_Click);
            // 
            // sendInfoRtb
            // 
            this.sendInfoRtb.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.sendInfoRtb.Location = new System.Drawing.Point(456, 85);
            this.sendInfoRtb.Name = "sendInfoRtb";
            this.sendInfoRtb.Size = new System.Drawing.Size(231, 178);
            this.sendInfoRtb.TabIndex = 40;
            this.sendInfoRtb.Text = "";
            // 
            // sendFileBtn
            // 
            this.sendFileBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.sendFileBtn.Location = new System.Drawing.Point(274, 319);
            this.sendFileBtn.Name = "sendFileBtn";
            this.sendFileBtn.Size = new System.Drawing.Size(75, 23);
            this.sendFileBtn.TabIndex = 41;
            this.sendFileBtn.Text = "发送文件";
            this.sendFileBtn.UseVisualStyleBackColor = true;
            this.sendFileBtn.Click += new System.EventHandler(this.sendFileBtn_Click);
            // 
            // TcpBtn
            // 
            this.TcpBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.TcpBtn.Location = new System.Drawing.Point(29, 319);
            this.TcpBtn.Name = "TcpBtn";
            this.TcpBtn.Size = new System.Drawing.Size(127, 23);
            this.TcpBtn.TabIndex = 42;
            this.TcpBtn.Text = "初始化服务";
            this.TcpBtn.UseVisualStyleBackColor = true;
            this.TcpBtn.Click += new System.EventHandler(this.TcpBtn_Click);
            // 
            // socketrbtn
            // 
            this.socketrbtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.socketrbtn.AutoSize = true;
            this.socketrbtn.Location = new System.Drawing.Point(162, 322);
            this.socketrbtn.Name = "socketrbtn";
            this.socketrbtn.Size = new System.Drawing.Size(59, 16);
            this.socketrbtn.TabIndex = 43;
            this.socketrbtn.TabStop = true;
            this.socketrbtn.Text = "Socket";
            this.socketrbtn.UseVisualStyleBackColor = true;
            // 
            // tcprbtn
            // 
            this.tcprbtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.tcprbtn.AutoSize = true;
            this.tcprbtn.Location = new System.Drawing.Point(227, 324);
            this.tcprbtn.Name = "tcprbtn";
            this.tcprbtn.Size = new System.Drawing.Size(41, 16);
            this.tcprbtn.TabIndex = 44;
            this.tcprbtn.TabStop = true;
            this.tcprbtn.Text = "Tcp";
            this.tcprbtn.UseVisualStyleBackColor = true;
            // 
            // autoSendBtn
            // 
            this.autoSendBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.autoSendBtn.Location = new System.Drawing.Point(372, 319);
            this.autoSendBtn.Name = "autoSendBtn";
            this.autoSendBtn.Size = new System.Drawing.Size(75, 23);
            this.autoSendBtn.TabIndex = 45;
            this.autoSendBtn.Text = "自动发送";
            this.autoSendBtn.UseVisualStyleBackColor = true;
            this.autoSendBtn.Click += new System.EventHandler(this.autoSendBtn_Click);
            // 
            // sendTxt
            // 
            this.sendTxt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.sendTxt.Location = new System.Drawing.Point(456, 323);
            this.sendTxt.Name = "sendTxt";
            this.sendTxt.Size = new System.Drawing.Size(100, 21);
            this.sendTxt.TabIndex = 46;
            // 
            // isServiceCkb
            // 
            this.isServiceCkb.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.isServiceCkb.AutoSize = true;
            this.isServiceCkb.Location = new System.Drawing.Point(29, 276);
            this.isServiceCkb.Name = "isServiceCkb";
            this.isServiceCkb.Size = new System.Drawing.Size(132, 16);
            this.isServiceCkb.TabIndex = 47;
            this.isServiceCkb.Text = "是否启动服务在本地";
            this.isServiceCkb.UseVisualStyleBackColor = true;
            // 
            // watchInfoLbl
            // 
            this.watchInfoLbl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.watchInfoLbl.AutoSize = true;
            this.watchInfoLbl.Location = new System.Drawing.Point(573, 324);
            this.watchInfoLbl.Name = "watchInfoLbl";
            this.watchInfoLbl.Size = new System.Drawing.Size(41, 12);
            this.watchInfoLbl.TabIndex = 48;
            this.watchInfoLbl.Text = "label1";
            this.watchInfoLbl.Click += new System.EventHandler(this.label1_Click);
            // 
            // SubscriberForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(717, 377);
            this.Controls.Add(this.watchInfoLbl);
            this.Controls.Add(this.isServiceCkb);
            this.Controls.Add(this.sendTxt);
            this.Controls.Add(this.autoSendBtn);
            this.Controls.Add(this.tcprbtn);
            this.Controls.Add(this.socketrbtn);
            this.Controls.Add(this.TcpBtn);
            this.Controls.Add(this.sendFileBtn);
            this.Controls.Add(this.sendInfoRtb);
            this.Controls.Add(this.sendBtn);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtTopicName);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.btnClearAstaListView);
            this.Name = "SubscriberForm";
            this.Text = "SubscriberForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtTopicName;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button btnClearAstaListView;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Button sendBtn;
        private System.Windows.Forms.RichTextBox sendInfoRtb;
        private System.Windows.Forms.Button sendFileBtn;
        private System.Windows.Forms.Button TcpBtn;
        private System.Windows.Forms.RadioButton socketrbtn;
        private System.Windows.Forms.RadioButton tcprbtn;
        private System.Windows.Forms.Button autoSendBtn;
        private System.Windows.Forms.TextBox sendTxt;
        private System.Windows.Forms.CheckBox isServiceCkb;
        private System.Windows.Forms.Label watchInfoLbl;
    }
}

