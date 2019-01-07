using System;

namespace MicroFocus.Adm.Octane.Api.Core.Connector.Authentication
{
    public interface ConnectionListener
    {
        /// <summary>
        /// Open the system browser redirecting to the given url
        /// </summary>
        /// <param name="url"></param>
        void OpenBrowser(String url);

        /// <summary>
        /// Closes the browser after connection succeeded or time to connect elapsed
        /// </summary>
        /// <param name="url"></param>
        void CloseBrowser();
    }
}
