using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Galaxy
{
    class Star : CelestialObj
    {
        public Star(Game game, string textureName, float _mass, ref Vector2 pos)
            : base(game, textureName, _mass)
        {
            Position = pos;
            pos = pos + Origin / 2;
            UpdateOrder = (int)((1 - this.Scale) * 100);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, null, Color.White, 0f, Origin, Scale, SpriteEffects.None, 0f);
            base.Draw(spriteBatch);
        }
    }
}
