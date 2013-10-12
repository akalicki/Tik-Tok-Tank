using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tik_Tok_Tank.Menus
{
    class ControlsMenu : Menu
    {
        public ControlsMenu(int playerIndex)
            : base()
        {
            PlayerIndex = playerIndex;
            if (PlayerIndex == 1)
            {
                Title = "1P Controls";
            }
            else
            {
                Title = "2P Controls";
            }

            AddMenuItem("Computer");
            AddMenuItem("XBOX 1");
            AddMenuItem("XBOX 2");
            AddMenuItem("Main Menu");
        }
    }
}
