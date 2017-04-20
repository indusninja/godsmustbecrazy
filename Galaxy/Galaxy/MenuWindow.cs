using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace Galaxy
{
    public enum WindowState { Starting, Active, Ending, Inactive }

    class MenuWindow
    {
        protected TimeSpan changeSpan; // How long does it take to fade-in or fade-out the menu
        protected WindowState windowState; // What is the current state of the window
        protected List<MenuItem> itemList; // What are the menu items
        protected int selectedItem; // Which item is currently selected
        protected double changeProgress; // How far along is the fading progress
        protected string menuTitle;
        protected Texture2D backgroundImage;
        protected SpriteFont TitleFont;
        protected SpriteFont OptionFont;

        // Struct to store the item's text as well as the menu it points to
        protected struct MenuItem
        {
            public string itemText;
            public MenuWindow itemLink;

            public MenuItem(string itemText, MenuWindow itemLink)
            {
                this.itemText = itemText;
                this.itemLink = itemLink;
            }
        }

        // Constructor to initialize the values of the class
        public MenuWindow(string menuTitle, Texture2D backgroundImage, SpriteFont TitleFont, SpriteFont OptionFont)
        {
            itemList = new List<MenuItem>();
            changeSpan = TimeSpan.FromMilliseconds(800);
            selectedItem = 0;
            changeProgress = 0;
            windowState = WindowState.Inactive;
            this.menuTitle = menuTitle;
            this.backgroundImage = backgroundImage;
            this.TitleFont = TitleFont;
            this.OptionFont = OptionFont;
        }

        // Process user input (because that is moderately important)
        public virtual MenuWindow ProcessInput(KeyboardState prevKeyState, KeyboardState nextKeyState)
        {
            // Select next menu item with the "down" key, down on the D-pad, or by moving the left thumbstick down
            if (prevKeyState.IsKeyUp(Keys.Down) && nextKeyState.IsKeyDown(Keys.Down))
                selectedItem++;

            // Select previous menu item with the "up" key, up on the D-pad, or by moving the left thumbstick up
            if (prevKeyState.IsKeyUp(Keys.Up) && nextKeyState.IsKeyDown(Keys.Up))
                selectedItem--;

            if (selectedItem < 0)
                selectedItem = 0;

            if (selectedItem >= itemList.Count)
                selectedItem = itemList.Count - 1;

            // Activate currently selected menu item with the Enter key or the A button
            if (prevKeyState.IsKeyUp(Keys.Enter) && nextKeyState.IsKeyDown(Keys.Enter))
            {
                windowState = WindowState.Ending;
                return itemList[selectedItem].itemLink;
            }
            // Return to the last menu with the Escape key
            else if (prevKeyState.IsKeyUp(Keys.Escape) && nextKeyState.IsKeyDown(Keys.Escape))
            {
                windowState = WindowState.Ending;
                return itemList[itemList.Count - 1].itemLink;
            }
            else
                return this;
        }

        // Method to add items to the menu
        public void AddMenuItem(string itemText, MenuWindow itemLink)
        {
            MenuItem newItem = new MenuItem(itemText, itemLink);
            itemList.Add(newItem);
        }

        // Method to activate an inactive menu
        public void WakeUp()
        {
            windowState = WindowState.Starting;
        }

        // Update it, motherfucker!
        public virtual void Update(double timePassedSinceLastFrame)
        {
            if ((windowState == WindowState.Starting) || (windowState == WindowState.Ending))
                changeProgress += timePassedSinceLastFrame / changeSpan.TotalMilliseconds;

            if (changeProgress >= 1.0f)
            {
                changeProgress = 0.0f;
                if (windowState == WindowState.Starting)
                    windowState = WindowState.Active;
                if (windowState == WindowState.Ending)
                    windowState = WindowState.Inactive;
            }
        }

        // Draw, pilgrim!
        public virtual void Draw(SpriteBatch spriteBatch, int ScreenWidth, int ScreenHeight)
        {
            if (windowState == WindowState.Inactive)
                return;

            float smoothedProgress = MathHelper.SmoothStep(0, 1, (float)changeProgress);

            int verPosition = 0;
            float centerPosition = ScreenWidth/2;
            float horPosition;
            float alphaValue;
            float bgLayerDepth;
            Color bgColor;

            // Adjust horizontal position and alpha for the menu items in the Starting or Ending modes
            switch (windowState)
            {
                case WindowState.Starting:
                    //horPosition -= 200 * (1.0f - (float)smoothedProgress);
                    alphaValue = smoothedProgress;
                    bgLayerDepth = 0.1f;
                    bgColor = new Color(new Vector4(1, 1, 1, alphaValue));
                    break;
                case WindowState.Ending:
                    //horPosition += 200 * (float)smoothedProgress;
                    alphaValue = 1.0f - smoothedProgress;
                    bgLayerDepth = 0.2f;
                    bgColor = new Color(new Vector4(1, 1, 1, alphaValue));
                    break;
                default:
                    alphaValue = 1;
                    bgLayerDepth = 0.1f;
                    bgColor = Color.White;
                    break;
            }

            Color defaultColor = new Color(new Vector4(1, 1, 1, alphaValue));
            Color selectedColor = new Color(new Vector4(1, 0.75f, 0, alphaValue));

            spriteBatch.Draw(backgroundImage, new Vector2(), new Rectangle(0, 0, ScreenWidth, ScreenHeight), bgColor, 0, Vector2.Zero, 1, SpriteEffects.None, bgLayerDepth);
            spriteBatch.DrawString(TitleFont, menuTitle, new Vector2(centerPosition - TitleFont.MeasureString(menuTitle).X / 2, verPosition), defaultColor, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            verPosition += 150;

            // Render the caption in the correct position for each menu item (red for selected item, black otherwise)
            for (int itemID = 0; itemID < itemList.Count; itemID++)
            {
                horPosition = centerPosition - OptionFont.MeasureString(itemList[itemID].itemText).X / 2;
                Vector2 itemPosition = new Vector2(horPosition, verPosition);
                Color itemColor = Color.White;

                if (itemID == selectedItem)
                    spriteBatch.DrawString(OptionFont, itemList[itemID].itemText, itemPosition, selectedColor, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                else
                    spriteBatch.DrawString(OptionFont, itemList[itemID].itemText, itemPosition, defaultColor, 0, Vector2.Zero, 1, SpriteEffects.None, 0);

                verPosition += 60;
            }
        }
    }
}
