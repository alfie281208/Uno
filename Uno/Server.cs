using System.Net;
using System.Net.Sockets;
using System.Text;
using Raylib_cs;

namespace Uno
{
    internal class Server
    {
        private Socket _listener;
        private IPEndPoint _listenerEndPoint;

        private CardStack _placedStack;
        private CardStack _reserveStack;

        public Server(int port)
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

            // Initialise the stacks
            _placedStack = new CardStack();
            _reserveStack = new CardStack();

            _placedStack.FillWithCards();
            _placedStack.Shuffle();

            _reserveStack.FillWithCards();
            _reserveStack.Shuffle();
        }

        ~Server()
        {
            _listener.Shutdown(SocketShutdown.Both);
            _listener.Close();
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
                    Console.WriteLine($"Client @ {clientConnection.RemoteEndPoint} has requested");

                    // Incoming data buffer and result
                    byte[] buffer = new byte[1024];
                    string request = null;
                    
                    // Get the sent data
                    while (true)
                    {
                        int bufferCount = clientConnection.Receive(buffer);
                        request += Encoding.ASCII.GetString(buffer, 0, bufferCount);

                        if (request.IndexOf("\0") > -1)
                            break;
                    }

                    Console.WriteLine($"Request : {request}");

                    string response = Respond(request);
                    clientConnection.Send(Encoding.ASCII.GetBytes(response));

                    clientConnection.Shutdown(SocketShutdown.Both);
                    clientConnection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Environment.Exit(1);
            }
        }

        private string Respond(string request)
        {
            string[] command = request.Split(":");

            switch (command[0])
            {
                case "PlacedTop":
                    return _placedStack.GetTop().ToString();

                case "Place":
                    if (command.Length == 2)
                        _placedStack.Push(Card.ToCard(command[1]));
                    break;

                case "Draw":
                    if (command.Length == 2)
                        return Draw(Convert.ToInt32(command[1]));
                    break;
            }

            return "";
        }

        private string Draw(int count)
        {
            string data = null;
            Card[] cards = _reserveStack.Remove(count);

            foreach (Card card in cards)
                data += card.ToString() + ",";

            return data;
        }
    }
}
