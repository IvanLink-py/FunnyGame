using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Item/Default item", order = 1)]
public class ItemInfo : ScriptableObject
{
    public string id;
    public string showName;
    public Sprite image;
    public int stackSize;
    
    public bool Equals(Object obj)
    {
        if (obj == null || GetType() != obj.GetType()) return false;
        return id == ((ItemInfo)obj).id;
    }
}
