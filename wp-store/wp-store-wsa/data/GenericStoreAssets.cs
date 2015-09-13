using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SoomlaWpStore.domain;
using SoomlaWpStore.domain.virtualCurrencies;
using SoomlaWpStore.domain.virtualGoods;
using SoomlaWpCore;
using SoomlaWpCore.util;

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
            try
            {
                SoomlaUtils.LogDebug(TAG,"Prepare :\n"+JsonStoreAssets);
                mVersion = version;

                JSONObject jSONObject = new JSONObject(JsonStoreAssets,-10);

                JSONObject virtualCurrencies = jSONObject[StoreJSONConsts.STORE_CURRENCIES];
                mVirtualCurrency = new VirtualCurrency[virtualCurrencies.Count];
                for (int i = 0; i < virtualCurrencies.Count; i++)
                {
                    JSONObject o = virtualCurrencies[i];
                    VirtualCurrency c = new VirtualCurrency(o);
                    mVirtualCurrency[i] = c;
                }

                JSONObject currencyPacks = jSONObject[StoreJSONConsts.STORE_CURRENCYPACKS];
                mVirtualCurrencyPack = new VirtualCurrencyPack[currencyPacks.Count];
                for (int i = 0; i < currencyPacks.Count; i++)
                {
                    JSONObject o = currencyPacks[i];
                    VirtualCurrencyPack pack = new VirtualCurrencyPack(o);
                    mVirtualCurrencyPack[i] = pack;
                }

                // The order in which VirtualGoods are created matters!
                // For example: VGU and VGP depend on other VGs
                JSONObject virtualGoods = jSONObject[StoreJSONConsts.STORE_GOODS];
                JSONObject suGoods = virtualGoods[StoreJSONConsts.STORE_GOODS_SU];
                JSONObject ltGoods = virtualGoods[StoreJSONConsts.STORE_GOODS_LT];
                JSONObject eqGoods = virtualGoods[StoreJSONConsts.STORE_GOODS_EQ];
                JSONObject upGoods = virtualGoods[StoreJSONConsts.STORE_GOODS_UP];
                JSONObject paGoods = virtualGoods[StoreJSONConsts.STORE_GOODS_PA];
                List<VirtualGood> goods = new List<VirtualGood>();
                for (int i = 0; i < suGoods.Count; i++)
                {
                    JSONObject o = suGoods[i];
                    SingleUseVG g = new SingleUseVG(o);
                    SoomlaUtils.LogDebug(TAG, "SingleUseVG " + g.getItemId());
                    goods.Add(g);
                }
                for (int i = 0; i < ltGoods.Count; i++)
                {
                    JSONObject o = ltGoods[i];
                    LifetimeVG g = new LifetimeVG(o);
                    SoomlaUtils.LogDebug(TAG, "LifetimeVG " + g.getItemId());
                    goods.Add(g);
                }
                for (int i = 0; i < eqGoods.Count; i++)
                {
                    JSONObject o = eqGoods[i];
                    EquippableVG g = new EquippableVG(o);
                    SoomlaUtils.LogDebug(TAG, "EquippableVG " + g.getItemId());
                    goods.Add(g);
                }
                for (int i = 0; i < paGoods.Count; i++)
                {
                    JSONObject o = paGoods[i];
                    SingleUsePackVG g = new SingleUsePackVG(o);
                    SoomlaUtils.LogDebug(TAG, "SingleUsePackVG " + g.getItemId());
                    goods.Add(g);
                }
                for (int i = 0; i < upGoods.Count; i++)
                {
                    JSONObject o = upGoods[i];
                    UpgradeVG g = new UpgradeVG(o);
                    SoomlaUtils.LogDebug(TAG, "UpgradeVG " + g.getItemId());
                    goods.Add(g);
                }

                mVirtualGood = new VirtualGood[goods.Count];
                for(int i = 0; i < goods.Count; i++)
                {
                    SoomlaUtils.LogDebug(TAG, "VirtualGood " + goods[i].getItemId());
                    mVirtualGood[i] = goods[i];
                }

                // categories depend on virtual goods. That's why the have to be initialized after!
                JSONObject virtualCategories = jSONObject[StoreJSONConsts.STORE_CATEGORIES];
                mVirtualCategory = new VirtualCategory[virtualCategories.Count];
                for (int i = 0; i < virtualCategories.Count; i++)
                {
                    JSONObject o = virtualCategories[i];
                    VirtualCategory category = new VirtualCategory(o);
                    mVirtualCategory[i] = category;
                }
                /*
                JArray nonConsumables = JSONObject.Value<JArray>(StoreJSONConsts.STORE_NONCONSUMABLES);
                mNonConsumableItem = new NonConsumableItem[nonConsumables.Count];
                for (int i = 0; i < nonConsumables.Count; i++)
                {
                    JSONObject o = nonConsumables.Value<JSONObject>(i);
                    NonConsumableItem non = new NonConsumableItem(o);
                    mNonConsumableItem[i] = non;
                }
                */
            }
            catch (Exception ex)
            {
                SoomlaUtils.LogError(TAG, "An error occurred while trying to prepare storeAssets" + ex.Message);
            }

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
        /*
        public NonConsumableItem[] GetNonConsumableItems()
        {
            return mNonConsumableItem;
        }*/

        private const String TAG = "SOOMLA GenericStoreAssets"; //used for Log messages

    }
}
