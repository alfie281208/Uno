using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Uno
{
    internal class Server
    {
        private Socket _listener;
        private IPEndPoint _listenerEndPoint;

        private string[] _players;
        private int _joined;
        private CardStack _placedStack;
        private CardStack _reserveStack;

        public Server(UInt16 port)
        {
            // Initialise the socket and end point
            try
            {
                // Default to the local IP address
                IPAddress ipAddress = IPAddress.Parse("192.168.0.57");
                _listenerEndPoint = new IPEndPoint(ipAddress, port);
                _listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Environment.Exit(1);
            }

            // Player list
            _players = new string[6];
            _joined = 0;

            // Initialise the stacks
            _placedStack = new CardStack();
            _reserveStack = new CardStack();
            _placedStack.FillWithNone();
            _reserveStack.FillWithCards();
            _reserveStack.Shuffle();
        }

        ~Server()
        {
            // Shutdown the listener
            _listener.Shutdown(SocketShutdown.Both);
            _listener.Close();

            // Dispose objects
            _listener.Dispose();
        }

        public void Run()
        {
            try
            {
                _listener.Bind(_listenerEndPoint);
                _listener.Listen(10);

                while (true)
                {
                    // Connect to client
                    Socket clientConnection = _listener.Accept();

                    // Get the username
                    byte[] buffer = new byte[12];
                    string data = "";
                    
                    while (true)
                    {
                        int bufferCount = clientConnection.Receive(buffer);
                        data += Encoding.ASCII.GetString(buffer, 0, bufferCount);

                        if (data.IndexOf("\0") > -1)
                            break;
                    }

                    if (data == "")
                        continue;

                    InterpretRequest(data);
                    clientConnection.Send(Encoding.ASCII.GetBytes(data));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Environment.Exit(1);
            }
        }

        public string InterpretRequest(string data)
        {
            switch (data[0])
            {
                case 'J':
                    _players[_joined++] = data[1..data.Length];
                    return "0";

                case 'L':
                    for (int i = 0; i < 6; i++)
                        if (_players[i] == data[1..data.Length])
                            _players[i] = "";

                    Console.WriteLine("LEFT");

                    return "0";

                default:
                    return "0";
            }
        }

        private string DrawCards(int count)
        {
            Card[] cards = _placedStack.Remove(count);
            string data = "";

            foreach (Card card in cards)
                data += card.ToString();

            return data;
        }
    }
}
