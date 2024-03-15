using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Uno
{
    internal class Client
    {
        private Socket _sender;
        private IPEndPoint _serverEndPoint;

        public readonly List<Card> Cards;
        private string _username; 

        public Client(string username, string ip, UInt16 port)
        {
            // Initialise socket and end point
            try
            {
                IPAddress ipAddress = IPAddress.Parse(ip);
                _serverEndPoint = new IPEndPoint(ipAddress, port);
                _sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Environment.Exit(1);
            }

            Cards = new List<Card>();
            _username = username;
        }

        ~Client()
        {
            // Send leave command to server and shutdown connection
            Message("L" + _username);
            _sender.Shutdown(SocketShutdown.Both);
            _sender.Close();

            // Dispose objects
            _sender.Dispose();
        }

        public void Connect()
        {
            try
            {
                _sender.Connect(_serverEndPoint);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Environment.Exit(1);
            }

            // Send join command to server
            Message("J" + _username);
        }

        private string Message(string what)
        {
            byte[] buffer = new byte[1024];
            int nBytes = 0;

            try
            {
                // Send a request for something from the server
                _sender.Send(Encoding.ASCII.GetBytes(what));
                // Get the reply data
                nBytes = _sender.Receive(buffer);
            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex.ToString());
                Environment.Exit(1);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Environment.Exit(1);
            }

            return Encoding.ASCII.GetString(buffer, 0, nBytes);
        }

    }
}
