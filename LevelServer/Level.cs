using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace LevelServer
{
    /// <summary>
    /// Defines how a single level in the game looks
    /// In a level, positions are integers. 
    /// They correspond to some sort of grid sysem (to be determined)
    /// The grid system starts in the bottom left corner (0,0). 
    /// Positive X is distance to right, Positive Y is altitude
    /// At the beginning of the level, the player is spawned with their feet centred at SpawnPoint
    /// The elements in LevelComponents are rendered in the world according to the grid system.
    /// This class only describes the structure of the level. Extend it if you want the level elements
    /// to contain game logic or textures or anything concrete.
    /// This class is serializable so that it can be sent to a client over a network.
    /// </summary>
    [Serializable()]
    public class Level
    {
        public int LevelId;

        public List<ILevelComponent> LevelComponents = new List<ILevelComponent>();
        public Point SpawnPoint;

        /// <summary>
        /// Encodes the level in base64 for transfer over a network. Use Level.Decode() to get it back
        /// </summary>
        /// <returns>A string containing the object in base64 encoding.</returns>
        public string Encode()
        {
            IFormatter formatter = new BinaryFormatter();
            Stream s = new MemoryStream();
            formatter.Serialize(s, this);
            if (s.Length > int.MaxValue)
                throw new ArgumentException("Can't serialize extremely large objects");
            byte[] buffer = new byte[s.Length];
            s.Seek(0, SeekOrigin.Begin);
            s.Read(buffer, 0, (int)s.Length);
            return Convert.ToBase64String(buffer);
        }

        /// <summary>
        /// Decodes a level from base64 encoding
        /// </summary>
        /// <param name="encoded">The string containing the encoded level</param>
        /// <returns>The level object</returns>
        public static Level Decode(string encoded)
        {
            IFormatter formatter = new BinaryFormatter();
            byte[] buffer = Convert.FromBase64String(encoded);
            Stream s = new MemoryStream();
            s.Write(buffer, 0, buffer.Length);
            s.Seek(0, SeekOrigin.Begin);
            return (Level)formatter.Deserialize(s);
        }
    }

    public interface ILevelComponent
    {
        Rectangle Position { get; }
    }
}
