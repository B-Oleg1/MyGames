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
    public static Transform[] allSceneWithQuestion;
    public static Button[] allBtns;
}

public enum ShopItem
{
    bomb,
    shield,
    armor,
    x2,
    x3,
    x4,
    takeaxe,
    giveaxe,
    doublemove,
    hint,
    sword
}