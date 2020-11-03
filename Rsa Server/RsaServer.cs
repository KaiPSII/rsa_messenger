using System;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;
using System.Data;

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
        byte[] ReadBytesFromNetworkStream(NetworkStream ns)
        {
            var bytes = new byte[1024];
            var l = ns.Read(bytes, 0, 1024);
            var t = ns.FlushAsync();
            t.Wait();
            var bytesT = new byte[l];
            for (int i = 0; i < l; i++)
            {
                bytesT[i] = bytes[i];
            }
            return bytesT;
        }
        public void UsernameServer()
        {
            //search for clients
            var listener = new TcpListener(IPAddress.Any, 24848);
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
                while (!ns.DataAvailable)
                {
                    Thread.Sleep(1000);
                }
                var r = Encoding.UTF8.GetString(ReadBytesFromNetworkStream(ns));
                var rs = r.Split('|');
                if (rs[0] == "ADDING USER")
                {
                    for (int i = 0; i < userInfos.Count; i++)
                    {
                        if (userInfos[i].name == rs[1])
                        {
                            userInfos.RemoveAt(i);
                        }
                    }
                    var userinfo = new UserInfo();
                    userinfo.name = rs[1];
                    userinfo.publicIP = rs[2];
                    userinfo.privateIP = rs[3];
                    userInfos.Add(userinfo);
                    Console.WriteLine(string.Format("adding user: {0}", rs[1]));
                }
                else if (rs[0] == "READING USER")
                {
                    Thread.Sleep(1000);
                    for (int i = 0; i < userInfos.Count; i++)
                    {
                        if (userInfos[i].name == rs[1])
                        {
                            ns.Write(Encoding.UTF8.GetBytes(userInfos[i].publicIP + "|"));
                            ns.Write(Encoding.UTF8.GetBytes(userInfos[i].privateIP));
                        }
                    }
                }
                ns.Close();
                client.Close();
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
