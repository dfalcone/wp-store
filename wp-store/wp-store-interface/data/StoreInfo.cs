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
using SoomlaWpStore.domain;
using SoomlaWpStore.domain.virtualGoods;
using System.Collections.Generic;
using SoomlaWpStore.domain.virtualCurrencies;
using SoomlaWpCore.util;

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
            return false;
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
            return null;
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
            return null;
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
            return null;
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
            return null;
        }

        public static List<VirtualCurrency> getCurrencies()
        {
            return null;
        }

        public static List<VirtualCurrencyPack> getCurrencyPacks()
        {
            return null;
        }

        public static List<VirtualGood> getGoods()
        {
            return null;
        }

        public static List<VirtualCategory> getCategories()
        {
            return null;
        }

        public static List<String> getAllProductIds()
        {
            return null;
        }

        /**
         * Converts <code>StoreInfo</code> to a <code>JSONObject</code>.
         *
         * @return a <code>JSONObject</code> representation of <code>StoreInfo</code>.
         */
        public static JSONObject toJSONObject()
        {
            return null;
        }

        /**
         * Saves the store's metadata in the database as JSON.
         */
        public static void save()
        {
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
        }

        public const String DB_NONCONSUMABLE_KEY_PREFIX = "nonconsumable.";

    }
}