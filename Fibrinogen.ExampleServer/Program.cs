using System;
using System.Net;

namespace Fibrinogen.ExampleServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new LogServer("log.txt");
            server.Start();
            Console.Write("Server is running! press ENTER key to stop");
            Console.Read();
            server.Stop();
        }
    }
}
