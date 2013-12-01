using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ServerCore
{
    public class NetworkClient
    {
        public delegate void MessageReceivedDelegate(NetworkClient client, string message);

        public delegate void ClientDisconnectedDelegate(NetworkClient client);

        private readonly TcpClient _socket;
        private NetworkStream _networkStream;
        private readonly int _id;

        public bool IsActive { get; set; }
        public int Id { get { return _id; } }
        public TcpClient Socket { get { return _socket; } }

        public event MessageReceivedDelegate MessageReceived;
        public event ClientDisconnectedDelegate ClientDisconnected;

        public NetworkClient(TcpClient socket, int id)
        {
            _socket = socket;
            _id = id;
        }

        private void MarkAsDisconnected()
        {
            IsActive = false;
            if (ClientDisconnected != null)
            {
                ClientDisconnected(this);
            }
        }

        public async Task ReceiveInput()
        {
            IsActive = true;
            _networkStream = _socket.GetStream();

            using (var reader = new StreamReader(_networkStream))
            {
                while (IsActive)
                {
                    try
                    {
                        var content = await reader.ReadLineAsync();

                        // If content is null, that means the connection has been gracefully disconnected
                        if (content == null)
                        {
                            MarkAsDisconnected();
                            return;
                        }

                        if (MessageReceived != null)
                        {
                            MessageReceived(this, content);
                        }
                    }

                    // If the tcp connection is ungracefully disconnected, it will throw an exception
                    catch (IOException)
                    {
                        MarkAsDisconnected();
                        return;
                    }
                }
            }
        }

        public async Task SendLine(string line)
        {
            if (!IsActive)
            {
                return;
            }

            try
            {
                // Don't use a using statement as we do not want the stream closed
                //    after the write is completed
                var writer = new StreamWriter(_networkStream);
                await writer.WriteLineAsync(line);
                writer.Flush();
            }
            catch (IOException)
            {
                // socket closed
                MarkAsDisconnected();
            }
        }
    }
}
