﻿namespace Uno
{
    internal class Program
    {
        static void Usage()
        {
            Console.WriteLine("usage:\n\tUno play [username (12 chars max)] [ip] [port]\n\tUno server [port]");
            Environment.Exit(0);
        }

        static int Main(string[] args)
        {
            if (args.Length != 4 && args.Length != 2)
                Usage();

            if (args[0] == "play")
            {
                if (args.Length != 4)
                    Usage();

                if (args[1].Length > 12)
                    Usage();

                Game? game = new Game(args[1], args[2], Convert.ToUInt16(args[3]));
                game.Play();

                game = null;
            }
            else if (args[0] == "server")
            {
                if (args.Length != 2)
                    Usage();

                Server? server = new Server(Convert.ToUInt16(args[1]));
                server.Run();

                server = null;
            }

            GC.Collect();
            GC.WaitForPendingFinalizers();

            return 0;
        }
    }
}
