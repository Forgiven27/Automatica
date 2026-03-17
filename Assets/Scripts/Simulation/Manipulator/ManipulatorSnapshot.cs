using System.Collections.Generic;
public class ManipulatorSnapshot
{
    public float baseYaw;
    public Dictionary<uint, float> bones;
    public ItemType? heldItem;

    public ManipulatorSnapshot(float baseYaw, Dictionary<uint, float> bones, ItemType? heldItem)
    {
        this.baseYaw = baseYaw;
        this.bones = bones;
        this.heldItem = heldItem;
    }
}
