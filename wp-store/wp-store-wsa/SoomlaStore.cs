﻿/// Copyright (C) 2012-2014 Soomla Inc.
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
using System.Globalization;
using SoomlaWpCore;
using SoomlaWpStore.domain;
using SoomlaWpStore.domain.virtualGoods;
using SoomlaWpStore.data;
using SoomlaWpStore.purchasesTypes;
using SoomlaWpStore.billing.wp.store;
using SoomlaWpStore.exceptions;
using SoomlaWpCore.util;
using SoomlaWpStore.events;
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
            if (mInitialized)
            {
                String err = "SoomlaStore is already initialized. You can't initialize it twice!";
                handleErrorResult(err);
                return false;
            }

            StoreConfig.STORE_TEST_MODE = testMode;

            initStoreManager();

            SoomlaUtils.LogDebug(TAG, "SoomlaStore Initializing ...");

            StoreInfo.setStoreAssets(storeAssets);

            // Update SOOMLA store from DB
            StoreInfo.initializeFromDB();

            refreshInventory();

            mInitialized = true;
            BusProvider.Instance.Post(new SoomlaStoreInitializedEvent());

            return true;
        }

        public void initStoreManager()
        {
            StoreManager.OnItemPurchasedCB += handleSuccessfulPurchase;
            StoreManager.OnItemPurchaseCancelCB += handleCancelledPurchase;
            StoreManager.OnListingLoadedCB += refreshMarketItemsDetails;
            StoreManager.GetInstance().Initialize();
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

            BusProvider.Instance.Post(new RestoreTransactionsStartedEvent());

            if (StoreConfig.STORE_TEST_MODE)
            {
                foreach (var item in StoreManager.licInfosMock.ProductLicenses)
                {
                    if (item.Value.IsActive)
                    {
                        SoomlaUtils.LogDebug(TAG, "Got owned item: " + item.Value.ProductId);
                        handleSuccessfulPurchase(new StoreTransaction() { ProductId = item.Value.ProductId });
                    }
                }
            }
            else
            {
                foreach (var item in StoreManager.licInfos.ProductLicenses)
                {
                    if (item.Value.IsActive)
                    {
                        SoomlaUtils.LogDebug(TAG, "Got owned item: " + item.Value.ProductId);
                        handleSuccessfulPurchase(new StoreTransaction() { ProductId = item.Value.ProductId });
                    }
                }
            }

            BusProvider.Instance.Post(new RestoreTransactionsFinishedEvent(true));

            if (followedByRefreshItemsDetails)
            {
                StoreManager.GetInstance().LoadListingInfo();
            }
            /*
            mInAppBillingService.initializeBillingService(
                    new IabCallbacks.IabInitListener() {

                        @Override
                        public void success(boolean alreadyInBg) {
                            if (!alreadyInBg) {
                                notifyIabServiceStarted();
                            }

                            SoomlaUtils.LogDebug(TAG,
                                    "Setup successful, restoring purchases");

                            IabCallbacks.OnRestorePurchasesListener restorePurchasesListener = new IabCallbacks.OnRestorePurchasesListener() {
                                @Override
                                public void success(List<IabPurchase> purchases) {
                                    SoomlaUtils.LogDebug(TAG, "Transactions restored");

                                    if (purchases.size() > 0) {
                                        for (IabPurchase iabPurchase : purchases) {
                                            SoomlaUtils.LogDebug(TAG, "Got owned item: " + iabPurchase.getSku());

                                            handleSuccessfulPurchase(iabPurchase);
                                        }
                                    }

                                    BusProvider.getInstance().post(
                                            new OnRestoreTransactionsFinishedEvent(true));

                                    if (followedByRefreshItemsDetails) {
                                        refreshMarketItemsDetails();
                                    }
                                }

                                @Override
                                public void fail(String message) {
                                    BusProvider.getInstance().post(new OnRestoreTransactionsFinishedEvent(false));
                                    handleErrorResult(message);
                                }
                            };

                            mInAppBillingService.restorePurchasesAsync(restorePurchasesListener);

                            BusProvider.getInstance().post(new OnRestoreTransactionsStartedEvent());
                        }

                        @Override
                        public void fail(String message) {
                            reportIabInitFailure(message);
                        }
                    }
            );
            */
        }

        /**
         * Queries the store for the details for all of the game's market items by product ids.
         * This operation will "fill" up the MarketItem objects with the information you provided in
         * the developer console including: localized price (as string), title and description.
         */
        public void refreshMarketItemsDetails(Dictionary<string, MarketProductInfos> marketInfos)
    {
        System.Globalization.NumberStyles styles = System.Globalization.NumberStyles.Currency | System.Globalization.NumberStyles.AllowCurrencySymbol | System.Globalization.NumberStyles.AllowDecimalPoint;
        CultureInfo culture = System.Globalization.CultureInfo.CurrentUICulture;
        List<MarketItem> marketItems = new List<MarketItem>();
        foreach (var mpi in marketInfos)
        {

            String productId = mpi.Value.ProductId;
            String title = mpi.Value.Name;
            String price = mpi.Value.FormattedPrice;
            String desc = mpi.Value.Description;
            decimal rawPrice;
            

            try
            {
                PurchasableVirtualItem pvi = StoreInfo.
                        getPurchasableItem(productId);
                MarketItem mi = ((PurchaseWithMarket)
                        pvi.GetPurchaseType()).getMarketItem();
                mi.setMarketTitle(title);
                mi.setMarketPriceAndCurrency(price);
                mi.setMarketDescription(desc);

                //Trying to parse the price to extract the raw value
                if (Decimal.TryParse(mpi.Value.FormattedPrice, styles, culture, out rawPrice))
                {
                    SoomlaUtils.LogDebug(TAG, "RAW Price Successfully parsed : " + rawPrice.ToString());
                    mi.setMarketPriceMicros((long)(rawPrice*1000000));
                    mi.setMarketCurrencyCode(RegionInfo.CurrentRegion.ISOCurrencySymbol);
                    mi.setIsPriceSuccessfullyParsed(true);
                }
                else
                {
                    SoomlaUtils.LogDebug(TAG, "Fail to parse : " + mpi.Value.FormattedPrice);
                     mi.setIsPriceSuccessfullyParsed(false);
                }


                marketItems.Add(mi);
            }
            catch (VirtualItemNotFoundException e)
            {
                String msg = "(refreshInventory) Couldn't find a "
                        + "purchasable item associated with: " + productId + " " + e.Message;
                SoomlaUtils.LogError(TAG, msg);
            }
        }
        BusProvider.Instance.Post(new MarketItemsRefreshFinishedEvent(marketItems));
        
        /*
        mInAppBillingService.initializeBillingService(
                new IabCallbacks.IabInitListener() {

                    @Override
                    public void success(boolean alreadyInBg) {
                        if (!alreadyInBg) {
                            notifyIabServiceStarted();
                        }
                        SoomlaUtils.LogDebug(TAG,
                                "Setup successful, refreshing market items details");

                        IabCallbacks.OnFetchSkusDetailsListener fetchSkusDetailsListener =
                                new IabCallbacks.OnFetchSkusDetailsListener() {

                                    @Override
                                    public void success(List<IabSkuDetails> skuDetails) {
                                        SoomlaUtils.LogDebug(TAG, "Market items details refreshed");

                                        List<MarketItem> marketItems = new ArrayList<MarketItem>();
                                        if (skuDetails.size() > 0) {
                                            for (IabSkuDetails iabSkuDetails : skuDetails) {
                                                String productId = iabSkuDetails.getSku();
                                                String price = iabSkuDetails.getPrice();
                                                String title = iabSkuDetails.getTitle();
                                                String desc = iabSkuDetails.getDescription();

                                                SoomlaUtils.LogDebug(TAG, "Got item details: " +
                                                        "\ntitle:\t" + iabSkuDetails.getTitle() +
                                                        "\nprice:\t" + iabSkuDetails.getPrice() +
                                                        "\nproductId:\t" + iabSkuDetails.getSku() +
                                                        "\ndesc:\t" + iabSkuDetails.getDescription());

                                                try {
                                                    PurchasableVirtualItem pvi = StoreInfo.
                                                            getPurchasableItem(productId);
                                                    MarketItem mi = ((PurchaseWithMarket)
                                                            pvi.getPurchaseType()).getMarketItem();
                                                    mi.setMarketTitle(title);
                                                    mi.setMarketPrice(price);
                                                    mi.setMarketDescription(desc);

                                                    marketItems.add(mi);
                                                } catch (VirtualItemNotFoundException e) {
                                                    String msg = "(refreshInventory) Couldn't find a "
                                                            + "purchasable item associated with: " + productId;
                                                    SoomlaUtils.LogError(TAG, msg);
                                                }
                                            }
                                        }
                                        BusProvider.getInstance().post(new OnMarketItemsRefreshFinishedEvent(marketItems));
                                    }

                                    @Override
                                    public void fail(String message) {

                                    }
                                };

                        final List<String> purchasableProductIds = StoreInfo.getAllProductIds();
                        mInAppBillingService.fetchSkusDetailsAsync(purchasableProductIds, fetchSkusDetailsListener);

                        BusProvider.getInstance().post(new OnMarketItemsRefreshStartedEvent());
                    }

                    @Override
                    public void fail(String message) {
                        reportIabInitFailure(message);
                    }
                }
        );
        */
    }

        /**
         * This runs restoreTransactions followed by market items refresh.
         * There are docs that explains restoreTransactions and refreshMarketItemsDetails on the actual
         * functions in this file.
         */
        public void refreshInventory()
        {
            restoreTransactions(true);
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
            SoomlaUtils.LogDebug(TAG, "TODO buyWithMarket");

            PurchasableVirtualItem pvi;
            try
            {
                pvi = StoreInfo.getPurchasableItem(marketItem.getProductId());
            }
            catch (VirtualItemNotFoundException e)
            {
                String msg = "Couldn't find a purchasable item associated with: " + marketItem.getProductId() + " " + e.Message;
                SoomlaUtils.LogError(TAG, msg);
                BusProvider.Instance.Post(new UnexpectedStoreErrorEvent(msg));
                return;
            }

            BusProvider.Instance.Post(new MarketPurchaseStartedEvent(pvi));
            StoreManager.GetInstance().PurchaseProduct(marketItem.getProductId());
            /*
            mInAppBillingService.initializeBillingService
                    (new IabCallbacks.IabInitListener() {

                        @Override
                        public void success(boolean alreadyInBg) {
                            if (!alreadyInBg) {
                                notifyIabServiceStarted();
                            }

                            IabCallbacks.OnPurchaseListener purchaseListener =
                                    new IabCallbacks.OnPurchaseListener() {

                                        @Override
                                        public void success(IabPurchase purchase) {
                                            handleSuccessfulPurchase(purchase);
                                        }

                                        @Override
                                        public void cancelled(IabPurchase purchase) {
                                            handleCancelledPurchase(purchase);
                                        }

                                        @Override
                                        public void alreadyOwned(IabPurchase purchase) {
                                            String sku = purchase.getSku();
                                            SoomlaUtils.LogDebug(TAG, "Tried to buy an item that was not" +
                                                    " consumed (maybe it's an already owned " +
                                                    "NonConsumable). productId: " + sku);

                                            try {
                                                PurchasableVirtualItem pvi = StoreInfo.getPurchasableItem(sku);
                                                consumeIfConsumable(purchase, pvi);

                                                if (pvi instanceof NonConsumableItem) {
                                                    String message = "(alreadyOwned) the user tried to " +
                                                            "buy a NonConsumableItem that was already " +
                                                            "owned. itemId: " + pvi.getItemId() +
                                                            "    productId: " + sku;
                                                    SoomlaUtils.LogDebug(TAG, message);
                                                    BusProvider.getInstance().post(new OnUnexpectedStoreErrorEvent(message));
                                                }
                                            } catch (VirtualItemNotFoundException e) {
                                                String message = "(alreadyOwned) ERROR : Couldn't find the "
                                                        + "VirtualCurrencyPack with productId: " + sku
                                                        + ". It's unexpected so an unexpected error is being emitted.";
                                                SoomlaUtils.LogError(TAG, message);
                                                BusProvider.getInstance().post(new OnUnexpectedStoreErrorEvent(message));
                                            }
                                        }

                                        @Override
                                        public void fail(String message) {
                                            handleErrorResult(message);
                                        }
                                    };
                            mInAppBillingService.launchPurchaseFlow(marketItem.getProductId(),
                                    purchaseListener, payload);
                            BusProvider.getInstance().post(new OnMarketPurchaseStartedEvent(pvi));
                        }

                        @Override
                        public void fail(String message) {
                            reportIabInitFailure(message);
                        }

                    });
             */

        }

        /**
         * Determines if Store Controller is initialized
         *
         * @return true if initialized, false otherwise
         */
        public bool isInitialized()
        {
            return mInitialized;
        }


        /*==================== Common callbacks for success \ failure \ finish ====================*/

        /**
         * Checks the state of the purchase and responds accordingly, giving the user an item,
         * throwing an error, or taking the item away and paying the user back.
         *
         * @param purchase purchase whose state is to be checked.
         */
        private void handleSuccessfulPurchase(/*IabPurchase*/ StoreTransaction transaction)
        {
            SoomlaUtils.LogDebug(TAG, "TODO handleSuccessfulPurchase");
            
            try
            {
                PurchasableVirtualItem pvi = StoreInfo.getPurchasableItem(transaction.ProductId);
                if (pvi == null)
                    return;

                SoomlaUtils.LogDebug(TAG, "IabPurchase successful.");

                consumeIfConsumable(pvi, transaction);

                Dictionary<string, string> extra = new Dictionary<string, string>();
                extra.Add("receipt", transaction.ReceiptXml);

                // Post event of general market purchase success
                BusProvider.Instance.Post(new MarketPurchaseEvent(pvi, null, extra));
                pvi.give(1);

                // Post event of item purchase success with receipt as payload
                BusProvider.Instance.Post(new ItemPurchasedEvent(pvi, null));
            }
            catch (VirtualItemNotFoundException e)
            {
                SoomlaUtils.LogError(TAG, "(handleSuccessfulPurchase - purchase or query-inventory) "
                        + "ERROR : Couldn't find the " +
                        " VirtualCurrencyPack OR MarketItem  with productId: " + transaction.ReceiptXml +
                        ". It's unexpected so an unexpected error is being emitted." + " " + e.Message);
                BusProvider.Instance.Post(new UnexpectedStoreErrorEvent("Couldn't find the productId "
                        + "of a product after purchase or query-inventory." + " " + e.Message));
                return;
            }
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
            SoomlaUtils.LogDebug(TAG, "TODO handleCancelledPurchase");

            try
            {
                PurchasableVirtualItem v = StoreInfo.getPurchasableItem(productId);
                BusProvider.Instance.Post(new MarketPurchaseCancelledEvent(v));
            }
            catch (VirtualItemNotFoundException e)
            {
                SoomlaUtils.LogError(TAG, "(purchaseActionResultCancelled) ERROR : Couldn't find the "
                        + "VirtualCurrencyPack OR MarketItem  with productId: " + productId
                        + ". It's unexpected so an unexpected error is being emitted.");
                BusProvider.Instance.Post(new UnexpectedStoreErrorEvent(e.Message));
            }

        }

        /**
         * Consumes the given purchase, or writes error message to log if unable to consume
         *
         * @param purchase purchase to be consumed
         */
        private void consumeIfConsumable(PurchasableVirtualItem pvi, StoreTransaction transaction)
        {
            try
            {
                if (!(pvi is SingleUseVG))
                {
                    if (pvi.GetPurchaseType() is PurchaseWithMarket)
                    {
                        PurchaseWithMarket pwm = (PurchaseWithMarket)pvi.GetPurchaseType();
                        string productId = pwm.getMarketItem().getProductId();
                        if (transaction.ProductId == productId)
                            StoreManager.GetInstance().Consume(transaction);
                    }
                }
            }
            catch (Exception e)
            {
                SoomlaUtils.LogDebug(TAG, "Error while consuming: itemId: " + pvi.getItemId());
                BusProvider.Instance.Post(new UnexpectedStoreErrorEvent(e.Message));
            }

        }

        /**
         * Posts an unexpected error event saying the purchase failed.
         *
         * @param message error message.
         */
        private void handleErrorResult(String message)
        {
            //BusProvider.getInstance().post(new OnUnexpectedStoreErrorEvent(message));
            BusProvider.Instance.Post(new UnexpectedStoreErrorEvent(message));
            SoomlaUtils.LogError(TAG, "ERROR: IabPurchase failed: " + message);
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
