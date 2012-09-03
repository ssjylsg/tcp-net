using System.Windows.Forms;
using MyMq;

namespace PubSubServer
{
    public partial class ServerForm : Form
    {
        public ServerForm()
        {
            InitializeComponent();
            HostPublishSubscribeServices();
        }

        private PubSubService _producterService;

        private void HostPublishSubscribeServices()
        {
            _producterService = new PubSubService();
            _producterService.InitTcpService();
            _producterService.TestForPublish(ProducerTcpService_ReceiveMessageEventHandler);

        }

        private int _messageCount = 1; // 消息计数器
        void ProducerTcpService_ReceiveMessageEventHandler(object message)
        {
            if (this.richTextBox1.InvokeRequired)
            {
                richTextBox1.Invoke(
                    new MethodInvoker(
                        delegate()
                        {
                            this.richTextBox1.AppendText(string.Format("第{1}条数据:{0}\r\n", message.ToString(),
                                                                       _messageCount++));
                        }));
            }
        }

        private void Server_Load(object sender, System.EventArgs e)
        {

        }
        /// <summary>
        /// 清空数据
        /// </summary>
        private void clearBtn_Click(object sender, System.EventArgs e)
        {
            this._messageCount = 1;
            this.richTextBox1.Clear();
        }

        private void startBtn_Click(object sender, System.EventArgs e)
        {
            _producterService.Start();
            this.startBtn.Enabled = false;
            this.stopBtn.Enabled = true;
        }

        private void stopBtn_Click(object sender, System.EventArgs e)
        {
            _producterService.Stop();
            this.startBtn.Enabled = true;
            this.stopBtn.Enabled = false;
        }
    }
}
