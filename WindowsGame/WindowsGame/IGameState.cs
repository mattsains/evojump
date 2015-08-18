using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace WindowsGame
{
    public interface IGameState
    {
        void LoadContent(GraphicsDevice graphicsDevice, ContentManager content);
        void Update(GameTime gameTime, GraphicsDevice graphicsDevice);
        void Draw(GameTime gameTime, GraphicsDevice graphicsDevice);
    }
}
