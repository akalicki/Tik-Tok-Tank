using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tik_Tok_Tank.Menus
{
    class PauseMenu : Menu
    {
        public PauseMenu()
            : base()
        {
            Title = "Game Paused";

            AddMenuItem("Resume");
            AddMenuItem("Main Menu");
            AddMenuItem("Quit Game");
        }
    }
}
