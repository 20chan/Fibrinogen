using System;
using System.Net;

namespace Fibrinogen.ExampleServer
{
    class Program
    {
        static void Main(string[] args)
        {
            MarkdownServer.BaseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            MarkdownServer.Start();
            Console.Write("MarkdownServer is running! press ENTER key to stop");
            Console.Read();
            MarkdownServer.Stop();
        }
    }
}
