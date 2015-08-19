﻿using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace EasierSockets
{
    public class ClientSocket
    {
        static Encoding ENCODING = Encoding.Unicode;


        /// <summary>
        /// Called when the server sends a message
        /// </summary>
        /// <param name="msg">the message the server sent</param>
        public delegate void ServerMessage(string msg);
        /// <summary>
        /// Called when the connection is closed by the server
        /// </summary>
        public delegate void ServerDisconnect();
        /// <summary>
        /// Makes client side of socket comms easier.
        /// </summary>

        private Socket sock;
        private string separator;
        private Thread t;
        private ServerDisconnect serverDisconnect;
        private ServerMessage serverMessage;
        /// <summary>
        /// Connects to a TCP host
        /// </summary>
        /// <param name="host">the server to connect to</param>
        /// <param name="port">the port to use</param>
        /// <param name="separator">what signals the end of a message?</param>
        public ClientSocket(string host, int port, string separator, ServerDisconnect discon, ServerMessage mess)
        {
            IPHostEntry hostip = Dns.GetHostEntry(host);
            IPAddress ip = hostip.AddressList.First(addr => addr.AddressFamily == AddressFamily.InterNetwork);
            IPEndPoint endpoint = new IPEndPoint(ip, port);
            //TODO: Allow different modes like UDP
            sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.separator = separator;
            this.serverDisconnect = discon;
            this.serverMessage = mess;
            sock.Connect(endpoint);
            t = new Thread(new ThreadStart(WaitForServer));
            t.Start();
        }
        /// <summary>
        /// Send a message to the server
        /// </summary>
        /// <param name="message">The message to send. The separator is added automatically</param>
        /// <returns>Whether the send was successful (also calls delegate if unsuccessful</returns>
        public bool Send(string message)
        {
            byte[] tx = ENCODING.GetBytes(message + separator);
            try
            {
                sock.Send(tx);
            }
            catch (SocketException) { serverDisconnect(); return false; }
            return true;
        }

        public class ConnectionInfo
        {
            public IPAddress ServerIP;
            public IPAddress ClientIP;
            public int ServerPort;
            public int ClientPort;
        }

        public ConnectionInfo GetClientConnectionInfo(int id)
        {
            IPEndPoint localEndpoint, remoteEndpoint;
            if (sock.LocalEndPoint.AddressFamily != AddressFamily.InterNetwork || sock.RemoteEndPoint.AddressFamily != AddressFamily.InterNetwork)
                return null;
            localEndpoint = (IPEndPoint)sock.LocalEndPoint;
            remoteEndpoint = (IPEndPoint)sock.RemoteEndPoint;
            return new ConnectionInfo()
            {
                ClientIP = localEndpoint.Address,
                ClientPort = localEndpoint.Port,
                ServerIP = remoteEndpoint.Address,
                ServerPort = remoteEndpoint.Port,
            };
        }

        /// <summary>
        /// endless function to receive messages from server
        /// </summary>
        private void WaitForServer()
        {
            string data = "";
            while (sock.Connected)
            {
                byte[] rx = new byte[1024];
                int bytesRec = 0;
                try
                {
                    bytesRec = sock.Receive(rx);
                }
                catch (SocketException)
                {
                    // let the user know the server has gone.
                    serverDisconnect();
                    return;
                }
                if (bytesRec == 0)
                {
                    serverDisconnect();
                    return;
                }
                data += ENCODING.GetString(rx, 0, bytesRec);
                if (data.Contains(separator))
                {
                    string[] messages = data.Split(new string[] { separator }, StringSplitOptions.None);
                    for (int i = 0; i < messages.Length - 1; i++)
                        serverMessage(messages[i]);
                    data = messages[messages.Length - 1];
                }
            }
            //inform the user that the server is gone
            serverDisconnect();
        }
        /// <summary>
        /// Destructor: closes the connection
        /// </summary>
        ~ClientSocket()
        {
            sock.Shutdown(SocketShutdown.Both);
            sock.Close();
        }
    }
}
