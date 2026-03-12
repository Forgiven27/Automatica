using System.Collections.Generic;

namespace Simulator
{
    public interface IItemSource
    {
        public bool TryExport(List<ItemType> itemRequiredTypes, out List<ItemType> itemExistTypes);
        public bool TryExport();
        public void Export(ItemType itemType);
        public ItemType Export();
    }
}