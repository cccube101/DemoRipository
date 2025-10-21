using UnityEngine;

public static class StoreProcess
{
    public enum ItemType
    {
        None,
        Skill_Fire,
        Skill_Water,
        Skill_Wind,
        Skill_Earth,

        Potion_Health,
    }

    public struct ItemData
    {
        public ItemType itemType;
        public int itemPrice;
        public int itemCount;
    }
    // ---------------------------- Field

    // リストでもスクリプタブルでもなんでも良い
    public static readonly ItemData[] storeItemDataArray = new ItemData[]
    {
        new (){ itemType = ItemType.Skill_Fire, itemPrice = 100, itemCount = 1 },
        new (){ itemType = ItemType.Skill_Water, itemPrice = 150, itemCount = 1 },
        new (){ itemType = ItemType.Skill_Wind, itemPrice = 200, itemCount = 1 },
        new (){ itemType = ItemType.Skill_Earth, itemPrice = 250, itemCount = 1 },
        new (){ itemType = ItemType.Potion_Health, itemPrice = 50, itemCount = 5 },
    };


    // ---------------------------- PublicMethod
    public static bool TryBuyItem(ItemType itemType, int playerPossessionMoney, out ItemData item)
    {
        var foundItem = System.Array.Find(storeItemDataArray, i => i.itemType == itemType);
        item = foundItem;
        if (foundItem.itemType != ItemType.None)
        {
            if (foundItem.itemCount <= 0
                || foundItem.itemPrice >= playerPossessionMoney)
            {
                return false;
            }

            item = foundItem;
            return true;
        }
        else
        {
            return false;
        }

    }




    // ---------------------------- PrivateMethod





}
