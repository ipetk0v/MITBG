using System;
using System.Collections.Generic;
using System.Text;
using Nop.Plugin.Shipping.Speedy.Models;

namespace Nop.Plugin.Shipping.Speedy
{
    public class SpeedyContext : ISpeedyContext
    {
        private Dictionary<int, SpeedyBolInfo> _billInfoes;
        private Dictionary<int, List<KeyValuePair<long, string>>> _officiesDictionary;

        private Dictionary<int, SpeedyBolInfo> BillInfoes
        {
            get
            {
                if (_billInfoes == null)
                    _billInfoes = new Dictionary<int, SpeedyBolInfo>();

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




        public void SetBillInfo(int customerId, SpeedyBolInfo bolInfo)
        {
            BillInfoes[customerId] = bolInfo;
        }

        public SpeedyBolInfo GetBillInfo(int customerId)
        {
            SpeedyBolInfo bol;
            return BillInfoes.TryGetValue(customerId, out bol) ? bol : null;
        }

        public void SetListOfficies(int siteId, List<KeyValuePair<long, string>> officies)
        {
            OfficiesDictionary[siteId] = officies;
        }

        public List<KeyValuePair<long, string>> GetListOfficies(int siteId)
        {
            List<KeyValuePair<long, string>> result;
            return OfficiesDictionary.TryGetValue(siteId, out result) ? result : null;
        }
    }

    public interface ISpeedyContext
    {
        void SetBillInfo(int customerId, SpeedyBolInfo bolInfo);
        SpeedyBolInfo GetBillInfo(int customerId);
        void SetListOfficies(int siteId, List<KeyValuePair<long, string>> officies);
        List<KeyValuePair<long, string>> GetListOfficies(int siteId);

        //void SetListServices(int siteId, int officeId, List<KeyValuePair<long, string>> services);
        //List<KeyValuePair<long, string>> GetListServices(int siteId, office);
    }
}
