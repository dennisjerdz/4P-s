using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace _4PsPH.Hubs
{
    public class feedHub : Hub
    {
        public void Send(string msg)
        {
            Clients.All.addmsg(msg);
        }
    }
}