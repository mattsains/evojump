using EasierSockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelServer
{
    class Program
    {
        const int SERVER_PORT = 15290;
        static ServerSocket ServerSocket;

        static void Main(string[] args)
        {
            ServerSocket = new ServerSocket("any", SERVER_PORT, "\n", OnClientStateChange, OnClientRequest);
            string command = Console.ReadLine();

            switch (command)
            {
                case "exit": ServerSocket.Close(); break;
            }
        }

        static void OnClientStateChange(int id, bool connected)
        {
            Console.WriteLine("Client {0}", connected ? "connected" : "disconnected");
        }

        static string OnClientRequest(int id, string request)
        {
            return "";
        }
    }
}
