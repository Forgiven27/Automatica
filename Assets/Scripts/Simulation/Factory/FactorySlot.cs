using System;
using System.Collections.Generic;
using System.Linq;


namespace Simulator { 
    [Serializable]
    public class FactorySlot
    {
        [NonSerialized] public string ID;
        public IOType ioType;
        public int maxCapacity;
        public int currentCount;
        public ItemType[] acceptableItemTypes;
        public ItemType currentSlotType;

        public bool TryGetLackOfItems(List<ItemType> listForCheck, out List<ItemType> itemTypes)
        {
            itemTypes = new List<ItemType>();
            if (currentCount == 0)
            {
                foreach (var acceptItem in acceptableItemTypes)
                {
                    if (listForCheck.Contains(acceptItem))
                    {
                        itemTypes.Add(acceptItem);
                    }
                }

            }
            else if (currentCount > 0 && currentCount < maxCapacity)
            {
                if (listForCheck.Contains(currentSlotType)) { 
                    itemTypes.Add(currentSlotType);
                }
            }
            else
            {
                return false;
            }
            if (itemTypes.Count == 0) return false;
            else return true;
        }

        public bool TryGetAbundanceOfItems(List<ItemType> listForCheck, out ItemType itemTypes)
        {
            itemTypes = default;
            if (currentCount == 0)
            {
                return false;
            }
            else if (currentCount > 0)
            {
                if (listForCheck.Contains(currentSlotType))
                {
                    itemTypes = currentSlotType;
                    return true;
                }
                return false;
            }
            else
            {
                return false;
            }
        }


        public bool CanImportOneItem(ItemType itemType)
        {
            if (currentCount >= maxCapacity) return false;
            if (currentCount > 0 && itemType != currentSlotType) return false;
            if (!acceptableItemTypes.Contains(itemType)) return false;
            return true;
        }

        public bool CanExportOneItem(ItemType itemType)
        {
            if (currentCount == 0) return false;
            if (currentSlotType != itemType) return false;
            return true;
        }


        public bool CanExportItems(ItemType itemType, int count)
        {
            if (currentCount == 0) return false;
            if (currentSlotType != itemType) return false;
            if (currentCount < count) return false;
            return true;
        }



        public void ImportOneItem(ItemType itemType)
        {
            if (currentCount == 0) currentSlotType = itemType;

            currentCount++;
        }

        public int ImportItems(ItemType itemType, int count)
        {
            if (currentCount == 0) currentSlotType = itemType;
            if (currentCount + count > maxCapacity)
            {
                currentCount = maxCapacity;

                return maxCapacity - (currentCount + count);
            }
            else
            {
                currentCount += count;
                return 0;
            }
        }

        public void ExportOneItem()
        {
            currentCount--;
        }
        public void ExportItems(int count)
        {
            currentCount -= count;
        }
    }
}