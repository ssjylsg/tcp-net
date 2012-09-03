using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;
using System.Configuration;
using MyMq;
using Timer = System.Windows.Forms.Timer;

namespace SocketSubscriber
{
    public partial class SubscriberForm : Form
    {
        #region 私有变量
        Boolean _isReceivingStarted = false;
        private ISubscribers _subscriber;
        private IPublish _publish;
        private string serverIP;
        private int serverPort;
        private IService _producterService;
        private IService _subscriberService;
        public delegate void AddToTextBoxDelegate(string message);
        private Timer _timer;
        private System.Diagnostics.Stopwatch _stopWach;
        private int _messageCount = 1;
        private int _sendCount = 1;
        #endregion
        public SubscriberForm()
        {
            InitializeComponent();
            serverIP = ConfigurationSettings.AppSettings["ServerIP"];
            txtTopicName.Text = "Bangladesh";
            sendInfoRtb.Text = DateTime.Now.ToString();
            this.Closed += new EventHandler(Subscriber_Closed);
        }
        #region Form_Closed
        /// <summary>
        /// Form_Closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        #endregion

        #region 初始化Socket
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
            _publish = new SocketPublish();

            _publish.Init(serverIP, 10002);
        }
        #endregion

        #region 发包服务失败
        /// <summary>
        /// 发包服务失败
        /// </summary>
        /// <param name="reason"></param>
        void _product_OnSendErrorHandler(NetServiceErrorReason reason)
        {
            MessageBox.Show(string.Format("发包失败:{0}", reason.Message));
        }
        #endregion

        #region 订阅失败

        private int _showErrorMsgCount = 0;
        void _subscriber_OnReceiveErrorHandler(NetServiceErrorReason reason)
        {
            if (_subscriber.IsClientConnected == false)
            {
                _subscriber.ReConnectServer();
                _subscriber.Subscribe(this.txtTopicName.Text);
            }
            if (this._showErrorMsgCount < 3) // 只提示三次
            {
                MessageBox.Show(string.Format("订阅者接受数据失败:{0}", reason.Message));
            }
            _showErrorMsgCount++;
        }
        #endregion

        #region 初始化Tcp
        private void InitTcp()
        {
            if (this.isServiceCkb.Checked)
            {
                _producterService = new PublishTcpService();
                _producterService.StartService();
                _subscriberService = new TcpSubscribersService();
                _subscriberService.StartService();
            }

            serverPort = Convert.ToInt32(ConfigurationSettings.AppSettings["ServerPort"]);
            _subscriber = new TcpSubscribers(serverIP, serverPort, new TimeSpan(0, 0, 0, 0, 50));

            _publish = new TcpPublish();
            _publish.Init(serverIP, 10002);
        }
        #endregion

        #region 订阅者接收到数据
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
                                                      string.Format("{0}.jpg", DateTime.Now.ToString("yyyy_MM-dd_hh_mm_ss")));
                    FileStream writer = new FileStream(fileName, FileMode.OpenOrCreate);

                    byte[] buffer = (byte[])(obj);
                    writer.BeginWrite(buffer, 0, buffer.Length, delegate(IAsyncResult result)
                            {
                                writer.EndWrite(result);
                                InvokeTextBox(this.sendInfoRtb, string.Format("接收文件读写完成，共{0}", buffer.Length));
                                writer.Close();
                            }, null);

                    this.AddToTextBox(string.Format("接受到文件"));
                }

            }
        }
        #endregion

        #region 附加信息
        private void InvokeTextBox(RichTextBox richTextBox, string message)
        {
            if (richTextBox.InvokeRequired)
            {
                richTextBox.Invoke(new MethodInvoker(delegate()
                {
                    richTextBox.AppendText(string.Format("{0}\r\n", message));
                }));
            }
            else
            {
                richTextBox.AppendText(string.Format("{0}\r\n", message));
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        private void AddToTextBox(string message)
        {
            message = string.Format("接受到信息{0}:{1}", _messageCount++, message.Trim(new char[] { '\0' }).Trim());
            InvokeTextBox(this.richTextBox1, message);

        }
        #endregion

        #region 清空信息
        /// <summary>
        /// 清空
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClearAstaListView_Click(object sender, EventArgs e)
        {
            this.richTextBox1.Clear();
        }
        #endregion

        #region 订阅/取消订阅
        /// <summary>
        /// 取消订阅
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                _showErrorMsgCount = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// 订阅
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        #endregion

        #region 手动发送
        /// <summary>
        /// 手动发送
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sendBtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.txtTopicName.Text))
            {
                return;
            }
            this.Send(this.sendInfoRtb.Text);
        }
        #endregion

        #region 发送数据
        /// <summary>
        /// 自动发送数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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


        void timer_Tick(object sender, EventArgs e)
        {
            try
            {
                this._timer.Enabled = false;
                this.Send(string.Format("{0}_{1}", this._sendCount++, Guid.NewGuid().ToString()));
                this.sendTxt.Text = this._sendCount.ToString();
                this._timer.Enabled = true;
            }
            catch (Exception ex)
            {
                this._timer.Stop();
                MessageBox.Show(ex.Message);
                this.autoSendBtn.Text = "自动发送";
                _stopWach.Stop();
                this.watchInfoLbl.Text = string.Format("{0}", _stopWach.ElapsedMilliseconds);
            }
        }
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="obj"></param>
        private void Send(object obj)
        {
            _publish.Send(this.txtTopicName.Text, obj);
        }
        /// <summary>
        /// 发送文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        #endregion

        #region 初始化服务
        /// <summary>
        /// 初始化服务
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
            // 注册事件
            _subscriber.ReceiveMessage += new ReceiveMessageEventHandler(_subscriber_OnReceiveMessageEventHandler);
            _subscriber.ReceiveMessageError += new ReceiveErrorHandler(_subscriber_OnReceiveErrorHandler);
            _publish.SendMessageError += new SendErrorHandler(_product_OnSendErrorHandler);
        }
        #endregion

        #region 无用
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

        private void label1_Click(object sender, EventArgs e)
        {

        }
        #endregion
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
