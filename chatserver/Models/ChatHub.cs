using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace chatserver.Models
{
    [HubName("chaturang")]
    public class ChatHub : Hub
    {
        
        public void broadcast(String name, String message)
        {
            Clients.All.broadcastMessage(name, message);
        }
    }
}