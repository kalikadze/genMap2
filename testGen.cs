//source http://ploobs.com.br/arquivos/1742

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace genMap
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public partial class testGen : Microsoft.Xna.Framework.Game
    {
        Random rand = new Random();
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D bmp;
        Rectangle canvas;
        Color[] pixels;
        KeyboardState kstate;
        PerlinNoise pn;


        public testGen()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            canvas = GraphicsDevice.PresentationParameters.Bounds;
            bmp = new Texture2D(GraphicsDevice, canvas.Width, canvas.Height, false, SurfaceFormat.Color);
            pixels = new Color[canvas.Width * canvas.Height];
            kstate = new KeyboardState();

            pn = new PerlinNoise(canvas.Width, canvas.Height);

            for (int y = 0; y < canvas.Height; y++)
                for (int x = 0; x < canvas.Width; x++)
                    pixels[y * canvas.Width + x] = new Color(rand.Next(255), rand.Next(255), rand.Next(255));

            pixels[0] = Color.Black;
            pixels[canvas.Width] = Color.White;
            pixels[(canvas.Height - 1) * canvas.Width + 0] = Color.Black;
            pixels[(canvas.Height - 1) * canvas.Width + canvas.Width - 1] = Color.White;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            canvas = GraphicsDevice.PresentationParameters.Bounds;
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            kstate = Keyboard.GetState();
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || kstate.IsKeyDown(Keys.Escape))

                this.Exit();

            // TODO: Add your update logic here
            float freq = 0.007f;
            float amp = 0.8f;
            float pers = 0.5f;
            int octave = 5;

            if (kstate.IsKeyDown(Keys.Space))
                divideMap(pixels, canvas.Width, canvas.Height, 0, 0, canvas.Width - 1, canvas.Height - 1);
            if (kstate.IsKeyDown(Keys.PageUp))
            {
                for (int i = 0; i < canvas.Width; i++)
                {
                    for (int j = 0; j < canvas.Height; j++)
                    {
                        float value = pn.GetRandomHeight(i, j, 1, freq, amp, pers, octave);
                        value = 0.5f * (1 + value);
                        pixels[i + j * canvas.Width] = new Color(value, value, value);
                    }
                }
            }
            
            //
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            GraphicsDevice.Textures[0] = null;
            bmp.SetData<Color>(pixels, 0, canvas.Width * canvas.Height);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            spriteBatch.Draw(bmp, new Rectangle(0, 0, canvas.Width, canvas.Height), Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
