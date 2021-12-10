using Assets;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainScript : MonoBehaviour
{
    [SerializeField]
    private Transform _questionObject;
    [SerializeField]
    private Transform _objectWithQuestions;
    [SerializeField]
    private Transform _objectObjectsWithButtons;

    private void Awake()
    {
        ModelsScript.allSceneWithQuestion = _objectWithQuestions.GetComponentsInChildren<Transform>();
        ModelsScript.allBtns = _objectObjectsWithButtons.GetComponentsInChildren<Button>();

        _questionObject.gameObject.SetActive(false);
        foreach (var item in ModelsScript.allSceneWithQuestion)
        {
            item.gameObject.SetActive(false);
        }

        ModelsScript.Players = new List<Player>
        {
            new Player
            {
                Name = "Команда1",
                Score = 0,
                ShopScore = 0,
                ProgressBattlePass = 0,
                Items = new List<ShopItem>(),
                BattlePassItems = new List<ShopItem>()
            },
            new Player
            {
                Name = "Команда2",
                Score = 0,
                ShopScore = 0,
                ProgressBattlePass = 0,
                Items = new List<ShopItem>(),
                BattlePassItems = new List<ShopItem>()
            },
            new Player
            {
                Name = "Команда3",
                Score = 0,
                ShopScore = 0,
                ProgressBattlePass = 0,
                Items = new List<ShopItem>(),
                BattlePassItems = new List<ShopItem>()
            }
        };

        GenerateNewBattlePass(0);
        GenerateNewBattlePass(1);
        GenerateNewBattlePass(2);

        ModelsScript.Players[0].BattlePassItems.ForEach(item => Debug.Log(item));
        ModelsScript.Players[1].BattlePassItems.ForEach(item => Debug.Log(item));
        ModelsScript.Players[2].BattlePassItems.ForEach(item => Debug.Log(item));
    }

    public void OnClickBsn(int i)
    {
        var btns = ModelsScript.allBtns[i];

        ModelsScript.currentQuestion = i;
        ModelsScript.currentPointsForQuestions = int.Parse(btns.GetComponentInChildren<Text>().text);

        _questionObject.gameObject.SetActive(true);
        _objectWithQuestions.gameObject.SetActive(true);
        ModelsScript.allSceneWithQuestion[i + 1].gameObject.SetActive(true);

        btns.enabled = false;
        var tmp = btns.GetComponent<Image>().color;
        tmp.a = 0;
        btns.GetComponent<Image>().color = tmp;

        btns.GetComponentInChildren<Text>().text = string.Empty;
    }

    public static void GiveOutPrize(int commandId)
    {
        if (ModelsScript.Players[commandId].ProgressBattlePass >= 2)
        {
            ModelsScript.Players[commandId].Items.Add(ShopItem.bomb);
        }
        else if(ModelsScript.Players[commandId].ProgressBattlePass >= 4)
        {
            ModelsScript.Players[commandId].Items.Add(ShopItem.bomb);
        }
        else if (ModelsScript.Players[commandId].ProgressBattlePass >= 6)
        {
            ModelsScript.Players[commandId].Items.Add(ShopItem.bomb);
        }
        else if (ModelsScript.Players[commandId].ProgressBattlePass >= 7)
        {
            ModelsScript.Players[commandId].Items.Add(ShopItem.bomb);
        }

        GenerateNewBattlePass(commandId);
    }

    private static List<ShopItem> GenerateNewBattlePass(int commandId)
    {
        return ModelsScript.Players[commandId].BattlePassItems = new List<ShopItem>
        {
            (ShopItem)Random.Range(0,1),
            (ShopItem)Random.Range(2,4),
            (ShopItem)Random.Range(5,9),
            (ShopItem)Random.Range(10,11)
        };
    }
}