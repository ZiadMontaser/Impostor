using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Impostor.Api.Net.Messages;
using Impostor.Hazel;
using Impostor.Hazel.Udp;
using Impostor.Server.Net.Hazel;
using Impostor.Server.Net.Manager;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;

namespace Impostor.Server.Net
{
    internal class Announcements
    {
        private readonly ClientManager _clientManager;
        private readonly ObjectPool<MessageReader> _readerPool;
        private readonly ILogger<Announcements> _logger;
        private readonly ILogger<HazelConnection> _connectionLogger;
        private UdpConnectionListener _connection;
        private readonly IMessageWriter _disconnectBucket;
        private readonly string _announcmentFilePath;

        private byte announcmentId;
        private string announcmentMessage;

        public Announcements(
            ILogger<Announcements> logger,
            ClientManager clientManager,
            ObjectPool<MessageReader> readerPool,
            ILogger<HazelConnection> connectionLogger)
        {
            _logger = logger;
            _clientManager = clientManager;
            _readerPool = readerPool;
            _connectionLogger = connectionLogger;
            _announcmentFilePath = Path.Combine(Directory.GetCurrentDirectory(), "announcment.txt");
            _disconnectBucket = MessageWriter.Get(MessageType.Reliable);
            _disconnectBucket.Write((byte)9);

            ReadAnnouncmentFile();
        }

        public async ValueTask StartAsync(IPEndPoint ipEndPoint)
        {
            var mode = ipEndPoint.AddressFamily switch
            {
                AddressFamily.InterNetwork => IPMode.IPv4,
                AddressFamily.InterNetworkV6 => IPMode.IPv6,
                _ => throw new InvalidOperationException()
            };

            _connection = new UdpConnectionListener(ipEndPoint, _readerPool, mode);
            _connection.NewConnection = OnNewConnection;

            await _connection.StartAsync();

            await Task.Delay(10 * 1000);

            SendAnnouncment("Hi Zirno");
        }

        public async ValueTask StopAsync()
        {
            await _connection.DisposeAsync();
        }

        private async ValueTask OnNewConnection(NewConnectionEventArgs e)
        {
            e.HandshakeData.ReadInt16();
            int id = e.HandshakeData.ReadPackedInt32();

            var connection = new HazelConnection(e.Connection, _connectionLogger);

            using var w = MessageWriter.Get(MessageType.Reliable);
            if (id < announcmentId)
            {
                w.StartMessage(1);
                w.WritePacked(announcmentId);
                w.Write(announcmentMessage);
                w.EndMessage();
            }
            else
            {
                w.StartMessage(0);
                w.EndMessage();
            }

            _logger.LogInformation("Client Requested an Announcements with {0} and server send {1}", id, announcmentId);

            await connection.SendAsync(w);

            await e.Connection.SendAsync(_disconnectBucket);
        }

        private void SendAnnouncment(string message) {
            FileStream fileStream = File.Create(_announcmentFilePath);
            fileStream.Close();
            using var sw = new StreamWriter(_announcmentFilePath);
            sw.WriteLine(announcmentId++);
            sw.WriteLine(message);
            sw.Close();
        }

        private void ReadAnnouncmentFile()
        {
            if (!File.Exists(_announcmentFilePath) || File.ReadAllText(_announcmentFilePath).Length == 0)
            {
                SendAnnouncment(string.Empty);
            }
            else
            {
                using var sr = new StreamReader(_announcmentFilePath);
                if (!byte.TryParse(sr.ReadLine(), out announcmentId))
                {
                    announcmentId = 0;
                }

                announcmentMessage = sr.ReadToEnd();
            }
        }
    }
}
