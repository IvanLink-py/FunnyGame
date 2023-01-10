using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Item/Default item", order = 1)]
public class ItemInfo : ScriptableObject
{
    public string id;
    public string showName;
    public Sprite image;
    public int stackSize;
    
}
