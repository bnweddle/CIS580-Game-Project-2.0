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
    public class Bush
    {
        /// <summary>
        /// Texture holding the bush obstacle image
        /// </summary>
        Texture2D bush;

        /// <summary>
        /// The portion of the spritesheet that is the helicopter
        /// </summary>
        BoundingRectangle Bounds = new BoundingRectangle
        {
            X = 0,
            Y = 0,
            Width = 150,
            Height = 68
        };

        float Scale { get; set; } = 1.0f;

        /// <summary>
        /// The bush's position in the world
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        /// How fast the bush moves
        /// </summary>
        public float Speed { get; set; } = 100;

        Game1 game;

        /// <summary>
        /// Constructs a player
        /// </summary>
        /// <param name="spritesheet">The player's spritesheet</param>
        public Bush(Game1 game)
        {
            this.game = game;
            this.Position = new Vector2(1000, 525);
        }

        public void LoadContent(ContentManager content)
        {
            bush = content.Load<Texture2D>("bush");
        }

        /// <summary>
        /// Updates the player position based on GamePad or Keyboard input
        /// </summary>
        /// <param name="gameTime">The GameTime object</param>
        public void Update(GameTime gameTime)
        {
            Vector2 direction = Vector2.Zero;
            direction.X -= 1;

            //Move the bush
            Position += (float)gameTime.ElapsedGameTime.TotalSeconds * Speed * direction;
        }

        /// <summary>
        /// Draws the player sprite
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(bush, Position, Bounds, Color.White, 0f, Vector2.Zero, Scale, SpriteEffects.None, 0);
        }

    }
}
