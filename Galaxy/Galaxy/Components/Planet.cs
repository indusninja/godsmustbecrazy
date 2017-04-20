using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Globalization;
using System.Diagnostics;

namespace Galaxy
{
    enum HabitationState
    {
        Uninhabitable,
        Colonizable,
        Inhabited
    }

    class Planet : CelestialObj
    {
        Vector2 starPosition;
        float a, b;
        float theta, deltaTheta = 0.01f;
        long population = 0;
        bool homeworld = false;
        float growth = 1.001f;
        long popcap = 0;
        int growthStop = 0;
        int poppct = 0;
        HabitationState planetstate = HabitationState.Uninhabitable;
        internal static int inhabitedPlanets = 0;

        public Planet(Game game, string textureName, float _a, float _b, float _mass, Vector2 starP,
            long _population, HabitationState _planetstate)
            : base(game, textureName, _mass)
        {
            this.a = _a;
            this.b = _b;
            this.starPosition = starP;
            theta = ((float)RandomGenerator.Generator.Next(-314, 314) / 100f);
            Position = GetPosition(theta) + starPosition;
            homeworld = _population > 0;
            population = _population;
            if (population > 0) inhabitedPlanets++;
            //growthStop = 300;
            growth = float.Parse(GalaxyReader.Settings["popgrowth"], CultureInfo.InvariantCulture);
            popcap = long.Parse(GalaxyReader.Settings["popcap"]);
            UpdateOrder = (int)((1 - this.Scale) * 100);
            planetstate = _planetstate;
        }

        internal Vector2 GetPosition(double angle)
        {
            Vector2 deltaPosition = new Vector2();
            float radius = (a * b) / ((float)Math.Sqrt((b * b * Math.Cos(angle) * Math.Cos(angle)) + (a * a * Math.Sin(angle) * Math.Sin(angle))));
            deltaPosition.X = radius * (float)Math.Cos(angle);
            deltaPosition.Y = radius * (float)Math.Sin(angle);
            deltaTheta = 1 / radius; // (float) (1 / Math.Pow(radius, 2.0d)) * 100f;
            //deltaTheta = a/(radius*100f);
            //deltaTheta = (float)Math.Exp((double)radius);
            return deltaPosition;
        }

        internal float Angle
        {
            get
            {
                return theta;
            }
        }

        internal Vector2 StarPosition
        {
            get
            {
                return starPosition;
            }
        }

        public override void Update(GameTime gameTime)
        {
            Position = GetPosition(theta) + starPosition;

            // This controls speed, so try making this update through gametime and have a seperate speed variable set from Level Designer
            theta += deltaTheta;

            if (theta > 3.14f)
                theta = -3.14f;
            if (theta < -3.14f)
                theta = 3.14f;

            UpdatePopulation();

            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Color PlanetColor = Color.White;
            spriteBatch.Draw(Texture, Position, null, PlanetColor, 0f, Origin, Scale, SpriteEffects.None, 0f);
            // Do not draw population bar if the planet is uninhabitable.
            if (PlanetState == HabitationState.Inhabited)
            {
                Rectangle bar = new Rectangle((int)Position.X, (int)(Position.Y + (Texture.Bounds.Height * Scale)),
                    (int)(Texture.Bounds.Width * Scale), 6);
                spriteBatch.Draw(Game.Content.Load<Texture2D>("Healthbar\\HealthBG"), bar, Color.White);
                Rectangle r2 = new Rectangle(bar.X + 1, bar.Y + 1, (int)(poppct / 20f * (bar.Width - 2)), 4);
                Color cBar = (poppct < 10 ? Color.Green : (poppct < 15 ? Color.Orange : Color.Red));
                spriteBatch.Draw(Game.Content.Load<Texture2D>("Healthbar\\HealthTile"), r2, cBar);
            }
            //spriteBatch.DrawString(Game.Content.Load<SpriteFont>("TempFont"), PlanetState.ToString(), Position, Color.White);
            base.Draw(spriteBatch);
        }

        public void StopGrowth(int ticks)
        {
            growthStop = ticks;
        }

        public override void TakeCollision(CelestialObj obj)
        {
            if (obj is Asteroid)
            {
                if (planetstate == HabitationState.Uninhabitable)
                    Audio.State = AudioState.CometHitUnHabitablePlanet;
                else
                    Audio.State = AudioState.CometHitHabitablePlanet;

                if (population > 0)
                {
                    StopGrowth(300);
                    population = population - Math.Max((long)(population * 0.3), 1000000000L);
                    if (population <= 0)
                    {
                        population = 0;
                        poppct = 0;
                        inhabitedPlanets--;
                        if (!homeworld) planetstate = HabitationState.Colonizable;
                        else
                        {
                            ChangeTexture(TextureName + "Dead");
                            planetstate = HabitationState.Uninhabitable;
                        }
                        if (inhabitedPlanets <= 0)
                            ((GalaxyGame)parentGame).EndGame(true);
                    }
                }
            }
        }

        public HabitationState PlanetState
        {
            get
            {
                if (homeworld)
                    return (population > 0) ? HabitationState.Inhabited : HabitationState.Uninhabitable;
                return planetstate;
            }
        }

        public float AverageRadius { get { return (a + b) / 2; } }

        private void UpdatePopulation()
        {
            if (PlanetState != HabitationState.Inhabited)
                return;
            if (growthStop > 0)
                growthStop--;
            else
                population = (long)(population + growth);

            if (population > popcap)
            {
                population = popcap;
                // Possibly launch colony ship
                if (RandomGenerator.Generator.Next(1000) <= 100) LaunchSpaceship();
            }
            poppct = (int)(population / (popcap / 20));
        }

        private void LaunchSpaceship()
        {
            long pax = long.Parse(GalaxyReader.Settings["shippax"]);
            population -= pax;
            Game.Components.Add(new SpaceShip(Game, "Deco/Ship", this, pax));
        }

        public void LandSpaceShip(SpaceShip colonyShip)
        {
            if (population == 0) inhabitedPlanets++;
            population += colonyShip.Passengers;
            planetstate = HabitationState.Inhabited;
            parentGame.Components.Remove(colonyShip);
        }


    }
}

