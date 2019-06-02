using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Text;

namespace Fibrinogen
{
    public class Server
    {
        const string _404Page = @"<body><h2>404 Not Found ¯\_(ツ)_/¯</h2></body>";
        /// <summary>
        /// Port, default value is 80
        /// </summary>
        public int Port = 80;
        readonly HttpListener listener;

        List<IRouter> routers;

        public Server(params IRouter[] router)
        {
            listener = new HttpListener();
            listener.IgnoreWriteExceptions = true;
            routers = new List<IRouter>();

            routers.AddRange(router);
        }

        public void AddRouter(IRouter router)
            => routers.Add(router);

        public void Start()
        {
            listener.Prefixes.Add($"http://*:{Port}/");
            listener.Start();
            ThreadPool.QueueUserWorkItem(RoutingThread);
        }

        public void Stop()
        {
            listener.Stop();
            listener.Close();
        }

        void RoutingThread(object o)
        {
            while (listener.IsListening)
                ThreadPool.QueueUserWorkItem((ctx) => Listen(ctx), listener.GetContext());
        }

        void Listen(object ctx)
        {
            var context = ctx as HttpListenerContext;
            try
            {
                string name = "";
                if (context.Request.Url.Segments.Length > 1)
                    name = context.Request.Url.Segments[1].Replace("/", "");
                var urlparams = context.Request.Url.Segments.Skip(2).Select(s => s.Replace("/", ""));

                foreach (var router in routers)
                {
                    if (router.RequestHandler(context.Request, context.Response))
                        return;
                }

                var buf = Encoding.UTF8.GetBytes(_404Page);
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                context.Response.ContentLength64 = buf.Length;
                context.Response.ContentType = "text/html; charset=utf-8";
                context.Response.ContentEncoding = Encoding.UTF8;
                context.Response.OutputStream.Write(buf, 0, buf.Length);
                context.Response.OutputStream.Flush();
            }
            finally
            {
                try {
                    context.Response.OutputStream.Close();
                }
                catch { }
            }
        }
    }
}
