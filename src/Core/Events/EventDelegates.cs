using System;
using System.Collections.Generic;
using System.Text;

namespace WheelMUD.Core.Events
{
    public delegate void WMudFinalEvent();
    public delegate void WMudChangeableEvent(bool cancel);
}
