using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Cors;

namespace chatserver
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            EnableCrossSiteRequests(config);
            AddRoutes(config);
        }

        private static void EnableCrossSiteRequests(HttpConfiguration config)
        {
            //since I'm using api as cross-origin I'm enabling it
            var cors = new EnableCorsAttribute(
                origins: "*",
                headers: "*",
                methods: "*");
            config.EnableCors(cors);
        }

        public static void AddRoutes(HttpConfiguration config)
        {

            //Configuring inline routes
            
            config.MapHttpAttributeRoutes();

        }
    }
}
