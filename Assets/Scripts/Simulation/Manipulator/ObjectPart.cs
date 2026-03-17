using UnityEngine;

public class ObjectPart : MonoBehaviour
{
    [Tooltip("ID Joint-а ")]
    [SerializeField] private uint JointID;
    [Tooltip("ID принадежности суставу определенному суставу ()")]
    [SerializeField] private int RelatedJointIndex;
    [SerializeField] private ObjectType type;
    [SerializeField] private string name; //TEMP
    public int GetRelatedJointIndex() => RelatedJointIndex;
    public uint GetJointID() => JointID;
    public ObjectType GetObjectType() => type;
}

public enum ObjectType
{
    Joint, // Сустав
    Bone,
    Base
}