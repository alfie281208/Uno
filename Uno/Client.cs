using System;
using System.Linq.Expressions;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;

namespace Uno
{
    internal class Client
    {
        private Socket _sender;
        private IPEndPoint _serverEndPoint;

        public Client(string username, string ip, int port)
        {
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

            // TODO : Request($"Join:{username}");
        }

        ~Client()
        {
            _sender.Shutdown(SocketShutdown.Both);
            _sender.Close();
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
        }

        public Card GetPlacedTop() => Card.ToCard(Request("PlacedTop"));

        public void PlaceCard(Card card) => Request("Place:" + card.ToString());

        public Card[] DrawCards(int count)
        {
            string response = Request("Draw:" + Convert.ToInt32(count));
            Card[] cards = new Card[count];

            Console.WriteLine(response);

            for (int i = 0; i < response.Length; i += 2)
            {
                Card.ToCard(response.Substring(i, i + 1));
            }

            return cards;
        }

        private string Request(string what)
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
