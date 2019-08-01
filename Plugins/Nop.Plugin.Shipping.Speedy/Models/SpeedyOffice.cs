using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Shipping.Speedy.Models
{
    public class SpeedyOffice
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public bool IsAutomat { get; set; }

        public SpeedyOffice(long id, string name, bool isAutomat)
        {
            Id = id;
            Name = name;
            IsAutomat = isAutomat;
        }
    }
}
