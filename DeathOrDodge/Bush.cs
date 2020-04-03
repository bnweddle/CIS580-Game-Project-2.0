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
    public class Bush : ISprite
    {
        /// <summary>
        /// Texture holding the bush obstacle image
        /// </summary>
        Texture2D bush;

        /// <summary>
        /// The portion of the spritesheet that is the helicopter
        /// </summary>
        public BoundingRectangle Bounds = new BoundingRectangle
        {
            X = 0,
            Y = 0,
            Width = 150,
            Height = 67
        };

        /// <summary>
        /// Scaling factor for the bush
        /// </summary>
        public float Scale = 0.70f;

        /// <summary>
        /// The bush's position in the world
        /// </summary>
        Vector2 Position;


        Game1 game;

        /// <summary>
        /// Constructs a bush
        /// </summary>
        /// <param name="spritesheet">The player's spritesheet</param>
        public Bush(Game1 game, Texture2D texture, Vector2 position)
        {
            this.game = game;
            this.bush = texture;
            this.Position = position;
        }

        /// <summary>
        /// Constructs a bush
        /// </summary>
        /// <param name="spritesheet">The player's spritesheet</param>
        public Bush(Game1 game, Texture2D texture)
        {
            this.game = game;
            this.bush = texture;
            Position = new Vector2(300, 543);
        }

        /// <summary>
        /// Updates the player position based on GamePad or Keyboard input
        /// </summary>
        /// <param name="gameTime">The GameTime object</param>
        public void Update(GameTime gameTime)
        {
            //check if fallen off screen then reset
            //layer has update  
            if (Position.X < 0)
            {
                Position.X = 1300;
            }
        }

        /// <summary>
        /// Draws the player sprite
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Draw(bush, Position, Bounds, Color.White, 0f, Vector2.Zero, Scale, SpriteEffects.None, 0);
        }

    }
}
