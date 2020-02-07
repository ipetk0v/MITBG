using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Mitbg.Plugin.Misc.XmlAutomationImport.Models;
using Nop.Core.Data;
using Nop.Core.Domain.Vendors;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using System.Linq;
using System.Xml;
using Microsoft.AspNetCore.Http;
using Mitbg.Plugin.Misc.XmlAutomationImport.Data.Entities;
using Mitbg.Plugin.Misc.XmlAutomationImport.Services;
using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Web.Areas.Admin.Controllers;

namespace Mitbg.Plugin.Misc.XmlAutomationImport.Controllers
{
    public class XmlAutomationImportController : BaseAdminController
    {
        private readonly IPermissionService _permissionService;
        private readonly IRepository<Vendor> _vendorsRep;
        private readonly ILocalizationService _localizationService;
        private readonly IRepository<XmlAutomationImportTemplate> _xmlAutomationImportTemplateRep;
        private readonly INotificationService _notificationService;
        private readonly IProductService _productService;
        private readonly IXmlAutomationImportService _xmlAutomationImportService;

        public XmlAutomationImportController(
            IPermissionService permissionService,
            IRepository<Vendor> vendorsRep,
            ILocalizationService localizationService,
            IRepository<XmlAutomationImportTemplate> xmlAutomationImportTemplateRep,
            INotificationService notificationService,
            IProductService productService,
            IXmlAutomationImportService xmlAutomationImportService)
        {
            _permissionService = permissionService;
            _vendorsRep = vendorsRep;
            _localizationService = localizationService;
            _xmlAutomationImportTemplateRep = xmlAutomationImportTemplateRep;
            _notificationService = notificationService;
            _productService = productService;
            _xmlAutomationImportService = xmlAutomationImportService;
        }

        [Area(AreaNames.Admin)]
        [HttpGet]
        public IActionResult TemplateList()
        {
            if (!_permissionService.Authorize(Permission.XmlAutomationImport))
                return AccessDeniedView();

            var model = new XmlAutomationImportSearchModel()
            {
                Vendors = _vendorsRep.Table.Where(w => !w.Deleted && w.Active)
                    .Select(ss => new SelectListItem(ss.Name, ss.Id.ToString())).ToList().OrderBy(w => w.Text).ToList()
            };

            model.Vendors.Insert(0, new SelectListItem(_localizationService.GetResource("XmlAutomationImport.Editor.AllVendors"), "-1", true));

            return View("~/Plugins/Mitbg.Plugin.Misc.XmlAutomationImport/Views/TemplateList.cshtml", model);
        }

        [HttpPost]
        public IActionResult TemplateList(XmlAutomationImportSearchModel searchModel)
        {
            if (!_permissionService.Authorize(Permission.XmlAutomationImport))
                return AccessDeniedView();

            var xmlAutomationImportTemplateQuery = _xmlAutomationImportTemplateRep.Table;

            if (searchModel.VendorId > 0)
                xmlAutomationImportTemplateQuery = xmlAutomationImportTemplateQuery.Where(w => w.VendorId == searchModel.VendorId);

            var totalCount = xmlAutomationImportTemplateQuery.Count();

            var templates = xmlAutomationImportTemplateQuery.OrderBy(w => w.VendorId).Skip(searchModel.Start).Take(searchModel.PageSize).ToList();

            var vendorsNames = _vendorsRep.Table.ToList().Where(w => templates.Select(s => s.VendorId).Contains(w.Id)).ToDictionary(w => w.Id, w => w.Name);

            var model = new XmlAutomationImportListModel
            {
                Data = templates.Select(s => new XmlAutomationImportModel
                {
                    Id = s.Id,
                    VendorId = s.VendorId,
                    Vendor = vendorsNames.ContainsKey(s.VendorId) ? vendorsNames[s.VendorId] : "",
                    XmlLinkUrl = s.XmlLinkUrl,
                    Prefix = s.Prefix,
                    LastActivity = s.LastActivity,
                    CreatedOn = s.CreatedOn,
                    IsActive = s.IsActive
                }),
                Total = totalCount,
                RecordsTotal = totalCount,
                RecordsFiltered = totalCount
            };



            return Json(model);
        }

        public virtual IActionResult CreateTemplate()
        {
            if (!_permissionService.Authorize(Permission.XmlAutomationImport))
                return AccessDeniedView();

            //prepare model
            var model = new XmlAutomationImportTemplateCreateEditModel
            {
                AvailableVendors = _vendorsRep.Table.Where(w => !w.Deleted && w.Active).Select(s => new SelectListItem(s.Name, s.Id.ToString())).ToList().OrderBy(w => w.Text).ToList(),
            };

            model.VendorId = int.Parse(model.AvailableVendors.First().Value);

            return View("~/Plugins/Mitbg.Plugin.Misc.XmlAutomationImport/Views/CreateTemplate.cshtml", model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public virtual IActionResult CreateTemplate(XmlAutomationImportTemplateCreateEditModel model, bool continueEditing, IFormCollection form)
        {
            if (!_permissionService.Authorize(Permission.XmlAutomationImport))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                if (model.VendorId == 0)
                    return RedirectToAction(nameof(TemplateList));

                var xmlAutomationImportTemplate = new XmlAutomationImportTemplate
                {
                    VendorId = model.VendorId,
                    XmlLinkUrl = model.XmlLink,
                    Prefix = model.Prefix,
                    Category = model.Category,
                    FullDescription = model.FullDescription,
                    Manufacturer = model.Manufacturer,
                    ManufacturerPartNumber = model.ManufacturerPartNumber,
                    Name = model.Name,
                    OldPrice = model.OldPrice,
                    Picture = model.Picture,
                    Price = model.Price,
                    ShortDescription = model.ShortDescription,
                    Sku = model.Sku,
                    StockQuantity = model.StockQuantity,
                    Weight = model.Weight,
                    CreatedOn = DateTime.Now,
                    LastActivity = DateTime.Now,
                    IsActive = model.IsActive,
                    ProductTemplate = model.ProductTemplate,
                    ProductForImport = model.ProductForImport
                };

                _xmlAutomationImportTemplateRep.Insert(xmlAutomationImportTemplate);
                _notificationService.SuccessNotification(_localizationService.GetResource("XmlAutomationImport.Editor.Created"));

                if (!continueEditing)
                    return RedirectToAction(nameof(TemplateList));

                return RedirectToAction(nameof(EditTemplate), new { id = xmlAutomationImportTemplate.Id });
            }

            model.AvailableVendors = _vendorsRep.Table.Where(w => !w.Deleted && w.Active)
                .Select(s => new SelectListItem(s.Name, s.Id.ToString())).ToList().OrderBy(w => w.Text).ToList();

            return View("~/Plugins/Mitbg.Plugin.Misc.XmlAutomationImport/Views/CreateTemplate.cshtml", model);
        }

        public virtual IActionResult EditTemplate(int id)
        {
            if (!_permissionService.Authorize(Permission.XmlAutomationImport))
                return AccessDeniedView();

            //try to get a customer with the specified id
            var xmlAutomationImportTemplate = _xmlAutomationImportTemplateRep.Table.FirstOrDefault(w => w.Id == id);
            if (xmlAutomationImportTemplate == null)
                return RedirectToAction(nameof(TemplateList));

            var model = new XmlAutomationImportTemplateCreateEditModel
            {
                AvailableVendors = _vendorsRep.Table.Where(w => !w.Deleted && w.Active)
                    .Select(s => new SelectListItem(s.Name, s.Id.ToString())).ToList().OrderBy(w => w.Text).ToList(),
                Id = xmlAutomationImportTemplate.Id,
                VendorId = xmlAutomationImportTemplate.VendorId,
                XmlLink = xmlAutomationImportTemplate.XmlLinkUrl,
                Prefix = xmlAutomationImportTemplate.Prefix,
                Category = xmlAutomationImportTemplate.Category,
                FullDescription = xmlAutomationImportTemplate.FullDescription,
                Manufacturer = xmlAutomationImportTemplate.Manufacturer,
                ManufacturerPartNumber = xmlAutomationImportTemplate.ManufacturerPartNumber,
                Name = xmlAutomationImportTemplate.Name,
                OldPrice = xmlAutomationImportTemplate.OldPrice,
                Picture = xmlAutomationImportTemplate.Picture,
                Price = xmlAutomationImportTemplate.Price,
                ShortDescription = xmlAutomationImportTemplate.ShortDescription,
                Sku = xmlAutomationImportTemplate.Sku,
                StockQuantity = xmlAutomationImportTemplate.StockQuantity,
                Weight = xmlAutomationImportTemplate.Weight,
                IsActive = xmlAutomationImportTemplate.IsActive,
                ProductTemplate = xmlAutomationImportTemplate.ProductTemplate,
                ProductForImport = xmlAutomationImportTemplate.ProductForImport
            };


            return View("~/Plugins/Mitbg.Plugin.Misc.XmlAutomationImport/Views/EditTemplate.cshtml", model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public virtual IActionResult Edit(XmlAutomationImportTemplateCreateEditModel model, bool continueEditing, IFormCollection form)
        {
            if (!_permissionService.Authorize(Permission.XmlAutomationImport))
                return AccessDeniedView();

            var xmlAutomationImportTemplate = _xmlAutomationImportTemplateRep.Table.FirstOrDefault(w => w.Id == model.Id);
            var existsXmlAutomationImportTemplate = _xmlAutomationImportTemplateRep.Table.FirstOrDefault(w =>
                w.VendorId == model.VendorId && w.Id != model.Id);

            if (xmlAutomationImportTemplate == null)
                ModelState.AddModelError(string.Empty, "Xml Automation Import Template not found");
            else
            {
                if (existsXmlAutomationImportTemplate != null)
                    ModelState.AddModelError(string.Empty, "Xml Automation Import Template with same params already exists");
                else
                {
                    if (ModelState.IsValid)
                    {
                        xmlAutomationImportTemplate.VendorId = model.VendorId;
                        xmlAutomationImportTemplate.XmlLinkUrl = model.XmlLink;
                        xmlAutomationImportTemplate.Prefix = model.Prefix;
                        xmlAutomationImportTemplate.Category = model.Category;
                        xmlAutomationImportTemplate.FullDescription = model.FullDescription;
                        xmlAutomationImportTemplate.Manufacturer = model.Manufacturer;
                        xmlAutomationImportTemplate.ManufacturerPartNumber = model.ManufacturerPartNumber;
                        xmlAutomationImportTemplate.Name = model.Name;
                        xmlAutomationImportTemplate.OldPrice = model.OldPrice;
                        xmlAutomationImportTemplate.Picture = model.Picture;
                        xmlAutomationImportTemplate.Price = model.Price;
                        xmlAutomationImportTemplate.ShortDescription = model.ShortDescription;
                        xmlAutomationImportTemplate.Sku = model.Sku;
                        xmlAutomationImportTemplate.StockQuantity = model.StockQuantity;
                        xmlAutomationImportTemplate.Weight = model.Weight;
                        xmlAutomationImportTemplate.IsActive = model.IsActive;
                        xmlAutomationImportTemplate.ProductTemplate = model.ProductTemplate;
                        xmlAutomationImportTemplate.ProductForImport = model.ProductForImport;

                        _xmlAutomationImportTemplateRep.Update(xmlAutomationImportTemplate);
                        _notificationService.SuccessNotification(_localizationService.GetResource("XmlAutomationImport.Editor.Updated"));

                        if (!continueEditing)
                            return RedirectToAction(nameof(TemplateList));

                        return RedirectToAction(nameof(EditTemplate), new { id = xmlAutomationImportTemplate.Id });
                    }
                }
            }

            model.AvailableVendors = _vendorsRep.Table.Where(w => !w.Deleted && w.Active)
                .Select(s => new SelectListItem(s.Name, s.Id.ToString())).ToList().OrderBy(w => w.Text).ToList();

            return View("~/Plugins/Mitbg.Plugin.Misc.XmlAutomationImport/Views/EditTemplate.cshtml", model);
        }

        [HttpPost]
        public virtual IActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(Permission.XmlAutomationImport))
                return AccessDeniedView();

            var xmlAutomationImportTemplate = _xmlAutomationImportTemplateRep.Table.FirstOrDefault(w => w.Id == id);
            if (xmlAutomationImportTemplate == null)
                return RedirectToAction(nameof(TemplateList));

            try
            {
                //delete
                _xmlAutomationImportTemplateRep.Delete(xmlAutomationImportTemplate);
                _notificationService.SuccessNotification(_localizationService.GetResource("XmlAutomationImport.Editor.Deleted"));
                return RedirectToAction(nameof(TemplateList));
            }
            catch (Exception exc)
            {
                _notificationService.ErrorNotification(exc.Message);
                return RedirectToAction(nameof(EditTemplate), new { id = xmlAutomationImportTemplate.Id });
            }
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("xmlexecute")]
        public virtual IActionResult ExecuteProductTransfer(int id)
        {
            var xmlAutomationImportTemplate = _xmlAutomationImportTemplateRep.Table.FirstOrDefault(w => w.Id == id);

            if (xmlAutomationImportTemplate == null)
                return RedirectToAction(nameof(TemplateList));

            try
            {
                _xmlAutomationImportService.Execute(xmlAutomationImportTemplate);
                _notificationService.SuccessNotification("Success xml automation import");
            }
            catch (Exception e)
            {
                _notificationService.ErrorNotification(e.Message);
            }

            return RedirectToAction(nameof(TemplateList));
        }
    }
}
