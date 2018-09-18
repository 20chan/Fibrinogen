using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace Fibrinogen
{
    public class LogServer : Server
    {
        readonly Stream file;
        readonly StreamWriter writer;
        readonly string path;
        public LogServer(string logPath)
        {
            path = logPath;
            if (!File.Exists(logPath))
                file = new FileStream(logPath, FileMode.Create);
            else
                file = new FileStream(logPath, FileMode.Append);

            writer = new StreamWriter(file);
            writer.WriteLine($"{DateTime.Now} Logging started");
        }

        public override void RequestHandler(HttpListenerRequest request, HttpListenerResponse response)
         {
            if (request.HttpMethod == "GET"
                && request.Url.Segments.Length == 2
                && request.Url.Segments[1].StartsWith("viewlogs"))
            {
                var buf = File.ReadAllBytes(path);
                response.ContentLength64 = buf.Length;
                response.ContentType = "text/plain; charset=utf-8";
                response.ContentEncoding = Encoding.UTF8;
                response.OutputStream.Write(buf, 0, buf.Length);
                response.OutputStream.Flush();
                return;
            }
            if (request.HttpMethod != "POST"
                || request.Url.Segments.Length != 2
                || request.Url.Segments[1].StartsWith("log/"))
            {
                response.StatusCode = 400;
                return;
            }

            var body = "";
            using (var reader = new StreamReader(request.InputStream))
                body = reader.ReadToEnd();

            Console.WriteLine($"got one!: {body}");
            
            writer.WriteLine(body);
            writer.Flush();
        }
    }
}
