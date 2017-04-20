using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Galaxy
{
    class CelestialObj : Microsoft.Xna.Framework.GameComponent
    {
        protected Vector2 position, origin;
        float mass;
        protected float scale;
        Texture2D texture;
        protected Game parentGame;

        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        public Vector2 Origin
        {
            get { return origin; }
            set { origin = value; }
        }

        public float Mass
        {
            get { return mass; }
            set { mass = value; }
        }

        public Texture2D Texture
        {
            get { return texture; }
            set { texture = value; }
        }

        public float Scale
        {
            get { return scale; }
        }

        public string TextureName { get; private set; }

        public CelestialObj(Microsoft.Xna.Framework.Game game, string textureName, float _mass)
            : base(game)
        {
            this.mass = _mass;
            this.scale = 1 / (11-_mass);
            this.parentGame = game;
            this.texture = game.Content.Load<Texture2D>(textureName);
            TextureName = textureName;
            Origin = new Vector2(Texture.Width * Scale * 0.5f, Texture.Height * Scale * 0.5f);
        }

        public override void Update(GameTime gameTime)
        {
            foreach (CelestialObj obj in parentGame.Components)
            {
                if (!this.Equals(obj))
                {
                    if (CheckCollision(obj))
                    {
                        obj.TakeCollision(this);
                        //parentGame.Components.Remove(this);
                        return;
                    }
                }
            }
            base.Update(gameTime);
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
        }

        public virtual void TakeCollision(CelestialObj obj)
        {
        }

        public virtual bool CheckCollision(Vector2 point)
        {
            if (point.X <= (this.Position.X + (2 * this.Origin.X)) &&
                point.X >= this.Position.X &&
                point.Y <= (this.Position.Y + (2 * this.Origin.Y)) &&
                point.Y >= this.Position.Y)
                return true;
            else
                return false;
        }

        public bool CheckCollision(CelestialObj obj)
        {
            float dSqaure = (float)Math.Pow((this.Origin.X + obj.Origin.X), 2f);
            Vector2 p1 = this.position;
            Vector2 p2 = obj.position;
            float centerDistance = getDistanceSquare(p1, p2);

            if (centerDistance < dSqaure)
                return true;
            else
                return false;
        }

        public void ChangeTexture(string textureName)
        {
            this.texture = parentGame.Content.Load<Texture2D>(textureName);
        }

        private float getDistanceSquare(Vector2 point1, Vector2 point2)
        {
            float d1 = (float)Math.Pow(point2.X - point1.X, 2.0);
            float d2 = (float)Math.Pow(point2.Y - point1.Y, 2.0);
            return d1 + d2;
        }
    }
}
