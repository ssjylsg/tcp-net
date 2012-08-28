using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Windows.Forms;
using System.Configuration;
using MyMq;
using Timer = System.Windows.Forms.Timer;

namespace SocketSubscriber
{
    public partial class Subscriber : Form
    {
        Boolean _isReceivingStarted = false;
        private ISubscribercs _subscriber;
        private IProduct _product;
        private string serverIP;
        private int serverPort;
        private IService _producterService;
        private IService _subscriberService;
        public Subscriber()
        {
            InitializeComponent();
            serverIP = ConfigurationSettings.AppSettings["ServerIP"];

            txtTopicName.Text = "Bangladesh";
            sendInfoRtb.Text = DateTime.Now.ToString();
            this.Closed += new EventHandler(Subscriber_Closed);
        }

        void Subscriber_Closed(object sender, EventArgs e)
        {
            if (this._subscriber != null)
            {
                this._subscriber.UnSubscribe(this.txtTopicName.Text);
            }
            if (this._producterService != null)
            {
                this._producterService.Stop();
            }
            if (this._subscriberService != null)
            {
                this._subscriberService.Stop();
            }
        }
        private void InitSocket()
        {
            if (this.isServiceCkb.Checked)
            {
                _producterService = new ProducerSocketService();
                _producterService.StartService();
                _subscriberService = new SocketSubscriberService();
                _subscriberService.StartService();
            }

            serverPort = Convert.ToInt32(ConfigurationSettings.AppSettings["ServerPort"]);
            _subscriber = new MyMq.SocketSubscriber(serverIP, serverPort, new TimeSpan(0, 0, 0, 0, 50));
            _subscriber.OnReceiveMessageEventHandler += new ReceiveMessageEventHandler(_subscriber_OnReceiveMessageEventHandler);

            _product = new SocketProduct();
            _product.Init(serverIP, 10002);
        }
        private void InitTcp()
        {
            if (this.isServiceCkb.Checked)
            {
                _producterService = new ProducerTcpService();
                _producterService.StartService();
                _subscriberService = new TcpSubscriberService();
                _subscriberService.StartService();
            }

            serverPort = Convert.ToInt32(ConfigurationSettings.AppSettings["ServerPort"]);
            _subscriber = new TcpSubscribercs(serverIP, serverPort, new TimeSpan(0, 0, 0, 0, 50));
            _subscriber.OnReceiveMessageEventHandler += new ReceiveMessageEventHandler(_subscriber_OnReceiveMessageEventHandler);

            _product = new TcpProduct();
            _product.Init(serverIP, 10002);
        }
        void _subscriber_OnReceiveMessageEventHandler(object obj)
        {
            if (obj == null)
            {
                AddToTextBox("收到为空的信息");
                return;
            }
            if (obj is string)
            {
                AddToTextBox(obj.ToString());
            }
            else if (obj is DateTime)
            {
                AddToTextBox(((DateTime)obj).ToString(CultureInfo.InvariantCulture));
            }
            else
            {
                if (obj is byte[])
                {
                    string fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                                      string.Format("{0}.log", DateTime.Now.ToString("yyyy_MM-dd_hh_mm_ss")));
                    using (FileStream writer = new FileStream(fileName, FileMode.OpenOrCreate))
                    {
                        byte[] buffer = (byte[])(obj);
                        writer.BeginWrite(buffer, 0, buffer.Length, null, null);
                    }
                    this.AddToTextBox(string.Format("接受到文件"));
                }

            }
        }
        public static byte[] ObjectToBytes(object obj)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                return ms.GetBuffer();
            }
        }
        public static object BytesToObject(byte[] Bytes)
        {
            using (MemoryStream ms = new MemoryStream(Bytes))
            {
                IFormatter formatter = new BinaryFormatter();
                return formatter.Deserialize(ms);
            }
        }


        public delegate void AddToTextBoxDelegate(string message);

        private int _messageCount = 1;
        public void AddToTextBox(string message)
        {
            message = string.Format("接受到信息{0}:{1}", _messageCount++, message.Trim(new char[] { '\0' }).Trim());
            if (this.richTextBox1.InvokeRequired)
            {
                this.richTextBox1.Invoke(new MethodInvoker(delegate()
                                                               {
                                                                   this.richTextBox1.AppendText(string.Format("{0}\r\n", message));
                                                               }));
            }
            else
            {
                this.richTextBox1.AppendText(string.Format("{0}\r\n", message));
            }

        }

        private void btnClearAstaListView_Click(object sender, EventArgs e)
        {
            this.richTextBox1.Clear();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                string topicName = txtTopicName.Text.Trim();
                if (string.IsNullOrEmpty(topicName))
                {
                    return;
                }
                _subscriber.UnSubscribe(topicName);
                ((Button)sender).Visible = false;
                button3.Visible = true;
            }
            catch
            {

            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string topicName = txtTopicName.Text.Trim();
            if (string.IsNullOrEmpty(topicName))
            {
                return;
            }
            ((Button)sender).Visible = false;
            button2.Visible = true;
            _subscriber.Subscribe(topicName);

            if (_isReceivingStarted == false)
            {
                _isReceivingStarted = true;
            }
        }

        private void sendBtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.txtTopicName.Text))
            {
                return;
            }
            this.Send(this.sendInfoRtb.Text);
        }
        private void Send(object obj)
        {
            _product.Send(this.txtTopicName.Text, obj);
        }
        private void sendFileBtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                using (FileStream stream = new FileStream(openFile.FileName, FileMode.Open))
                {
                    byte[] buffer = new byte[stream.Length];
                    stream.Read(buffer, 0, buffer.Length);
                    this.Send(buffer);
                }
            }
        }

        private TcpClient _tcpClient;
        private TcpServer _tcpServer;
        private void SendByTcpClient(byte[] data)
        {

            if (_tcpClient == null)
            {
                _tcpServer = new TcpServer(serverIP);
                _tcpServer.OnReceiveMessageEventHandler += new ReceiveByteEventHandler(_tcpServer_OnReceiveMessageEventHandler);
                _tcpClient = new TcpClient();
                _tcpClient.Connect(serverIP, 20003);
            }
            NetworkStream stream = _tcpClient.GetStream();
            if (stream.CanWrite)
            {
                stream.Write(data, 0, data.Length);
            }
        }

        void _tcpServer_OnReceiveMessageEventHandler(byte[] buffer)
        {

        }

        private void TcpBtn_Click(object sender, EventArgs e)
        {
            if (this.socketrbtn.Checked)
            {
                this.InitSocket();
            }
            else
            {
                this.InitTcp();
            }
        }

        private Timer _timer;
        private System.Diagnostics.Stopwatch _stopWach;
        private void autoSendBtn_Click(object sender, EventArgs e)
        {
            if (this.autoSendBtn.Text == "自动发送")
            {
                this._sendCount = 1;
                _messageCount = 1;
                if (this._timer == null)
                {
                    _timer = new Timer();
                    _timer.Interval = 20;
                    _timer.Tick += new EventHandler(timer_Tick);
                    _stopWach = new Stopwatch();
                }
                _timer.Start();
                _stopWach.Reset();
                _stopWach.Start();
                this.autoSendBtn.Text = "停止";
            }
            else
            {
                if (this._timer != null)
                {
                    this._timer.Stop();
                    this.autoSendBtn.Text = "自动发送";
                    _stopWach.Stop();
                    this.watchInfoLbl.Text = string.Format("{0}", _stopWach.ElapsedMilliseconds);

                }
            }
        }

        private int _sendCount = 1;
        void timer_Tick(object sender, EventArgs e)
        {
            this.Send(string.Format("{0}_{1}", this._sendCount++, Guid.NewGuid().ToString()));
            this.sendTxt.Text = this._sendCount.ToString();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }

    public delegate void ReceiveByteEventHandler(byte[] buffer);
    public class TcpServer
    {
        public event ReceiveByteEventHandler OnReceiveMessageEventHandler;

        private void OnOnReceiveMessageEventHandler(byte[] buffer)
        {
            ReceiveByteEventHandler handler = OnReceiveMessageEventHandler;
            if (handler != null) handler(buffer);
        }

        public TcpServer(string serverIP)
        {
            TcpListener listener = new TcpListener(IPAddress.Parse(serverIP), 20003);
            listener.Start();
            Thread thread = new Thread(ExecuteThread);
            thread.IsBackground = true;
            thread.Start(listener);

        }
        private void ExecuteThread(object obj)
        {
            TcpListener listener = (TcpListener)obj;
            while (true)
            {
                using (TcpClient client = listener.AcceptTcpClient())
                using (NetworkStream stream = client.GetStream())
                {
                    if (stream.CanRead)
                    {

                        byte[] bytes = new byte[client.ReceiveBufferSize];

                        int readCount = stream.Read(bytes, 0, bytes.Length);
                        if (readCount >= 0)
                        {
                            byte[] receiveBytes = new byte[readCount];
                            Array.Resize(ref receiveBytes, readCount);
                            OnOnReceiveMessageEventHandler(receiveBytes);
                        }

                    }
                }
            }
        }
    }
}
