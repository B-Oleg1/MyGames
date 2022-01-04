using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemScript : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private int _commandId;

    [SerializeField]
    private int _itemId;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            UseItem();
            eventData.Reset();
        }
        else if (eventData.button == PointerEventData.InputButton.Middle)
        {
            BuyItem();
            eventData.Reset();
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            SellItem();
            eventData.Reset();
        }
    }

    private void UseItem()
    {
        if (ModelsScript.Players[_commandId].Items.Any(item => item == (ShopItem)_itemId))
        {
            switch (_itemId)
            {
                case 0:
                    ModelsScript.setFreeze = true;                   
                    break;
                case 1:
                    ModelsScript.setBomb = true;
                    break;
                case 2:
                    ModelsScript.currentBonus.Add(ShopItem.shield);
                    break;
                case 3:
                    ModelsScript.Players[_commandId].WearingArmor = 3;
                    break;
                case 4:
                    ModelsScript.currentBonus.Add(ShopItem.x2);
                    break;
                case 5:
                    ModelsScript.currentBonus.Add(ShopItem.x3);
                    break;
                case 6:
                    ModelsScript.currentCommandIdResponds = _commandId;
                    break;
                case 7:
                    ModelsScript.giveQuestion = true;
                    break;
                case 8:
                    ModelsScript.currentBonus.Add(ShopItem.doublemove);
                    break;
                case 9:
                    ModelsScript.currentBonus.Add(ShopItem.hint);
                    break;
                case 10:
                    ModelsScript.currentBonus.Add(ShopItem.x4);
                    break;
                case 11:
                    ModelsScript.attack[0] = 1;
                    ModelsScript.attack[1] = _commandId;
                    // TODO: Sword
                    break;
                default:
                    break;
            }

            ModelsScript.Players[_commandId].Items.Remove((ShopItem)_itemId);

            ModelsScript.needUpdateCommandId = _commandId;
        }
    }

    private void BuyItem()
    {
        if (ModelsScript.Players[_commandId].ShopScore >= ModelsScript.priceItems[_itemId, 0])
        {
            ModelsScript.Players[_commandId].ShopScore -= ModelsScript.priceItems[_itemId, 0];
            ModelsScript.Players[_commandId].Items.Add((ShopItem)_itemId);

            ModelsScript.needUpdateCommandId = _commandId;
        }
    }

    private void SellItem()
    {
        if (ModelsScript.Players[_commandId].Items.Any(item => item == (ShopItem)_itemId))
        {
            ModelsScript.Players[_commandId].Items.Remove((ShopItem)_itemId);
            ModelsScript.Players[_commandId].ShopScore += ModelsScript.priceItems[_itemId, 1];

            ModelsScript.needUpdateCommandId = _commandId;
        }
    }
}