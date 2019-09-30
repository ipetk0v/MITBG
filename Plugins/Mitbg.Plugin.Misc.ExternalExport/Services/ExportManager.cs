using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Media;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Seo;
using Nop.Web.Factories;
using Nop.Web.Infrastructure.Cache;
using Nop.Web.Models.Media;

namespace Mitbg.Plugin.Misc.ExternalExport.Services
{
    public class ExportManager : IExportManager
    {
        private readonly IProductService _productService;
        private readonly IRepository<Product> _productsRep;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IPictureService _pictureService;
        private readonly ILocalizationService _localizationService;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;
        private readonly ICacheManager _cacheManager;
        private readonly MediaSettings _mediaSettings;
        private readonly IWebHelper _webHelper;

        public ExportManager(IProductService productService, IRepository<Product> productsRep, IUrlRecordService urlRecordService, IPictureService pictureService, ILocalizationService localizationService, IStoreContext storeContext, IWorkContext workContext, ICacheManager cacheManager, MediaSettings mediaSettings, IWebHelper webHelper)
        {
            _productService = productService;
            _productsRep = productsRep;
            _urlRecordService = urlRecordService;
            _pictureService = pictureService;
            _localizationService = localizationService;
            _storeContext = storeContext;
            _workContext = workContext;
            _cacheManager = cacheManager;
            _mediaSettings = mediaSettings;
            _webHelper = webHelper;
        }

        public void GenerateFile()
        {
            var productIds = _productsRep.Table.Where(w => !w.Deleted && w.Published&&w.ProductManufacturers.Any()).Select(w => w.Id).ToList();
            var page = 1;
            var pageSize = 100;
            var xProducts = new XElement("Products");
            while (productIds.Count > page * pageSize - pageSize)
            {
                var products = _productService.GetProductsByIds(productIds.Skip(page * pageSize - pageSize).Take(pageSize).ToArray());

                foreach (var product in products)
                {

                    var seName = _urlRecordService.GetSeName(product);
                    var category = product.ProductCategories.FirstOrDefault();
                    var manufacturer = product.ProductManufacturers.FirstOrDefault();
                    var picture = PrepareProductOverviewPictureModel(product, 1024);

                    var xAttributes = new XElement("Attributes");

                    foreach (var attr in product.ProductSpecificationAttributes)
                    {
                        xAttributes.Add(new XElement("Attribute",
                            new XElement("Attribute_name", attr.SpecificationAttributeOption.SpecificationAttribute.Name),
                            new XElement("Attribute_value", attr.SpecificationAttributeOption.Name)
                            ));
                    }

                    var xProduct = new XElement("Product",
                        new XElement("Identifier", product.Id.ToString("00000000")),
                        new XElement("Productid", product.ManufacturerPartNumber),
                        new XElement("Manufacturer", manufacturer != null ? manufacturer.Manufacturer.Name : ""),
                        new XElement("Name", product.Name),
                        new XElement("Product_url", _webHelper.GetStoreLocation() + seName),
                        new XElement("Price", product.Price),
                        new XElement("Image_url", picture.FullSizeImageUrl),
                        new XElement("Category", category != null ? category.Category.Name : ""),
                        new XElement("Description", product.FullDescription),
                        new XElement("Delivery_Time", "3 дни"),
                        new XElement("Delivery_Cost", "5 лв"),
                        new XElement("EAN_code", product.Gtin)
                    );
                    
                    if (product.ProductSpecificationAttributes.Any())
                        xProduct.Add(xAttributes);
                    
                    xProducts.Add(xProduct);

                }

                page++;
            }
            var xDoc = new XDocument(xProducts);
            xDoc.Save("wwwroot/products.xml");
        }


        /// <summary>
        /// Prepare the product overview picture model
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="productThumbPictureSize">Product thumb picture size (longest side); pass null to use the default value of media settings</param>
        /// <returns>Picture model</returns>
        protected virtual PictureModel PrepareProductOverviewPictureModel(Product product, int? productThumbPictureSize = null)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            var productName = _localizationService.GetLocalized(product, x => x.Name);
            //If a size has been set in the view, we use it in priority
            var pictureSize = productThumbPictureSize ?? _mediaSettings.ProductThumbPictureSize;

            //prepare picture model
            var cacheKey = string.Format(NopModelCacheDefaults.ProductDefaultPictureModelKey,
                product.Id, pictureSize, true, _workContext.WorkingLanguage.Id, _webHelper.IsCurrentConnectionSecured(),
                _storeContext.CurrentStore.Id);

            var defaultPictureModel = _cacheManager.Get(cacheKey, () =>
            {
                var picture = _pictureService.GetPicturesByProductId(product.Id, 1).FirstOrDefault();
                var pictureModel = new PictureModel
                {
                    ImageUrl = _pictureService.GetPictureUrl(picture, pictureSize),
                    FullSizeImageUrl = _pictureService.GetPictureUrl(picture),
                    //"title" attribute
                    Title = (picture != null && !string.IsNullOrEmpty(picture.TitleAttribute))
                        ? picture.TitleAttribute
                        : string.Format(_localizationService.GetResource("Media.Product.ImageLinkTitleFormat"),
                            productName),
                    //"alt" attribute
                    AlternateText = (picture != null && !string.IsNullOrEmpty(picture.AltAttribute))
                        ? picture.AltAttribute
                        : string.Format(_localizationService.GetResource("Media.Product.ImageAlternateTextFormat"),
                            productName)
                };

                return pictureModel;
            });

            return defaultPictureModel;
        }

    }

    public interface IExportManager
    {
        void GenerateFile();
    }
}