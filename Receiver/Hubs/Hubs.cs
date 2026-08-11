using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
namespace reciever.Hubs
{
    public class Hubs : Hub
    {
        //public async Task SendMessage(string orderjson)
        //{
        //    await Clients.All.SendAsync("ReceiveMessage",orderjson);
        //}
    }
}
