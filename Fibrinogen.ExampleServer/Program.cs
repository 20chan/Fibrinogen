using System;
using System.Net;

namespace Fibrinogen.ExampleServer
{
    class Program
    {
        static void Main(string[] args)
        {
            FileServer.BaseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            FileServer.Start();
            Console.Write("FileServer is running! press ENTER key to stop");
            Console.Read();
            FileServer.Stop();
        }
    }
}
