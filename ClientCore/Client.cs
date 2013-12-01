using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ClientCore
{
    public class Client
    {
        private TcpClient _client;
        private readonly string _host;
        private readonly int _port;
        private readonly int _bufferSize = 1024;
        private readonly byte[] _buffer;

        public delegate void TextReceivedDelegate(string text);

        public event TextReceivedDelegate TextReceived;


        private NetworkStream ClientStream
        {
            get { return _client.GetStream(); }
        }

        public Client(string host, int port)
        {
            _host = host;
            _port = port;
            _client = new TcpClient();
            _buffer = new byte[_bufferSize];
        }

        public void Connect()
        {
            _client.ConnectAsync(_host, _port).ContinueWith(ClientConntected);
        }

        private void ClientConntected(Task obj)
        {
            obj.Wait();
            Console.WriteLine("Connected");
            ClientStream.ReadAsync(_buffer, 0, _bufferSize).ContinueWith(BufferReading);
        }

        private void BufferReading(Task task)
        {
            //bug receives something weird
            using (var ms = new MemoryStream(_buffer))
            {
                using (var sr = new StreamReader(ms))
                {
                    var result = sr.ReadLine();
                    TextReceived(result);
                }
            }

            ClientStream.ReadAsync(_buffer, 0, _bufferSize).ContinueWith(BufferReading);
        }

        public void Write(string text)
        {
            using (var sr = new StreamWriter(ClientStream))
            {
                sr.Write(text);
            }
        }
    }
}
