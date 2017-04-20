using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace Galaxy
{
    class SpaceShip : CelestialObj
    {
        long passengers = 0;
        Planet homePlanet, destination;
        Vector2 targetCoordinates;
        bool launched = false;
        bool arrived = false;
        bool escaping = false;

        public SpaceShip(Game game, string textureName, Planet _origin, long _passengers)
            : base(game, textureName, 0.1f)
        {
            passengers = _passengers;
            homePlanet = _origin;
        }

        public override void Update(GameTime gameTime)
        {
            if (!launched)
            {
                Audio.State = AudioState.ShipLaunch;
                launched = true;
                DetermineDestination();
            }

            if (!arrived)
            {
                // Move towards target coordinates
                Vector2 direction = targetCoordinates - this.Position;
                if (Math.Abs(direction.X) < 2 && Math.Abs(direction.Y) < 2) arrived = true;
                direction.Normalize();
                if (!arrived)
                    Position = Position + direction;
                else if (escaping)
                    ((GalaxyGame)parentGame).EndGame(false);
            }
            base.Update(gameTime);
        }

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, null, Color.White, 0f, Origin, Scale * 0.75f, SpriteEffects.None, 0f);
            base.Draw(spriteBatch);
        }

        public long Passengers
        {
            get
            {
                return passengers;
            }
        }

        private void DetermineDestination()
        {
            Position = homePlanet.Position;
            Planet closest = null;
            float deltaRadius = 10000.0f;
            foreach (var obj in parentGame.Components)
            {
                if (obj.GetType() == typeof(Planet) &&
                    ((Planet)obj).PlanetState == HabitationState.Colonizable && // colonizable
                        ((Planet)obj).StarPosition == homePlanet.StarPosition && obj != homePlanet)  // same stellar system, different planet.
                {
                    Planet p = obj as Planet;
                    float diff = 0f;
                    if (p.AverageRadius > homePlanet.AverageRadius) diff = p.AverageRadius - homePlanet.AverageRadius;
                    else diff = homePlanet.AverageRadius - p.AverageRadius;
                    if (diff < deltaRadius)
                    {
                        deltaRadius = diff;
                        closest = p;
                    }
                }
            }

            if (closest == null)
            {
                targetCoordinates = new Vector2(GalaxyGame.ScreenWidth / RandomGenerator.Generator.Next(1, 5), GalaxyGame.ScreenHeight + 100);
                escaping = true;
            }
            else
            {
                destination = closest;
                targetCoordinates = closest.StarPosition + closest.GetPosition(homePlanet.Angle + 2 / closest.AverageRadius);
            }
        }

        public override void TakeCollision(CelestialObj obj)
        {
            if (obj is Asteroid || obj is Star)
            {
                Audio.State = AudioState.ShipDeath;
                parentGame.Components.Remove(this);
            }
            else if (obj is Planet && obj == destination)
            {
                Audio.State = AudioState.ShipLanding;
                destination.LandSpaceShip(this);
            }
            base.TakeCollision(obj);
        }

    }
}
