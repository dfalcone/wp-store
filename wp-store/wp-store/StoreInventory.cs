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
using SoomlaWpStore.data;
using SoomlaWpStore.domain;
using SoomlaWpStore.domain.virtualGoods;
using SoomlaWpStore.domain.virtualCurrencies;
using SoomlaWpStore.exceptions;
using System.Collections.Generic;

namespace SoomlaWpStore
{
    /**
     * This class will help you do your day to day virtual economy operations easily.
     * You can give or take items from your users. You can buy items or upgrade them.
     * You can also check their equipping status and change it.
     */
    public class StoreInventory
    {

        /**
         * Buys the item with the given <code>itemId</code>.
         *
         * @param itemId id of item to be purchased
         * @param payload a string you want to be assigned to the purchase. This string
         *   is saved in a static variable and will be given bacl to you when the
         *   purchase is completed.
         * @throws InsufficientFundsException
         * @throws VirtualItemNotFoundException
         */
        public static void Buy(String itemId, String payload)
        {
            PurchasableVirtualItem pvi = (PurchasableVirtualItem)StoreInfo.getVirtualItem(itemId);
            pvi.buy(payload);
        }

        /** VIRTUAL ITEMS **/

        /**
         * Retrieves the balance of the virtual item with the given <code>itemId</code>.
         *
         * @param itemId id of the virtual item to be fetched.
         * @return balance of the virtual item with the given <code>itemId</code>.
         * @throws VirtualItemNotFoundException
         */
        public static int GetVirtualItemBalance(String itemId)
        {
            VirtualItem item = StoreInfo.getVirtualItem(itemId);
            return StorageManager.getVirtualItemStorage(item).getBalance(item);
        }

        /**
         * Gives your user the given amount of the virtual item with the given <code>itemId</code>.
         * For example, when your user plays your game for the first time you GIVE him/her 1000 gems.
         *
         * NOTE: This action is different than buy -
         * You use <code>give(int amount)</code> to give your user something for free.
         * You use <code>buy()</code> to give your user something and you get something in return.
         *
         * @param itemId id of the virtual item to be given
         * @param amount amount of the item to be given
         * @throws VirtualItemNotFoundException
         */
        public static void GiveItem(String itemId, int amount)
        {
            VirtualItem item = StoreInfo.getVirtualItem(itemId);
            item.give(amount);
        }

        /**
         * Takes from your user the given amount of the virtual item with the given <code>itemId</code>.
         * For example, when your user requests a refund you need to TAKE the item he/she is returning.
         *
         * @param itemId id of the virtual item to be taken
         * @param amount amount of the item to be given
         * @throws VirtualItemNotFoundException
         */
        public static void TakeItem(String itemId, int amount)
        {
            VirtualItem item = StoreInfo.getVirtualItem(itemId);
            item.take(amount);
        }

        /** VIRTUAL GOODS **/

        /**
         * Equips the virtual good with the given <code>goodItemId</code>.
         * Equipping means that the user decides to currently use a specific virtual good.
         * For more details and examples see {@link com.soomla.store.domain.virtualGoods.EquippableVG}.
         *
         * @param goodItemId id of the virtual good to be equipped
         * @throws VirtualItemNotFoundException
         * @throws ClassCastException
         * @throws NotEnoughGoodsException
         */
        public static void EquipVirtualGood(String goodItemId)
        {
            EquippableVG good = (EquippableVG)StoreInfo.getVirtualItem(goodItemId);

            try
            {
                good.equip();
            }
            catch (NotEnoughGoodsException e)
            {
                SoomlaUtils.LogError("StoreInventory", "UNEXPECTED! Couldn't equip something");
                throw e;
            }
        }

        /**
         * Unequips the virtual good with the given <code>goodItemId</code>. Unequipping means that the
         * user decides to stop using the virtual good he/she is currently using.
         * For more details and examples see {@link com.soomla.store.domain.virtualGoods.EquippableVG}.
         *
         * @param goodItemId id of the virtual good to be unequipped
         * @throws VirtualItemNotFoundException
         * @throws ClassCastException
         */
        public static void UnEquipVirtualGood(String goodItemId)
        {
            EquippableVG good = (EquippableVG)StoreInfo.getVirtualItem(goodItemId);

            good.unequip();
        }

        /**
         * Checks if the virtual good with the given <code>goodItemId</code> is currently equipped.
         *
         * @param goodItemId id of the virtual good who we want to know if is equipped
         * @return true if the virtual good is equipped, false otherwise
         * @throws VirtualItemNotFoundException
         * @throws ClassCastException
         */
        public static bool IsVirtualGoodEquipped(String goodItemId)
        {
            EquippableVG good = (EquippableVG)StoreInfo.getVirtualItem(goodItemId);

            return StorageManager.getVirtualGoodsStorage().isEquipped(good);
        }

        /**
         * Retrieves the upgrade level of the virtual good with the given <code>goodItemId</code>.
         *
         * For Example:
         * Let's say there's a strength attribute to one of the characters in your game and you provide
         * your users with the ability to upgrade that strength on a scale of 1-3.
         * This is what you've created:
         *  1. <code>SingleUseVG</code> for "strength"
         *  2. <code>UpgradeVG</code> for strength 'level 1'.
         *  3. <code>UpgradeVG</code> for strength 'level 2'.
         *  4. <code>UpgradeVG</code> for strength 'level 3'.
         * In the example, this function will retrieve the upgrade level for "strength" (1, 2, or 3).
         *
         * @param goodItemId id of the virtual good whose upgrade level we want to know
         * @return upgrade level of the good with the given id
         * @throws VirtualItemNotFoundException
         */
        public static int GetGoodUpgradeLevel(String goodItemId)
        {
            VirtualGood good = (VirtualGood)StoreInfo.getVirtualItem(goodItemId);
            UpgradeVG upgradeVG = StorageManager.getVirtualGoodsStorage().getCurrentUpgrade(good);
            if (upgradeVG == null)
            {
                return 0; //no upgrade
            }

            UpgradeVG first = StoreInfo.getGoodFirstUpgrade(goodItemId);
            int level = 1;
            while (!first.Equals(upgradeVG))
            {
                first = (UpgradeVG)StoreInfo.getVirtualItem(first.getNextItemId());
                level++;
            }

            return level;
        }

        /**
         * Retrieves the itemId of the current upgrade of the virtual good with the given
         * <code>goodItemId</code>.
         *
         * @param goodItemId id of the virtual good whose upgrade id we want to know
         * @return upgrade id if exists, or empty string otherwise
         * @throws VirtualItemNotFoundException
         */
        public static String GetGoodCurrentUpgrade(String goodItemId)
        {
            VirtualGood good = (VirtualGood)StoreInfo.getVirtualItem(goodItemId);
            UpgradeVG upgradeVG = StorageManager.getVirtualGoodsStorage().getCurrentUpgrade(good);
            if (upgradeVG == null)
            {
                return "";
            }
            return upgradeVG.getItemId();
        }

        /**
         * Upgrades the virtual good with the given <code>goodItemId</code> by doing the following:
         * 1. Checks if the good is currently upgraded or if this is the first time being upgraded.
         * 2. If the good is currently upgraded, upgrades to the next upgrade in the series, or in
         *    other words, <code>buy()</code>s the next upgrade. In case there are no more upgrades
         *    available(meaning the current upgrade is the last available), the function returns.
         * 3. If the good has never been upgraded before, the function upgrades it to the first
         *    available upgrade, or in other words, <code>buy()</code>s the first upgrade in the series.
         *
         * @param goodItemId the id of the virtual good to be upgraded
         * @throws VirtualItemNotFoundException
         * @throws InsufficientFundsException
         */
        public static void UpgradeVirtualGood(String goodItemId)
        {
            VirtualGood good = (VirtualGood)StoreInfo.getVirtualItem(goodItemId);
            UpgradeVG upgradeVG = StorageManager.getVirtualGoodsStorage().getCurrentUpgrade(good);
            if (upgradeVG != null)
            {
                String nextItemId = upgradeVG.getNextItemId();
                if (String.IsNullOrEmpty(nextItemId))
                {
                    return;
                }
                UpgradeVG vgu = (UpgradeVG)StoreInfo.getVirtualItem(nextItemId);
                vgu.buy("");
            }
            else
            {
                UpgradeVG first = StoreInfo.getGoodFirstUpgrade(goodItemId);
                if (first != null)
                {
                    first.buy("");
                }
            }
        }

        /**
         * Upgrades the good with the given <code>upgradeItemId</code> for FREE (you are GIVING him/her
         * the upgrade). In case that the good is not an upgradeable item, an error message will be
         * produced. <code>ForceUpgrade()</code> is different than <code>UpgradeVirtualGood()<code>
         * because <code>ForceUpgrade()</code> is a FREE upgrade.
         *
         * @param upgradeItemId id of the virtual good who we want to force an upgrade upon
         * @throws VirtualItemNotFoundException
         */
        public static void ForceUpgrade(String upgradeItemId)
        {
            try
            {
                UpgradeVG upgradeVG = (UpgradeVG)StoreInfo.getVirtualItem(upgradeItemId);
                upgradeVG.give(1);
            }
            catch (InvalidCastException ex)
            {
                SoomlaUtils.LogError("SOOMLA StoreInventory",
                        "The given itemId was of a non UpgradeVG VirtualItem. Can't force it." + " " + ex.Message);
            }
        }

        /**
         * Removes all upgrades from the virtual good with the given <code>goodItemId</code>.
         *
         * @param goodItemId id of the virtual good we want to remove all upgrades from
         * @throws VirtualItemNotFoundException
         */
        public static void RemoveUpgrades(String goodItemId)
        {
            List<UpgradeVG> upgrades = StoreInfo.getGoodUpgrades(goodItemId);
            foreach (UpgradeVG upgrade in upgrades)
            {
                StorageManager.getVirtualGoodsStorage().remove(upgrade, 1, true);
            }
            VirtualGood good = (VirtualGood)StoreInfo.getVirtualItem(goodItemId);
            StorageManager.getVirtualGoodsStorage().removeUpgrades(good);
        }

        public static Dictionary<String, Dictionary<String, Object>> AllItemsBalances() {
        SoomlaUtils.LogDebug(TAG, "Fetching all items balances");

        Dictionary<String, Dictionary<String, Object>> itemsDict = new Dictionary<String, Dictionary<String, Object>>();

        SoomlaUtils.LogDebug(TAG, "Fetching balances for Currencies");
        // we're cloning the list to avoid situations where someone else tries to manipulate list while we iterate
        List<VirtualCurrency> currencies = new List<VirtualCurrency>(StoreInfo.getCurrencies());
        foreach(VirtualCurrency currency in currencies) {
            Dictionary<String, Object> updatedValues = new Dictionary<String, Object>();
            updatedValues.Add("balance", StorageManager.getVirtualCurrencyStorage().getBalance(currency));

            itemsDict.Add(currency.getItemId(), updatedValues);
        }

        SoomlaUtils.LogDebug(TAG, "Fetching balances for Goods");
        // we're cloning the list to avoid situations where someone else tries to manipulate list while we iterate
        List<VirtualGood> goods = new List<VirtualGood>(StoreInfo.getGoods());
        foreach(VirtualGood good in goods) {
            Dictionary<String, Object> updatedValues = new Dictionary<String, Object>();

            updatedValues.Add("balance", StorageManager.getVirtualGoodsStorage().getBalance(good));

            if (good is EquippableVG) {
                updatedValues.Add("equipped", StorageManager.getVirtualGoodsStorage().isEquipped((EquippableVG)good));
            }

            if (StoreInfo.hasUpgrades(good.getItemId())) {
                String vguId = StorageManager.getVirtualGoodsStorage().getCurrentUpgrade(good).getItemId();
                updatedValues.Add("currentUpgrade", (String.IsNullOrEmpty(vguId) ? "none" : vguId ));
            }

            itemsDict.Add(good.getItemId(), updatedValues);
        }

        return itemsDict;
    }

        public static bool resetAllItemsBalances(Dictionary<String, Dictionary<String, Object>> replaceBalances) {
        if (replaceBalances == null) {
            return false;
        }

        SoomlaUtils.LogDebug(TAG, "Resetting balances");

        clearCurrentState();

        SoomlaUtils.LogDebug(TAG, "Current state was cleared");

        try {
            foreach (String itemId in replaceBalances.Keys) {
                Dictionary<String, Object> updatedValues = replaceBalances[itemId];

                VirtualItem item = null;
                try {
                    item = StoreInfo.getVirtualItem(itemId);
                } catch (VirtualItemNotFoundException e) {
                    SoomlaUtils.LogError(TAG, "The given itemId " + itemId + " was not found. Can't force it. "+e.Message);
                    continue;
                }

                Object rawBalance = updatedValues["balance"];
                if (rawBalance != null) {
                    int updatedBalance = (int) rawBalance;
                    if (item != null) {
                        item.resetBalance(updatedBalance, false);
                        SoomlaUtils.LogDebug(TAG, "finished balance sync for itemId: " + itemId);
                    }
                }

                Object rawEquippedState = updatedValues["equipped"];
                if (rawEquippedState != null) {
                    try {
                        EquippableVG equippableItem = (EquippableVG) item;
                        if (equippableItem != null) {
                            Boolean equipState = (Boolean) rawEquippedState;
                            if (equipState) {
                                equippableItem.equip(false);
                            } else {
                                equippableItem.unequip(false);
                            }
                        }
                        SoomlaUtils.LogDebug(TAG, "finished equip balance sync for itemId: " + itemId);
                    } catch (NotEnoughGoodsException) {
                        SoomlaUtils.LogError(TAG, "the item " + itemId + " was not purchased, so cannot be equipped");
                    } catch (InvalidCastException exx) {
                        SoomlaUtils.LogError(TAG, "tried to equip a non-equippable item: " + itemId+ " "+exx.Message);
                    }
                }

                Object rawCurrentUpgrade = updatedValues["currentUpgrade"];
                if (rawCurrentUpgrade != null) {
                    String currentUpgradeId = (String) rawCurrentUpgrade;
                    if (!String.IsNullOrEmpty(currentUpgradeId)) {
                        try {
                            UpgradeVG upgradeVG = (UpgradeVG) StoreInfo.getVirtualItem(currentUpgradeId);
                            upgradeVG.give(1, false);

                            SoomlaUtils.LogDebug(TAG, "finished upgrade balance sync for itemId: " + itemId);
                        } catch (VirtualItemNotFoundException ex) {
                            SoomlaUtils.LogError(TAG, "The given upgradeId " + currentUpgradeId + " was not found. Can't force it. " + ex.Message);
                        } catch (InvalidCastException ex) {
                            SoomlaUtils.LogError(TAG, "The given upgradeId was of a non UpgradeVG VirtualItem. Can't force it. "+ex.Message);
                        }
                    }
                }
            }

            return true;
        }
        catch (Exception e) {
            SoomlaUtils.LogError(TAG, "Unknown error has occurred while resetting item balances " + e.Message);
        }

        return false;
    }
        
        private static void clearCurrentState() {
        List<KeyValue> allKeys = KeyValueStorage.GetEncryptedKeys();

        foreach (KeyValue kv in allKeys)
        {
            String key = kv.Key;
            if (key.StartsWith(StoreInfo.DB_NONCONSUMABLE_KEY_PREFIX) ||
                    key.StartsWith(VirtualCurrencyStorage.DB_CURRENCY_KEY_PREFIX) ||
                    key.StartsWith(VirtualGoodsStorage.DB_KEY_GOOD_PREFIX)) {
                KeyValueStorage.DeleteKeyValue(key);
            }
        }
    }

        private const String TAG = "SOOMLA StoreInventory"; //used for Log messages
    }
}