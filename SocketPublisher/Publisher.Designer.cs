namespace SocketPublisher
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            this.txtTopicName = new System.Windows.Forms.TextBox();
            this.txtEventData = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnFireAuto = new System.Windows.Forms.Button();
            this.btnFireAutoStop = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.txtEventCount = new System.Windows.Forms.TextBox();
            this.button3 = new System.Windows.Forms.Button();
            this.tmrEvent = new System.Windows.Forms.Timer(this.components);
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtTopicName
            // 
            this.txtTopicName.Location = new System.Drawing.Point(138, 69);
            this.txtTopicName.Name = "txtTopicName";
            this.txtTopicName.Size = new System.Drawing.Size(84, 20);
            this.txtTopicName.TabIndex = 1;
            // 
            // txtEventData
            // 
            this.txtEventData.Location = new System.Drawing.Point(138, 111);
            this.txtEventData.Multiline = true;
            this.txtEventData.Name = "txtEventData";
            this.txtEventData.Size = new System.Drawing.Size(147, 82);
            this.txtEventData.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(53, 72);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 13);
            this.label3.TabIndex = 38;
            this.label3.Text = "Topic Name";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(53, 145);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 13);
            this.label1.TabIndex = 39;
            this.label1.Text = "Event Data";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnFireAuto);
            this.groupBox1.Controls.Add(this.btnFireAutoStop);
            this.groupBox1.Location = new System.Drawing.Point(363, 67);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(169, 127);
            this.groupBox1.TabIndex = 44;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Auto Event";
            // 
            // btnFireAuto
            // 
            this.btnFireAuto.Location = new System.Drawing.Point(6, 42);
            this.btnFireAuto.Name = "btnFireAuto";
            this.btnFireAuto.Size = new System.Drawing.Size(143, 23);
            this.btnFireAuto.TabIndex = 3;
            this.btnFireAuto.Text = "Fire Auto Event";
            this.btnFireAuto.UseVisualStyleBackColor = true;
            this.btnFireAuto.Click += new System.EventHandler(this.btnFireAuto_Click);
            // 
            // btnFireAutoStop
            // 
            this.btnFireAutoStop.Location = new System.Drawing.Point(37, 78);
            this.btnFireAutoStop.Name = "btnFireAutoStop";
            this.btnFireAutoStop.Size = new System.Drawing.Size(87, 23);
            this.btnFireAutoStop.TabIndex = 5;
            this.btnFireAutoStop.Text = "Stop";
            this.btnFireAutoStop.UseVisualStyleBackColor = true;
            this.btnFireAutoStop.Click += new System.EventHandler(this.btnFireAutoStop_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(256, 471);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(52, 23);
            this.button2.TabIndex = 43;
            this.button2.Text = "Reset";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(329, 219);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(123, 13);
            this.label4.TabIndex = 42;
            this.label4.Text = "Total Events  Fired :";
            // 
            // txtEventCount
            // 
            this.txtEventCount.Location = new System.Drawing.Point(491, 219);
            this.txtEventCount.Name = "txtEventCount";
            this.txtEventCount.Size = new System.Drawing.Size(41, 20);
            this.txtEventCount.TabIndex = 41;
            this.txtEventCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(332, 26);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(225, 23);
            this.button3.TabIndex = 40;
            this.button3.Text = "Fire a single  Event";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // tmrEvent
            // 
            this.tmrEvent.Tick += new System.EventHandler(this.tmrEvent_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(600, 286);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtEventCount);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtEventData);
            this.Controls.Add(this.txtTopicName);
            this.Name = "Form1";
            this.Text = "Publisher";
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtTopicName;
        private System.Windows.Forms.TextBox txtEventData;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnFireAuto;
        private System.Windows.Forms.Button btnFireAutoStop;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtEventCount;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Timer tmrEvent;
    }
}

