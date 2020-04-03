﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace DeathOrDodge
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont font;

        Player player;
        List<Bush> bushes = new List<Bush>();

        // To see if Mute/Spacebar was pressed 
        KeyboardState newState;
        KeyboardState oldState;

        //To keep track of lives and levels
        Texture2D heart;
        int lives;

        // Whether the game is over or has started
        bool beginGame;
        bool endGame;

        Random random = new Random();

        // Sound effects for losing lives, hitting paddle, ending game;
        SoundEffect loseLife;
        SoundEffect gameOver;

        // For background start and end page
        Texture2D backgroundStart;
        Texture2D backgroundEnd;

        bool mute = false;

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
            graphics.PreferredBackBufferWidth = 1042;
            graphics.PreferredBackBufferHeight = 768;
            graphics.ApplyChanges();

            lives = 5;
            // Keep track of the game starting and ending
            endGame = false;
            beginGame = false;

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
            loseLife = Content.Load<SoundEffect>("lose_life");
            gameOver = Content.Load<SoundEffect>("game_over");
            var playerSheet = Content.Load<Texture2D>("newPlayer");
            var bushSheet = Content.Load<Texture2D>("bush");
            
            //foreach random if good spot contine else generate new spot

            font = Content.Load<SpriteFont>("DefaultFont");
            heart = Content.Load<Texture2D>("heart");
            // The image before starting the game
            backgroundStart = Content.Load<Texture2D>("start");
            backgroundEnd = Content.Load<Texture2D>("end");

            // TODO: use this.Content to load your game content here

            var bluesky = Content.Load<Texture2D>("bluesky");
            var blueskySprite = new StaticSprite(bluesky, 0.75f);
            var backgroundLayer = new ParallaxLayer(this);
            backgroundLayer.Sprites.Add(blueskySprite);
            backgroundLayer.DrawOrder = 0;
            Components.Add(backgroundLayer);

            player = new Player(this, playerSheet);
            player.Initialize();
            var playerLayer = new ParallaxLayer(this);
            playerLayer.Sprites.Add(player);
            playerLayer.DrawOrder = 3;
            Components.Add(playerLayer);

            var bushLayer = new ParallaxLayer(this);

            float offset = 150;
            for (int i = 0; i < 10; i++)
            {
               
                Bush bush = new Bush(this, bushSheet, new Vector2(300 + offset, 543));
                bushLayer.Sprites.Add(bush);
                bushes.Add(bush);
                bushLayer.DrawOrder = 4;
                offset += random.Next(150, 300);
            }
            Components.Add(bushLayer);

            var mountains = Content.Load<Texture2D>("mountains");
            var ground = Content.Load<Texture2D>("ground");

            var mountainSprite = new StaticSprite(mountains, new Vector2(0, 100), 0.25f);
            var groundSprite = new StaticSprite(ground, new Vector2(0, 435), 0.25f);

            var groundLayer = new ParallaxLayer(this);
            var mountainsLayer = new ParallaxLayer(this);

            groundLayer.Sprites.Add(groundSprite);
            mountainsLayer.Sprites.Add(mountainSprite);

            mountainsLayer.DrawOrder = 1;
            groundLayer.DrawOrder = 2;

            Components.Add(mountainsLayer);
            Components.Add(groundLayer);

            mountainsLayer.ScrollController = new PlayerTrackingScrollController(player, 0.4f);
            playerLayer.ScrollController = new PlayerTrackingScrollController(player, 1.0f);
            groundLayer.ScrollController = new PlayerTrackingScrollController(player, 1.0f);
            bushLayer.ScrollController = new PlayerTrackingScrollController(player, 1.0f);

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
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

            newState = Keyboard.GetState();

            BeginGame();
            // Why do I have to hit spacebar twice?
            MuteSound(newState, oldState);

            if (newState.IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            player.Update(gameTime);
            // bush.Update(gameTime);

            foreach (Bush b in bushes){
                if (player.Bounds.CollidesWith(b.Bounds))
                {
                    lives--;
                    if (lives > 0)
                        loseLife.Play();
                    else if (lives == 0)
                    {
                        gameOver.Play();
                        endGame = true;
                    }
                }
            }
         
            oldState = newState;
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.SeaGreen);

            // TODO: Add your drawing code here
            spriteBatch.Begin();

            if (!beginGame)
            {
                // Fill the screen with black before the game starts
                spriteBatch.Draw(backgroundStart, new Rectangle(0, 0,
                (int)graphics.PreferredBackBufferWidth, (int)graphics.PreferredBackBufferHeight), Color.White);

            }
            else if (lives <= 0)
            {
                spriteBatch.Draw(backgroundEnd, new Rectangle(0, 0,
                (int)graphics.PreferredBackBufferWidth, (int)graphics.PreferredBackBufferHeight), Color.White);
                endGame = true;
            }
            else
            {

                if (mute == false)
                {
                    spriteBatch.DrawString(font, "Press SPACE to mute", new Vector2(425, 0), Color.White);
                }
                if (mute == true)
                {
                    spriteBatch.DrawString(font, "Press SPACE to unmute", new Vector2(420, 0), Color.White);
                }

                int start = 50;
                for (int i = 0; i < lives; i++)
                {
                    spriteBatch.Draw(heart, new Rectangle(graphics.PreferredBackBufferWidth - start, graphics.PreferredBackBufferHeight - 50, 50, 50), Color.White);
                    start += 50;
                }
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }

        void BeginGame()
        {
            // Got general idea for outline of code from 
            // https://docs.microsoft.com/en-us/windows/uwp/get-started/get-started-tutorial-game-mg2d
            KeyboardState keyboardState = Keyboard.GetState();

            // Quit the game if Escape is pressed.
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || keyboardState.IsKeyDown(Keys.Escape))
                Exit();

            // Start the game if Space is pressed.
            // Exit the keyboard handler method early, preventing the dino from jumping on the same keypress.
            if (!beginGame)
            {
                if (keyboardState.IsKeyDown(Keys.Space))
                {
                    endGame = false;
                    beginGame = true;
                }
            }

            // Restart the game if Enter is pressed
            if (endGame)
            {
                if (keyboardState.IsKeyDown(Keys.Enter))
                {
                    lives = 5;
                    endGame = false;
                    mute = false;
                    SoundEffect.MasterVolume = 1.0f;
                }
            }
        }


        public void MuteSound(KeyboardState newS, KeyboardState oldS)
        {
            // idea of implementation from 
            //https://www.gamefromscratch.com/post/2015/07/25/MonoGame-Tutorial-Audio.aspx
            if (!oldState.IsKeyDown(Keys.Space) && newS.IsKeyDown(Keys.Space))
            {
                if (SoundEffect.MasterVolume == 1.0f)
                {
                    SoundEffect.MasterVolume = 0.0f;
                    mute = true;
                }
                else
                {
                    SoundEffect.MasterVolume = 1.0f;
                    mute = false;
                }
            }
        }
    }
}
