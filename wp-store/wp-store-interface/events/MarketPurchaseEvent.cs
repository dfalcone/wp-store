using SoomlaWpStore.domain;
using System.Collections.Generic;
using SoomlaWpCore.events;

namespace SoomlaWpStore.events
{
    public class MarketPurchaseEvent : SoomlaEvent
    {
        private PurchasableVirtualItem mPurchasableVirtualItem = null;
        private string mPayload = null;
        private Dictionary<string, string> mExtra = null;

        public PurchasableVirtualItem GetPurchasableVirtualItem()
        {
            return mPurchasableVirtualItem;
        }

        public string GetPayload()
        {
            return mPayload;
        }

        public Dictionary<string, string> GetExtra()
        {
            return mExtra;
        }

        public MarketPurchaseEvent(PurchasableVirtualItem item, string payload, Dictionary<string, string> extra)
            : this(item, payload, extra, null)
        {
        }

        public MarketPurchaseEvent(PurchasableVirtualItem item, string payload, Dictionary<string, string> extra, object sender)
            : base(sender)
        {
            mPurchasableVirtualItem = item;
            mPayload = payload;
            mExtra = extra;
        }
    }
}
