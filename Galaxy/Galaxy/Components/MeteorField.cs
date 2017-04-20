using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Galaxy
{
    class MeteorField : CelestialObj
    {
        List<Vector2> drawPositions;
        List<float> drawSizes;
        float rad;

        public MeteorField(Game game, string textureName, Vector2 pos, float radius)
            : base(game, textureName, 1f)
        {
            Position = pos;
            rad = radius;
            drawPositions = new List<Vector2>();
            drawSizes = new List<float>();
            while (drawPositions.Count < radius)
            {
                Vector2 newMet = pos;
                newMet += new Vector2(((float)RandomGenerator.Generator.NextDouble() * 2f * radius) - radius,
                    ((float)RandomGenerator.Generator.NextDouble() * 2f * radius) - radius);
                drawPositions.Add(newMet);
                drawSizes.Add((float)RandomGenerator.Generator.NextDouble()/10f);
            }
            UpdateOrder = (int)((1 - this.Scale) * 100);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < drawPositions.Count; i++)
            {
                spriteBatch.Draw(Texture, drawPositions[i], null, Color.White, 0f, Origin*drawSizes[i], drawSizes[i], SpriteEffects.None, 0f);
            }
            base.Draw(spriteBatch);
        }

        public override void TakeCollision(CelestialObj obj)
        {
            base.TakeCollision(obj);
        }

        public override bool CheckCollision(Vector2 point)
        {
            if (point.X <= (this.Position.X + rad) &&
                point.X >= (this.Position.X - rad) &&
                point.Y <= (this.Position.Y + rad) &&
                point.Y >= (this.Position.Y - rad))
                return true;
            else
                return false;
        }
    }
}
