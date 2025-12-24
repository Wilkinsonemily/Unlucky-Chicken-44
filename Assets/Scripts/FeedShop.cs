using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedShop : MonoBehaviour, IInteractable2D
{
    [Header("Shop Config")]
    public int feedPrice = 50;
    public int buyQuantity = 1;

    public void Interact(GameObject interactor)
    {
        if (!PlayerInventory.I.SpendCoins(feedPrice))
        {
            Debug.Log("Not enough coins to buy feed.");
            ToastUI.Say("Not enough coins to buy feed.");
            return;
        }

        PlayerInventory.I.AddFeed(buyQuantity);
        Debug.Log($"+{buyQuantity} Feed purchased for {feedPrice} coins.");
        ToastUI.Say($"+{buyQuantity + 1} Feed purchased.");
    }
}
