using Client.Connection;
using Client.Persistance;
using Client.ViewModel;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using Shared.Connection;
using Shared.Model;
using Shared.Models;
using Shared.Packets;
using Shared.Utilities;
using System;
using System.Formats.Asn1;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IPacketProcessor _packetProcessor;
        private readonly IClient _client;
        private readonly PersonViewModel viewModel;
        private readonly ApplicationContext _context;
        public MainWindow()
        {

            InitializeComponent();
            viewModel = new PersonViewModel();
            _context = new ApplicationContext();
            _context.Database.EnsureCreated();
            _packetProcessor = new ClientPacketProcessor(_context, viewModel);
            _client = new Client.Connection.Client(disconnectedHandler, exceptionHander, _packetProcessor);
            Title = "Cleint";
            data.DataContext = viewModel;

        }
        private void exceptionHander(object sender, Exception ex)
        {
            Dispatcher.Invoke(() =>
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            });
        }
        private void disconnectedHandler(object sender)
        {
            Dispatcher.Invoke(() =>
            {
                Bar.IsEnabled = false;
                ServerIP.IsEnabled = true;
                ServerPort.IsEnabled = true;
                BtnAction.Content = "Connect to server";
                BtnAction.Background = new SolidColorBrush(Colors.Green);
            });
        }
        private void BtnAction_Click(object sender, RoutedEventArgs e)
        {
            int port = 0;
            string ip = ServerIP.Text;
            if (Validator.IsIPv4Address(ServerIP.Text))
            {
                if (!int.TryParse(ServerPort.Text, out port))
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
            Thread thread = new Thread(() => { _client.Connect(ip, port, connectedHandler); });
            thread.Start();
        }
        private void connectedHandler(object sender, string error)
        {
            Thread thread = new Thread(() => { _client.Run(); });
            thread.Start();
            Dispatcher.Invoke(() =>
            {
                ServerIP.IsEnabled = false;
                ServerPort.IsEnabled = false;
                Bar.IsEnabled = true;
                BtnAction.Content = "Disconnect";
                BtnAction.Background = new SolidColorBrush(Colors.Red);
            });
        }
        private void BtnImport_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "CSV Files (*.csv)|*.csv|All Files (*.*)|*.*",
                Title = "Select a CSV File"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                Thread thread = new Thread(() =>
                {
                    try
                    {
                        using (var reader = new StreamReader(filePath))
                        using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture) { HeaderValidated = null }))
                        {
                            ActionEnable(false);
                            var records = csv.GetRecords<Person>().ToList();
                            var buffer = PacketSerializer.Serialize(new Packet() { Type = Shared.Enums.PacketType.Import, Data = records });
                            _client.Send(buffer);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("An error occurred: " + ex.Message);
                    }
                    finally
                    {
                        ActionEnable(true);
                    }
                });
                thread.Start();
            }
        }


        public void ActionEnable(bool t)
        {
            Dispatcher.Invoke(() =>
            {
                BtnImport.IsEnabled = t;
                BtnSearch.IsEnabled = t;
                BtnNew.IsEnabled = t;
                OnlineDatabase.IsEnabled = t;
                Search.IsEnabled = t;
                Filter.IsEnabled = t;
            });
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {

        }
        private IQueryable<Person> QueryGenerator(int filter , string search)
        {
            switch (filter)
            {
                case 1:
                    if (long.TryParse(search, out long Id))
                        return _context.People.Where(x => x.Id == Id);
                    else
                        throw new Exception("ID must be a number.");
                case 2:
                    return _context.People.Where(c => EF.Functions.Like(c.FirstName, $"%{search}%") || EF.Functions.Like(c.LastName, $"%{search}%"));
                case 3:
                    return _context.People.Where(c => EF.Functions.Like(c.FirstName, $"%{search}%"));
                case 4:
                    return _context.People.Where(c => EF.Functions.Like(c.LastName, $"%{search}%"));
                case 5:
                    return _context.People.Where(c => EF.Functions.Like(c.Mail, $"%{search}%"));
                case 6:
                    return _context.People.Where(c => EF.Functions.Like(c.City, $"%{search}%"));
                default:
                    return _context.People;
            }
        }
        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {

            if (OnlineDatabase.IsChecked.Value)
            {
                Thread thread = new Thread(() =>
                {
                    try
                    {
                        ActionEnable(false);
                        string search = "";
                        Dispatcher.Invoke(() =>
                        {
                            search  = QueryGenerator(Filter.SelectedIndex, Search.Text).ToQueryString();
                        });
                        var buffer = PacketSerializer.Serialize(new Packet() { Type = Shared.Enums.PacketType.Query, Data = search });
                        _client.Send(buffer);

                    }
                    catch (Exception ex)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        });
                    }
                    finally
                    {
                        ActionEnable(true);
                    }
                });
                thread.Start();
            }
            else
            {

            }
        }
    }
}
