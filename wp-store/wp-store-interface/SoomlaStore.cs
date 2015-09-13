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
using SoomlaWpStore.domain;
using SoomlaWpStore.billing.wp.store;

namespace SoomlaWpStore
{
    public class SoomlaStore
    {

        /// <summary>
        /// Initializes the SOOMLA SDK.
        /// This initializer also initializes StoreInfo.
        /// </summary>
        ///
        /// <param name="storeAssets">  The store assets. </param>
        /// <param name="testMode">     Run in testmode for store IAP </param>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        public bool initialize(IStoreAssets storeAssets, bool testMode)
        {
            return true;
        }

        public void initStoreManager()
        {

        }

        /// <summary>
        /// Restoring old purchases for the current user (device).
        /// Here we just call the private function without refreshing market items details.
        /// </summary>
        public void restoreTransactions()
        {
            restoreTransactions(false);
        }

        /// <summary>   Restoring old purchases for the current user (device). </summary>
        ///
        /// <param name="followedByRefreshItemsDetails">    determines weather we should perform a
        ///                                                 refresh market items operation right after a
        ///                                                 restore purchase success. </param>
        private void restoreTransactions(bool followedByRefreshItemsDetails)
        {

        }

        /**
         * Queries the store for the details for all of the game's market items by product ids.
         * This operation will "fill" up the MarketItem objects with the information you provided in
         * the developer console including: localized price (as string), title and description.
         */
        public void refreshMarketItemsDetails(Dictionary<string, MarketProductInfos> marketInfos)
        {
            
        }

        /**
         * This runs restoreTransactions followed by market items refresh.
         * There are docs that explains restoreTransactions and refreshMarketItemsDetails on the actual
         * functions in this file.
         */
        public void refreshInventory()
        {

        }

        /**
         * Starts a purchase process in the market.
         *
         * @param marketItem The item to purchase - this item has to be defined EXACTLY the same in
         *                   the market
         * @param payload A payload to get back when this purchase is finished.
         * @throws IllegalStateException
         */
        public void buyWithMarket(MarketItem marketItem, String payload)
        {
            
        }

        /**
         * Determines if Store Controller is initialized
         *
         * @return true if initialized, false otherwise
         */
        public bool isInitialized()
        {
            return false;
        }


        /*==================== Common callbacks for success \ failure \ finish ====================*/

        /**
         * Checks the state of the purchase and responds accordingly, giving the user an item,
         * throwing an error, or taking the item away and paying the user back.
         *
         * @param purchase purchase whose state is to be checked.
         */
        private void handleSuccessfulPurchase(/*IabPurchase*/ string productId)
        {

        }

        /**
         * Handles a cancelled purchase by either posting an event containing a
         * <code>PurchasableVirtualItem</code> corresponding to the given purchase, or an unexpected
         * error event if the item was not found.
         *
         * @param purchase cancelled purchase to handle.
         */
        private void handleCancelledPurchase(String productId, bool error)
        {

        }

        /**
         * Consumes the given purchase, or writes error message to log if unable to consume
         *
         * @param purchase purchase to be consumed
         */
        private void consumeIfConsumable(PurchasableVirtualItem pvi)
        {

        }

        /**
         * Posts an unexpected error event saying the purchase failed.
         *
         * @param message error message.
         */
        private void handleErrorResult(String message)
        {

        }

        /* Singleton */
        private static SoomlaStore sInstance = null;

        /**
         * Retrieves the singleton instance of <code>SoomlaStore</code>
         *
         * @return singleton instance of <code>SoomlaStore</code>
         */
        public static SoomlaStore GetInstance()
        {
            if (sInstance == null)
            {
                sInstance = new SoomlaStore();
            }
            return sInstance;
        }

        /**
         * Constructor
         */
        private SoomlaStore()
        {

        }

        /* Private Members */

        private const String TAG = "SOOMLA SoomlaStore"; //used for Log messages
        private bool mInitialized = false;

    }
}
