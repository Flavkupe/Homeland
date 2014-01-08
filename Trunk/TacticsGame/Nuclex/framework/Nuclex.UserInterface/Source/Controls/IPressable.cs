using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nuclex.UserInterface.Source.Controls
{
    public interface IPressable
    {
        event EventHandler Pressed;
    }
}
