using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeathOrDodge
{
    public class ParallaxLayer : DrawableGameComponent
    {
       
        /// <summary>
        /// The list of ISprites that compose this parallax layer
        /// </summary>
        public List<ISprite> Sprites = new List<ISprite>();

        /// <summary>
        /// The transformation to apply to this parallax layer
        /// </summary>
        Matrix transform = Matrix.Identity;

        // <summary>
        /// The SpriteBatch to use to draw the layer
        /// </summary>
        SpriteBatch spriteBatch;

        /// <summary>
        /// Constructs the ParallaxLayer instance 
        /// </summary>
        /// <param name="game">The game this layer belongs to</param>
        public ParallaxLayer(Game game) : base(game)
        {
            spriteBatch = new SpriteBatch(game.GraphicsDevice);
        }


        /// <summary>
        /// Draws the Parallax layer
        /// </summary>
        /// <param name="gameTime">The GameTime object</param>
        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, transform);
            foreach (var sprite in Sprites)
            {
                sprite.Draw(spriteBatch, gameTime);
            }
            spriteBatch.End();
        }
    }
}
