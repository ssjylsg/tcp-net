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

        private IService _producterService; // 发布服务
        private IService _subscriberService; // 订阅服务
        private void HostPublishSubscribeServices()
        {
            ProducerTcpService.ReceiveMessageEventHandler += new ReceiveMessageEventHandler(ProducerTcpService_ReceiveMessageEventHandler);
            _producterService = new ProducerTcpService();

            _producterService.StartService();
            _subscriberService = new TcpSubscriberService();
            _subscriberService.StartService();

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
    }
}
