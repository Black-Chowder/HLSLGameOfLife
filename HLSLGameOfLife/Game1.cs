using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;

namespace HLSLGameOfLife
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch spriteBatch;

        private Effect gol;

        private int width = 800;
        private int height = 800;

        private float scale = 1f;

        private Texture2D startState;
        private RenderTarget2D prevState;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _graphics.PreferredBackBufferWidth = (int)(width * scale);
            _graphics.PreferredBackBufferHeight = (int)(height * scale);
            _graphics.ApplyChanges();
        }

        protected override void Initialize()
        {
            Random rand = new Random();

            startState = new Texture2D(GraphicsDevice, width, height);
            Color[] data = new Color[width * height];
            for (int i = 0; i < data.Count(); i++)
            {
                data[i] = rand.Next(0, 2) == 0 ? Color.White : Color.Black;
            }
            startState.SetData(data);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            gol = Content.Load<Effect>("GameOfLife");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            RenderTarget2D rt = new RenderTarget2D(GraphicsDevice, width, height);
            GraphicsDevice.SetRenderTarget(rt);

            spriteBatch.Begin(SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp);
            gol.Parameters["dim"].SetValue(new Vector2(width, height));
            gol.CurrentTechnique.Passes[0].Apply();
            if (prevState == null) spriteBatch.Draw(startState, Vector2.Zero, Color.White);
            else spriteBatch.Draw(prevState, Vector2.Zero, Color.White);
            spriteBatch.End();

            GraphicsDevice.SetRenderTarget(null);

            spriteBatch.Begin(SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp);
            spriteBatch.Draw(rt,
                Vector2.Zero,
                new Rectangle(0, 0, width, height),
                Color.White,
                0,
                Vector2.Zero,
                scale,
                SpriteEffects.None,
                0f);
            spriteBatch.End();

            if (prevState != null) prevState.Dispose();
            prevState = rt;

            base.Draw(gameTime);
        }
    }
}