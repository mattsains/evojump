﻿using LevelServer.DataObjects;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading;

namespace LevelServer
{
    static class MaintainLevels
    {
        const int MIN_LEVELS = 20;
        const int MAX_LEVELS = 10000;
        static Random R = new Random();

        public static void Initialize()
        {
            Database.SetInitializer<LevelsContext>(new DropCreateDatabaseAlways<LevelsContext>());

            using (var levelsContext = new LevelsContext())
            {
                int count = levelsContext.Levels.Count();

                if (count < MIN_LEVELS)
                {
                    for (int i = count; i < MIN_LEVELS; i++)
                        levelsContext.Levels.Add(GenerateRandomLevel());
                }
                else if (count > MAX_LEVELS)
                {
                        int toRemove = MAX_LEVELS - count;
                        var levelsToRemove = levelsContext.Levels.OrderBy(level => levelsContext.GetAverageRatingForLevel(level)).Take(toRemove);

                        levelsContext.Levels.RemoveRange(levelsToRemove);
                }
                levelsContext.SaveChanges();
            }
            MaintainThread = new Thread(Maintain);
            MaintainThread.Start();
        }

        private static Level GenerateRandomLevel()
        {
            return new Level();
        }

        public static bool IsClosing { get; private set; }

        public static void Close()
        {
            IsClosing = true;
            MaintainThread.Join();
        }

        static Thread MaintainThread;

        /// <summary>
        /// This method runs for the life of the program, slowly evolving the level set.
        /// </summary>
        private static void Maintain()
        {
            int lastRatingsCount = 0;
            while (!IsClosing)
            {
                Thread.Sleep(1000);
                using (var levelsContext = new LevelsContext())
                {
                    int ratingsCount = levelsContext.Ratings.Count();
                    if (ratingsCount < lastRatingsCount)
                        lastRatingsCount = ratingsCount;

                    if (ratingsCount - lastRatingsCount > 10)
                    {
                        for (int i = 0; i < 3; i++)
                        {

                        }
                    }
                    lastRatingsCount = ratingsCount;
                }
            }
        }
    }
}
