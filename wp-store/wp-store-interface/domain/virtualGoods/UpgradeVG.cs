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
using SoomlaWpStore.data;
using SoomlaWpStore.exceptions;
using SoomlaWpStore.purchasesTypes;
using SoomlaWpCore.util;

/**
 * An upgrade virtual good is one VG in a series of VGs that define an upgrade scale of an
 * associated <code>VirtualGood</code>.
 *
 * This type of virtual good is best explained with an example:
 * Let's say there's a strength attribute to one of the characters in your game and that strength is
 * on a scale of 1-5. You want to provide your users with the ability to upgrade that strength.
 *
 * This is what you'll need to create:
 *  1. <code>SingleUseVG</code> for 'strength'
 *  2. <code>UpgradeVG</code> for strength 'level 1'
 *  3. <code>UpgradeVG</code> for strength 'level 2'
 *  4. <code>UpgradeVG</code> for strength 'level 3'
 *  5. <code>UpgradeVG</code> for strength 'level 4'
 *  6. <code>UpgradeVG</code> for strength 'level 5'
 *
 * When the user buys this <code>UpgradeVG</code>, we check and make sure the appropriate conditions
 * are met and buy it for you (which actually means we upgrade the associated VirtualGood).
 *
 * NOTE: In case you want this item to be available for purchase with real money
 * you will need to define the item in the market (Google Play, Amazon App Store, etc...).
 *
 * Inheritance: UpgradeVG >
 * {@link com.soomla.store.domain.virtualGoods.VirtualGood} >
 * {@link com.soomla.store.domain.PurchasableVirtualItem} >
 * {@link com.soomla.store.domain.VirtualItem}
 */

namespace SoomlaWpStore.domain.virtualGoods
{
    public class UpgradeVG : LifetimeVG
    {

        /** Constructor
         *
         * @param goodItemId the itemId of the <code>VirtualGood</code> associated with this upgrade.
         * @param prevItemId the itemId of the <code>UpgradeVG</code> before, or if this is the first
         *                   <code>UpgradeVG</code> in the scale then the value is null.
         * @param nextItemId the itemId of the <code>UpgradeVG</code> after, or if this is the last
         *                   <code>UpgradeVG</code> in the scale then the value is null.
         * @param mName see parent
         * @param mDescription see parent
         * @param mItemId see parent
         * @param purchaseType see parent
         */
        public UpgradeVG(String goodItemId,
                         String prevItemId, String nextItemId,
                         String mName, String mDescription,
                         String mItemId,
                         PurchaseType purchaseType) : base(mName, mDescription, mItemId, purchaseType)
        {

            mGoodItemId = goodItemId;
            mPrevItemId = prevItemId;
            mNextItemId = nextItemId;
        }

        /**
         * Constructor
         *
         * @param jsonObject see parent
         * @throws JSONException
         */
        public UpgradeVG(JSONObject jsonObject) : base(jsonObject)
        {

        }

        /**
         * @{inheritDoc}
         */
        public override JSONObject toJSONObject()
        {
            return null;
        }

        /**
         * Assigns the current upgrade to the associated <code>VirtualGood</code> (mGood).
         *
         * NOTE: This action doesn't check anything! It just assigns the current UpgradeVG to the
         * associated mGood.
         *
         * @param amount is NOT USED HERE!
         * @return 1 if the user was given the good, 0 otherwise
         */
        public override int give(int amount, bool notify)
        {
            return 0;
        }

        /**
        * Takes upgrade from the user, or in other words DOWNGRADES the associated
        * <code>VirtualGood</code> (mGood).
        * Checks if the current Upgrade is really associated with the <code>VirtualGood</code> and:
        *
        *   if YES - downgrades to the previous upgrade (or remove upgrades in case of null).
        *   if NO  - returns 0 (does nothing).
        *
        * @param amount is NOT USED HERE!
        * @param notify see parent
        * @return see parent
        */
        public override int take(int amount, bool notify)
        {
            return 0;
        }

        /**
         * Determines if the user is in a state that allows him/her to buy an <code>UpgradeVG</code>
         * This method enforces allowing/rejecting of upgrades here so users won't buy them when
         * they are not supposed to.
         * If you want to give your users free upgrades, use the <code>give</code> function.
         *
         * @return true if can buy, false otherwise
         */
        protected override bool CanBuy()
        {
            return false;
        }

        /** Setters and Getters **/

        public String getGoodItemId()
        {
            return mGoodItemId;
        }

        public String getPrevItemId()
        {
            return mPrevItemId;
        }

        public String getNextItemId()
        {
            return mNextItemId;
        }


        /** Private Members **/

        private const String TAG = "SOOMLA UpgradeVG"; //used for Log messages

        private String mGoodItemId; //the itemId of the VirtualGood associated with this upgrade

        /**
         * The itemId of the UpgradeVG before, or if this is the first UpgradeVG in the scale then
         * the value is null.
         */
        private String mPrevItemId;

        /**
         * The itemId of the UpgradeVG after, or if this is the last UpgradeVG in the scale then
         * the value is null.
         */
        private String mNextItemId;

    }
}