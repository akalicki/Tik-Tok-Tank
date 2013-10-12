using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tik_Tok_Tank.Menus
{
    class Menu
    {
        private List<string> MenuItems;
        private int iterator;
        private string title = "";

        public int PlayerIndex; // used for controls menu

        public int Iterator
        {
            get { return iterator; }
            set
            {
                iterator = (int)MathHelper.Clamp(value, 0, MenuItems.Count - 1);
            }
        }

        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        public Menu()
        {
            MenuItems = new List<string>();
            Iterator = 0;
        }

        public int GetNumberOfOptions()
        {
            return MenuItems.Count;
        }

        public string GetMenuItem(int index)
        {
            return MenuItems[index];
        }

        public void AddMenuItem(string menuItem)
        {
            MenuItems.Add(menuItem);
        }

        public void DrawMenu(SpriteBatch spriteBatch, Texture2D menuBackground, int screenWidth, int screenHeight, SpriteFont font)
        {
            spriteBatch.Draw(menuBackground, new Rectangle(0, 0, screenWidth, screenHeight), Color.White);
            int yPos = 210;

            float titleLength = font.MeasureString(Title).X;
            spriteBatch.DrawString(font, Title, new Vector2(screenWidth / 2 - titleLength / 2, yPos), Color.DarkRed);
            yPos += 60;

            for (int i = 0; i < GetNumberOfOptions(); i++)
            {
                Color color = Color.Brown;
                if (i == Iterator)
                {
                    color = Color.Black;
                }
                string nextItem = GetMenuItem(i);
                float stringLength = font.MeasureString(nextItem).X;
                spriteBatch.DrawString(font, nextItem, new Vector2(screenWidth / 2 - stringLength / 2, yPos), color);
                yPos += 50;
            }
        }
    }
}
