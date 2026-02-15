using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Tools
{
    public static class DevTools
    {
        public static T DeepCopy<T>(this T source) where T : class
        {
            if (source == null) return null;

            string json = JsonUtility.ToJson(source);
            return JsonUtility.FromJson<T>(json);
        }

        public static List<T> DeepCopyList<T>(this List<T> source) where T : class
        {
            if (source == null) return null;

            List<T> newList = new List<T>();
            foreach (var item in source)
            {
                if (item == null)
                    newList.Add(null);
                else
                    newList.Add(item.DeepCopy());
            }
            return newList;
        }

    }
}
