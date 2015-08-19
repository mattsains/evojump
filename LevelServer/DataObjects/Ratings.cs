using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelServer.DataObjects
{
    public class LevelRating
    {
        public Level Level { get; set; }
        public int Rating { get; set; }

        public LevelRating(Level level, int rating)
        {
            this.Level = level;
            this.Rating = rating;
        }
    }

    public class RatingsContext : DbContext
    {
        public RatingsContext() : base("MySQLDb") { }

        public DbSet<LevelRating> Ratings { get; set; }

        public IQueryable<LevelRating> GetRatingsForLevel(Level l)
        {
            return this.Ratings.Where(rating => rating.Level == l);
        }

        public double GetAverageRatingForLevel(Level l)
        {
            return GetRatingsForLevel(l).Average(rating => rating.Rating);
        }

        public void AddRating(int lid, int rating)
        {
            using (var levelContext = new LevelsDataContext())
            {
                Level l = levelContext.GetLevelByID(lid);
                Ratings.Add(new LevelRating(l, rating));
            }
        }
    }

}
