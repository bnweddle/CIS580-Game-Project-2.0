/* Game: DeathOrDodge
 * Author: Nathan Bean
 * Modified by Bethany Weddle
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
    /// An enumeration of possible player animation states
    /// </summary>
    enum PlayerAnimState
    {
        Idle,
        JumpingLeft,
        JumpingRight,
        WalkingLeft,
        WalkingRight,
        FallingLeft,
        FallingRight
    }

    /// <summary>
    /// An enumeration of possible player veritcal movement states
    /// </summary>
    enum VerticalMovementState
    {
        OnGround,
        Jumping,
        DoubleJumping,
        Falling
    }
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

    public class Player : ISprite
    {
        // how much the animation moves per frames 
        const int FRAME_RATE = 124;
        // The duration of a player's jump, in milliseconds
        const int JUMP_TIME = 500;
        // the speed of the player
        const float PLAYER_SPEED = 100;
        // width of animation frames
        public const int FRAME_WIDTH = 67;
        // height of animation frames
        const int FRAME_HEIGHT = 100;
        // The currently rendered frame
        int currentFrame = 0;
        // The player's animation state
        PlayerAnimState animationState;
        // The player's vertical movement state
        VerticalMovementState verticalState;
        // A timer for jumping
        TimeSpan jumpTimer;
        // A timer for animations
        TimeSpan animationTimer;
        // The origin of the sprite (centered on its feet)
        Vector2 Origin = new Vector2(10, 21);


        Game1 game;
        Texture2D player;
        State state;
        public Vector2 Position;
        public BoundingRectangle Bounds;
        KeyboardState oldState;
        int frame;

        public Player(Game1 game,Texture2D texture)
        {
            this.game = game;
            player = texture;
        }

        public void Initialize()
        {
            Position = new Vector2(200, 500);
            state = State.Idle;
            animationState = PlayerAnimState.Idle;
            verticalState = VerticalMovementState.OnGround;
            Bounds.Width = FRAME_WIDTH;
            Bounds.Height = FRAME_HEIGHT;
        }

        public void Update(GameTime gameTime)
        {
            //Movement
            KeyboardState keyboard = Keyboard.GetState();
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Bounds.X = Position.X;
            Bounds.Y = Position.Y;

            // Vertical movement
            switch (verticalState)
            {
                case VerticalMovementState.OnGround:
                    if (keyboard.IsKeyDown(Keys.Up))
                    {
                        verticalState = VerticalMovementState.Jumping;
                        jumpTimer = new TimeSpan(0);
                    }
                    break;
                case VerticalMovementState.Jumping:
                    jumpTimer += gameTime.ElapsedGameTime;
                    // Simple jumping with platformer physics
                    Position.Y -= (350 / (float)jumpTimer.TotalMilliseconds);
                    if (jumpTimer.TotalMilliseconds >= JUMP_TIME)
                        verticalState = VerticalMovementState.Falling;
                    break;
                case VerticalMovementState.Falling:
                    Position.Y += delta * PLAYER_SPEED;
                    // TODO: This needs to be replaced with collision logic
                    if (Position.Y > 500)
                    {
                        Position.Y = 500;
                        verticalState = VerticalMovementState.OnGround;
                    }
                    break;
            }


            // Horizontal movement
            if (keyboard.IsKeyDown(Keys.Left))
            {
                if (verticalState == VerticalMovementState.Jumping || verticalState == VerticalMovementState.Falling)
                    animationState = PlayerAnimState.JumpingLeft;
                else
                {
                    state = State.West;
                    animationState = PlayerAnimState.WalkingLeft;
                }
                Position.X -= delta * PLAYER_SPEED;
            }
            else if (keyboard.IsKeyDown(Keys.Right))
            {
                if (verticalState == VerticalMovementState.Jumping || verticalState == VerticalMovementState.Falling)
                    animationState = PlayerAnimState.JumpingRight;
                else
                {
                    state = State.East;
                    animationState = PlayerAnimState.WalkingRight;
                }
                Position.X += delta * PLAYER_SPEED;
            }
            else
            {
                state = State.Idle;
                animationState = PlayerAnimState.Idle;
            }

            // Apply animations
            switch (animationState)
            {
                case PlayerAnimState.Idle:
                    currentFrame = 0;
                    animationTimer = new TimeSpan(0);
                    break;

                case PlayerAnimState.JumpingLeft:
                    currentFrame = 7;
                    break;

                case PlayerAnimState.JumpingRight:
                    currentFrame = 7;
                    break;

                case PlayerAnimState.WalkingLeft:
                    animationTimer += gameTime.ElapsedGameTime;
                    // Walking frames are 9 & 10
                    if (animationTimer.TotalMilliseconds > FRAME_RATE * 2)
                    {
                        animationTimer = new TimeSpan(0);
                    }
                    currentFrame = (int)Math.Floor(animationTimer.TotalMilliseconds / FRAME_RATE) + 9;
                    break;

                case PlayerAnimState.WalkingRight:
                    animationTimer += gameTime.ElapsedGameTime;
                    // Walking frames are 9 & 10
                    if (animationTimer.TotalMilliseconds > FRAME_RATE * 2)
                    {
                        animationTimer = new TimeSpan(0);
                    }
                    currentFrame = (int)Math.Floor(animationTimer.TotalMilliseconds / FRAME_RATE) + 9;
                    break;

            }

            // Making sure player doesn't go off screen
            if (Position.Y < 0)
            {
                Position.Y = 0;
            }
            if (Position.X < 0)
            {
                Position.X = 0;
            }
            if (Position.Y > game.GraphicsDevice.Viewport.Height - FRAME_HEIGHT)
            {
                Position.Y = game.GraphicsDevice.Viewport.Height - FRAME_HEIGHT;
            }
            if (Position.X > game.GraphicsDevice.Viewport.Width - FRAME_WIDTH)
            {
                Position.X = game.GraphicsDevice.Viewport.Width - FRAME_WIDTH;
            }

            // update animation timer when the player is moving
            if (animationState != PlayerAnimState.Idle)
                animationTimer += gameTime.ElapsedGameTime;

            // Check if animation should increase by more than one frame
            while (animationTimer.TotalMilliseconds > FRAME_RATE)
            {
                // increase frame
                frame++;
                // Decrease the timer by one frame duration
                animationTimer -= new TimeSpan(0, 0, 0, 0, FRAME_RATE);
            }

            frame %= 4;
            oldState = keyboard;
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {

            Rectangle rectSource = new Rectangle(
                frame * FRAME_WIDTH,  // X value
                (int)state % 4 * FRAME_HEIGHT, // Y value
                FRAME_WIDTH,
                FRAME_HEIGHT
                );

            spriteBatch.Draw(player, Position, rectSource, Color.White);

        }

    }

}

