using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tik_Tok_Tank.Menus
{
    class GameModesMenu : Menu
    {
        public GameModesMenu()
            : base()
        {
            Title = "Game Modes";

            AddMenuItem("Standard");
            AddMenuItem("Duel");
            AddMenuItem("Co-op");
            AddMenuItem("Adventure");
            AddMenuItem("Main Menu");
        }
    }
}
