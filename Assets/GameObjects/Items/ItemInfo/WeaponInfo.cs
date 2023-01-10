using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Item/Weapon", order = 2)]
public class WeaponInfo : ItemInfo
{
    public GunInfo gunInfo;
    public ItemInfo ammoItemInfo;
}
