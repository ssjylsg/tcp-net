using System.Windows.Forms;
using MyMq;

namespace PubSubServer
{
    public partial class Server : Form
    {
        public Server()
        {
            InitializeComponent();
            HostPublishSubscribeServices();
        }
        private IService _producterService;
        private IService _subscriberService;
        private void HostPublishSubscribeServices()
        {
            ProducerTcpService.ReceiveMessageEventHandler += new ReceiveMessageEventHandler(ProducerTcpService_ReceiveMessageEventHandler);
            _producterService = new MyMq.ProducerTcpService();

            _producterService.StartService();
            _subscriberService = new MyMq.TcpSubscriberService();
            _subscriberService.StartService();
            //new MyMq.ProducerSocketService().StartService();
            //new MyMq.SocketSubscriberService().StartService();
        }

        void ProducerTcpService_ReceiveMessageEventHandler(object message)
        {
            if (this.richTextBox1.InvokeRequired)
            {
                richTextBox1.Invoke(
                    new MethodInvoker(
                        delegate() { this.richTextBox1.AppendText(string.Format("{0}\r\n", message.ToString())); }));


            }
        }

        private void Server_Load(object sender, System.EventArgs e)
        {

        }
    }
}
