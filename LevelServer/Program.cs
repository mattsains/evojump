using EasierSockets;
using LevelServer.DataObjects;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;

namespace LevelServer
{
    class Program
    {
        const int SERVER_PORT = 15290;
        static ServerSocket ServerSocket;
        static Random R = new Random();

        static void Main(string[] args)
        {
            ServerSocket = new ServerSocket("any", SERVER_PORT, "\n", OnClientStateChange, OnClientRequest);
            Console.WriteLine("Server listening on port {0}", SERVER_PORT);

            

            using (var levelscontext = new LevelsDataContext())
            {
                levelscontext.Levels.Add(new LevelEntity());
                levelscontext.SaveChanges();
            }

            for (; ; )
            {
                Console.Write("> ");
                string message = Console.ReadLine();

                switch (message)
                {
                    case "exit": Exit(); return;
                    case "echo": Console.WriteLine("Hello"); break;
                }
            }
        }

        static void Exit()
        {
            ServerSocket.Close();
            MaintainLevels.Close();
        }

        /// <summary>
        /// Called when a client's state changes. This method must be thread-safe
        /// </summary>
        /// <param name="id">The unique identifier of the client</param>
        /// <param name="connected">If this is true, a client has just connected. Otherwise a client has just disconnected</param>
        static void OnClientStateChange(int id, bool connected)
        {
            Console.WriteLine("Client {0}", connected ? "connected" : "disconnected");
        }

        /// <summary>
        /// Called when a request is received from a client. This method must be thread-safe
        /// </summary>
        /// <param name="id">The unique identifier of the client</param>
        /// <param name="request">A string containing the request received</param>
        /// <returns>A string response, or "" if nothing must be sent</returns>
        static string OnClientRequest(int id, string request)
        {
            string[] requestParts = request.Split(' ');

            switch (requestParts[0])
            {
                case "level":
                    using (var levelsContext = new LevelsDataContext())
                    {
                        int lid = int.Parse(requestParts[1]);
                        Level l = levelsContext.GetLevelByID(lid);
                        return l.Encode();
                    }
                case "random":
                    using (var levelsContext = new LevelsDataContext())
                    {
                        Task<LevelEntity> task = levelsContext.Levels.SqlQuery(
                            "SELECT name " +
                                "FROM random AS r1 JOIN " +
                                    "(SELECT CEIL(RAND() * " +
                                        "(SELECT MAX(id) FROM random)) AS id) " +
                                "AS r2 " +
                            "WHERE r1.id >= r2.id ORDER BY r1.id ASC LIMIT 1").FirstAsync();
                        task.Wait();
                        return task.Result.Level.Encode();
                    }
                case "rate":
                    using (var ratingsContext = new RatingsContext())
                    {
                        int lid = int.Parse(requestParts[1]);
                        int rating = int.Parse(requestParts[2]);
                        ratingsContext.AddRating(lid, rating);
                        ratingsContext.SaveChanges();
                    }
                    return "done";
                default: return "";
            }
        }



    }
}
