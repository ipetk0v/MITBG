using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Shipping.Speedy.Domain
{
    public enum BolCreatingStatus : int
    {
        Pending = 1,
        BolIsCreated = 2,
        ErrorBolCreating = 3,
        Cancelled = 4
    }
}
