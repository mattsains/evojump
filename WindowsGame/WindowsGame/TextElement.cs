using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsGame
{
    public delegate void TextEvent();

    public class TextElement
    {
        public SpriteFont Font;
        public Vector2 Centre;
        public Color Color;

        public Vector2 Size;
        public string Text;

        public Vector2 TopLeft { get { return Centre - _actualSize / 2; } }
        public Vector2 TopRight { get { return Centre + new Vector2(_actualSize.X, -_actualSize.Y) / 2; } }
        public Vector2 BottomLeft { get { return Centre + new Vector2(-_actualSize.X, _actualSize.Y) / 2; } }
        public Vector2 BottomRight { get { return Centre + _actualSize / 2; } }

        public float Top { get { return TopLeft.Y; } }
        public float Left { get { return TopLeft.X; } }
        public float Right { get { return BottomRight.X; } }
        public float Bottom { get { return BottomRight.Y; } }

        public event TextEvent OnClick;
        public event TextEvent OnEnter;
        public event TextEvent OnExit;

        private bool _gotMouse = false;
        protected GraphicsDevice _graphicsDevice;
        protected SpriteBatch _spriteBatch;
        protected Vector2 _actualSize;
        protected float _scale;

        public TextElement(GraphicsDevice graphicsDevice, SpriteFont font, Vector2 centre, Color color, Vector2 size, string text)
        {
            this.Font = font;
            this.Centre = centre;
            this.Color = color;
            this.Size = size;
            this.Text = text;
            this._graphicsDevice = graphicsDevice;
            this._spriteBatch = new SpriteBatch(graphicsDevice);

            this.OnClick += () => { };
            this.OnEnter += () => { };
            this.OnExit += () => { };

        }

        public void Update()
        {
            //calculate position of text for drawing
            Vector2 fontSize = Font.MeasureString(Text);
            float fontRatio = fontSize.X / fontSize.Y;

            if (fontRatio >= Size.X / Size.Y)
            {
                //text is wider than expected
                _actualSize = new Vector2(Size.X, Size.X / fontRatio);
                _scale = _actualSize.Y / fontSize.Y;
            }
            else
            {
                //text is higher than expected
                _actualSize = new Vector2(Size.Y * fontRatio, Size.Y);
                _scale = _actualSize.X / fontSize.X;
            }

            MouseState ms = Mouse.GetState();
            Point mousePos = new Point(ms.X, ms.Y);

            if (mousePos.X >= Left && mousePos.X <= Right &&
                mousePos.Y >= Top && mousePos.Y <= Bottom)
            {
                if (!_gotMouse)
                    OnEnter();
                _gotMouse = true;
                if (ms.LeftButton == ButtonState.Pressed)
                    OnClick();
            }
            else
            {
                if (_gotMouse)
                    OnExit();
                _gotMouse = false;
            }
        }

        public virtual void Draw()
        {
            _spriteBatch.Begin();
            _spriteBatch.DrawString(Font, Text, Centre - _actualSize / 2, Color, 0, Vector2.Zero, _scale, SpriteEffects.None, 0);
            _spriteBatch.End();
        }
    }

    public class ClickableTextElement : TextElement
    {
        protected Color HoverColor;
        protected bool IsGlowing = false;
        public ClickableTextElement(GraphicsDevice graphicsDevice, SpriteFont font, Vector2 centre, Color color, Color hoverColor, Vector2 size, string text)
            : base(graphicsDevice, font, centre, color, size, text)
        {
            this.HoverColor = hoverColor;
            OnEnter += () => { IsGlowing = true; };
            OnExit += () => { IsGlowing = false; };
        }

        public override void Draw()
        {
            if (IsGlowing)
            {
                _spriteBatch.Begin();
                _spriteBatch.DrawString(Font, Text, Centre - _actualSize * 1.005f / 2, HoverColor, 0, Vector2.Zero, _scale * 1.005f, SpriteEffects.None, 0);
                _spriteBatch.End();
            }
            else
                base.Draw();
        }
    }
}
