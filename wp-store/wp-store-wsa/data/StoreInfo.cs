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
using SoomlaWpCore;
using SoomlaWpCore.data;
using SoomlaWpCore.util;
using SoomlaWpStore;
using SoomlaWpStore.domain;
using SoomlaWpStore.domain.virtualGoods;
using SoomlaWpStore.domain.virtualCurrencies;
using SoomlaWpStore.exceptions;
using SoomlaWpStore.purchasesTypes;
using System.Collections.Generic;

namespace SoomlaWpStore.data
{

    /**
     * This class holds the store's metadata including:
     * virtual currencies,
     * virtual currency packs,
     * all kinds of virtual goods,
     * virtual categories, and
     * non-consumable items
     */
    public class StoreInfo
    {

        /**
         * Initializes <code>StoreInfo</code>.
         * On first initialization, when the database doesn't have any previous version of the store
         * metadata, <code>StoreInfo</code> gets loaded from the given <code>IStoreAssets</code>.
         * After the first initialization, <code>StoreInfo</code> will be initialized from the database.
         *
         * IMPORTANT: If you want to override the current <code>StoreInfo</code>, you'll have to bump
         * the version of your implementation of <code>IStoreAssets</code> in order to remove the
         * metadata when the application loads. Bumping the version is done by returning a higher number
         * in {@link com.soomla.store.IStoreAssets#getVersion()}.
         */
        public static void setStoreAssets(IStoreAssets storeAssets)
        {
            if (storeAssets == null)
            {
                SoomlaUtils.LogError(TAG, "The given store assets can't be null!");
                return;
            }

            mCurrentAssetsVersion = storeAssets.GetVersion();

            checkMetadataVersion();

            // we always initialize from the database, unless this is the first time the game is
            // loaded - in that case we initialize with setStoreAssets.
            if (!initializeFromDB())
            {
                initializeWithStoreAssets(storeAssets);
            }
        }


        public static void BuildStoreAssetsFromJson(String JsonStoreAsset)
        {

        }

        /**
         * Initializes <code>StoreInfo</code> from the database. This action should be performed only
         * once during the lifetime of a session of the game. <code>SoomlaStore</code> automatically
         * initializes <code>StoreInfo</code>. Don't do it if you don't know what you're doing.
         *
         * @return success
         */
        public static bool initializeFromDB()
        {
            checkMetadataVersion();

            String key = keyMetaStoreInfo();
            String val = KeyValueStorage.GetValue(key);

            if (val == null && String.IsNullOrEmpty(val))
            {
                SoomlaUtils.LogDebug(TAG, "store json is not in DB yet.");
                return false;
            }

            SoomlaUtils.LogDebug(TAG, "the metadata-economy json (from DB) is " + val);

            try
            {
                fromJSONObject(new JSONObject(val, -10));

                // everything went well... StoreInfo is initialized from the local DB.
                // it's ok to return now.

                return true;
            }
            catch (Exception e)
            {
                SoomlaUtils.LogDebug(TAG, "Can't parse metadata json. Going to return false and make "
                        + "StoreInfo load from static data: " + val + " " + e.Message);
            }

            return false;

        }

        /**
         * Checks if the <code>VirtualGood</code>d with the given <code>goodItemId</code> has upgrades.
         *
         * @param goodItemId the item id of the <code>VirtualGood</code> to check if has upgrades.
         * @return true if upgrades found for the <code>VirtualGood</code> with the given
         * <code>goodItemId</code>, otherwise false.
         */
        public static bool hasUpgrades(String goodItemId)
        {
            return mGoodsUpgrades.ContainsKey(goodItemId);
        }


        /** Setters and Getters */

        /**
         * Retrieves a single <code>VirtualItem</code> that resides in the metadata.
         *
         * @param itemId the itemId of the required <code>VirtualItem</code>
         * @return virtual item for the given <code>itemId</code>
         * @throws VirtualItemNotFoundException if no <code>VirtualItem</code> with the given
         * <code>itemId</code> was found.
         */
        public static VirtualItem getVirtualItem(String itemId)
        {
            VirtualItem item;
            try
            {
                item = mVirtualItems[itemId];
            }
            catch (KeyNotFoundException e)
            {
                SoomlaUtils.LogDebug(TAG, e.Message);
                throw new VirtualItemNotFoundException("itemId", itemId);
            }
            return item;
        }

        /**
         * Retrieves a single <code>PurchasableVirtualItem</code> that resides in the metadata.
         * IMPORTANT: The retrieved <code>PurchasableVirtualItem</code> has a <code>PurchaseType</code>
         * of <code>PurchaseWithMarket</code> (This is why we fetch here with <code>productId</code>).
         *
         * @param productId the product id of the purchasable item to be fetched
         * @return <code>PurchasableVirtualItem</code>
         * @throws VirtualItemNotFoundException if no PurchasableVirtualItem with the given
         *         productId was found.
         */
        public static PurchasableVirtualItem getPurchasableItem(String productId)
        {
            PurchasableVirtualItem item;
            try
            {
                item = mPurchasableItems[productId];
            }
            catch (KeyNotFoundException e)
            {
                SoomlaUtils.LogDebug(TAG, e.Message);
                throw new VirtualItemNotFoundException("productId", productId);
            }

            return item;
        }

        /**
         * Retrieves the <code>VirtualCategory</code> that the virtual good with the given
         * <code>goodItemId</code> belongs to.
         *
         * @param goodItemId the id of the virtual good whose category is to be fetched
         * @return the virtual category that the good with the given <code>goodItemId</code> belongs to
         * @throws VirtualItemNotFoundException if the given <code>goodItemId</code> is not found
         */
        public static VirtualCategory getCategory(String goodItemId)
        {
            VirtualCategory item;
            try
            {
                item = mGoodsCategories[goodItemId];
            }
            catch (KeyNotFoundException e)
            {
                SoomlaUtils.LogDebug(TAG, e.Message);
                throw new VirtualItemNotFoundException("goodItemId", goodItemId);
            }
            return item;
        }

        /**
         * Retrieves the first <code>UpgradeVG</code> for the given <code>goodItemId</code>.
         *
         * @param goodItemId The item id of the <code>VirtualGood</code> whose upgrade we are looking
         *                   for.
         * @return The first upgrade for the virtual good with the given <code>goodItemId</code>, or
         * null if it has no upgrades.
         */
        public static UpgradeVG getGoodFirstUpgrade(String goodItemId)
        {
            List<UpgradeVG> upgrades = mGoodsUpgrades[goodItemId];
            if (upgrades != null)
            {
                foreach (UpgradeVG upgradeVG in upgrades)
                {
                    if (String.IsNullOrEmpty(upgradeVG.getPrevItemId()))
                    {
                        return upgradeVG;
                    }
                }
            }
            return null;
        }

        /**
         * Retrieves the last <code>UpgradeVG</code> for the given <code>goodItemId</code>.
         *
         * @param goodItemId The item id of the <code>VirtualGood</code> whose upgrade we are looking
         *                   for.
         * @return The last upgrade for the virtual good with the given <code>goodItemId</code> or null
         *     if there are no upgrades.
         */
        public static UpgradeVG getGoodLastUpgrade(String goodItemId)
        {
            List<UpgradeVG> upgrades = mGoodsUpgrades[goodItemId];
            if (upgrades != null)
            {
                foreach (UpgradeVG upgradeVG in upgrades)
                {
                    if (String.IsNullOrEmpty(upgradeVG.getNextItemId()))
                    {
                        return upgradeVG;
                    }
                }
            }
            return null;
        }

        /**
         * Retrieves all <code>UpgradeVGs</code> for the virtual good with the given
         * <code>goodItemId</code>.
         *
         * @param goodItemId The item id of the <code>VirtualGood</code> whose upgrades we are looking
         *                   for.
         * @return list of all UpgradeVGs for the virtual good with the given <code>goodItemId</code>
         */
        public static List<UpgradeVG> getGoodUpgrades(String goodItemId)
        {
            return mGoodsUpgrades[goodItemId];
        }

        public static List<VirtualCurrency> getCurrencies()
        {
            return mCurrencies;
        }

        public static List<VirtualCurrencyPack> getCurrencyPacks()
        {
            return mCurrencyPacks;
        }

        public static List<VirtualGood> getGoods()
        {
            return mGoods;
        }
        /*
        public static List<NonConsumableItem> getNonConsumableItems() {
            return mNonConsumables;
        }*/

        public static List<VirtualCategory> getCategories()
        {
            return mCategories;
        }

        public static List<String> getAllProductIds()
        {
            return new List<String>(mPurchasableItems.Keys);
        }


        /** Private functions **/
        /**
         * Transforms given JSONObject to StoreInfo
         *
         * @param JSONObject
         * @throws JSONException
         */
        private static void fromJSONObject(JSONObject JSONObject)
        {

            mVirtualItems = new Dictionary<String, VirtualItem>();
            mPurchasableItems = new Dictionary<String, PurchasableVirtualItem>();
            mGoodsCategories = new Dictionary<String, VirtualCategory>();
            mGoodsUpgrades = new Dictionary<String, List<UpgradeVG>>();
            mCurrencyPacks = new List<VirtualCurrencyPack>();
            mGoods = new List<VirtualGood>();
            mCategories = new List<VirtualCategory>();
            mCurrencies = new List<VirtualCurrency>();
            //mNonConsumables = new List<NonConsumableItem>();
            if (JSONObject.HasField(StoreJSONConsts.STORE_CURRENCIES))
            {
                JSONObject virtualCurrencies = JSONObject[StoreJSONConsts.STORE_CURRENCIES];
                for (int i = 0; i < virtualCurrencies.Count; i++)
                {
                    JSONObject o = virtualCurrencies[i];
                    VirtualCurrency c = new VirtualCurrency(o);
                    mCurrencies.Add(c);

                    mVirtualItems.Add(c.getItemId(), c);
                }
            }

            if (JSONObject.HasField(StoreJSONConsts.STORE_CURRENCYPACKS))
            {
                JSONObject currencyPacks = JSONObject[StoreJSONConsts.STORE_CURRENCYPACKS];
                for (int i = 0; i < currencyPacks.Count; i++)
                {
                    JSONObject o = currencyPacks[i];
                    VirtualCurrencyPack pack = new VirtualCurrencyPack(o);
                    mCurrencyPacks.Add(pack);

                    mVirtualItems.Add(pack.getItemId(), pack);

                    PurchaseType purchaseType = pack.GetPurchaseType();
                    if (purchaseType is PurchaseWithMarket)
                    {
                        mPurchasableItems.Add(((PurchaseWithMarket)purchaseType)
                                .getMarketItem().getProductId(), pack);
                    }
                }
            }

            // The order in which VirtualGoods are created matters!
            // For example: VGU and VGP depend on other VGs
            if (JSONObject.HasField(StoreJSONConsts.STORE_GOODS))
            {
                JSONObject virtualGoods = JSONObject[StoreJSONConsts.STORE_GOODS];
                if (virtualGoods.HasField(StoreJSONConsts.STORE_GOODS_SU))
                {
                    JSONObject suGoods = virtualGoods[StoreJSONConsts.STORE_GOODS_SU];
                    for (int i = 0; i < suGoods.Count; i++)
                    {
                        JSONObject o = suGoods[i];
                        SingleUseVG g = new SingleUseVG(o);
                        addVG(g);
                    }
                }


                if (virtualGoods.HasField(StoreJSONConsts.STORE_GOODS_LT))
                {
                    JSONObject ltGoods = virtualGoods[StoreJSONConsts.STORE_GOODS_LT];
                    for (int i = 0; i < ltGoods.Count; i++)
                    {
                        JSONObject o = ltGoods[i];
                        LifetimeVG g = new LifetimeVG(o);
                        addVG(g);
                    }
                }


                if (virtualGoods.HasField(StoreJSONConsts.STORE_GOODS_EQ))
                {
                    JSONObject eqGoods = virtualGoods[StoreJSONConsts.STORE_GOODS_EQ];
                    for (int i = 0; i < eqGoods.Count; i++)
                    {
                        JSONObject o = eqGoods[i];
                        EquippableVG g = new EquippableVG(o);
                        addVG(g);
                    }
                }

                if (virtualGoods.HasField(StoreJSONConsts.STORE_GOODS_PA))
                {
                    JSONObject paGoods = virtualGoods[StoreJSONConsts.STORE_GOODS_PA];
                    for (int i = 0; i < paGoods.Count; i++)
                    {
                        JSONObject o = paGoods[i];
                        SingleUsePackVG g = new SingleUsePackVG(o);
                        addVG(g);
                    }
                }


                if (virtualGoods.HasField(StoreJSONConsts.STORE_GOODS_UP))
                {
                    JSONObject upGoods = virtualGoods[StoreJSONConsts.STORE_GOODS_UP];
                    for (int i = 0; i < upGoods.Count; i++)
                    {
                        JSONObject o = upGoods[i];
                        UpgradeVG g = new UpgradeVG(o);
                        addVG(g);

                        List<UpgradeVG> upgrades = mGoodsUpgrades[g.getGoodItemId()];
                        if (upgrades == null)
                        {
                            upgrades = new List<UpgradeVG>();
                            mGoodsUpgrades.Add(g.getGoodItemId(), upgrades);
                        }
                        upgrades.Add(g);
                    }
                }

            }

            // Categories depend on virtual goods. That's why the have to be initialized after!
            if (JSONObject.HasField(StoreJSONConsts.STORE_CATEGORIES))
            {
                JSONObject virtualCategories = JSONObject[StoreJSONConsts.STORE_CATEGORIES];
                for (int i = 0; i < virtualCategories.Count; i++)
                {
                    JSONObject o = virtualCategories[i];
                    VirtualCategory category = new VirtualCategory(o);
                    mCategories.Add(category);
                    foreach (String goodItemId in category.getGoodsItemIds())
                    {
                        mGoodsCategories.Add(goodItemId, category);
                    }
                }
            }
            /*
            if (JSONObject.TryGetValue(StoreJSONConsts.STORE_NONCONSUMABLES, out value)) {
                JSONObject nonConsumables = JSONObject.Value<JSONObject>(StoreJSONConsts.STORE_NONCONSUMABLES);
                for (int i=0; i<nonConsumables.Count; i++){
                    JSONObject o = nonConsumables.Value<JSONObject>(i);
                    NonConsumableItem non = new NonConsumableItem(o);
                    mNonConsumables.Add(non);

                    mVirtualItems.Add(non.getItemId(), non);

                    PurchaseType purchaseType = non.GetPurchaseType();
                    if (purchaseType is PurchaseWithMarket) {
                        mPurchasableItems.Add(((PurchaseWithMarket) purchaseType)
                                .getMarketItem().getProductId(), non);
                    }
                }
            }*/
        }

        /**
         * Adds the given virtual good to <code>StoreInfo</code>'s <code>mGoods</code>,
         * <code>mVirtualItems</code>, and if the good has purchase type <code>PurchaseWithMarket</code>
         * then it is also added to <code>mPurchasableItems</code>.
         *
         * @param g virtual good to be added
         */
        private static void addVG(VirtualGood g)
        {
            mGoods.Add(g);

            mVirtualItems.Add(g.getItemId(), g);

            PurchaseType purchaseType = g.GetPurchaseType();
            if (purchaseType is PurchaseWithMarket)
            {
                mPurchasableItems.Add(((PurchaseWithMarket)purchaseType)
                        .getMarketItem().getProductId(), g);
            }
        }

        /**
         * Converts <code>StoreInfo</code> to a <code>JSONObject</code>.
         *
         * @return a <code>JSONObject</code> representation of <code>StoreInfo</code>.
         */
        public static JSONObject toJSONObject()
        {

            JSONObject currencies = new JSONObject(JSONObject.Type.ARRAY);
            foreach (VirtualCurrency c in mCurrencies)
            {
                currencies.Add(c.toJSONObject());
            }

            JSONObject currencyPacks = new JSONObject(JSONObject.Type.ARRAY);
            foreach (VirtualCurrencyPack pack in mCurrencyPacks)
            {
                currencyPacks.Add(pack.toJSONObject());
            }

            JSONObject suGoods = new JSONObject(JSONObject.Type.ARRAY);
            JSONObject ltGoods = new JSONObject(JSONObject.Type.ARRAY);
            JSONObject eqGoods = new JSONObject(JSONObject.Type.ARRAY);
            JSONObject paGoods = new JSONObject(JSONObject.Type.ARRAY);
            JSONObject upGoods = new JSONObject(JSONObject.Type.ARRAY);
            foreach (VirtualGood good in mGoods)
            {
                if (good is SingleUseVG)
                {
                    suGoods.Add(good.toJSONObject());
                }
                else if (good is UpgradeVG)
                {
                    upGoods.Add(good.toJSONObject());
                }
                else if (good is EquippableVG)
                {
                    eqGoods.Add(good.toJSONObject());
                }
                else if (good is SingleUsePackVG)
                {
                    paGoods.Add(good.toJSONObject());
                }
                else if (good is LifetimeVG)
                {
                    ltGoods.Add(good.toJSONObject());
                }
            }


            JSONObject categories = new JSONObject(JSONObject.Type.ARRAY);
            foreach (VirtualCategory cat in mCategories)
            {
                categories.Add(cat.toJSONObject());
            }
            /*
            JSONObject nonConsumableItems = new JSONObject();
            foreach(NonConsumableItem non in mNonConsumables){
                nonConsumableItems.Add(non.toJSONObject());
            }*/

            JSONObject storeAssetsObj = new JSONObject(JSONObject.Type.OBJECT);
            JSONObject goods = new JSONObject(JSONObject.Type.OBJECT);
            try
            {
                goods.AddField(StoreJSONConsts.STORE_GOODS_SU, suGoods);
                goods.AddField(StoreJSONConsts.STORE_GOODS_LT, ltGoods);
                goods.AddField(StoreJSONConsts.STORE_GOODS_EQ, eqGoods);
                goods.AddField(StoreJSONConsts.STORE_GOODS_PA, paGoods);
                goods.AddField(StoreJSONConsts.STORE_GOODS_UP, upGoods);

                storeAssetsObj.AddField(StoreJSONConsts.STORE_CATEGORIES, categories);
                storeAssetsObj.AddField(StoreJSONConsts.STORE_CURRENCIES, currencies);
                storeAssetsObj.AddField(StoreJSONConsts.STORE_GOODS, goods);
                storeAssetsObj.AddField(StoreJSONConsts.STORE_CURRENCYPACKS, currencyPacks);
                //JSONObject.AddField(StoreJSONConsts.STORE_NONCONSUMABLES, nonConsumableItems);
            }
            catch (Exception e)
            {
                SoomlaUtils.LogError(TAG, "An error occurred while generating JSON object." + " " + e.Message);
            }

            return storeAssetsObj;
        }

        /**
         * Saves the store's metadata in the database as JSON.
         */
        public static void save()
        {
            String store_json = toJSONObject().ToString();
            SoomlaUtils.LogDebug(TAG, "saving StoreInfo to DB. json is: " + store_json);
            String key = keyMetaStoreInfo();
            KeyValueStorage.SetValue(key, store_json);
        }

        /**
         * Replaces the given virtual item, and then saves the store's metadata.
         *
         * @param virtualItem the virtual item to replace
         */
        public static void save(VirtualItem virtualItem)
        {
            replaceVirtualItem(virtualItem);
            save();
        }

        /**
         * Replaces an old virtual item with a new one by doing the following:
         * 1. Determines the type of the given virtual item.
         * 2. Looks for the given virtual item in the relevant list, according to its type.
         * 3. If found, removes it.
         * 4. Adds the given virtual item.
         *
         * @param virtualItem the virtual item that replaces the old one if exists.
         */
        public static void replaceVirtualItem(VirtualItem virtualItem)
        {
            mVirtualItems.Add(virtualItem.getItemId(), virtualItem);

            if (virtualItem is VirtualCurrency)
            {
                for (int i = 0; i < mCurrencies.Count; i++)
                {
                    if (mCurrencies[i].getItemId() == virtualItem.getItemId())
                    {
                        mCurrencies.RemoveAt(i);
                        break;
                    }
                }
                mCurrencies.Add((VirtualCurrency)virtualItem);
            }

            if (virtualItem is VirtualCurrencyPack)
            {
                VirtualCurrencyPack vcp = (VirtualCurrencyPack)virtualItem;
                PurchaseType purchaseType = vcp.GetPurchaseType();
                if (purchaseType is PurchaseWithMarket)
                {
                    mPurchasableItems.Add(((PurchaseWithMarket)purchaseType).getMarketItem()
                            .getProductId(), vcp);
                }

                for (int i = 0; i < mCurrencyPacks.Count; i++)
                {
                    if (mCurrencyPacks[i].getItemId() == vcp.getItemId())
                    {
                        mCurrencyPacks.RemoveAt(i);
                        break;
                    }
                }
                mCurrencyPacks.Add(vcp);
            }

            if (virtualItem is VirtualGood)
            {
                VirtualGood vg = (VirtualGood)virtualItem;

                if (vg is UpgradeVG)
                {
                    List<UpgradeVG> upgrades = mGoodsUpgrades[((UpgradeVG)vg).getGoodItemId()];
                    if (upgrades == null)
                    {
                        upgrades = new List<UpgradeVG>();
                        mGoodsUpgrades.Add(((UpgradeVG)vg).getGoodItemId(), upgrades);
                    }
                    upgrades.Add((UpgradeVG)vg);
                }

                PurchaseType purchaseType = vg.GetPurchaseType();
                if (purchaseType is PurchaseWithMarket)
                {
                    mPurchasableItems.Add(((PurchaseWithMarket)purchaseType).getMarketItem()
                            .getProductId(), vg);
                }

                for (int i = 0; i < mGoods.Count; i++)
                {
                    if (mGoods[i].getItemId() == vg.getItemId())
                    {
                        mGoods.RemoveAt(i);
                        break;
                    }
                }
                mGoods.Add(vg);
            }
            /*
            if (virtualItem is NonConsumableItem) {
                NonConsumableItem non = (NonConsumableItem) virtualItem;

                PurchaseType purchaseType = non.GetPurchaseType();
                if (purchaseType is PurchaseWithMarket) {
                    mPurchasableItems.Add(((PurchaseWithMarket) purchaseType).getMarketItem()
                            .getProductId(), non);
                }

                for(int i=0; i<mNonConsumables.Count; i++) {
                    if (mNonConsumables[i].getItemId() == non.getItemId()) {
                        mNonConsumables.RemoveAt(i);
                        break;
                    }
                }
                mNonConsumables.Add(non);
            }*/
        }

        /**
         * Initializes from <code>IStoreAssets</code>.
         * This happens only once - when the game is loaded for the first time.
         *
         * @param storeAssets game economy
         */
        private static void initializeWithStoreAssets(IStoreAssets storeAssets)
        {
            // fall-back here if the json doesn't exist,
            // we load the store from the given {@link IStoreAssets}.
            mCurrencies = new List<VirtualCurrency>();
            mCurrencies.AddRange(storeAssets.GetCurrencies());
            mCurrencyPacks = new List<VirtualCurrencyPack>();
            mCurrencyPacks.AddRange(storeAssets.GetCurrencyPacks());
            mGoods = new List<VirtualGood>();
            mGoods.AddRange(storeAssets.GetGoods());
            mCategories = new List<VirtualCategory>();
            mCategories.AddRange(storeAssets.GetCategories());
            //mNonConsumables = new List<NonConsumableItem>();
            //mNonConsumables.AddRange(storeAssets.GetNonConsumableItems());

            mVirtualItems = new Dictionary<String, VirtualItem>();
            mPurchasableItems = new Dictionary<String, PurchasableVirtualItem>();
            mGoodsCategories = new Dictionary<String, VirtualCategory>();
            mGoodsUpgrades = new Dictionary<String, List<UpgradeVG>>();

            foreach (VirtualCurrency vi in mCurrencies)
            {
                mVirtualItems.Add(vi.getItemId(), vi);
            }

            foreach (VirtualCurrencyPack vi in mCurrencyPacks)
            {
                mVirtualItems.Add(vi.getItemId(), vi);

                PurchaseType purchaseType = vi.GetPurchaseType();
                if (purchaseType is PurchaseWithMarket)
                {
                    mPurchasableItems.Add(((PurchaseWithMarket)purchaseType).getMarketItem()
                            .getProductId(), vi);
                }
            }

            foreach (VirtualGood vi in mGoods)
            {
                mVirtualItems.Add(vi.getItemId(), vi);

                if (vi is UpgradeVG)
                {
                    List<UpgradeVG> upgrades;
                    if (mGoodsUpgrades.ContainsKey(((UpgradeVG)vi).getGoodItemId()))
                    {
                        upgrades = mGoodsUpgrades[((UpgradeVG)vi).getGoodItemId()];
                    }
                    else
                    {
                        upgrades = null;
                    }

                    if (upgrades == null)
                    {
                        upgrades = new List<UpgradeVG>();
                        mGoodsUpgrades.Add(((UpgradeVG)vi).getGoodItemId(), upgrades);
                    }
                    upgrades.Add((UpgradeVG)vi);
                }

                PurchaseType purchaseType = vi.GetPurchaseType();
                if (purchaseType is PurchaseWithMarket)
                {
                    mPurchasableItems.Add(((PurchaseWithMarket)purchaseType).getMarketItem()
                            .getProductId(), vi);
                }
            }
            /*
            foreach(NonConsumableItem vi in mNonConsumables) {
                mVirtualItems.Add(vi.getItemId(), vi);

                PurchaseType purchaseType = vi.GetPurchaseType();
                if (purchaseType is PurchaseWithMarket) {
                    mPurchasableItems.Add(((PurchaseWithMarket) purchaseType).getMarketItem()
                            .getProductId(), vi);
                }
            }
            */
            foreach (VirtualCategory category in mCategories)
            {
                foreach (String goodItemId in category.getGoodsItemIds())
                {
                    mGoodsCategories.Add(goodItemId, category);
                }
            }

            save();
        }

        private static void checkMetadataVersion()
        {
            Windows.Storage.ApplicationDataContainer prefs = Windows.Storage.ApplicationData.Current.LocalSettings;
            //IsolatedStorageSettings prefs = IsolatedStorageSettings.ApplicationSettings;

            int mt_ver = -1;

            //if (prefs.Contains("MT_VER"))
            if (prefs.Values.ContainsKey("MT_VER"))
            {
                object outVal = null;
                prefs.Values.TryGetValue("MT_VER", out outVal);
                mt_ver = (int)outVal;

                //prefs.TryGetValue<int>("MT_VER", out mt_ver);
            }

            int sa_ver_old = -1;

            //if (prefs.Contains("SA_VER_OLD"))
            if (prefs.Containers.ContainsKey("SA_VER_OLD"))
            {
                object outVal = null;
                prefs.Values.TryGetValue("SA_VER_OLD", out outVal);
                sa_ver_old = (int)outVal;

                //prefs.TryGetValue<int>("SA_VER_OLD", out sa_ver_old);
            }
            if (mt_ver < StoreConfig.METADATA_VERSION || sa_ver_old < mCurrentAssetsVersion)
            {
                prefs.Values["MT_VER"] = StoreConfig.METADATA_VERSION;
                prefs.Values["SA_VER_OLD"] = mCurrentAssetsVersion;
                //prefs.Save();
                if (mt_ver > -1)
                {
                    KeyValueStorage.DeleteKeyValue(keyMetaStoreInfo());
                }
            }
        }


        /** Private Members **/

        private static String keyMetaStoreInfo()
        {
            return "meta.storeinfo";
        }

        private const String TAG = "SOOMLA StoreInfo"; //used for Log messages
        public const String DB_NONCONSUMABLE_KEY_PREFIX = "nonconsumable.";

        // convenient hash of virtual items
        private static Dictionary<String, VirtualItem> mVirtualItems;

        // convenient hash of purchasable virtual items
        private static Dictionary<String, PurchasableVirtualItem> mPurchasableItems;

        // convenient hash of goods-categories
        private static Dictionary<String, VirtualCategory> mGoodsCategories;

        // convenient hash of good-upgrades
        private static Dictionary<String, List<UpgradeVG>> mGoodsUpgrades;

        // list of virtual currencies
        private static List<VirtualCurrency> mCurrencies;

        // list of currency-packs
        private static List<VirtualCurrencyPack> mCurrencyPacks;

        // list of virtual goods
        private static List<VirtualGood> mGoods;

        // list of virtul categories
        private static List<VirtualCategory> mCategories;

        // list of non consumable items
        //private static List<NonConsumableItem> mNonConsumables;

        private static int mCurrentAssetsVersion = 0;
    }
}