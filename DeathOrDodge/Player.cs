/* Game: DeathOrDodge
 * 
 * */
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeathOrDodge
{
    /// <summary>
    /// For which direction the player is facing
    /// </summary>
    public enum State
    {
        South = 0,
        North = 1,
        West = 2,
        East = 3,
        Idle = 4
    }

    public class Player
    {
        // how much the animation moves per frames 
        const int ANIMATION_FRAME_RATE = 124;
        // the speed of the player
        const float PLAYER_SPEED = 200;
        // width of animation frames
        public const int FRAME_WIDTH = 67;
        // height of animation frames
        const int FRAME_HEIGHT = 100;


        Game1 game;
        Texture2D player;
        State state;
        TimeSpan timer;
        public Vector2 position;
        public BoundingRectangle Bounds;
        KeyboardState oldState;
        int frame;


        public Player(Game1 game)
        {
            this.game = game;
        }

        public void Initialize()
        {
            timer = new TimeSpan(0);
            position = new Vector2(200, 200);
            state = State.Idle;
            Bounds.Width = FRAME_WIDTH;
            Bounds.Height = FRAME_HEIGHT;
        }

        public void LoadContent(ContentManager content)
        {
            player = content.Load<Texture2D>("newPlayer");
        }

        public void Update(GameTime gameTime)
        {
            //Movement
            KeyboardState newState = Keyboard.GetState();
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Bounds.X = position.X;
            Bounds.Y = position.Y;

            if (newState.IsKeyDown(Keys.Up))
            {
                state = State.North;
                position.Y -= delta * PLAYER_SPEED;
            }
            else if (newState.IsKeyDown(Keys.Left))
            {
                state = State.West;
                position.X -= delta * PLAYER_SPEED;
            }
            else if (newState.IsKeyDown(Keys.Right))
            {
                state = State.East;
                position.X += delta * PLAYER_SPEED;
            }
            else if (newState.IsKeyDown(Keys.Down))
            {
                state = State.South;
                position.Y += delta * PLAYER_SPEED;
            }
            else state = State.Idle;

            // Making sure player doesn't go off screen
            if (position.Y < 0)
            {
                position.Y = 0;
            }
            if (position.X < 0)
            {
                position.X = 0;
            }
            if (position.Y > game.GraphicsDevice.Viewport.Height - FRAME_HEIGHT)
            {
                position.Y = game.GraphicsDevice.Viewport.Height - FRAME_HEIGHT;
            }
            if (position.X > game.GraphicsDevice.Viewport.Width - FRAME_WIDTH)
            {
                position.X = game.GraphicsDevice.Viewport.Width - FRAME_WIDTH;
            }

            // update animation timer when the player is moving
            if (state != State.Idle)
                timer += gameTime.ElapsedGameTime;

            // Check if animation should increase by more than one frame
            while (timer.TotalMilliseconds > ANIMATION_FRAME_RATE)
            {
                // increase frame
                frame++;
                // Decrease the timer by one frame duration
                timer -= new TimeSpan(0, 0, 0, 0, ANIMATION_FRAME_RATE);
            }

            frame %= 4;
            oldState = newState;
        }

        public void Draw(SpriteBatch spriteBatch)
        {

            Rectangle rectSource = new Rectangle(
                frame * FRAME_WIDTH,  // X value
                (int)state % 4 * FRAME_HEIGHT, // Y value
                FRAME_WIDTH,
                FRAME_HEIGHT
                );

            spriteBatch.Draw(player, position, rectSource, Color.White);

        }

    }

}

