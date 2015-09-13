/// Copyright (C) 2012-2014 Soomla Inc.
///
/// Licensed under the Apache License, Version 2.0 (the "License");
/// you may not use this file except in compliance with the License.
/// You may obtain a copy of the License at
///
///      http://www.apache.org/licenses/LICENSE-2.0
///
/// Unless required by applicable law or agreed to in writing, software
/// distributed under the License is distributed on an "AS IS" BASIS,
/// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
/// See the License for the specific language governing permissions and
/// limitations under the License.
using System;
using System.Collections.Generic;


namespace SoomlaWpStore.billing.wp.store
{
    public class StoreManager
    {
        public static Dictionary<string,MarketProductInfos> marketProductInfos;
        private bool Initialized = false;

        public void Initialize()
        {
        }

        public void LoadListingInfo() { }

        static public StoreManager GetInstance()
        {
            return null;
        }

        /// <summary>   Launch the purchase flow for the specified productid. </summary>
        ///
        /// <param name="productId">    Identifier for the product. </param>
        public void PurchaseProduct(string productId)
        {
        }

        /// <summary>   Consumes a MANAGED product. </summary>
        ///
        /// <param name="productId">    Identifier for the product. </param>
        public void Consume(string productId)
        {
        }
        

        public bool IsPurchased(string productId)
        {
            return false;
        }

        public delegate void OnItemPurchasedEventHandler(string _item);
        public static event OnItemPurchasedEventHandler OnItemPurchasedCB;

        public delegate void OnItemPurchaseCancelEventHandler(string _item, bool _error);
        public static event OnItemPurchaseCancelEventHandler OnItemPurchaseCancelCB;

        public delegate void OnListingLoadedEventHandler(Dictionary<string,MarketProductInfos> marketInfos);
        public static event OnListingLoadedEventHandler OnListingLoadedCB;

        private const String TAG = "SOOMLA StoreManager"; //used for Log messages
    }

}
