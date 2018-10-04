using System.Net;

namespace Fibrinogen
{
    public interface IRouter
    {
        /// <summary>
        /// Handle request and return is any page matched
        /// </summary>
        /// <returns>If not found any matching page, return false</returns>
        bool RequestHandler(HttpListenerRequest request, HttpListenerResponse response);
    }
}
