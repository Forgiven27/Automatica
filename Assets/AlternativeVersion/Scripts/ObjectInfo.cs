using UnityEngine;
namespace DullVersion
{
    public class ObjectInfo : MonoBehaviour
    {
        public Item item;
        public ItemQuality quality;
    }

    public enum Item
    {
        Iron,
        Copper
    }
    public enum ItemQuality
    {
        LowQuality, MediumQuality, HighQuality
    }
}