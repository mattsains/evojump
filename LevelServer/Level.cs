using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

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
        public List<ILevelComponent> LevelComponents;
        public Point SpawnPoint;
    }

    public interface ILevelComponent
    {
        Rectangle Position { get; }
    }
}
