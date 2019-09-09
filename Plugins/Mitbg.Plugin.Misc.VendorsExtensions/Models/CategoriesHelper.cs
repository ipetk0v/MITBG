using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nop.Core.Domain.Catalog;

namespace Mitbg.Plugin.Misc.VendorsExtensions.Models
{
    public static class CategoriesHelper
    {
        public static IList<CategoryTreeItem> BuildTree(this IEnumerable<Category> source)
        {
            var dict = source.Select(s => new CategoryTreeItem()
            {
                Id = s.Id,
                Name = s.Name,
                ParentId = s.ParentCategoryId,
                Child = new List<CategoryTreeItem>()
            }).ToDictionary(w => w.Id, w => w);

            foreach (var item in dict)
            {
                if (item.Value.ParentId > 0)
                {
                    dict[item.Value.ParentId].Child.Add(item.Value);
                }
            }

            return dict.Where(w => w.Value.ParentId == 0).Select(w => w.Value).ToList();
        }
    }
}
