using System;
using System.Net;

namespace Fibrinogen.ExampleServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Server.Register(Callback);
            Server.Start();
            Console.Write("Server is running! press ENTER key to stop");
            Console.Read();
            Server.Stop();
        }

        static string Callback(HttpListenerRequest request)
        {
            return $@"
<body>
    <h2>Segments: </h2>
    {string.Join(' ', request.Url.Segments)}
    <br>
    <h2>Query: </h2>
    {request.Url.Query}
</body>";
        }
    }
}
