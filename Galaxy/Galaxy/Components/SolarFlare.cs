using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Galaxy
{
    class SolarFlare : CelestialObj
    {
        Vector2 direction;
        float rDirection;
        int updateCounter = 0;
        public SolarFlare(Game game, string textureName, Vector2 _origin, Vector2 _direction)
            : base(game, textureName, 1f)
        {
            direction = _direction;
            Vector2 normal = new Vector2(0, -1);
            normal.Normalize();
            if (_direction.X < 0) _direction.X = _direction.X * -1;
            _direction.Normalize();
            
            rDirection = (float)Math.Acos(Vector2.Dot(normal, _direction));
            if (direction.X < 0) rDirection *= -1;

            direction.Normalize();
            direction = direction * 2;
            Vector2 spincenter;
            if (direction.X > 0 && direction.Y < 0) // top right
                spincenter = new Vector2(0, -Origin.Y);
            else if (direction.X > 0 && direction.Y > 0) // bottom right
                spincenter = new Vector2(Origin.X, 0);
            else if (direction.X < 0 && direction.Y < 0) // top left
                spincenter = new Vector2(0, Origin.Y *4);
            else  // bottom right
                spincenter = new Vector2(Origin.X, Origin.Y * 4);
            Position = _origin + spincenter;
            Audio.State = AudioState.SolarFlare;
        }

        public override void Update(GameTime gameTime)
        {
            if (++updateCounter > 1000) Game.Components.Remove(this);
            Position += direction;
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, null, Color.White, rDirection, Origin, Scale, SpriteEffects.None, 0f);
            //spriteBatch.DrawString(Game.Content.Load<SpriteFont>("TempFont"), MathHelper.ToDegrees(rDirection).ToString(), Position, Color.White);
            base.Draw(spriteBatch);
        }

        public override void TakeCollision(CelestialObj obj)
        {
            if (obj is Planet)
                ((Planet)obj).StopGrowth(1000);
            else if (obj is SpaceShip)
                parentGame.Components.Remove(obj);
            base.TakeCollision(obj);
        }
    }
}
