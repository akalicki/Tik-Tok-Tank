using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tik_Tok_Tank.Menus
{
    class OptionsMenu : Menu
    {
        public OptionsMenu()
            : base()
        {
            Title = "Options";

            AddMenuItem("1P Controls");
            AddMenuItem("2P Controls");
            AddMenuItem("Sound ON / OFF");
            AddMenuItem("Main Menu");
        }
    }
}
