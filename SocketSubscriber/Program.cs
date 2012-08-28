using System;
using System.IO;
using System.Windows.Forms;

namespace SocketSubscriber
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo("log4net.config"));
            log4net.LogManager.GetLogger(typeof(Program)).Info("程序启动");
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
            Application.Run(new Subscriber());
        }

        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            log4net.ILog log = log4net.LogManager.GetLogger(typeof(Program));
            log.Error(sender);
            log.Error(e);
            System.Threading.Thread.Sleep(10);
            Application.Restart();
        }
    }
}
