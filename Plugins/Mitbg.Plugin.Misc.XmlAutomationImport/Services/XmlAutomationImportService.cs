using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Xml;
using Microsoft.AspNetCore.StaticFiles;
using Mitbg.Plugin.Misc.XmlAutomationImport.Data.Entities;
using Mitbg.Plugin.Misc.XmlAutomationImport.Models;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Media;
using Nop.Core.Http;
using Nop.Core.Infrastructure;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Web.Areas.Admin.Models.Catalog;

namespace Mitbg.Plugin.Misc.XmlAutomationImport.Services
{
    public class XmlAutomationImportService : IXmlAutomationImportService
    {
        private const string IMAGE_HASH_ALGORITHM = "SHA1";
        private const string UPLOADS_TEMP_PATH = "~/App_Data/TempUploads";

        private readonly IProductService _productService;
        private readonly INopFileProvider _fileProvider;
        private readonly ILogger _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IPictureService _pictureService;
        private readonly IDataProvider _dataProvider;
        private readonly ILocalizationService _localizationService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IEncryptionService _encryptionService;
        private readonly MediaSettings _mediaSettings;
        private readonly IWorkContext _workContext;
        private readonly IManufacturerService _manufacturerService;
        private readonly IRepository<ProductTemplate> _productTemplateRepository;
        private readonly IUrlRecordService _urlRecordService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly IRepository<XmlAutomationImportTemplate> _xmlAutomationImportTemplateRepo;
        private readonly IRepository<Manufacturer> _manufacturerRepository;

        public XmlAutomationImportService(
            IProductService productService,
            INopFileProvider fileProvider,
            ILogger logger,
            IHttpClientFactory httpClientFactory,
            IPictureService pictureService,
            IDataProvider dataProvider,
            IEncryptionService encryptionService,
            MediaSettings mediaSettings,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            IWorkContext workContext,
            IManufacturerService manufacturerService,
            IRepository<ProductTemplate> productTemplateRepository,
            IUrlRecordService urlRecordService,
            ILocalizedEntityService localizedEntityService,
            IRepository<XmlAutomationImportTemplate> xmlAutomationImportTemplateRepo,
            IRepository<Manufacturer> manufacturerRepository)
        {
            _localizationService = localizationService;
            _customerActivityService = customerActivityService;
            _productService = productService;
            _fileProvider = fileProvider;
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _pictureService = pictureService;
            _dataProvider = dataProvider;
            _encryptionService = encryptionService;
            _mediaSettings = mediaSettings;
            _workContext = workContext;
            _manufacturerService = manufacturerService;
            _productTemplateRepository = productTemplateRepository;
            _urlRecordService = urlRecordService;
            _localizedEntityService = localizedEntityService;
            _xmlAutomationImportTemplateRepo = xmlAutomationImportTemplateRepo;
            _manufacturerRepository = manufacturerRepository;
        }

        protected virtual string GetMimeTypeFromFilePath(string filePath)
        {
            new FileExtensionContentTypeProvider().TryGetContentType(filePath, out var mimeType);

            //set to jpeg in case mime type cannot be found
            return mimeType ?? MimeTypes.ImageJpeg;
        }

        private void LogPictureInsertError(string picturePath, Exception ex)
        {
            var extension = _fileProvider.GetFileExtension(picturePath);
            var name = _fileProvider.GetFileNameWithoutExtension(picturePath);

            var point = string.IsNullOrEmpty(extension) ? string.Empty : ".";
            var fileName = _fileProvider.FileExists(picturePath) ? $"{name}{point}{extension}" : string.Empty;
            _logger.Error($"Insert picture failed (file name: {fileName})", ex);
        }

        private string DownloadFile(string urlString, IList<string> downloadedFiles)
        {
            if (string.IsNullOrEmpty(urlString))
                return string.Empty;

            if (!Uri.IsWellFormedUriString(urlString, UriKind.Absolute))
                return urlString;

            //if (!_catalogSettings.ExportImportAllowDownloadImages)
            //    return string.Empty;

            //ensure that temp directory is created
            var tempDirectory = _fileProvider.MapPath(UPLOADS_TEMP_PATH);
            _fileProvider.CreateDirectory(tempDirectory);

            var fileName = _fileProvider.GetFileName(urlString);
            if (string.IsNullOrEmpty(fileName))
                return string.Empty;

            var filePath = _fileProvider.Combine(tempDirectory, fileName);
            try
            {
                var client = _httpClientFactory.CreateClient(NopHttpDefaults.DefaultHttpClient);
                var fileData = client.GetByteArrayAsync(urlString).Result;
                using (var fs = new FileStream(filePath, FileMode.OpenOrCreate))
                {
                    fs.Write(fileData, 0, fileData.Length);
                }

                downloadedFiles?.Add(filePath);
                return filePath;
            }
            catch (Exception ex)
            {
                _logger.Error("Download image failed", ex);
            }

            return string.Empty;
        }

        protected virtual void ImportProductImagesUsingServices(IList<ProductPictureMetadata> productPictureMetadata)
        {
            foreach (var product in productPictureMetadata)
            {
                foreach (var picturePath in new[] { product.Picture1Path, product.Picture2Path, product.Picture3Path })
                {
                    if (string.IsNullOrEmpty(picturePath))
                        continue;

                    var mimeType = GetMimeTypeFromFilePath(picturePath);
                    var newPictureBinary = _fileProvider.ReadAllBytes(picturePath);
                    var pictureAlreadyExists = false;
                    if (!product.IsNew)
                    {
                        //compare with existing product pictures
                        var existingPictures = _pictureService.GetPicturesByProductId(product.ProductItem.Id);
                        foreach (var existingPicture in existingPictures)
                        {
                            var existingBinary = _pictureService.LoadPictureBinary(existingPicture);
                            //picture binary after validation (like in database)
                            var validatedPictureBinary = _pictureService.ValidatePicture(newPictureBinary, mimeType);
                            if (!existingBinary.SequenceEqual(validatedPictureBinary) &&
                                !existingBinary.SequenceEqual(newPictureBinary))
                                continue;
                            //the same picture content
                            pictureAlreadyExists = true;
                            break;
                        }
                    }

                    if (pictureAlreadyExists)
                        continue;

                    try
                    {
                        var newPicture = _pictureService.InsertPicture(newPictureBinary, mimeType, _pictureService.GetPictureSeName(product.ProductItem.Name));
                        product.ProductItem.ProductPictures.Add(new ProductPicture
                        {
                            //EF has some weird issue if we set "Picture = newPicture" instead of "PictureId = newPicture.Id"
                            //pictures are duplicated
                            //maybe because entity size is too large
                            PictureId = newPicture.Id,
                            DisplayOrder = 1
                        });
                        _productService.UpdateProduct(product.ProductItem);
                    }
                    catch (Exception ex)
                    {
                        LogPictureInsertError(picturePath, ex);
                    }
                }
            }
        }

        protected virtual void ImportProductImagesUsingHash(IList<ProductPictureMetadata> productPictureMetadata, IList<Product> allProductsBySku)
        {
            //performance optimization, load all pictures hashes
            //it will only be used if the images are stored in the SQL Server database (not compact)
            var takeCount = _dataProvider.SupportedLengthOfBinaryHash - 1;
            var productsImagesIds = _productService.GetProductsImagesIds(allProductsBySku.Select(p => p.Id).ToArray());
            var allPicturesHashes = _pictureService.GetPicturesHash(productsImagesIds.SelectMany(p => p.Value).ToArray());

            foreach (var product in productPictureMetadata)
            {
                foreach (var picturePath in new[] { product.Picture1Path, product.Picture2Path, product.Picture3Path })
                {
                    if (string.IsNullOrEmpty(picturePath))
                        continue;
                    try
                    {
                        var mimeType = GetMimeTypeFromFilePath(picturePath);
                        var newPictureBinary = _fileProvider.ReadAllBytes(picturePath);
                        var pictureAlreadyExists = false;
                        if (!product.IsNew)
                        {
                            var newImageHash = _encryptionService.CreateHash(newPictureBinary.Take(takeCount).ToArray(),
                                IMAGE_HASH_ALGORITHM);
                            var newValidatedImageHash = _encryptionService.CreateHash(_pictureService.ValidatePicture(newPictureBinary, mimeType)
                                .Take(takeCount)
                                .ToArray(), IMAGE_HASH_ALGORITHM);

                            var imagesIds = productsImagesIds.ContainsKey(product.ProductItem.Id)
                                ? productsImagesIds[product.ProductItem.Id]
                                : new int[0];

                            pictureAlreadyExists = allPicturesHashes.Where(p => imagesIds.Contains(p.Key))
                                .Select(p => p.Value).Any(p => p == newImageHash || p == newValidatedImageHash);
                        }

                        if (pictureAlreadyExists)
                            continue;

                        var newPicture = _pictureService.InsertPicture(newPictureBinary, mimeType, _pictureService.GetPictureSeName(product.ProductItem.Name));
                        product.ProductItem.ProductPictures.Add(new ProductPicture
                        {
                            //EF has some weird issue if we set "Picture = newPicture" instead of "PictureId = newPicture.Id"
                            //pictures are duplicated
                            //maybe because entity size is too large
                            PictureId = newPicture.Id,
                            DisplayOrder = 1
                        });
                        _productService.UpdateProduct(product.ProductItem);
                    }
                    catch (Exception ex)
                    {
                        LogPictureInsertError(picturePath, ex);
                    }
                }
            }
        }

        public void Execute(XmlAutomationImportTemplate xmlAutomationImportTemplate)
        {
            var xmlDoc = new XmlDocument(); // Create an XML document object
            xmlDoc.Load(xmlAutomationImportTemplate.XmlLinkUrl); // Load the XML document from the specified file

            // Get elements
            var category = string.Empty;
            var manufacturer = string.Empty;
            var manufacturerPartNumber = string.Empty;
            var oldPrice = string.Empty;
            var pictureLink = string.Empty;
            var price = string.Empty;
            var sku = string.Empty;
            var quantity = string.Empty;
            var weight = string.Empty;
            var shortDescription = string.Empty;
            var productName = string.Empty;
            var fullDescription = string.Empty;

            var productTemplateSimple = _productTemplateRepository.Table.FirstOrDefault(pt => pt.Name == "Simple product");

            XmlNodeList xmlNL = xmlDoc.GetElementsByTagName(xmlAutomationImportTemplate.ProductTemplate);
            var productCounts = xmlAutomationImportTemplate.ProductForImport == 0 ? xmlNL.Count : xmlAutomationImportTemplate.ProductForImport;

            for (int i = 0; i < productCounts; i++)
            {
                if (!string.IsNullOrEmpty(xmlAutomationImportTemplate.Name))
                {
                    var pn = xmlDoc.GetElementsByTagName(xmlAutomationImportTemplate.Name);
                    productName = pn[i].InnerText;
                }

                if (!string.IsNullOrEmpty(xmlAutomationImportTemplate.FullDescription))
                {
                    var fd = xmlDoc.GetElementsByTagName(xmlAutomationImportTemplate.FullDescription);
                    fullDescription = fd[i].InnerText;
                }

                if (!string.IsNullOrEmpty(xmlAutomationImportTemplate.ShortDescription))
                {
                    var sd = xmlDoc.GetElementsByTagName(xmlAutomationImportTemplate.ShortDescription);
                    shortDescription = sd[i].InnerText;
                }

                if (!string.IsNullOrEmpty(xmlAutomationImportTemplate.Category))
                {
                    var cat = xmlDoc.GetElementsByTagName(xmlAutomationImportTemplate.Category);
                    category = cat[i].InnerText;
                }

                if (!string.IsNullOrEmpty(xmlAutomationImportTemplate.Manufacturer))
                {
                    var man = xmlDoc.GetElementsByTagName(xmlAutomationImportTemplate.Manufacturer);
                    manufacturer = man[i].InnerText;
                }

                if (!string.IsNullOrEmpty(xmlAutomationImportTemplate.ManufacturerPartNumber))
                {
                    var manPn = xmlDoc.GetElementsByTagName(xmlAutomationImportTemplate.ManufacturerPartNumber);
                    manufacturerPartNumber = manPn[i].InnerText;
                }

                if (!string.IsNullOrEmpty(xmlAutomationImportTemplate.OldPrice))
                {
                    var op = xmlDoc.GetElementsByTagName(xmlAutomationImportTemplate.OldPrice);
                    oldPrice = op[i].InnerText;
                }

                if (!string.IsNullOrEmpty(xmlAutomationImportTemplate.Picture))
                {
                    var pl = xmlDoc.GetElementsByTagName(xmlAutomationImportTemplate.Picture);
                    pictureLink = pl[i].InnerText;
                }

                if (!string.IsNullOrEmpty(xmlAutomationImportTemplate.Price))
                {
                    var p = xmlDoc.GetElementsByTagName(xmlAutomationImportTemplate.Price);
                    price = p[i].InnerText;
                }

                if (!string.IsNullOrEmpty(xmlAutomationImportTemplate.Sku))
                {
                    var sk = xmlDoc.GetElementsByTagName(xmlAutomationImportTemplate.Sku);
                    sku = sk[i].InnerText;
                }

                if (!string.IsNullOrEmpty(xmlAutomationImportTemplate.StockQuantity))
                {
                    var q = xmlDoc.GetElementsByTagName(xmlAutomationImportTemplate.StockQuantity);
                    quantity = q[i].InnerText;
                }

                if (!string.IsNullOrEmpty(xmlAutomationImportTemplate.Weight))
                {
                    var w = xmlDoc.GetElementsByTagName(xmlAutomationImportTemplate.Weight);
                    weight = w[i].InnerText;
                }

                var randomManPartNumber = RandomString(10);

                if (!string.IsNullOrEmpty(xmlAutomationImportTemplate.Prefix))
                {
                    randomManPartNumber = xmlAutomationImportTemplate.Prefix + randomManPartNumber;
                    sku = xmlAutomationImportTemplate.Prefix + sku;
                }

                var productExist = _productService.GetProductBySku(sku);

                if (productExist != null)
                {
                    var changed = false;

                    var stockQuantityExist = !string.IsNullOrEmpty(quantity) ? int.Parse(quantity) : 0;
                    var priceExist = !string.IsNullOrEmpty(price) ? decimal.Parse(price) : 0;
                    var oldPriceExist = !string.IsNullOrEmpty(oldPrice) ? decimal.Parse(oldPrice) : 0;

                    changed = changed == false ? stockQuantityExist != productExist.StockQuantity : true;
                    changed = changed == false ? priceExist != productExist.Price : true;
                    changed = changed == false ? oldPriceExist != productExist.OldPrice : true;

                    if (changed)
                    {
                        productExist.StockQuantity = stockQuantityExist;
                        productExist.Price = priceExist;
                        productExist.OldPrice = oldPriceExist;

                        _productService.UpdateProduct(productExist);
                        //activity log
                        _customerActivityService.InsertActivity("EditProduct",
                            string.Format(_localizationService.GetResource("ActivityLog.EditProduct"), productExist.Name), productExist);
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(manufacturer))
                    {
                        var existManufacturer = _manufacturerRepository.Table.FirstOrDefault(c => c.Name == manufacturer);
                        if (existManufacturer == null)
                            _manufacturerService.InsertManufacturer(new Manufacturer { Name = manufacturer, Published = false, CreatedOnUtc = DateTime.Now, ManufacturerTemplateId = 1, PageSize = 6, PageSizeOptions = "6, 3, 9" });
                    }

                    var product = new Product
                    {
                        Name = productName,
                        ShortDescription = !string.IsNullOrEmpty(shortDescription) ? shortDescription : productName,
                        FullDescription = !string.IsNullOrEmpty(fullDescription) ? fullDescription : "",
                        ManufacturerPartNumber = !string.IsNullOrEmpty(manufacturerPartNumber) ? manufacturerPartNumber : randomManPartNumber,
                        Price = !string.IsNullOrEmpty(price) ? decimal.Parse(price) : 0,
                        OldPrice = !string.IsNullOrEmpty(oldPrice) ? decimal.Parse(oldPrice) : 0,
                        Sku = !string.IsNullOrEmpty(sku) ? sku : randomManPartNumber,
                        StockQuantity = !string.IsNullOrEmpty(quantity) ? int.Parse(quantity) : 0,
                        Weight = !string.IsNullOrEmpty(weight) ? (decimal.Parse(weight) == 0 ? 1 : decimal.Parse(weight)) : 1,
                        VendorId = xmlAutomationImportTemplate.VendorId,
                        ProductType = ProductType.SimpleProduct,
                        ProductTemplateId = productTemplateSimple.Id,
                        ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                        AllowCustomerReviews = true,
                        LowStockActivity = LowStockActivity.Unpublish,
                        BackorderMode = BackorderMode.NoBackorders,
                        VisibleIndividually = true,
                        MarkAsNew = true,
                        MarkAsNewEndDateTimeUtc = DateTime.Now.AddDays(14),
                        IsShipEnabled = true,
                        DeliveryDateId = 1,
                        OrderMinimumQuantity = 1,
                        OrderMaximumQuantity = 1000,
                        CreatedOnUtc = DateTime.UtcNow,
                        UpdatedOnUtc = DateTime.UtcNow,
                        ProductManufacturers = { !string.IsNullOrEmpty(manufacturer) ? new ProductManufacturer { Manufacturer = _manufacturerRepository.Table.FirstOrDefault(c => c.Name == manufacturer), DisplayOrder = 0 } : new ProductManufacturer() },
                    };

                    _productService.InsertProduct(product);

                    if (pictureLink != null)
                    {
                        var downloadedFiles = new List<string>();
                        var productPictureMetadata = new List<ProductPictureMetadata>
                    {
                        new ProductPictureMetadata
                        {
                            ProductItem = product,
                            Picture1Path = DownloadFile(pictureLink, downloadedFiles),
                            IsNew = true
                        }
                    };

                        var allProductsBySku = _productService.GetProductsBySku(new[] { product.Sku }, _workContext.CurrentVendor?.Id ?? 0);

                        if (_mediaSettings.ImportProductImagesUsingHash && _pictureService.StoreInDb && _dataProvider.SupportedLengthOfBinaryHash > 0)
                            ImportProductImagesUsingHash(productPictureMetadata, allProductsBySku);
                        else
                            ImportProductImagesUsingServices(productPictureMetadata);

                        foreach (var downloadedFile in downloadedFiles)
                        {
                            if (!_fileProvider.FileExists(downloadedFile))
                                continue;

                            try
                            {
                                _fileProvider.DeleteFile(downloadedFile);
                            }
                            catch
                            {
                                // ignored
                            }
                        }
                    }

                    _localizedEntityService.SaveLocalizedValue(product,
                        x => x.FullDescription,
                        fullDescription,
                        _workContext.WorkingLanguage.Id);

                    _urlRecordService.SaveSlug(product, _urlRecordService.ValidateSeName(product, null, product.Name, true), _workContext.WorkingLanguage.Id);

                    //activity log
                    _customerActivityService.InsertActivity("AddNewProduct",
                        string.Format(_localizationService.GetResource("ActivityLog.AddNewProduct"), product.Name), product);
                }
            }

            var xmlAutomationTempl = _xmlAutomationImportTemplateRepo.GetById(xmlAutomationImportTemplate.Id);
            xmlAutomationTempl.LastActivity = DateTime.Now;
            _xmlAutomationImportTemplateRepo.Update(xmlAutomationTempl);
        }

        private static string RandomString(int length)
        {
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
