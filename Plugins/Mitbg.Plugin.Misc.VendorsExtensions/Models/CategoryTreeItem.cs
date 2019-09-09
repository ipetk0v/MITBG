using System;
using System.Collections.Generic;
using System.Text;

namespace Mitbg.Plugin.Misc.VendorsExtensions.Models
{
    public class CategoryTreeItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ParentId { get; set; }
        public IList<CategoryTreeItem> Child { get; set; }
    }
}
