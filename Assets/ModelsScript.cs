using Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class ModelsScript
{
    public static List<Player> Players;

    public static bool setBomb = false;
    public static bool setFreeze = false;

    public static int currentQuestion = 0;
    public static int currentPointsForQuestions = 0;
    public static List<ShopItem> currentBonus = new List<ShopItem>();
    public static List<int> lvlsWithBomb = new List<int>();
    public static Dictionary<int, int> lvlsWithFreeze = new Dictionary<int, int>();

    public static float[,] priceItems = { { 1.5f, 0.5f },
                                          { 1.2f, 0.4f },
                                          { 1.5f, 0.7f },
                                          { 2, 0.6f },
                                          { 2.5f, 1.2f },
                                          { 4, 2 },
                                          { 7, 3 },
                                          { 7, 3 },
                                          { 8.5f, 4 },
                                          { 8, 4 },
                                          { 7, 3 },
                                          { 10, 5 } };

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