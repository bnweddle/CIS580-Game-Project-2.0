using Microsoft.Xna.Framework;
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

        Player player;
        List<Bush> bushes = new List<Bush>();
        Random random = new Random();

        // To see if Mute/Spacebar was pressed 
        KeyboardState newState;
        KeyboardState oldState;

        //To keep track of lives and levels
        int lives;

        // Sound effects for losing lives, hitting paddle, ending game;
        SoundEffect loseLife;
        SoundEffect gameOver;

        ParallaxLayer playerLayer;
        ParallaxLayer backgroundLayer;
        ParallaxLayer bushLayer;
        ParallaxLayer gameLayer;

        StaticSprite[] gameSprites;

        bool mute = false;
        bool endGame = false;

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
            graphics.PreferredBackBufferWidth = 1020;
            graphics.PreferredBackBufferHeight = 768;
            graphics.ApplyChanges();

            lives = 5;
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

            // TODO: use this.Content to load your game content here

            if(endGame == false)
            {
                var bluesky = Content.Load<Texture2D>("bluesky");
                var blueskySprites = new StaticSprite[]
                {
                    new StaticSprite(bluesky, 1.0f),
                    new StaticSprite(bluesky, new Vector2(1968, 0), 1.0f)
                };
                backgroundLayer = new ParallaxLayer(this);
                backgroundLayer.Sprites.AddRange(blueskySprites);
                backgroundLayer.DrawOrder = 1;
                Components.Add(backgroundLayer);

                var gameTextures = new List<Texture2D>()
                {
                    Content.Load<Texture2D>("start"),
                    Content.Load<Texture2D>("end"),
                };

                gameSprites = new StaticSprite[]
                {
                    new StaticSprite(gameTextures[0], 1.0f),
                    new StaticSprite(gameTextures[1], 1.0f)
                };

                gameLayer = new ParallaxLayer(this);
                gameLayer.Sprites.Add(gameSprites[0]);
                gameLayer.DrawOrder = 5;
                Components.Add(gameLayer);

                player = new Player(this, playerSheet);
                player.Initialize();
                playerLayer = new ParallaxLayer(this);
                playerLayer.Sprites.Add(player);
                playerLayer.DrawOrder = 3;
                Components.Add(playerLayer);

                bushLayer = new ParallaxLayer(this);

                float offset = 200;
                for (int i = 0; i < 10; i++)
                {

                    Bush bush = new Bush(this, bushSheet, new Vector2(300 + offset, 543), player);
                    bushLayer.Sprites.Add(bush);
                    bushes.Add(bush);
                    bushLayer.DrawOrder = 4;
                    offset += random.Next(200, 300);
                }

                Components.Add(bushLayer);

                var ground = Content.Load<Texture2D>("ground");
                var mountainsTextures = new List<Texture2D>()
                {
                    Content.Load<Texture2D>("mountains"),
                    Content.Load<Texture2D>("mountains2"),
                    Content.Load<Texture2D>("mountains"),
                    Content.Load<Texture2D>("mountains2")
                };

                var mountainSprites = new List<StaticSprite>();
                for (int i = 0; i < mountainsTextures.Count; i++)
                {
                    var position = new Vector2(i * 1447, 100);
                    var sprite = new StaticSprite(mountainsTextures[i], position, 0.25f);
                    mountainSprites.Add(sprite);
                }

                var mountainsLayer = new ParallaxLayer(this);
                foreach (var sprite in mountainSprites)
                {
                    mountainsLayer.Sprites.Add(sprite);
                }

                mountainsLayer.DrawOrder = 1;

                var groundSprites = new List<StaticSprite>();
                for (int i = 0; i < 5; i++)
                {
                    var position = new Vector2(i * 2216, 435);
                    var sprite = new StaticSprite(ground, position, 0.25f);
                    groundSprites.Add(sprite);
                }

                var groundLayer = new ParallaxLayer(this);
                foreach (var sprite in groundSprites)
                {
                    groundLayer.Sprites.Add(sprite);
                }

                groundLayer.DrawOrder = 2;

                Components.Add(mountainsLayer);
                Components.Add(groundLayer);

                gameLayer.ScrollController = new PlayerTrackingScrollController(player, 0.0f);
                mountainsLayer.ScrollController = new PlayerTrackingScrollController(player, 0.4f);
                playerLayer.ScrollController = new PlayerTrackingScrollController(player, 1.0f);
                groundLayer.ScrollController = new PlayerTrackingScrollController(player, 1.0f);
                bushLayer.ScrollController = new PlayerTrackingScrollController(player, 1.0f);

            }
            else
            {
                gameLayer.Sprites.Add(gameSprites[1]);
                gameLayer.DrawOrder = 5;
                Components.Add(gameLayer);
            }

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

            // Why do I have to hit spacebar twice?
            MuteSound(newState, oldState);

            if (newState.IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            if (newState.IsKeyDown(Keys.Space))
            {
                gameLayer.DrawOrder = 0;
                Components.Remove(gameLayer);
            }
            

            player.Update(gameTime);

            foreach (Bush b in bushes){

                var bb = new BoundingRectangle(b.Position.X, b.Position.Y, b.Bounds.Width, b.Bounds.Height);
                b.Update(gameTime);
                if (player.Bounds.CollidesWith(bb))
                {
                    player.Hit();
                    lives--;
                    if (lives > 0)
                        loseLife.Play();
                    else if (lives == 0)
                    {
                        endGame = true;
                        LoadContent();
                        gameOver.Play();
                        // player.Dead();
                        mute = true;
                        break;
                    }
                }
            }

            
            if (newState.IsKeyDown(Keys.Enter))
            {

                Components.Remove(gameLayer);
                //player.Restart();
                mute = false;
                lives = 5;
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

            spriteBatch.End();
            base.Draw(gameTime);
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
