using System.IO;
using System.Net;
using System.Linq;
using Markdig;

namespace Fibrinogen
{
    public static class MarkdownServer
    {
        public static string BaseDirectory;
        const string _404Page = @"<body><h2>404 Not Found ¯\_(ツ)_/¯</h2></body>";

        public static void Start()
        {
            Server.Register(RequestHandler);
            Server.Start();
        }

        public static void Stop()
            => Server.Stop();

        static string RequestHandler(HttpListenerRequest request)
        {
            var parameter = request.Url.Segments.Skip(1).Select(s => s.Replace("/", ""));
            var lastpath = Path.Combine(parameter.Take(parameter.Count() - 1).ToArray());
            var fullpath = Path.Combine(BaseDirectory, lastpath);
            var file = Path.Combine(fullpath, $"{parameter.Last()}.md");

            if (!Directory.Exists(fullpath) || !File.Exists(file))
                return _404Page;

            var content = File.ReadAllText(file);
            return HtmlHelper.WrapWithTag("body", Markdown.ToHtml(content));
        }
    }
}
