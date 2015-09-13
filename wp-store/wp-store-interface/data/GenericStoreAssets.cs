using System;
using System.Collections.Generic;
using System.Linq;
using SoomlaWpStore.domain;
using SoomlaWpStore.domain.virtualCurrencies;
using SoomlaWpStore.domain.virtualGoods;

namespace SoomlaWpStore.data
{
    /// <summary>   Generic IStoreAssets to build a StoreAssets from a JSON string. </summary>
    public class GenericStoreAssets : IStoreAssets
    {
        VirtualCurrency[] mVirtualCurrency;
        VirtualGood[] mVirtualGood;
        VirtualCurrencyPack[] mVirtualCurrencyPack;
        VirtualCategory[] mVirtualCategory;
        //NonConsumableItem[] mNonConsumableItem;
        int mVersion;

        private static GenericStoreAssets instance;

        public static GenericStoreAssets GetInstance()
        {
            if(instance == null)
            {
                instance = new GenericStoreAssets();
            }
            return instance;
        }
        public void Prepare(int version, String JsonStoreAssets)
        {
        }

        public int GetVersion()
        {
            return mVersion;
        }
        public VirtualCurrency[] GetCurrencies()
        {
            return mVirtualCurrency;
        }
        public VirtualGood[] GetGoods()
        {
            return mVirtualGood;
        }
        public VirtualCurrencyPack[] GetCurrencyPacks()
        {
            return mVirtualCurrencyPack;
        }
        public VirtualCategory[] GetCategories()
        {
            return mVirtualCategory;
        }

        VirtualCurrency[] IStoreAssets.GetCurrencies()
        {
            throw new NotImplementedException();
        }

        VirtualCurrencyPack[] IStoreAssets.GetCurrencyPacks()
        {
            throw new NotImplementedException();
        }

        VirtualCategory[] IStoreAssets.GetCategories()
        {
            throw new NotImplementedException();
        }

        /*
public NonConsumableItem[] GetNonConsumableItems()
{
   return mNonConsumableItem;
}*/

        private const String TAG = "SOOMLA GenericStoreAssets"; //used for Log messages

    }
}
