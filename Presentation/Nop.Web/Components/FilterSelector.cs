using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Components;
using Nop.Web.Models.Catalog;

namespace Nop.Web.Components
{
    public class FilterSelectorViewComponent : NopViewComponent
    {
        public IViewComponentResult Invoke(CatalogPagingFilteringModel model)
        {
            return View(model);
        }
    }
}
