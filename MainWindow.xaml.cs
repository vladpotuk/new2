using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace RPS_Client
{
    public partial class MainWindow : Window
    {
        private TcpClient client;
        private NetworkStream stream;
        private Thread receiveThread;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                client = new TcpClient("127.0.0.1", 5000);
                stream = client.GetStream();
                receiveThread = new Thread(new ThreadStart(ReceiveData));
                receiveThread.Start();
                MessageBox.Show("Підключено до сервера");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка підключення: " + ex.Message);
            }
        }

        private void DisconnectButton_Click(object sender, RoutedEventArgs e)
        {
            if (client != null)
            {
                stream.Close();
                client.Close();
                receiveThread.Abort();
                MessageBox.Show("Відключено від сервера");
            }
        }

        private void MoveButton_Click(object sender, RoutedEventArgs e)
        {
            if (client != null)
            {
                string move = (sender as Button).Content.ToString();
                byte[] moveBytes = Encoding.UTF8.GetBytes(move);
                stream.Write(moveBytes, 0, moveBytes.Length);
            }
            else
            {
                MessageBox.Show("Спочатку підключіться до сервера");
            }
        }

        private void OfferDrawButton_Click(object sender, RoutedEventArgs e)
        {
            if (client != null)
            {
                string message = "Запропонувати нічию";
                byte[] messageBytes = Encoding.UTF8.GetBytes(message);
                stream.Write(messageBytes, 0, messageBytes.Length);
            }
            else
            {
                MessageBox.Show("Спочатку підключіться до сервера");
            }
        }

        private void ConcedeButton_Click(object sender, RoutedEventArgs e)
        {
            if (client != null)
            {
                string message = "Визнати поразку";
                byte[] messageBytes = Encoding.UTF8.GetBytes(message);
                stream.Write(messageBytes, 0, messageBytes.Length);
            }
            else
            {
                MessageBox.Show("Спочатку підключіться до сервера");
            }
        }

        private void ReceiveData()
        {
            while (true)
            {
                try
                {
                    byte[] buffer = new byte[256];
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0)
                        break;

                    string serverMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Dispatcher.Invoke(() =>
                    {
                        ResultTextBlock.Text += serverMessage + Environment.NewLine;
                    });
                }
                catch (Exception)
                {
                    Dispatcher.Invoke(() =>
                    {
                        ResultTextBlock.Text = "Відключено від сервера";
                    });
                    break;
                }
            }
        }
    }
}
