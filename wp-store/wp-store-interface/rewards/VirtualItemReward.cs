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

using System.Collections;
using SoomlaWpStore;
using SoomlaWpStore.exceptions;
using SoomlaWpCore.util;


namespace SoomlaWpCore.rewards
{

    /// <summary>
    /// A specific type of <code>Reward</code> is the one you'll use to give your
    /// users some amount of a virtual item when they complete something.
    /// </summary>
    public class VirtualItemReward : Reward
    {
        private static string TAG = "SOOMLA VirtualItemReward";

        public string AssociatedItemId;
        public int Amount;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="rewardId">see parent</param>
        /// <param name="name">see parent</param>
        public VirtualItemReward(string rewardId, string name, string associatedItemId, int amount)
            : this(null)
        {
            AssociatedItemId = associatedItemId;
            Amount = amount;
        }

        /// <summary>
        /// see parent.
        /// </summary>
        public VirtualItemReward(JSONObject jsonReward)
            : base(jsonReward)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <returns>see parent</returns>
        public override JSONObject toJSONObject()
        {
            return null;
        }

        protected override bool giveInner()
        {
            return true;
        }

        protected override bool takeInner()
        {
            return true;
        }

    }


}
