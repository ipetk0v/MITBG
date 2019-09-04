using System.Collections.Generic;
using Mitbg.Plugin.Misc.VendorsCore.Domain;

namespace Nop.Plugin.Shipping.Speedy.Models
{
    public class FormViewModel
    {
        public FormViewModel()
        {
            DeliveryOption = DeliveryOption.Automat;
        }

        public bool IsSaved { get; set; }
        public int SiteId { get; set; }
        public string SiteName { get; set; }
        public int? OfficeId { get; set; }

        public string District { get; set; }
        public string StreetName { get; set; }
        public string QuarterText { get; set; }
        public string QuarterName { get; set; }
        public string QuarterType { get; set; }
        public long QuarterId { get; set; }
        public string StreetNo { get; set; }
        public int StreetId { get; set; }
        public string Block { get; set; }
        public string Entrance { get; set; }
        public string Floor { get; set; }
        public string ApNumber { get; set; }
        public string AddressNote { get; set; }
        public string Comment { get; set; }

        public bool DisableAutomats { get; set; }
        public bool DisableOfficies { get; set; }

        public bool UseCod { get; set; }

        public DeliveryOption DeliveryOption { get; set; }
        public List<KeyValuePair<long, string>> Officies { get; set; }
        public int? ServiceId { get; set; }
        public List<KeyValuePair<long, string>> Services { get; set; }
    }
}
