using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Galaxy
{
    public class GalaxyGame : Microsoft.Xna.Framework.Game
    {
        public static int ScreenWidth = 1280;
        public static int ScreenHeight = 720;
        /*public static int ScreenWidth = 1600;
        public static int ScreenHeight = 1200;*/
        /*public static int ScreenWidth = 2048;
        public static int ScreenHeight = 1152;*/
        /*public static int ScreenWidth = 1440/2;
        public static int ScreenHeight = 900/2;*/

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        int level = 1;

        MouseState previousState, currentState;
        KeyboardState prevKeyState, nextKeyState;
        //int enemyCount, playerCount;

        Vector2 starLoc = new Vector2(ScreenWidth / 2, ScreenHeight / 2);
        Vector2 lastAsteroidSpawn = Vector2.Zero; 
        bool drawOrbit = false;
        bool canLaunch = false;
        Dictionary<string, string> settings = new Dictionary<string,string>();

        Texture2D orbitTexture;
        Texture2D TextureBackgroundMenu;
        Texture2D MainMenu;
        Texture2D Help;
        Texture2D Credits;
        Texture2D explosion;
        HUD hud;

        SpriteFont Verdana40;
        SpriteFont CloisterBlack;

        List<MenuWindow> menuList = new List<MenuWindow>();
        MenuWindow activeMenu;
        MenuWindow menuMain;
        MenuWindow menuHelp;
        MenuWindow menuCredits;
        MenuWindow startGame;
        MenuWindow quitGame;

        bool menusRunning;
        bool endGame = false;
        bool winState = false;
        TimeSpan endgameStart = TimeSpan.Zero; 
        TimeSpan endgameShown = TimeSpan.Zero;

        public static List<Vector2> ExplosionLocations = new List<Vector2>();
        public static List<int> CurrentKeyTime = new List<int>();
        private int keyTime = 50;

        public GalaxyGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            //graphics.PreferredBackBufferWidth = ScreenWidth;
            //graphics.PreferredBackBufferHeight = ScreenHeight;
            //graphics.IsFullScreen = true;

            //ScreenWidth = graphics.GraphicsDevice.DisplayMode.Width;
            //ScreenHeight = graphics.GraphicsDevice.DisplayMode.Height;
            graphics.PreferredBackBufferWidth = ScreenWidth;
            graphics.PreferredBackBufferHeight = ScreenHeight;
            graphics.IsFullScreen = true;

            graphics.ApplyChanges();

            this.IsMouseVisible = true;
            menusRunning = true;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            orbitTexture = Content.Load<Texture2D>("Deco/dash");
            explosion = Content.Load<Texture2D>("seq");
            TextureBackgroundMenu = Content.Load<Texture2D>("TextureBackgroundMenu");
            MainMenu = Content.Load<Texture2D>("MainMenu");
            Help = Content.Load<Texture2D>("Help");
            Credits = Content.Load<Texture2D>("Credits");
            Verdana40 = Content.Load<SpriteFont>("Verdana40");
            CloisterBlack = Content.Load<SpriteFont>("CloisterBlack");

            Audio.LoadAudio(Content, "Content/Audio/game.audio");
            
            // Create menus and add them to the menuList
            menuMain = new MenuWindow("The Gods Must Be Crazy", MainMenu, CloisterBlack, Verdana40);
            menuCredits = new MenuWindow("Credits", Credits, CloisterBlack, Verdana40);
            menuHelp = new MenuWindow("How To Play", Help, CloisterBlack, Verdana40);

            startGame = new MenuWindow(null, null, null, null);
            quitGame = new MenuWindow(null, null, null, null);

            // Add all the menus to our menu list
            menuList.Add(menuMain);
            menuList.Add(menuCredits);
            menuList.Add(menuHelp);
            // Give each of the menus its options
            menuMain.AddMenuItem("New Game", startGame);
            menuMain.AddMenuItem("How To Play", menuHelp);
            menuMain.AddMenuItem("Credits", menuCredits);
            menuMain.AddMenuItem("Quit Game", quitGame);

            menuHelp.AddMenuItem("", menuMain);
            menuCredits.AddMenuItem("", menuMain);
            
            menuMain.WakeUp();

            activeMenu = menuMain;
        }
        
        private void LoadLevel()
        {
            Planet.inhabitedPlanets = 0;
            if (!GalaxyReader.LoadLevel(this, true, @"Content\levels\level" + level++ + ".txt"))
            {
                level = 1;
                GalaxyReader.LoadLevel(this, true, @"Content\levels\level" + level++ + ".txt");
            }
            hud = new HUD(Content, "Deco/EnergyBar", "Deco/EnergyBarFill", 0f, 100f, 50f, 0.005f);
        }

        protected override void Update(GameTime gameTime)
        {
            previousState = currentState;
            currentState = Mouse.GetState();
            prevKeyState = nextKeyState;
            nextKeyState = Keyboard.GetState();

            for (int i = 0; i < CurrentKeyTime.Count; i++)
                CurrentKeyTime[i] += gameTime.ElapsedGameTime.Milliseconds;

            int iter=0;
            while (iter < CurrentKeyTime.Count)
            {
                if (CurrentKeyTime[iter] >= 8 * keyTime)
                {
                    CurrentKeyTime.RemoveAt(iter);
                    ExplosionLocations.RemoveAt(iter);
                }
                else
                    iter++;
            }

            if (endGame)
            {
                Components.Clear();
                if (endgameStart == TimeSpan.Zero)
                    endgameStart = gameTime.TotalGameTime;
                else
                    endgameShown = gameTime.TotalGameTime - endgameStart;

                if (endgameShown.TotalMilliseconds >= 7000)
                {
                    endGame = false;
                    endgameStart = endgameShown = TimeSpan.Zero;
                    LoadLevel();
                }
            }
            if (!menusRunning)
            {
                // Allows the game to exit
                if ((GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed) || (prevKeyState.IsKeyDown(Keys.Escape) && nextKeyState.IsKeyUp(Keys.Escape)))
                    this.Exit();

                // Reset Game
                if ((GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed) || (prevKeyState.IsKeyDown(Keys.R) && nextKeyState.IsKeyUp(Keys.R)))
                    LoadLevel();

                if ((previousState.LeftButton != ButtonState.Pressed) && (currentState.LeftButton == ButtonState.Pressed))
                {
                    lastAsteroidSpawn = new Vector2((float)currentState.X, (float)currentState.Y);
                    // Get sorted list so UpdateOrder also defines render order
                    IEnumerable<GameComponent> sortedComponents = Components.Cast<GameComponent>().OrderBy(gameComponent => gameComponent.UpdateOrder);
                    foreach (CelestialObj gameObject in sortedComponents)
                    {
                        if (gameObject != null && gameObject is MeteorField&&!canLaunch)
                        {
                            canLaunch = gameObject.CheckCollision(lastAsteroidSpawn);
                        }
                    }

                    if (canLaunch)
                        drawOrbit = true;
                    else
                    {
                        lastAsteroidSpawn = Vector2.Zero;
                        drawOrbit = false;
                    }
                }

                if ((previousState.LeftButton == ButtonState.Pressed) && (currentState.LeftButton != ButtonState.Pressed) && canLaunch)
                {
                    canLaunch = false;
                    Asteroid a = new Asteroid(this, "Meteor/Meteor", 1f);
                    a.Position = lastAsteroidSpawn - a.Origin;
                    Vector2 temp = new Vector2(-lastAsteroidSpawn.X + (float)currentState.X,
                        -lastAsteroidSpawn.Y + (float)currentState.Y);
                    temp /= 50f;
                    float length = temp.Length();
                    bool doAdd = true;
                    if (hud.IsAddEnergyPossible(-20f - length))
                    {
                        a.Velocity = temp;
                        hud.Energy -= (20f + length);
                    }
                    else if (hud.IsAddEnergyPossible(-20f))
                    {
                        hud.Energy -= 20f;
                        temp.Normalize();
                        temp *= hud.Energy;
                        hud.Energy = 0f;
                    }
                    else
                    {
                        doAdd = false;
                    }
                    if (doAdd)
                        Components.Add(a);
                    drawOrbit = false;
                }

                if ((previousState.RightButton == ButtonState.Pressed) && (currentState.RightButton != ButtonState.Pressed))
                {
                    if (hud.IsAddEnergyPossible(-10f))
                    {
                        hud.Energy -= 10f;

                        Vector2 click = new Vector2(currentState.X, currentState.Y);
                        Vector2 bestDiff = new Vector2(10000, 10000);
                        Vector2 origin = new Vector2();
                        foreach (var item in Components)
                        {
                            if (item is Star)
                            {
                                Vector2 diff = (click - ((Star)item).Position);
                                if (diff.LengthSquared() < bestDiff.LengthSquared())
                                {
                                    bestDiff = diff;
                                    origin = ((Star)item).Position;
                                }
                            }
                        }
                        if (bestDiff.X != 10000)
                        {
                            SolarFlare sf = new SolarFlare(this, "SolarFlare/Shockwave", origin, bestDiff);
                            Components.Add(sf);
                        }
                    }

                }

                hud.Update(gameTime);
            }

            MenuInput();

            // Update all the menus
            foreach (MenuWindow currentMenu in menuList)
                currentMenu.Update(gameTime.ElapsedGameTime.TotalMilliseconds);

            switch (Audio.State)
            {
                case AudioState.Win:
                    Microsoft.Xna.Framework.Media.MediaPlayer.Stop();
                    Audio.WinAudioInstance.Play();
                    Audio.State = AudioState.None;
                    break;
                case AudioState.Lose:
                    Microsoft.Xna.Framework.Media.MediaPlayer.Stop();
                    Audio.LoseAudioInstance.Play();
                    Audio.State = AudioState.None;
                    break;
                case AudioState.CometShoot:
                    Audio.CometShootAudioInstance.Play();
                    Audio.State = AudioState.Ambient;
                    break;
                case AudioState.CometHitHabitablePlanet:
                    Audio.CometHitHabitablePlanetAudioInstance.Play();
                    Audio.State = AudioState.Ambient;
                    break;
                case AudioState.CometHitUnHabitablePlanet:
                    Audio.CometHitUnHabitablePlanetAudioInstance.Play();
                    Audio.State = AudioState.Ambient;
                    break;
                case AudioState.ShipLaunch:
                    Audio.ShipLaunchAudioInstance.Play();
                    Audio.State = AudioState.Ambient;
                    break;
                case AudioState.ShipDeath:
                    Audio.ShipDeathAudioInstance.Play();
                    Audio.State = AudioState.Ambient;
                    break;
                case AudioState.ShipLanding:
                    Audio.ShipLandingAudioInstance.Play();
                    Audio.State = AudioState.Ambient;
                    break;
                case AudioState.SolarFlare:
                    Audio.SolarFlareAudioInstance.Play();
                    Audio.State = AudioState.Ambient;
                    break;
                case AudioState.Ambient:
                    if (Microsoft.Xna.Framework.Media.MediaPlayer.State != Microsoft.Xna.Framework.Media.MediaState.Playing)
                        Microsoft.Xna.Framework.Media.MediaPlayer.Play(Audio.AmbientAudio);
                    break;
                case AudioState.None:
                    Microsoft.Xna.Framework.Media.MediaPlayer.Stop();
                    break;
                default:
                    break;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);

            // Draw all the menus
            if (menusRunning)
            {
                foreach (MenuWindow currentMenu in menuList)
                    currentMenu.Draw(spriteBatch, ScreenWidth, ScreenHeight);
            }
            else if (endGame)
            {
                //string endgameText = winState ? "You have exterminated all known life\nin the universe, excellent!" : "Life forms have escaped your grasp,\nyou are the laughing stock of the gods.";
                //spriteBatch.DrawString(Verdana40, endgameText, new Vector2(0, 0), Color.White);
                Texture2D screen = winState ? Content.Load<Texture2D>("EndScreenWin") : Content.Load<Texture2D>("EndScreenLose");
                spriteBatch.Draw(screen, new Rectangle(0,0,ScreenWidth, ScreenHeight), Color.White);
            }
            else
            {
                //spriteBatch.Draw(orbitTexture, pos, null, Color.White, 0f, origin, scale, SpriteEffects.None, 0f);

                spriteBatch.Draw(TextureBackgroundMenu, new Rectangle(0, 0, GalaxyGame.ScreenWidth, GalaxyGame.ScreenHeight), Color.White);

                if (drawOrbit)
                {
                    DrawPath(currentState.X, currentState.Y, 1f);
                }

                // Get sorted list so UpdateOrder also defines render order
                IEnumerable<GameComponent> sortedComponents = Components.Cast<GameComponent>().OrderBy(gameComponent => gameComponent.UpdateOrder);
                foreach (CelestialObj gameObject in sortedComponents)
                {
                    if (gameObject != null)
                    {
                        gameObject.Draw(spriteBatch);
                    }
                }

                for (int i = 0; i < ExplosionLocations.Count; i++)
                {
                    Rectangle dest = new Rectangle((int)ExplosionLocations[i].X, (int)ExplosionLocations[i].Y, 64, 64);
                    Rectangle source = new Rectangle((int)(CurrentKeyTime[i] / keyTime) * 64, 0, 64, 64);
                    spriteBatch.Draw(explosion, dest, source, Color.White);
                    //Console.WriteLine("Source: " + source.ToString() + "; Dest: " + dest.ToString());
                }

                hud.Draw(spriteBatch);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        private Vector2 ResolveVelocity(Vector2 vel, Vector2 pos, float mymass)
        {
            Vector2 tempVelocity = vel;
            foreach (CelestialObj obj in Components)
            {
                if (!this.Equals(obj))
                {
                    Vector2 direction = obj.Position - pos;
                    direction.Normalize();
                    Vector2 attraction = Vector2.Zero;
                    if (obj is Galaxy.Star)
                        attraction = direction * (0.25f * obj.Mass / (obj.Position - pos).Length());
                    else
                        attraction = direction * (obj.Mass / (obj.Position - pos).Length());
                    tempVelocity += attraction;
                }
            }
            return tempVelocity;
        }

        private void DrawPath(int headingX, int headingY, float mass)
        {
            Vector2 vel = new Vector2(-lastAsteroidSpawn.X + (float)headingX,
                    -lastAsteroidSpawn.Y + (float)headingY);
            float delta = vel.Length()/500f;
            int iterations = 50;
            vel /= 50f;
            Vector2 pos = lastAsteroidSpawn;
            for (int i = 0; i < iterations; i++)
            {
                pos += (vel * delta);
                float scale = 1f;
                Vector2 origin = new Vector2(orbitTexture.Width * scale * 0.5f, orbitTexture.Height * scale * 0.5f);
                spriteBatch.Draw(orbitTexture, pos, null, Color.White, 0f, origin, scale, SpriteEffects.None, 0f);
                vel = ResolveVelocity(vel, pos, mass);
            }
        }

        // Pass the previous and current keyboard and gamepad states to the ProcessInput method of the currently active menu
        private void MenuInput()
        {
            if (!menusRunning)
                return;

            MenuWindow newActive = activeMenu.ProcessInput(prevKeyState, nextKeyState);

            if (newActive == quitGame)
                this.Exit();
            else if (newActive == startGame)
            {
                menusRunning = false;
                LoadLevel();
            }
            else if (newActive == null)
                this.Exit();
            else if (newActive != activeMenu)
                newActive.WakeUp();

            activeMenu = newActive;
        }

        public void EndGame(bool win)
        {
            winState = win;
            Audio.State = win ? AudioState.Win : AudioState.Lose;
            if (!winState) level--;
            endGame = true;
        }
    }
}
