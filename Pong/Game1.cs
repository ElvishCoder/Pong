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

namespace Pong
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont font;

        
        //pong entities
        Rectangle blueBar;
        Rectangle redBar;
        Rectangle ball;
       
        // our pong clone textures
        Texture2D grass;
        Texture2D spritesheet;

        // our sound effects
        SoundEffect ballBounce;
        SoundEffect playerScored;

        // source rectangles of our graphics
        Rectangle blueSrcRect = new Rectangle(0, 0, 32, 128);
        Rectangle redSrcRect = new Rectangle(32, 0, 32, 128);
        Rectangle ballSrcRect = new Rectangle(64, 0, 32, 32);

        // ball speed
        Vector2 ballVelocity = Vector2.Zero;

        // scores
        int blueScore = 0;
        int redScore = 0;

        public Game1()
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
            // initializing our entities
            blueBar = new Rectangle(32, GraphicsDevice.Viewport.Bounds.Height / 2 - 64, 32,128);
            redBar = new Rectangle(GraphicsDevice.Viewport.Bounds.Width - 64, GraphicsDevice.Viewport.Bounds.Height / 2 - 64, 32,128);
            ball = new Rectangle(GraphicsDevice.Viewport.Bounds.Width / 2 - 16, GraphicsDevice.Viewport.Bounds.Height / 2 - 16,32,32);
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

            //load our textures from the content pipeline
            grass = Content.Load<Texture2D>("Grass");
            spritesheet = Content.Load<Texture2D>("Objects");

            //load our sound effects from the content Pipeline
            ballBounce = Content.Load<SoundEffect>("Bounce");
            playerScored = Content.Load<SoundEffect>("Supporters");

            //load our score font
            font = Content.Load<SpriteFont>(@"ScoreFont");

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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // handling keyboard inputs for player one
            if (GamePad.GetState(PlayerIndex.One).DPad.Up == ButtonState.Pressed)
                blueBar.Y -= 10; //move the blue bar up
            else if (GamePad.GetState(PlayerIndex.One).DPad.Down == ButtonState.Pressed)
                blueBar.Y += 10; // move the blue bar down

            // handling keyboard inputs for player two
            if (GamePad.GetState(PlayerIndex.Two).DPad.Up == ButtonState.Pressed)
                redBar.Y -= 10; //move the red bar up
            else if (GamePad.GetState(PlayerIndex.Two).DPad.Down == ButtonState.Pressed)
                redBar.Y += 10; // move the red bar down

            // handling ball initialization
            if (GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed)
                InitBall();

            //limiting the bar movement to the screen bounds
            if (redBar.Y < 0) // upper bound
                redBar.Y = 0; // limit
            if (blueBar.Y < 0)
                blueBar.Y = 0;
            if (redBar.Y + redBar.Height > GraphicsDevice.Viewport.Bounds.Height)
                redBar.Y = GraphicsDevice.Viewport.Bounds.Height - redBar.Height;
            if (blueBar.Y + blueBar.Height > GraphicsDevice.Viewport.Bounds.Height)
                blueBar.Y = GraphicsDevice.Viewport.Bounds.Height - blueBar.Height;

            // move the ball
            ball.X += (int)ballVelocity.X;
            ball.Y += (int)ballVelocity.Y;

            //collision handling
            if (ball.Y < 0 || // if the ball reach the upper bound of the screen
            ball.Y + ball.Height > GraphicsDevice.Viewport.Bounds.Height)
            {
               ballVelocity.Y = -ballVelocity.Y; // make if bounce by inverting the Y velocity
               ballBounce.Play();
            }

            if (ball.Intersects(redBar) || // if the ball collide with blue bar
            ball.Intersects(blueBar)) // or the red one
            {
              ballVelocity.X = -ballVelocity.X; // make if bounce by inverting the X
              ballBounce.Play();
            }
        
            //score handling
            if (ball.X < 0) //red scores
            {
                redScore++;
                playerScored.Play();
                InitBall(); //re-init the ball
            }
            else if (ball.X + ball.Width > GraphicsDevice.Viewport.Bounds.Width) // blue scores
            {
                blueScore++;
                playerScored.Play();
                InitBall();
            }
            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            spriteBatch.Draw(grass, GraphicsDevice.Viewport.Bounds,Color.White);
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            //draw the red bar
            spriteBatch.Draw(spritesheet, redBar, redSrcRect, Color.White);

            //draw the blue bar
            spriteBatch.Draw(spritesheet, blueBar, blueSrcRect, Color.White);

            //draw the ball
            spriteBatch.Draw(spritesheet, ball, ballSrcRect, Color.White);
            spriteBatch.End();

            // draw the score
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            spriteBatch.DrawString( // draw our score string
            font, // our score font
            blueScore.ToString() + " - " + redScore.ToString(), // building the string
            new Vector2 ( // text position
            GraphicsDevice.Viewport.Bounds.Width / 2 - 25, // half the screen and a little to the left
            10.0f),
            Color.Yellow); // yellow text
            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void InitBall()
        {
            int speed = 5; //default velocity
            Random rand = new Random();

            //randomize the ball orientation
            switch (rand.Next(4))
            {
                case 0: ballVelocity.X = speed; ballVelocity.Y = speed; break;
                case 1: ballVelocity.X = -speed; ballVelocity.Y = speed; break;
                case 2: ballVelocity.X = speed; ballVelocity.Y = -speed; break;
                case 3: ballVelocity.X = -speed; ballVelocity.Y = -speed; break;
            }
            // initialize the ball to the center of the screen
            ball.X = GraphicsDevice.Viewport.Bounds.Width / 2 - ball.Width / 2;
            ball.Y = GraphicsDevice.Viewport.Bounds.Height / 2 - ball.Height / 2;

        }

       
    }
}
