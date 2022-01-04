using Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class ModelsScript
{
    public static MainScript mainScript;

    public static List<Player> Players;

    public static bool setFreeze = false;
    public static bool setBomb = false;
    public static bool giveQuestion = false;
    public static int[] attack = new int[3];

    public static int currentCommandIdMove = 0;
    public static int currentCommandIdResponds = 0;

    public static int currentQuestion = 0;
    public static int currentPointsForQuestions = 0;
    public static List<ShopItem> currentBonus = new List<ShopItem>();
    public static List<int> lvlsWithBomb = new List<int>();
    public static Dictionary<int, int> lvlsWithFreeze = new Dictionary<int, int>();

    public static double[,] priceItems = { { 1.5, 0.7 },
                                          { 1.5, 0.7 },
                                          { 2, 1 },
                                          { 4, 2 },
                                          { 4, 2 },
                                          { 6, 3 },
                                          { 6, 3 },
                                          { 6, 3 },
                                          { 9, 4.5 },
                                          { 7, 3.5 },
                                          { 9, 4.5 },
                                          { 10, 5 } };

    public static Transform[] allSceneWithQuestion = new Transform[112];
    public static Button[] allBtns = new Button[112];

    public static AudioSource mainMusic = null;
    public static AudioSource supportMusic = null;

    public static AudioClip[] allClips;

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