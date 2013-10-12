using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tik_Tok_Tank.Menus
{
    class TitleMenu : Menu
    {
        public TitleMenu()
            : base()
        {
            AddMenuItem("Play Game");
            AddMenuItem("Options");
            AddMenuItem("Quit Game");
        }
    }
}
