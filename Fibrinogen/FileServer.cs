using System.IO;
using System.Net;
using System.Text;
using System.Linq;

namespace Fibrinogen
{
    public class FileServer
    {
        public static string BaseDirectory;
        const string _404Page = @"<body><h2>404 Not Found ¯\_(ツ)_/¯</h2></body>";

        public static void Start()
        {
            Server.Start();
            Server.Register(RequestHandler);
        }

        public static void Stop()
            => Server.Stop();

        static void RequestHandler(HttpListenerRequest request, HttpListenerResponse response)
        {
            var parameter = request.Url.Segments.Skip(1).Select(s => s.Replace("/", ""));
            var lastpath = Path.Combine(parameter.Take(parameter.Count() - 1).ToArray());
            var fullpath = Path.Combine(BaseDirectory, lastpath, parameter.Last());

            if (!File.Exists(fullpath))
            {
                var buf = Encoding.UTF8.GetBytes(_404Page);

                response.StatusCode = (int)HttpStatusCode.NotFound;
                response.ContentLength64 = buf.Length;
                response.ContentType = "text/html; charset=utf-8";
                response.ContentEncoding = Encoding.UTF8;
                response.OutputStream.Write(buf, 0, buf.Length);
                response.OutputStream.Flush();
                return;
            }

            using (var input = new FileStream(fullpath, FileMode.Open))
            {
                response.ContentType = "application/octet-stream";
                response.ContentLength64 = input.Length;
                var buf = new byte[1024 * 32];
                int nbytes;
                while ((nbytes = input.Read(buf, 0, buf.Length)) > 0)
                    response.OutputStream.Write(buf, 0, nbytes);
                response.OutputStream.Flush();
            }
        }
    }
}
