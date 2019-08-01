using System;
using System.Collections.Generic;
using System.Text;
using Nop.Plugin.Shipping.Speedy.Services;
using Nop.Services.Shipping.Tracking;

namespace Nop.Plugin.Shipping.Speedy
{
    public class SpeedyTracker : IShipmentTracker
    {
        private readonly ISpeedyShipmentsService _srv;
        public SpeedyTracker(ISpeedyShipmentsService srv)
        {
            _srv = srv;
        }

        public bool IsMatch(string trackingNumber)
        {
            long bolNum;
            return long.TryParse(trackingNumber, out bolNum);
        }

        public string GetUrl(string trackingNumber)
        {
            return $"https://www.speedy.bg/bg/track-shipment?shipmentNumber={trackingNumber}";
        }

        public IList<ShipmentStatusEvent> GetShipmentEvents(string trackingNumber)
        {
            return _srv.GetTrackingEvents(trackingNumber);
        }
    }
}
