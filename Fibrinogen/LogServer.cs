using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace Fibrinogen
{
    public class LogServer : Server
    {
        readonly Stream filew, filer;
        readonly StreamWriter writer;
        readonly StreamReader reader;
        readonly string path;
        public LogServer(string logPath)
        {
            path = logPath;
            if (!File.Exists(logPath))
                filew = new FileStream(logPath, FileMode.Create, FileAccess.Write, FileShare.Read);
            else
                filew = new FileStream(logPath, FileMode.Append, FileAccess.Write, FileShare.Read);

            filer = new FileStream(logPath, FileMode.Open, FileAccess.Read, FileShare.Write);

            writer = new StreamWriter(filew);
            reader = new StreamReader(filer);
            writer.WriteLine($"{DateTime.Now} Logging started");
        }

        public override void RequestHandler(HttpListenerRequest request, HttpListenerResponse response)
         {
            if (request.HttpMethod == "GET"
                && request.Url.Segments.Length == 2
                && request.Url.Segments[1].StartsWith("viewlogs"))
            {
                filer.Position = 0;
                reader.DiscardBufferedData();
                response.ContentType = "text/plain; charset=utf-8";
                response.ContentEncoding = Encoding.UTF8;
                filer.CopyTo(response.OutputStream);
                response.OutputStream.Flush();
                return;
            }
            if (request.HttpMethod != "POST"
                || request.Url.Segments.Length != 2
                || !request.Url.Segments[1].StartsWith("log"))
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
