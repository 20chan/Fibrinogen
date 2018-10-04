using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Fibrinogen
{
    /// <summary>
    /// localhost/viewlogs -> localhost/viewlogs-0
    /// localhost/log -> localhost/log-0
    /// </summary>
    public class LogServer : Server
    {
        const string _404Page = @"<body><h2>404 Not Found ¯\_(ツ)_/¯</h2></body>";
        readonly Regex viewlogsRegex, logRegex;
        readonly string logdir;
        Logger[] loggers = new Logger[10];
        public LogServer(string logDirectory)
        {
            logdir = logDirectory;

            viewlogsRegex = new Regex(@"viewlogs-([0-9]+)");
            logRegex = new Regex(@"log-([0-9]+)");

            for (int i = 0; i < 10; i++)
                loggers[i] = new Logger(Path.Combine(logdir, $"log-{i}.txt"));
        }

        public override void RequestHandler(HttpListenerRequest request, HttpListenerResponse response)
        {
            var segfixed = request.Url.Segments.Select(s => s.Replace("/", "")).ToArray();
            if (request.HttpMethod == "GET"
                && request.Url.Segments.Length == 2
                && segfixed[1].StartsWith("viewlogs"))
            {
                int num = 0;
                if (segfixed[1] == "viewlogs" || segfixed[1] == "viewlogs-")
                    num = 0;
                else
                {
                    var matches = viewlogsRegex.Match(segfixed[1]);
                    if (matches.Groups.Count < 2)
                    {
                        _404(); return;
                    }
                    num = int.Parse(matches.Groups[1].Value);
                }

                if (num >= loggers.Length)
                {
                    _404(); return;
                }

                loggers[num].ClearReader();
                response.ContentType = "text/plain; charset=utf-8";
                response.ContentEncoding = Encoding.UTF8;
                loggers[num].FileR.CopyTo(response.OutputStream);
                response.OutputStream.Flush();
                return;
            }
            if (request.HttpMethod == "POST"
                && request.Url.Segments.Length == 2
                && request.Url.Segments[1].StartsWith("log"))
            {
                int num = 0;
                if (segfixed[1] == "log" || segfixed[1] == "log-")
                    num = 0;
                else
                {
                    var matches = logRegex.Match(segfixed[1]);
                    if (matches.Groups.Count < 2)
                    {
                        _404(); return;
                    }
                    num = int.Parse(matches.Groups[1].Value);
                }

                if (num >= loggers.Length)
                {
                    _404(); return;
                }

                var body = "";
                using (var reader = new StreamReader(request.InputStream))
                    body = reader.ReadToEnd();

                loggers[num].Writer.WriteLine(body);
                loggers[num].Writer.Flush();
                return;
            }

            _404();

            void _404() {
                var buf = Encoding.UTF8.GetBytes(_404Page);
                response.StatusCode = (int)HttpStatusCode.NotFound;
                response.ContentLength64 = buf.Length;
                response.ContentType = "text/html; charset=utf-8";
                response.ContentEncoding = Encoding.UTF8;
                response.OutputStream.Write(buf, 0, buf.Length);
                response.OutputStream.Flush();
            }
        }
    }
}
