using System.IO;
using System.Net;
using System.Text;
using System.Linq;

namespace Fibrinogen
{
    public class FileServer : IRouter
    {
        public string BaseDirectory;

        public bool RequestHandler(HttpListenerRequest request, HttpListenerResponse response)
        {
            var parameter = request.Url.Segments.Skip(1).Select(s => s.Replace("/", ""));
            var lastpath = Path.Combine(parameter.Take(parameter.Count() - 1).ToArray());
            var fullpath = Path.Combine(BaseDirectory, lastpath, parameter.Last());

            if (!File.Exists(fullpath))
                return false;

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

            return true;
        }
    }
}
