using System.Linq;
using System.Net;
using System.Threading;

namespace Fibrinogen
{
    public abstract class Server
    {
        /// <summary>
        /// Port, default value is 80
        /// </summary>
        public int Port = 80;
        readonly HttpListener listener;

        public Server()
        {
            listener = new HttpListener();
        }

        public virtual void Start()
        {
            listener.Prefixes.Add($"http://*:{Port}/");
            listener.Start();
            ThreadPool.QueueUserWorkItem(RoutingThread);
        }

        public virtual void Stop()
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
                RequestHandler(context.Request, context.Response);
            }
            finally
            {
                context.Response.OutputStream.Close();
            }
        }

        public abstract void RequestHandler(HttpListenerRequest request, HttpListenerResponse response);
    }
}
