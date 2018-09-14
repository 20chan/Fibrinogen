using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Text;
using System.Reflection;

namespace Fibrinogen
{
    public delegate string WebCallback(HttpListenerRequest request);
    internal class Server
    {
        /// <summary>
        /// Port, default value is 80
        /// </summary>
        public static int Port = 80;

        static readonly HttpListener listener;
        static WebCallback callback;

        static Server()
        {
            listener = new HttpListener();
        }

        public static void Register(WebCallback webcallback)
        {
            callback += webcallback;
        }

        public static void Start()
        {
            listener.Prefixes.Add($"http://localhost:{Port}/");
            listener.Start();
            ThreadPool.QueueUserWorkItem(RoutingThread);
        }

        public static void Stop()
        {
            listener.Stop();
            listener.Close();
        }

        static void RoutingThread(object o)
        {
            while (listener.IsListening)
                ThreadPool.QueueUserWorkItem((ctx) => Listen(ctx), listener.GetContext());
        }

        static void Listen(object ctx)
        {
            var context = ctx as HttpListenerContext;
            try
            {
                var name = context.Request.Url.Segments[1].Replace("/", "");
                var urlparams = context.Request.Url.Segments.Skip(2).Select(s => s.Replace("/", ""));
                var res = callback(context.Request);
                byte[] buf = Encoding.UTF8.GetBytes(res);
                context.Response.ContentLength64 = buf.Length;
                context.Response.OutputStream.Write(buf, 0, buf.Length);
            }
            catch { }
            finally
            {
                context.Response.OutputStream.Close();
            }
        }
    }
}
