using System.Collections.Generic;

namespace DullVersion
{
    public struct ItemFilter
    {
        public bool isEmptyFilter
        {
            get
            {
                if ((itemsWhiteList != null && itemsWhiteList.Count > 0) ||
                    (itemsBlackList != null && itemsBlackList.Count > 0) ||
                    (qualityWhiteList != null && qualityWhiteList.Count > 0) ||
                    (qualityBlackList != null && qualityBlackList.Count > 0))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
        public List<Item> itemsWhiteList;
        public List<Item> itemsBlackList;
        public List<ItemQuality> qualityWhiteList;
        public List<ItemQuality> qualityBlackList;


        public bool CheckObject(ObjectInfo objectInfo)
        {
            if (isEmptyFilter) return true;
            if ((itemsBlackList != null && itemsBlackList.Count > 0 && itemsBlackList.Contains(objectInfo.item)) ||
                (qualityBlackList != null && qualityBlackList.Count > 0 && qualityBlackList.Contains(objectInfo.quality)))
            {
                return false;
            }
            if ((itemsWhiteList != null && itemsWhiteList.Count > 0 && !itemsWhiteList.Contains(objectInfo.item)) ||
                (qualityWhiteList != null && qualityWhiteList.Count > 0 && !qualityWhiteList.Contains(objectInfo.quality)))
            {
                return false;
            }
            return true;
        }
    }
}