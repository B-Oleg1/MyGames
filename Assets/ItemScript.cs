using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemScript : MonoBehaviour
{
    [SerializeField]
    private int _commandId;

    [SerializeField]
    private int _itemId;

    private Camera _camera;
    private bool _itemUse = false;

    private void Start()
    {
        _camera = GameObject.Find("Main Camera").GetComponent<Camera>();
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            RaycastHit2D hit = new RaycastHit2D();
            hit = Physics2D.Raycast(_camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null && hit.collider.CompareTag("Item") && !_itemUse)
            {
                _itemUse = true;
                UseItem();
            }
        }
        else if (Input.GetKeyUp(KeyCode.Mouse1) && !_itemUse)
        {
            _itemUse = true;
            BuyItem();
        }
        else if (Input.GetKeyUp(KeyCode.Mouse2) && !_itemUse)
        {
            _itemUse = true;
            SellItem();
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
                    ModelsScript.Players[_commandId].WearingArmor = true;
                    break;
                case 4:
                    ModelsScript.currentBonus.Add(ShopItem.x2);
                    break;
                case 5:
                    ModelsScript.currentBonus.Add(ShopItem.x3);
                    break;
                case 6:
                    // take
                    break;
                case 7:
                    // give
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
                    // TODO: Sword
                    break;
                default:
                    break;
            }

            ModelsScript.Players[_commandId].Items.Remove((ShopItem)_itemId);

            ModelsScript.needUpdateCommandId = _commandId;
        }

        _itemUse = false;
    }

    private void BuyItem()
    {
        if (ModelsScript.Players[_commandId].ShopScore >= ModelsScript.priceItems[_itemId, 0])
        {
            ModelsScript.Players[_commandId].ShopScore -= ModelsScript.priceItems[_itemId, 0];
            ModelsScript.Players[_commandId].Items.Add((ShopItem)_itemId);

            ModelsScript.needUpdateCommandId = _commandId;
        }

        _itemUse = false;
    }

    private void SellItem()
    {
        if (ModelsScript.Players[_commandId].Items.Any(item => item == (ShopItem)_itemId))
        {
            ModelsScript.Players[_commandId].Items.Remove((ShopItem)_itemId);
            ModelsScript.Players[_commandId].ShopScore += ModelsScript.priceItems[_itemId, 1];

            ModelsScript.needUpdateCommandId = _commandId;
        }

        _itemUse = false;
    }
}