using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace PanicBuying
{
    public struct ItemData : INetworkSerializeByMemcpy
    {
        public ItemData(int InId, int InCount)
        {
            id = InId;
            count = InCount;
        }

        int id;
        public int Id { get => id; }
        int count;
        public int Count { get => count; private set => count = value; }

        public bool Accumulate(ref ItemData Other)
        {
            if (Other.Id != id ||
                GetExtraCount() <= 0 ||
                Other.Count <= 0)
            {
                return false;
            }
            else if (Other.Count <= GetExtraCount())
            {
                count += Other.Count;
                Other.count = 0;
                return true;
            }
            else
            {
                Other.count -= GetExtraCount();
                count = GetMaxCount();
                return true;
            }
        }

        public int GetMaxCount()
        {
            return int.MaxValue;
        }

        public int GetExtraCount()
        {
            return Math.Max(GetMaxCount() - Count, 0);
        }
    }
}
