using System;
using System.Collections.Generic;

namespace pnyx.net.util
{
    public static class DictionaryUtil
    {
        public static VType getValue<KType, VType>(this Dictionary<KType, VType> map, KType key) where KType : IComparable
        {
            if (map.ContainsKey(key))
                return map[key];
            return default(VType);
        }

        public static void setValue<KType, VType>(this Dictionary<KType, VType> map, KType key, VType value) where KType : IComparable
        {
            if (map.ContainsKey(key))
                map[key] = value;
            else
                map.Add(key, value);
        }    

        public static int increaseCount<KType>(this Dictionary<KType, int> map, KType key, int toAdd = 1) where KType : IComparable
        {
            if (!map.ContainsKey(key))
            {
                map.Add(key, toAdd);
                return toAdd;
            }
            
            int newValue = map[key] + toAdd;
            map[key] = newValue;

            return newValue;
        }
    }
}