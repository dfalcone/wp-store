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
using System.Net;
using System.Windows;
using System.Diagnostics;
using System.Collections.Generic;
using System.Xml.Linq;
using SoomlaWpCore;
using SoomlaWpCore.util;
using SoomlaWpStore.events;


//using MockIAPLib;
//using MockStore = MockIAPLib;
//using MockCurApp = MockIAPLib.CurrentApp;

using Store = Windows.ApplicationModel.Store;
using CurApp = Windows.ApplicationModel.Store.CurrentApp;
using CurAppSimulator = Windows.ApplicationModel.Store.CurrentAppSimulator;



namespace SoomlaWpStore.billing.wp.store
{
    public class StoreManager
    {
        static private StoreManager instance;
        public static Store.LicenseInformation licInfos;
        public static Store.ListingInformation listingInfos;
        public static Store.LicenseInformation licInfosMock;
        public static Store.ListingInformation listingInfosMock;

        public static Dictionary<string,MarketProductInfos> marketProductInfos;

        public static string purchaseReceipt = null; // Receipt for current purchase if any
        public static Guid purchaseTransactionId = default(Guid); // Windows Store transaction id for current purchase if any

        private bool Initialized = false;

        public void Initialize()
        {
            if(Initialized==false)
            {
                if (StoreConfig.STORE_TEST_MODE)
                {
                    SoomlaUtils.LogDebug(TAG, "WARNING You are running in Store Test Mode! Don't forget to disable the test mode before you publish the app.");
                    SetupMockIAP();
                    licInfosMock = CurAppSimulator.LicenseInformation;

                }
                else
                {
                    licInfos = CurApp.LicenseInformation;
                }
                marketProductInfos = new Dictionary<string, MarketProductInfos>();
                //LoadListingInfo();
                Initialized = true;
            }
        }

        /// <summary>   Loads Windows Store IAP informations. </summary>
        public async void LoadListingInfo()
        {
            BusProvider.Instance.Post(new MarketItemsRefreshStartedEvent());
            try
            {

                
                if (StoreConfig.STORE_TEST_MODE)
                {
                    listingInfosMock = await CurAppSimulator.LoadListingInformationAsync();

                    marketProductInfos.Clear();
                    if (listingInfosMock.ProductListings.Count > 0)
                    {
                        foreach (KeyValuePair<string, Store.ProductListing> pair in listingInfosMock.ProductListings)
                        {
                            MarketProductInfos marketProduct = new MarketProductInfos();
                            marketProduct.Name = pair.Value.Name;
                            marketProduct.Description = null; // not implemented
                            marketProduct.FormattedPrice = pair.Value.FormattedPrice;
                            marketProduct.ImageUri = null; // not implemented
                            marketProduct.Keywords = null; // not implemented
                            marketProduct.ProductId = pair.Value.ProductId;
                            
                            switch (pair.Value.ProductType)
                            {
                                case Windows.ApplicationModel.Store.ProductType.Consumable:
                                    marketProduct.ProductType = MarketProductInfos.MarketProductType.CONSUMABLE;
                                    break;
                                case Windows.ApplicationModel.Store.ProductType.Durable:
                                    marketProduct.ProductType = MarketProductInfos.MarketProductType.DURABLE;
                                    break;
                                case Windows.ApplicationModel.Store.ProductType.Unknown:
                                    marketProduct.ProductType = MarketProductInfos.MarketProductType.UNKNOWN;
                                    break;
                            }
                            marketProduct.Tag = null; // not implemented
                            marketProductInfos.Add(pair.Key, marketProduct);
                        }
                    }
                }
                else
                {
                    listingInfos = await Store.CurrentApp.LoadListingInformationAsync();
                    IReadOnlyDictionary<string, Store.ProductListing> productListing;
                    productListing = listingInfos.ProductListings;

                    marketProductInfos.Clear();
                    if (productListing.Count > 0)
                    {
                        foreach (KeyValuePair<string, Store.ProductListing> pair in listingInfos.ProductListings)
                        {
                            MarketProductInfos marketProduct = new MarketProductInfos();
                            marketProduct.Name = pair.Value.Name;
                            marketProduct.Description = null; // not implemented
                            marketProduct.FormattedPrice = pair.Value.FormattedPrice;
                            marketProduct.ImageUri = null; // not implemented
                            marketProduct.Keywords = null; // not implemented
                            marketProduct.ProductId = pair.Value.ProductId;

                            switch (pair.Value.ProductType)
                            {
                                case Windows.ApplicationModel.Store.ProductType.Consumable:
                                    marketProduct.ProductType = MarketProductInfos.MarketProductType.CONSUMABLE;
                                    break;
                                case Windows.ApplicationModel.Store.ProductType.Durable:
                                    marketProduct.ProductType = MarketProductInfos.MarketProductType.DURABLE;
                                    break;
                                case Windows.ApplicationModel.Store.ProductType.Unknown:
                                    marketProduct.ProductType = MarketProductInfos.MarketProductType.UNKNOWN;
                                    break;
                            }
                            marketProduct.Tag = null; // not implemented
                            marketProductInfos.Add(pair.Key, marketProduct);
                        }
                    }
                }

            }
            catch (Exception e)
            {
                SoomlaUtils.LogDebug(TAG,e.Message);
            }

            OnListingLoadedCB(marketProductInfos);
        }
        /// <summary>   Setup the Windows Store test mode by loading the IAPMock.xml file at the root path. </summary>
        private void SetupMockIAP()
        {
            object task = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {
                Windows.Storage.StorageFolder proxyDataFolder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("Data");
                Windows.Storage.StorageFile proxyFile = await proxyDataFolder.GetFileAsync("IAPMock.xml");
                await CurAppSimulator.ReloadSimulatorAsync(proxyFile);
                SoomlaUtils.LogDebug(TAG, "WStorePlugin Mock XML " + proxyFile.ToString());
            });
      
            // WP8 implementation
            //MockIAP.Init();
            //MockIAP.RunInMockMode(true);
            //MockIAP.SetListingInformation(1, "en-us", "A description", "0", "TestApp");
            //var xDocument = XDocument.Load("IAPMock.xml");
            //SoomlaUtils.LogDebug(TAG,"WStorePlugin Mock XML "+xDocument.ToString());
            //MockIAP.PopulateIAPItemsFromXml(xDocument.ToString());
        }

        static public StoreManager GetInstance()
        {
            if (instance == null)
            {
                instance = new StoreManager();
            }
            return instance;
        }
        /// <summary>   Launch the purchase flow for the specified productid. </summary>
        ///
        /// <param name="productId">    Identifier for the product. </param>
        public void PurchaseProduct(string productId)
        {
            object task = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                DoPurchase(productId);
            });
        }

        private async void DoPurchase(string productId)
        {
            try
            {
                bool licenceActiv = false;
                if (StoreConfig.STORE_TEST_MODE)
                {
                    SoomlaUtils.LogDebug(TAG, "DoPurchase test mode " + productId);

                    var results = await CurAppSimulator.RequestProductPurchaseAsync(productId);
                    purchaseReceipt = results.ReceiptXml;
                    purchaseTransactionId = results.TransactionId;

                    licInfosMock = CurAppSimulator.LicenseInformation;
                    licenceActiv = licInfosMock.ProductLicenses[productId].IsActive;
                    
                }
                else
                {
                    SoomlaUtils.LogDebug(TAG, "DoPurchase real store" + productId);
                    
                    var results = await CurApp.RequestProductPurchaseAsync(productId);
                    licInfosMock = CurAppSimulator.LicenseInformation;
                    licenceActiv = licInfosMock.ProductLicenses[productId].IsActive;

                    licInfos = CurApp.LicenseInformation;
                    licenceActiv = licInfos.ProductLicenses[productId].IsActive;
                }

                if (licenceActiv)
                {
                    
                    OnItemPurchasedCB(productId);
                }
                else
                {
                    SoomlaUtils.LogDebug(TAG,"Purchase cancelled " + productId);
                    OnItemPurchaseCancelCB(productId, false);
                }


            }
            catch (Exception ex)
            {
                // When the user does not complete the purchase (e.g. cancels or navigates back from the Purchase Page), an exception with an HRESULT of E_FAIL is expected.
                SoomlaUtils.LogDebug(TAG,ex.Message);
                OnItemPurchaseCancelCB(productId, true);
            }
        }
        /// <summary>   Consumes a MANAGED product. </summary>
        ///
        /// <param name="productId">    Identifier for the product. </param>
        public void Consume(string productId, Guid transactionId)
        {
            SoomlaUtils.LogDebug(TAG, "WStorePlugin consume " + productId);
            try
            {
                if (StoreConfig.STORE_TEST_MODE)
                {
                    var task = CurAppSimulator.ReportConsumableFulfillmentAsync(productId, transactionId);
                }
                else
                {
                    var task = CurApp.ReportConsumableFulfillmentAsync(productId, transactionId);
                }
            }
            catch (InvalidOperationException e)
            {
                SoomlaUtils.LogDebug(TAG, e.Message);
            }
        }
        

        public bool IsPurchased(string productId)
        {
            bool isPurchased = false;
            SoomlaUtils.LogDebug(TAG,"Licence " + productId + " " + licInfos.ProductLicenses[productId].IsActive.ToString());

            bool containKey = false;
            if (StoreConfig.STORE_TEST_MODE)
            {
                containKey = licInfosMock.ProductLicenses.ContainsKey(productId);
            }
            else
            {
                containKey = licInfos.ProductLicenses.ContainsKey(productId);
            }

            if (containKey)
            {
                if (StoreConfig.STORE_TEST_MODE)
                {
                    isPurchased = licInfosMock.ProductLicenses[productId].IsActive;
                }
                else
                {
                    isPurchased = licInfos.ProductLicenses[productId].IsActive;
                }
                
                SoomlaUtils.LogDebug(TAG,productId + " has licence");
                if (isPurchased)
                {
                    SoomlaUtils.LogDebug(TAG,productId + " has active licence");
                }
            }
            else
            {
                SoomlaUtils.LogDebug(TAG,productId + " no license");
            }

            return isPurchased;
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
