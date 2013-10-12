using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tik_Tok_Tank.Menus
{
    class GameOverMenu : Menu
    {
        public GameOverMenu(string title = "Game Over!")
            : base()
        {
            Title = title;

            AddMenuItem("Main Menu");
            AddMenuItem("Quit Game");
        }
    }
}
