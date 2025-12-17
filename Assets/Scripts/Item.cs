using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObject")]
public class Item : ScriptableObject
{
    public GameObject goItem;
    public string nameItem;
    public string descriptionItem;
    public ItemType itemType;
}
public enum ItemType
{
    None,
    BaseMaterial,
    Detail,
    ResultItem
}