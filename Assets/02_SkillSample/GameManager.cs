using System;
using UnityEngine;




public class GameManager : MonoBehaviour
{
    // ---------------------------- SerializeField
    [SerializeField] private PlayerCtrl _playerCtrl;

    // ---------------------------- Field


    // ---------------------------- UnityMessage




    // ---------------------------- PublicMethod
    public void TryBuyItem(StoreProcess.ItemType type)
    {
        if (StoreProcess.TryBuyItem(type, _playerCtrl.PlayerData.money, out var itemData))
        {
            _playerCtrl.GetItem(itemData);
            Debug.Log($"Bought item: {itemData.itemType}, Price: {itemData.itemPrice}");
        }
        else
        {
            Debug.Log("Can't Buy.");
        }
    }




    // ---------------------------- PrivateMethod





}
