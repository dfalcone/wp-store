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

namespace SoomlaWpStore.domain.virtualCurrencies
{
    /**
     * Every game has its virtual currencies. This class represents a pack of a specific
     * {@link com.soomla.store.domain.virtualCurrencies.VirtualCurrency}.
     *
     * Real Game Example: If the virtual currency in your game is a 'Coin', you will sell packs of
     * 'Coins' such as "10 Coins Set" or "Super Saver Pack".
     *
     * NOTE: In case you want this item to be available for purchase with real money  you will need to
     * define the item in the market (Google Play, Amazon App Store, etc...).
     *
     * Inheritance: VirtualCurrencyPack >
     * {@link com.soomla.store.domain.PurchasableVirtualItem} >
     * {@link com.soomla.store.domain.VirtualItem}
     */
    public class VirtualCurrencyPack : PurchasableVirtualItem {

    /**
     * Constructor
     *
     * @param mName see parent
     * @param mDescription see parent
     * @param mItemId see parent
     * @param mCurrencyAmount the amount of currency in the pack
     * @param mCurrencyItemId the item id of the currency associated with this pack
     * @param purchaseType see parent
     */
    public VirtualCurrencyPack(String mName, String mDescription, String mItemId,
                               int mCurrencyAmount,
                               String mCurrencyItemId,
                               PurchaseType purchaseType) : base(mName, mDescription, mItemId, purchaseType){
        
        this.mCurrencyItemId = mCurrencyItemId;
        this.mCurrencyAmount = mCurrencyAmount;
    }

    /**
     * Constructor
     *
     * @param jsonObject see parent
     * @throws JSONException
     */
    public VirtualCurrencyPack(JSONObject jsonObject) : base(jsonObject) {
    }


    /** Setters and Getters **/

    public int getCurrencyAmount() {
        return mCurrencyAmount;
    }

    public String getCurrencyItemId() {
        return mCurrencyItemId;
    }

        protected override bool CanBuy()
        {
            throw new NotImplementedException();
        }

        public override int give(int amount, bool notify)
        {
            throw new NotImplementedException();
        }

        public override int take(int amount, bool notify)
        {
            throw new NotImplementedException();
        }

        public override int resetBalance(int balance, bool notify)
        {
            throw new NotImplementedException();
        }

        public override JSONObject toJSONObject()
        {
            throw new NotImplementedException();
        }


        /** Private Members **/

        private const String TAG = "SOOMLA VirtualCurrencyPack"; //used for Log messages

    private int mCurrencyAmount; //the amount of currency in the pack

    private String mCurrencyItemId; //the itemId of the currency associated with this pack
}
}