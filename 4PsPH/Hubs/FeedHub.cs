using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;
using _4PsPH.Extensions;

namespace _4PsPH.Hubs
{
    [Authorize]
    public class FeedHub : Hub
    {
        public void Send(string msg)
        {
            Clients.All.addmsg(msg);
        }

        public override Task OnConnected()
        {
            string name = Context.User.Identity.Name;

            string city = Context.User.Identity.GetCityName();

            string role = "";

            if (Context.User.IsInRole("Social Worker"))
            {
                role = "Social Worker";
            }

            if (Context.User.IsInRole("OIC"))
            {
                role = "OIC";
            }

            if (Context.User.IsInRole("4P's Officer"))
            {
                role = "4P's Officer";
            }

            //create client specific group from username
            Groups.Add(Context.ConnectionId, name);
            
            //join role-city group
            Groups.Add(Context.ConnectionId, role+"-"+city);

            //join city group
            Groups.Add(Context.ConnectionId, city);

            //notify login - does not fit requirements. Signalr initializes per page, hence log in is notified per page.
            //var signalr = GlobalHost.ConnectionManager.GetHubContext<FeedHub>();
            //signalr.Clients.Group(city).addmsg("User: "+Context.User.Identity.GetFullName()+" has logged in.");

            return base.OnConnected();
        }
    }
}