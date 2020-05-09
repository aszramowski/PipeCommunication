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

namespace Pipe.Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        PipeCommunication.Client client;
        public MainWindow()
        {
            InitializeComponent();

            client = new PipeCommunication.Client("fastPipe");
        }

        private void btn_Connect_Click(object sender, RoutedEventArgs e)
        {
            client.Connect(3000);
        }

        private void btn_sendCheck_Click(object sender, RoutedEventArgs e)
        {
            var status = client.CheckStatus();
            run_received.Text = status.ToString();
        }

        private void btn_sendBreak_Click(object sender, RoutedEventArgs e)
        {
            var status = client.Break();
            run_received.Text = status.ToString();
        }

        private void btn_sendText_Click(object sender, RoutedEventArgs e)
        {
            var status = client.Start();
            run_received.Text = status.ToString();
        }

        private void btn_Disconnect_Click(object sender, RoutedEventArgs e)
        {
            var status = client.Disconnect();
            run_received.Text = status.ToString();
        }

        private void btn_sendStart_Click(object sender, RoutedEventArgs e)
        {
            var status = client.Start();
            run_received.Text = status.ToString();
        }
    }
}
