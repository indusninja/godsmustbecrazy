using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Globalization;

namespace Galaxy
{
    class Asteroid : CelestialObj
    {
        Vector2 velocity;

        public Vector2 Velocity
        {
            // Velocity is "super-lowered" here
            set 
            { 
                velocity = value;
            }
            get { return velocity; }
        }

        public Asteroid(Game game, string textureName, float _mass)
            : base(game, textureName, _mass)
        {
            Position = Vector2.Zero;
            velocity = Vector2.Zero;
            UpdateOrder = (int)((1-this.Scale) * 100);
            Audio.State = AudioState.CometShoot;
        }

        public override void Update(GameTime gameTime)
        {
            velocity = ResolveVelocity(velocity);

            Position += (velocity * gameTime.ElapsedGameTime.Milliseconds * 0.1f);

            base.Update(gameTime);
        }

        private Vector2 ResolveVelocity(Vector2 vel)
        {
            Vector2 tempVelocity = velocity;
            foreach (CelestialObj obj in parentGame.Components)
            {
                if (!this.Equals(obj))
                {
                    Vector2 direction = obj.Position - this.Position;
                    direction.Normalize();
                    Vector2 attraction = Vector2.Zero;
                    float starContrib = float.Parse(GalaxyReader.Settings["starMassContribution"], CultureInfo.InvariantCulture);
                    if(obj is Galaxy.Star)
                        attraction = direction * (starContrib * obj.Mass / (obj.Position - this.Position).Length());
                    else
                        attraction = direction * (obj.Mass / (obj.Position - this.Position).Length());
                    tempVelocity += attraction;
                }
            }
            return tempVelocity;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, null, Color.White, 0f, Origin, Scale, SpriteEffects.None, 0f);
            base.Draw(spriteBatch);
        }
        
        public override void TakeCollision(CelestialObj obj)
        {
            if (!(obj is MeteorField))
            {
                GalaxyGame.ExplosionLocations.Add(this.position);
                GalaxyGame.CurrentKeyTime.Add(0);
                parentGame.Components.Remove(this);
                return;
            }
        }
    }
}
