using System;
using System.Windows.Forms;
using System.Configuration;
using MyMq;

namespace SocketPublisher
{
    public partial class Form1 : Form
    { 
        int _noOfEventsFired = 0; 
        private MyMq.SocketProduct _product; 
        public Form1()
        {
            InitializeComponent();
            txtEventData.Text = "Cox's Bazar sea Beach of Bangladesh is really an exceptional creation of God"; 
            string serverIP = ConfigurationSettings.AppSettings["ServerIP"]; 
            int serverPort = Convert.ToInt32(ConfigurationSettings.AppSettings["ServerPort"]); 
            _product = new SocketProduct();
            _product.Init(serverIP,serverPort); 
            txtTopicName.Text = "Bangladesh";
            txtEventCount.Text = "0"; 
        }



        private void button3_Click(object sender, EventArgs e)
        {
            string topicName = txtTopicName.Text.Trim();
            if (string.IsNullOrEmpty(topicName))
            {
                MessageBox.Show("Please Enter a Topic Name");
                return;
            }
            SendASingleEvent();
        }

        private void SendASingleEvent()
        {
            String topicName = txtTopicName.Text;
            string eventData = txtEventData.Text; 
            _product.Send(topicName,eventData);
            _noOfEventsFired++;
            txtEventCount.Text = _noOfEventsFired.ToString();
        }

        private void tmrEvent_Tick(object sender, EventArgs e)
        {
            SendASingleEvent();
        }

        private void btnFireAutoStop_Click(object sender, EventArgs e)
        {
            if (tmrEvent.Enabled)
                tmrEvent.Enabled = false;
        }

        private void btnFireAuto_Click(object sender, EventArgs e)
        {
            string topicName = txtTopicName.Text.Trim();
            if (string.IsNullOrEmpty(topicName))
            {
                MessageBox.Show("Please Enter a Topic Name");
                return;
            }
            int interval = 1000;
            tmrEvent.Interval = interval;
            tmrEvent.Enabled = true;

        }
         
    }
}
