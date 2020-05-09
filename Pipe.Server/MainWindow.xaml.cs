using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Pipe.Server
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _working = true;
        private PipeCommunication.Server server;

        public MainWindow()
        {
            InitializeComponent();
            server = new PipeCommunication.Server("fastPipe");

            server.Started += Server_Started;
            server.Broken += Server_Broken;
        }

        private void StartServerHandler(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                server.Start();                
            });
        }

        private void StopServerHandler(object sender, RoutedEventArgs e)
        {
            _working = false;
        }

        private void Server_Broken(object sender, EventArgs e)
        {
            Dispatcher.Invoke((Action)(() =>
            {
                tb_pipeStatus.Text = "Broken";
            }));
        }

        private void Server_Started(object sender, EventArgs e)
        {
            Dispatcher.Invoke((Action)(() =>
            {
                tb_pipeStatus.Text = "Started";
            }));
        }
    }
}
