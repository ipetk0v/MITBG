using System;
using System.Collections.Generic;
using System.Text;
using Mitbg.Plugin.Misc.VendorsCore;


namespace Mitbg.Plugin.Misc.VendorsCore
{
    public class ShipmentContext : IShipmentContext
    {
        private Dictionary<int, ShipmentInfo> _billInfoes;
        private Dictionary<int, List<KeyValuePair<long, string>>> _officiesDictionary;

        private Dictionary<int, ShipmentInfo> BillInfoes
        {
            get
            {
                if (_billInfoes == null)
                    _billInfoes = new Dictionary<int, ShipmentInfo>();

                return _billInfoes;
            }
        }
        private Dictionary<int, List<KeyValuePair<long, string>>> OfficiesDictionary
        {
            get
            {
                if (_officiesDictionary == null)
                    _officiesDictionary = new Dictionary<int, List<KeyValuePair<long, string>>>();

                return _officiesDictionary;
            }
        }




        public void SetBillInfo(int customerId, ShipmentInfo bolInfo)
        {
            BillInfoes[customerId] = bolInfo;
        }

        public ShipmentInfo GetBillInfo(int customerId)
        {
            return BillInfoes.TryGetValue(customerId, out var bol) ? bol : null;
        }

        public void SetListOfficies(int siteId, List<KeyValuePair<long, string>> officies)
        {
            OfficiesDictionary[siteId] = officies;
        }

        public List<KeyValuePair<long, string>> GetListOfficies(int siteId)
        {
            return OfficiesDictionary.TryGetValue(siteId, out var result) ? result : null;
        }
    }

    public interface IShipmentContext
    {
        void SetBillInfo(int customerId, ShipmentInfo bolInfo);
        ShipmentInfo GetBillInfo(int customerId);
        void SetListOfficies(int siteId, List<KeyValuePair<long, string>> officies);
        List<KeyValuePair<long, string>> GetListOfficies(int siteId);

        //void SetListServices(int siteId, int officeId, List<KeyValuePair<long, string>> services);
        //List<KeyValuePair<long, string>> GetListServices(int siteId, office);
    }
}
