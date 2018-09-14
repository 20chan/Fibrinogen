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
    public class Server
    {
        /// <summary>
        /// Port, default value is 80
        /// </summary>
        public static int Port = 80;

        static readonly HttpListener listener;
        static readonly List<string> prefixes;
        static readonly Dictionary<string, WebCallback> callbackMap;

        static Server()
        {
            listener = new HttpListener();
            prefixes = new List<string>();
            callbackMap = new Dictionary<string, WebCallback>();
        }

        public static void Register(string prefix, WebCallback callback)
        {
            callbackMap.Add(prefix, callback);
            prefixes.Add(prefix);
        }

        public static void Start()
        {
            if (prefixes.Count == 0)
                throw new Exception("You have to add at least one prefix");
            foreach (var p in prefixes)
                listener.Prefixes.Add($"http://localhost:{Port}/{p}");
            listener.Start();
            ThreadPool.QueueUserWorkItem(RoutingThread);
        }

        static void RoutingThread(object o)
        {
            while (listener.IsListening)
                ThreadPool.QueueUserWorkItem((ctx) => Listen(ctx));
        }

        static void Listen(object ctx)
        {
            var context = ctx as HttpListenerContext;
            try
            {
                var name = context.Request.Url.Segments[1].Replace("/", "");
                var urlparams = context.Request.Url.Segments.Skip(2).Select(s => s.Replace("/", ""));
                if (!callbackMap.ContainsKey(name))
                    return;
                var res = callbackMap[name](context.Request);
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
