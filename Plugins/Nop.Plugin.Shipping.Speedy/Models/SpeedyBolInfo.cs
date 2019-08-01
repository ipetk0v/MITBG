using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Nop.Plugin.Shipping.Speedy.Models
{
    [Serializable]
    public class SpeedyBolInfo
    {

        public SpeedyBolInfo()
        {
            CountryId = 100;
            ServiceId = 505; //Preselected default "СТАНДАРТ 24 ЧАСА "
        }

        public int ServiceId { get; set; }
        //  public bool UseCod { get; set; }
        public long? OfficeId { get; set; }
        public long CountryId { get; set; }
        public long SiteId { get; set; }
        public string SiteName { get; set; }
        public string QuarterText { get; set; }
        public string QuarterName { get; set; }
        public string QuarterType { get; set; }
        public long QuarterId { get; set; }
        public string StreetName { get; set; }
        public string StreetNo { get; set; }
        public string Block { get; set; }
        public string Entrance { get; set; }
        public string Floor { get; set; }
        public string ApNumber { get; set; }
        public string AddressNote { get; set; }
        public string Comment { get; set; }

        [XmlIgnore]
        public DeliveryOption DeliveryOption { get; set; }
        [XmlIgnore]
        public List<SpeedyOffice> AllOfficies { get; set; }
        [XmlIgnore]
        public List<KeyValuePair<long, string>> Officies { get; set; }
        //public List<KeyValuePair<long, string>> Services { get; set; }
    }
}
