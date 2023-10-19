using MaterialDesignColors;
using Server.Connection;
using Shared.Connection;
using Shared.Model;
using Shared.Utilities;
using System;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

namespace Server
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IPacketProcessor _packetProcessor;
        private readonly IListener _server;
        private readonly IAcceptedClientRepository _repository;
        public MainWindow()
        {
            InitializeComponent();
            _server = new Listener(socketAcceptedHandler, exceptionHandler);
            _repository = new AcceptedClientRepository();
            _packetProcessor = new ServerPacketProcessor(_repository, _server);
            Listener.GetAllIpAsync().Result.ForEach(x => IP.Items.Add(x));
            Title = "Server";
        }

        private void socketAcceptedHandler(object sender, Socket e)
        {
            ThreadPool.QueueUserWorkItem(ClientHandel, e);
        }

        private void ClientHandel(object? state)
        {
            var result = _repository.Create((Socket)state, _packetProcessor, exceptionHander);
            result.Run();
            AddTree(result);
        }

        private void exceptionHander(object sender, Exception ex)
        {
            if(ex.Message.ToLower().Contains("disconnected"))
            {
                var Accepted = sender as AcceptedClient;
                _repository.Delete(Accepted.Id);
                RemoveTree(Accepted);
            }
        }
        private void AddTree(AcceptedClient acceptedClient)
        {
            StringBuilder b = new StringBuilder();
            b.Append(@$"<TreeViewItem  x:Name=""ClientInfo_{acceptedClient.Id}""  xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' xmlns:materialDesign='http://materialdesigninxaml.net/winfx/xaml/themes' >
                    <TreeViewItem.Header>
                        <StackPanel Orientation=""Horizontal"">
                            <Viewbox Width=""16"" Height=""16"">
                                <materialDesign:PackIcon Kind=""Laptop"" Width=""25"" Height=""25"" />
                            </Viewbox>
                            <StackPanel Orientation=""Horizontal"">
                                <TextBlock Margin=""8,0,0,0"" Text=""{acceptedClient.Id}"" />
                                <TextBlock Margin=""8,0,0,0"" Text=""{((IPEndPoint)acceptedClient.Accepted.RemoteEndPoint).Address}"" />
                                <TextBlock Margin=""8,0,0,0"" Text=""{((IPEndPoint)acceptedClient.Accepted.RemoteEndPoint).Port}"" />
                            </StackPanel>
                        </StackPanel>
                    </TreeViewItem.Header>
                </TreeViewItem>");
            Dispatcher.Invoke(() => {
                var tn = (TreeViewItem)XamlReader.Parse(b.ToString());
                treeviewer.Items.Add(tn);
            });
        }

        private void RemoveTree(AcceptedClient acceptedClient)
        {
            Dispatcher.Invoke(() => {
                var res = treeviewer.Items.OfType<TreeViewItem>().FirstOrDefault(x => x.Name == $"ClientInfo_{acceptedClient.Id}");
                treeviewer.Items.Remove(res);
            });
        }


        private void exceptionHandler(object sender, Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            BtnAction.Background = new SolidColorBrush(Colors.Green);
            BtnAction.Content = "Start Server";
        }

        private void BtnAction_Click(object sender, RoutedEventArgs e)
        {
            int port = 0;
            if (Validator.IsIPv4Address(IP.Text))
            {
                if (!int.TryParse(Port.Text, out port))
                {
                    MessageBox.Show("The port must be a number between 0 and 65535.", "Invalid port", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            else
            {
                MessageBox.Show("The entered IP is not valid.", "Invalid Ip", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            _server.Start(IP.Text, port);
            BtnAction.Content = "Stop Server";
            BtnAction.Background = new SolidColorBrush(Colors.Red);
        }
    }
}
