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

    [DbConfigurationType(typeof(MySql.Data.Entity.MySqlEFConfiguration))]
    class LevelsDataContext : DbContext
    {
        public LevelsDataContext() : base("MySQLDb") { }
        public DbSet<LevelEntity> Levels { get; set; }

        public LevelEntity GetLevelByID(int id)
        {
            return Levels.First(level => level.Id == id);
        }
    }
}
