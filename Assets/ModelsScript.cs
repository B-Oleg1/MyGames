using Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class ModelsScript
{
    public static List<Player> Players;

    public static bool setFreeze = false;
    public static bool setBomb = false;
    public static int[] attack = new int[3];

    public static int currentQuestion = 0;
    public static int currentPointsForQuestions = 0;
    public static List<ShopItem> currentBonus = new List<ShopItem>();
    public static List<int> lvlsWithBomb = new List<int>();
    public static Dictionary<int, int> lvlsWithFreeze = new Dictionary<int, int>();

    public static double[,] priceItems = { { 1.5, 0.7 },
                                          { 1.5, 0.7 },
                                          { 1.5, 0.7 },
                                          { 2, 0.9 },
                                          { 2.5, 1.2 },
                                          { 4, 2 },
                                          { 7, 3 },
                                          { 7, 3 },
                                          { 8.5, 4 },
                                          { 8, 4 },
                                          { 7, 3 },
                                          { 11, 5.5 } };

    public static Transform[] allSceneWithQuestion = new Transform[96];
    public static Button[] allBtns = new Button[96];

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