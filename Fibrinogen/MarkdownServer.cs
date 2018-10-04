using System.IO;
using System.Text;
using System.Net;
using System.Linq;
using Markdig;

namespace Fibrinogen
{
    public class MarkdownServer : IRouter
    {
        public string BaseDirectory;
        
        public bool RequestHandler(HttpListenerRequest request, HttpListenerResponse response)
        {
            string result = "";
            var parameter = request.Url.Segments.Skip(1).Select(s => s.Replace("/", ""));
            var lastpath = Path.Combine(parameter.Take(parameter.Count() - 1).ToArray());
            var fullpath = Path.Combine(BaseDirectory, lastpath);
            var file = Path.Combine(fullpath, $"{parameter.Last()}.md");

            if (!Directory.Exists(fullpath) || !File.Exists(file))
                return false;
            else
            {
                var content = File.ReadAllText(file);
                result = HtmlHelper.WrapWithTag("body", Markdown.ToHtml(content));
            }

            var buf = Encoding.UTF8.GetBytes(result);
            
            response.ContentLength64 = buf.Length;
            response.ContentType = "text/html; charset=utf-8";
            response.ContentEncoding = Encoding.UTF8;
            response.OutputStream.Write(buf, 0, buf.Length);
            response.OutputStream.Flush();

            return true;
        }
    }
}
