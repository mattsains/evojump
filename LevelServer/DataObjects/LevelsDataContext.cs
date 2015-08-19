using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelServer.DataObjects
{

    class LevelEntity
    {
        [NotMapped]
        public Level Level;

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id
        {
            get { return Level.LevelId; }
            set { Level.LevelId = value; }
        }

        public string LevelData
        {
            get { return Level.Encode(); }
            set { Level = Level.Decode(value); }
        }

        public LevelEntity()
        {
            Level = new Level();
        }

        public static implicit operator LevelEntity(Level l)
        {
            return new LevelEntity() { Level = l };
        }
        public static implicit operator Level(LevelEntity l)
        {
            return l.Level;
        }
    }

    class LevelRating
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public LevelEntity Level { get; set; }
        public int Rating { get; set; }

        public LevelRating(Level level, int rating)
        {
            this.Level = level;
            this.Rating = rating;
        }
    }

    [DbConfigurationType(typeof(MySql.Data.Entity.MySqlEFConfiguration))]
    class LevelsContext : DbContext
    {
        public LevelsContext() : base("MySQLDb") { }
        public DbSet<LevelEntity> Levels { get; set; }
        public DbSet<LevelRating> Ratings { get; set; }

        public LevelEntity GetLevelByID(int id)
        {
            return Levels.First(level => level.Id == id);
        }

        public IQueryable<LevelRating> GetRatingsForLevel(Level l)
        {
            return this.Ratings.Where(rating => rating.Level.Level == l);
        }

        public double GetAverageRatingForLevel(Level l)
        {
            return GetRatingsForLevel(l).Average(rating => rating.Rating);
        }

        public void AddRating(int lid, int rating)
        {
            Level l = GetLevelByID(lid);
            Ratings.Add(new LevelRating(l, rating));
        }
    }
}
