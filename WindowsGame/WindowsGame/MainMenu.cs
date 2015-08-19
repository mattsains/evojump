using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsGame
{
    class MainMenu : IGameState
    {
        SpriteFont Audiowide;
        SpriteBatch spriteBatch;
        TextElement TitleText;
        TextElement StartGameText;
        public void LoadContent(GraphicsDevice graphicsDevice, ContentManager content)
        {
            Audiowide = content.Load<SpriteFont>("audiowide");
            spriteBatch = new SpriteBatch(graphicsDevice);
            TitleText = new TextElement(graphicsDevice, Audiowide, new Vector2(graphicsDevice.Viewport.Width / 2, 100), Color.WhiteSmoke, new Vector2(graphicsDevice.Viewport.Width - 100, 200), "Evo Jump");
            StartGameText = new ClickableTextElement(graphicsDevice, Audiowide, new Vector2(graphicsDevice.Viewport.Width / 2, 250), Color.WhiteSmoke, Color.Turquoise, new Vector2(graphicsDevice.Viewport.Width - 400, 150), "Start Game");
            Program.Game1.IsMouseVisible = true;
        }

        public void Update(GameTime gameTime, GraphicsDevice graphicsDevice)
        {
            TitleText.Centre = new Vector2(graphicsDevice.Viewport.Width / 2, 100);
            TitleText.Size = new Vector2(graphicsDevice.Viewport.Width - 100, 200);
            StartGameText.Centre = new Vector2(graphicsDevice.Viewport.Width / 2, 250);
            StartGameText.Size = new Vector2(graphicsDevice.Viewport.Width - 400, 150);
            TitleText.Update();
            StartGameText.Update();
        }

        public void Draw(GameTime gameTime, GraphicsDevice graphicsDevice)
        {
            graphicsDevice.Clear(new Color(0.05f, 0.05f, 0.07f));
            TitleText.Draw();
            StartGameText.Draw();
        }
    }
}
