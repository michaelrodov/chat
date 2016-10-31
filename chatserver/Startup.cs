using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using Microsoft.Owin.Cors;
using Microsoft.AspNet.SignalR;

[assembly: OwinStartup(typeof(chatserver.Startup))]

namespace chatserver
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //app.UseCors(CorsOptions.AllowAll);

            app.Map("/signalr", map =>
            {
                // Setup the CORS middleware to run before SignalR.
                // By default this will allow all origins. You can 
                // configure the set of origins and/or http verbs by
                // providing a cors options with a different policy.
                map.UseCors(CorsOptions.AllowAll);
                var hubConfiguration = new HubConfiguration();
                //{
                //    // You can enable JSONP by uncommenting line below.
                //    // JSONP requests are insecure but some older browsers (and some
                //    // versions of IE) require JSONP to work cross domain
                //    //EnableJSONP = true
                //};

                hubConfiguration.EnableJavaScriptProxies = true;
                hubConfiguration.EnableDetailedErrors = true;

                map.RunSignalR(hubConfiguration);
            });
        }
    }
}