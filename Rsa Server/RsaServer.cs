using System;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;

namespace Rsa_Server
{
    class RsaServer
    {
        public List<UserInfo> userInfos = new List<UserInfo>();
        static void Main(string[] args)
        {
            var p = new RsaServer();
            p.UsernameServer();
        }
        public void UsernameServer()
        {
            //search for clients
            var listener = new TcpListener(IPAddress.Any, 24846);
            listener.Start();
            while (true)
            {
                TcpClient client = null;
                while (client == null)
                {
                    if (listener.Pending())
                    {
                        client = listener.AcceptTcpClient();
                    }
                    else
                    {
                        Thread.Sleep(1000);
                    }
                }
                var ns = client.GetStream();
                Thread.Sleep(1000);
                var adding = false;
                if (ns.DataAvailable)
                {
                    adding = true;
                }
                if (adding)
                {
                    var userinfo = new UserInfo();
                    //name
                    Thread.Sleep(1000);
                    while (!ns.DataAvailable)
                    {
                        Thread.Sleep(1000);
                    }
                    var otherNameBytes = new byte[1024];
                    var l = ns.Read(otherNameBytes, 0, 1024);
                    var t = ns.FlushAsync();
                    t.Wait();
                    userinfo.name = Encoding.UTF8.GetString(otherNameBytes).Remove(l);
                    Console.WriteLine(string.Format("Other client name received: {0}", userinfo.name));
                    //public ip
                    Thread.Sleep(1000);
                    while (!ns.DataAvailable)
                    {
                        Thread.Sleep(1000);
                    }
                    var publicipbytes = new byte[1024];
                    l = ns.Read(publicipbytes, 0, 1024);
                    t = ns.FlushAsync();
                    t.Wait();
                    userinfo.publicIP = Encoding.UTF8.GetString(publicipbytes).Remove(l);
                    Console.WriteLine(string.Format("Other client name received: {0}", userinfo.publicIP));
                    //private ip
                    Thread.Sleep(1000);
                    while (!ns.DataAvailable)
                    {
                        Thread.Sleep(1000);
                    }
                    var privateipbytes = new byte[1024];
                    l = ns.Read(privateipbytes, 0, 1024);
                    t = ns.FlushAsync();
                    t.Wait();
                    userinfo.privateIP = Encoding.UTF8.GetString(privateipbytes).Remove(l);
                    Console.WriteLine(string.Format("Other client name received: {0}", userinfo.privateIP));

                    userInfos.Add(userinfo);
                }
                else
                {

                }
            }
        }
    }
    class UserInfo
    {
        public string name;
        public string publicIP;
        public string privateIP;
    }
}
