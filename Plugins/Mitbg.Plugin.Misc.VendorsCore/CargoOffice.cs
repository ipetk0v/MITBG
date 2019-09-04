using System;
using System.Collections.Generic;
using System.Text;

namespace Mitbg.Plugin.Misc.VendorsCore
{
    public class CargoOffice
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public bool IsAutomat { get; set; }

        public CargoOffice(long id, string name, bool isAutomat)
        {
            Id = id;
            Name = name;
            IsAutomat = isAutomat;
        }
    }
}
