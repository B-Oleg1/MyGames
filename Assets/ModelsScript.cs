using Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class ModelsScript
{
    public static List<Player> Players;

    public static int currentQuestion = 0;
    public static int currentPointsForQuestions = 0;
    public static Transform[] allSceneWithQuestion = new Transform[48];
    public static Button[] allBtns;

    public static int needUpdateCommandId = -1;
}

public enum ShopItem
{
    freeze,
    bomb,
    shield,
    armor,
    x2,
    x3,    
    takeaxe,
    giveaxe,
    doublemove,
    hint,
    x4,
    sword
}