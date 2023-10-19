using Shared.Connection;
using System.Threading.Tasks;
using System;
using Shared.Models;
using Shared.Packets;
using System.Collections.Generic;
using System.Threading;
using Client.Persistance;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Client.ViewModel;
using System.Windows;

namespace Client.Connection
{
    public class ClientPacketProcessor : IPacketProcessor
    {
        private readonly ApplicationContext _context;
        private PersonViewModel viewModel;

        public ClientPacketProcessor(ApplicationContext context, PersonViewModel viewModel)
        {
            _context = context;
            this.viewModel = viewModel;
        }

        public void Process(byte[] _buffer, int Validcount, object transfer)
        {
            lock (transfer)
            {
                var connection = (transfer as Client);
                lock (_buffer)
                {
                    var packet = PacketSerializer.Deserialize(_buffer, Validcount);
                    switch (packet.Type)
                    {
                        case Shared.Enums.PacketType.Query:
                            var data = _context.People.FromSqlRaw(packet.Data as string).ToList();
                            packet.Data = data;
                            packet.Type = Shared.Enums.PacketType.QueryResult;
                            connection.Send(PacketSerializer.Serialize(packet));
                            break;
                        case Shared.Enums.PacketType.QueryResult:
                            if (packet.Data != null)
                            {
                                var results = ((Newtonsoft.Json.Linq.JArray)packet.Data).ToObject<List<Person>>();
                                viewModel.AddRange(results);
                            }
                            else
                            {
                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    MessageBox.Show("Not found!");
                                });
                            }
                            break;
                        case Shared.Enums.PacketType.Command:
                            break;
                        case Shared.Enums.PacketType.Import:
                            var records = ((Newtonsoft.Json.Linq.JArray)packet.Data).ToObject<List<Person>>();
                            try
                            {
                                _context.People.AddRange(records);
                                _context.SaveChanges();
                            }
                            catch (Exception)
                            {
                                foreach (var item in records)
                                {
                                    try
                                    {
                                        _context.People.Add(item);
                                        _context.SaveChanges();
                                    }
                                    catch
                                    {

                                    }
                                }
                            }
                            break;
                        default: break;
                    }
                }
            }

        }

    }
}
