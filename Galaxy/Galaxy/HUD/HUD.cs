using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Galaxy
{
    class HUD
    {
        Texture2D energyBar, barFill;
        float lowerBound, upperBound;
        float energy, increment;

        public float Energy
        {
            get { return energy; }
            set { energy = value; }
        }

        public HUD(ContentManager content, string energyBarTex, string fillBarTex, float lower, float upper, float start, float delta)
        {
            energyBar = content.Load<Texture2D>(energyBarTex);
            barFill = content.Load<Texture2D>(fillBarTex);
            lowerBound = lower;
            upperBound = upper;
            energy = start;
            increment = delta;
        }

        public bool IsAddEnergyPossible(float value)
        {
            if (energy + value < lowerBound)
                return false;
            else
                return true;
        }

        public void Update(GameTime gameTime)
        {
            energy += (increment * gameTime.ElapsedGameTime.Milliseconds);
            if (energy > upperBound)
                energy = upperBound;
            if (energy < lowerBound)
                energy = lowerBound;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            float scale = 0.5f;
            Vector2 drawPosition = new Vector2(GalaxyGame.ScreenWidth - (energyBar.Width * scale) + 50f,
                GalaxyGame.ScreenHeight - (energyBar.Height * scale) + 20f);
            Vector2 origin = new Vector2(energyBar.Width * scale * 0.5f, energyBar.Height * scale * 0.5f);
            //spriteBatch.Draw(Texture, Position, null, Color.Orange, 0f, Origin, Scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(energyBar, drawPosition, null, Color.White, 0f, origin, scale, SpriteEffects.None, 0f);
            int width = 124 + (int)((560f - 124f) * energy * 0.01f);
            Rectangle source = new Rectangle(0, 0, width, (int)(barFill.Height));
            spriteBatch.Draw(barFill, drawPosition, source, Color.White, 0f, origin, scale, SpriteEffects.None, 0f);
        }
    }
}
