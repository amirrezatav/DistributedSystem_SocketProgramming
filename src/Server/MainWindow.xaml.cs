using Server.Connection;
using Shared.Connection;
using Shared.Model;
using Shared.Utilities;
using System;
using System.Windows;
using System.Windows.Media;

namespace Server
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Transfer _transfer;
        private IServer _server;
        public MainWindow()
        {
            InitializeComponent();
            //_server = new Listener(socketAcceptedHandler);
            //_transfer = new Transfer(_server, new ServerPacketProcessor());
            //Title = "Server";
        }

        private void socketAcceptedHandler(object sender, AcceptedSocket e)
        {
            throw new NotImplementedException();
        }
    }
}
