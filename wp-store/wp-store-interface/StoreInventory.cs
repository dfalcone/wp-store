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
            return 0;
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
            return false;
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
            return 0;
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
            return null;
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

        }

        /**
         * Removes all upgrades from the virtual good with the given <code>goodItemId</code>.
         *
         * @param goodItemId id of the virtual good we want to remove all upgrades from
         * @throws VirtualItemNotFoundException
         */
        public static void RemoveUpgrades(String goodItemId)
        {

        }

        public static Dictionary<String, Dictionary<String, Object>> AllItemsBalances()
        {
            return null;
        }

        public static bool resetAllItemsBalances(Dictionary<String, Dictionary<String, Object>> replaceBalances)
        { 
            return false;
        }

        private static void clearCurrentState()
        {

        }

        private const String TAG = "SOOMLA StoreInventory"; //used for Log messages
    }

}