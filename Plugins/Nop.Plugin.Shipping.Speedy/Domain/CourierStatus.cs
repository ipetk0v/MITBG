using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Shipping.Speedy.Domain
{
    public enum CourierStatus : int
    {
        NotRequested = 1,
        Requested = 2,
        BringToOffice = 3,
        ErrorRequested = 4
    }
}
