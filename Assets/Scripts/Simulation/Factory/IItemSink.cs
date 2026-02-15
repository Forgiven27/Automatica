namespace Simulator
{
    public interface IItemSink
    {
        public bool TryImport(ItemType itemType);
        public void Import(ItemType itemType);
    }
}