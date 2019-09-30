using FluentValidation;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Core.Domain.Catalog;
using Nop.Data;
using Nop.Services.Localization;
using Nop.Services.Seo;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Catalog
{
    public partial class ProductValidator : BaseNopValidator<ProductModel>
    {
        public ProductValidator(ILocalizationService localizationService, IDbContext dbContext)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Catalog.Products.Fields.Name.Required"));
            RuleFor(x => x.SeName).Length(0, NopSeoDefaults.SearchEngineNameLength)
                .WithMessage(string.Format(localizationService.GetResource("Admin.SEO.SeName.MaxLengthValidation"), NopSeoDefaults.SearchEngineNameLength));

            RuleFor(x => x.ShortDescription).NotEmpty().WithMessage(localizationService.GetResource("Admin.Catalog.Products.Fields.ShortDescription.Required"));
            RuleFor(x => x.FullDescription).NotEmpty().WithMessage(localizationService.GetResource("Admin.Catalog.Products.Fields.FullDescription.Required"));
            RuleFor(x => x.SelectedCategoryIds).NotNull().WithMessage(localizationService.GetResource("Admin.Catalog.Products.Fields.SelectedCategoryIds.Required"));
            RuleFor(x => x.Price).NotEqual(0).WithMessage(localizationService.GetResource("Admin.Catalog.Products.Fields.Price.Required"));
            RuleFor(x => x.Weight).NotEqual(0).WithMessage(localizationService.GetResource("Admin.Catalog.Products.Fields.Weight.Required"));
            RuleFor(x => x.DeliveryDateId).NotEqual(0).WithMessage(localizationService.GetResource("Admin.Catalog.Products.Fields.DeliveryDate.Required"));

            SetDatabaseValidationRules<Product>(dbContext);
        }
    }
}