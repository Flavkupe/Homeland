using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TacticsGame.UI.Dialogs
{
    public interface ICanBeClosed
    {
        bool CloseOnRightClick { get; }

        event EventHandler CloseClicked;
    }
}
