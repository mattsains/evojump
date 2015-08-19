using LevelServer.DataObjects;
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
        Random R = new Random();

        public void Initialize()
        {
            Database.SetInitializer<LevelsDataContext>(new DropCreateDatabaseAlways<LevelsDataContext>());

            using (var levelsContext = new LevelsDataContext())
            {
                int count = levelsContext.Levels.Count();

                if (count < MIN_LEVELS)
                {
                    for (int i = count; i < MIN_LEVELS; i++)
                        levelsContext.Levels.Add(GenerateRandomLevel());
                }
                else if (count > MAX_LEVELS)
                {
                    using (var ratingsContext = new RatingsContext())
                    {
                        int toRemove = MAX_LEVELS - count;
                        var levelsToRemove = levelsContext.Levels.OrderBy(level => ratingsContext.GetAverageRatingForLevel(level)).Take(toRemove);

                        levelsContext.Levels.RemoveRange(levelsToRemove);
                    }
                }
                levelsContext.SaveChanges();
            }
            MaintainThread = new Thread(Maintain);
            MaintainThread.Start();
        }

        private Level GenerateRandomLevel()
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
        private void Maintain()
        {
            int lastRatingsCount = 0;
            while (!IsClosing)
            {
                Thread.Sleep(1000);
                using (var ratingsContext = new RatingsContext())
                {
                    int ratingsCount = ratingsContext.Ratings.Count();
                    if (ratingsCount < lastRatingsCount)
                        lastRatingsCount = ratingsCount;

                    if (ratingsCount - lastRatingsCount > 10)
                    {
                        for (int i=0; i<3; i++)
                        {

                        }
                    }
                    lastRatingsCount = ratingsCount;
                }
            }
        }
    }
}
