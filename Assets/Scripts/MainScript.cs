using Assets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class MainScript : MonoBehaviour
{
    [SerializeField]
    private Image _cursor;

    [SerializeField]
    private Transform _questionObject;
    [SerializeField]
    private Transform[] _objectObjectsWithButtons;

    [SerializeField]
    private Sprite[] _allItemsSprites;

    [SerializeField]
    private StatisticCommand[] _allCommandStatistics;

    [SerializeField]
    private Transform _wheelFortune;

    [SerializeField]
    private Animator[] _startAnimations;
    [SerializeField]
    private Animator[] _newQuestionsAnimations;
    
    [SerializeField]
    private AudioSource _mainMusic;
    [SerializeField]
    private AudioSource _supportMusic;

    [SerializeField]
    private RenderTexture _renderTexture;

    [SerializeField]
    private AudioClip[] _allClips;

    private int _countQuestionEnd = 0;
    private Vector3 _lastMousePosition;

    private void Awake()
    {
        int s = 0;
        for (int a = 0; a < _objectObjectsWithButtons.Length; a++)
        {
            for (int i = 0; i < _objectObjectsWithButtons[a].GetChild(0).childCount; i++)
            {
                for (int j = 0; j < _objectObjectsWithButtons[a].GetChild(0).GetChild(i).childCount; j++)
                {
                    if (_objectObjectsWithButtons[a].GetChild(0).GetChild(i).GetChild(j).GetComponent<Button>())
                    {
                        ModelsScript.allBtns[s] = _objectObjectsWithButtons[a].GetChild(0).GetChild(i).GetChild(j).GetComponent<Button>();
                        s++;
                    }
                }
            }
        }

        int idName = 0;
        for (int i = 0; i < _objectObjectsWithButtons.Length; i++)
        {
            for (int a = 0; a < _objectObjectsWithButtons[i].GetChild(1).childCount; a++)
            {
                Debug.Log(idName);
                _objectObjectsWithButtons[i].GetChild(1).GetChild(a).GetComponent<TextMeshProUGUI>().text = ModelsScript.savedCategory[idName].Name;
                idName++;
            }
        }

        _questionObject.gameObject.SetActive(false);

        ModelsScript.Players = new List<Player>
        {
            new Player
            {
                Name = "Лютые",
                Score = 0,
                ShopScore = 0,
                ProgressBattlePass = 0,
                Items = new List<ShopItem>(),
                BattlePassItems = new List<ShopItem>()
            },
            new Player
            {
                Name = "t1poKARATE",
                Score = 0,
                ShopScore = 0,
                ProgressBattlePass = 0,
                Items = new List<ShopItem>(),
                BattlePassItems = new List<ShopItem>()
            },
            new Player
            {
                Name = "Амогусы",
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

        UpdateStatistic(0);
        UpdateStatistic(1);
        UpdateStatistic(2);
    }

    private void Start()
    {
        StartCoroutine(StartAnimation(0));

        ModelsScript.mainScript = this;

        ModelsScript.mainMusic = _mainMusic;
        ModelsScript.supportMusic = _supportMusic;
        ModelsScript.allClips = _allClips;
        _lastMousePosition = Input.mousePosition;
        _cursor.transform.position = _lastMousePosition;
    }

    private void Update()
    {
        if (ModelsScript.needUpdateCommandId >= 0 && ModelsScript.needUpdateCommandId <= 2)
        {
            UpdateStatistic(ModelsScript.needUpdateCommandId);
            ModelsScript.needUpdateCommandId = -1;
        }

        if (Input.GetKey(KeyCode.N))
        {
            if (_countQuestionEnd == 48)
            {
                _objectObjectsWithButtons[0].gameObject.SetActive(false);
                _objectObjectsWithButtons[1].gameObject.SetActive(true);

                StartCoroutine(StartAnimation(2));
            }
            else if (_countQuestionEnd != 48 && ModelsScript.lvlsWithFreeze.Count > 0)
            {
                int b = 0;
                int iter = 0;
                while (b < ModelsScript.lvlsWithFreeze.Count)
                {
                    if (ModelsScript.lvlsWithFreeze.ContainsKey(iter))
                    {
                        ModelsScript.lvlsWithFreeze[iter]--;
                        if (ModelsScript.lvlsWithFreeze[iter] == 0)
                        {
                            ModelsScript.allBtns[iter].interactable = true;
                            ModelsScript.lvlsWithFreeze.Remove(iter);
                        }
                        b++;
                    }

                    iter++;
                }
            }
        }
        Cursor.visible = false;
        if (Input.mousePosition != _lastMousePosition)
        {
            _lastMousePosition = Input.mousePosition;
            _cursor.transform.localPosition = new Vector2(_lastMousePosition.x - 938, _lastMousePosition.y - 565);
        }
    }

    [Obsolete]
    public void OnClickBsn(int currentRoundId)
    {
        var btn = ModelsScript.allBtns[currentRoundId];
        if (ModelsScript.setFreeze)
        {
            ModelsScript.setFreeze = false;
            btn.interactable = false;
            ModelsScript.lvlsWithFreeze.Add(currentRoundId, 3);
        }
        else if (ModelsScript.setBomb)
        {
            ModelsScript.setBomb = false;
            ModelsScript.lvlsWithBomb.Add(currentRoundId);
        }
        else
        {
            ModelsScript.currentQuestion = currentRoundId;
            ModelsScript.currentPointsForQuestions = int.Parse(btn.GetComponentInChildren<Text>().text);

            for (int i = 0; i < ModelsScript.savedCategory[(currentRoundId + 1) / 7].SaveRound[currentRoundId % 7].SaveLayouts.Length; i++)
            {
                for (int a = 0; a < ModelsScript.savedCategory[(currentRoundId + 1) / 7].SaveRound[currentRoundId % 7].SaveLayouts[i].SaveText.Count; a++)
                {
                    var newGameObject = new GameObject();
                    newGameObject.AddComponent<Text>();
                    newGameObject.GetComponent<Text>().text = ModelsScript.savedCategory[(currentRoundId + 1) / 7].SaveRound[currentRoundId % 7].SaveLayouts[i].SaveText[a].Text;
                    newGameObject.GetComponent<Text>().font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
                    newGameObject.GetComponent<Text>().fontSize = ModelsScript.savedCategory[(currentRoundId + 1) / 7].SaveRound[currentRoundId % 7].SaveLayouts[i].SaveText[a].FontSize;
                    newGameObject.GetComponent<Text>().color = new Color(ModelsScript.savedCategory[(currentRoundId + 1) / 7].SaveRound[currentRoundId % 7].SaveLayouts[i].SaveText[a].RedColor,
                                                                         ModelsScript.savedCategory[(currentRoundId + 1) / 7].SaveRound[currentRoundId % 7].SaveLayouts[i].SaveText[a].GreenColor,
                                                                         ModelsScript.savedCategory[(currentRoundId + 1) / 7].SaveRound[currentRoundId % 7].SaveLayouts[i].SaveText[a].BlueColor,
                                                                         ModelsScript.savedCategory[(currentRoundId + 1) / 7].SaveRound[currentRoundId % 7].SaveLayouts[i].SaveText[a].AlphaColor);
                    if (ModelsScript.savedCategory[(currentRoundId + 1) / 7].SaveRound[currentRoundId % 7].SaveLayouts[i].SaveText[a].AlignmentText == "Left")
                    {
                        newGameObject.GetComponent<Text>().alignment = TextAnchor.UpperLeft;
                    }
                    else if (ModelsScript.savedCategory[(currentRoundId + 1) / 7].SaveRound[currentRoundId % 7].SaveLayouts[i].SaveText[a].AlignmentText == "Center")
                    {
                        newGameObject.GetComponent<Text>().alignment = TextAnchor.UpperCenter;
                    }
                    else if (ModelsScript.savedCategory[(currentRoundId + 1) / 7].SaveRound[currentRoundId % 7].SaveLayouts[i].SaveText[a].AlignmentText == "Right")
                    {
                        newGameObject.GetComponent<Text>().alignment = TextAnchor.UpperRight;
                    }

                    newGameObject.transform.SetParent(_questionObject.GetChild(0).GetChild(0).GetChild((i - 1) * -1));

                    newGameObject.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
                    newGameObject.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);

                    newGameObject.GetComponent<RectTransform>().offsetMin = new Vector2(ModelsScript.savedCategory[(currentRoundId + 1) / 7].SaveRound[currentRoundId % 7].SaveLayouts[i].SaveText[a].Left,
                                                                                        ModelsScript.savedCategory[(currentRoundId + 1) / 7].SaveRound[currentRoundId % 7].SaveLayouts[i].SaveText[a].Bottom);
                    newGameObject.GetComponent<RectTransform>().offsetMax = new Vector2(ModelsScript.savedCategory[(currentRoundId + 1) / 7].SaveRound[currentRoundId % 7].SaveLayouts[i].SaveText[a].Right,
                                                                                        ModelsScript.savedCategory[(currentRoundId + 1) / 7].SaveRound[currentRoundId % 7].SaveLayouts[i].SaveText[a].Top);

                    newGameObject.transform.localScale = new Vector3(1f, 1f, 1f);
                }
                for (int a = 0; a < ModelsScript.savedCategory[(currentRoundId + 1) / 7].SaveRound[currentRoundId % 7].SaveLayouts[i].SaveImage.Count; a++)
                {
                    var newGameObject = new GameObject();
                    newGameObject.AddComponent<Image>();

                    if (File.Exists(ModelsScript.savedCategory[(currentRoundId + 1) / 7].SaveRound[currentRoundId % 7].SaveLayouts[i].SaveImage[a].PathToImage))
                    {
                        FileStream imgFile = new FileStream(ModelsScript.savedCategory[(currentRoundId + 1) / 7].SaveRound[currentRoundId % 7].SaveLayouts[i].SaveImage[a].PathToImage, FileMode.Open);

                        byte[] imgByte = new byte[imgFile.Length];

                        imgFile.Read(imgByte, 0, imgByte.Length);

                        imgFile.Close();

                        Texture2D texture2d = new Texture2D(1920, 1080);
                        texture2d.LoadImage(imgByte);

                        Sprite spr = Sprite.Create(texture2d, new Rect(0, 0, texture2d.width, texture2d.height), Vector2.zero);
                        newGameObject.GetComponent<Image>().sprite = spr;
                    }

                    newGameObject.transform.SetParent(_questionObject.GetChild(0).GetChild(0).GetChild((i - 1) * -1));

                    newGameObject.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
                    newGameObject.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);

                    newGameObject.GetComponent<RectTransform>().offsetMin = new Vector2(ModelsScript.savedCategory[(currentRoundId + 1) / 7].SaveRound[currentRoundId % 7].SaveLayouts[i].SaveImage[a].Left,
                                                                                        ModelsScript.savedCategory[(currentRoundId + 1) / 7].SaveRound[currentRoundId % 7].SaveLayouts[i].SaveImage[a].Bottom);
                    newGameObject.GetComponent<RectTransform>().offsetMax = new Vector2(ModelsScript.savedCategory[(currentRoundId + 1) / 7].SaveRound[currentRoundId % 7].SaveLayouts[i].SaveImage[a].Right,
                                                                                        ModelsScript.savedCategory[(currentRoundId + 1) / 7].SaveRound[currentRoundId % 7].SaveLayouts[i].SaveImage[a].Top);

                    newGameObject.transform.localScale = new Vector3(1f, 1f, 1f);
                }
                for (int a = 0; a < ModelsScript.savedCategory[(currentRoundId + 1) / 7].SaveRound[currentRoundId % 7].SaveLayouts[i].SaveAudio.Count; a++)
                {
                    var newGameObject = new GameObject();
                    newGameObject.AddComponent<AudioSource>().playOnAwake = false;

                    if (File.Exists(ModelsScript.savedCategory[(currentRoundId + 1) / 7].SaveRound[currentRoundId % 7].SaveLayouts[i].SaveAudio[a].PathToAudio))
                    {
                        StartCoroutine(LoadAudio(ModelsScript.savedCategory[(currentRoundId + 1) / 7].SaveRound[currentRoundId % 7].SaveLayouts[i].SaveAudio[a].PathToAudio, newGameObject));
                    }

                    newGameObject.transform.SetParent(_questionObject.GetChild(0).GetChild(0).GetChild((i - 1) * -1));
                }
                for (int a = 0; a < ModelsScript.savedCategory[(currentRoundId + 1) / 7].SaveRound[currentRoundId % 7].SaveLayouts[i].SaveVideo.Count; a++)
                {
                    var newGameObject = new GameObject();
                    newGameObject.AddComponent<RectTransform>();
                    newGameObject.AddComponent<VideoPlayer>().source = VideoSource.Url;
                    newGameObject.GetComponent<VideoPlayer>().targetTexture = _renderTexture;
                    newGameObject.GetComponent<VideoPlayer>().playOnAwake = false;

                    if (File.Exists(ModelsScript.savedCategory[(currentRoundId + 1) / 7].SaveRound[currentRoundId % 7].SaveLayouts[i].SaveVideo[a].PathToVideo))
                    {
                        newGameObject.GetComponent<VideoPlayer>().url = ModelsScript.savedCategory[(currentRoundId + 1) / 7].SaveRound[currentRoundId % 7].SaveLayouts[i].SaveVideo[a].PathToVideo;
                    }

                    newGameObject.transform.SetParent(_questionObject.GetChild(0).GetChild(0).GetChild((i - 1) * -1));

                    newGameObject.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
                    newGameObject.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);

                    newGameObject.GetComponent<RectTransform>().offsetMin = new Vector2(ModelsScript.savedCategory[(currentRoundId + 1) / 7].SaveRound[currentRoundId % 7].SaveLayouts[i].SaveVideo[a].Left,
                                                                                        ModelsScript.savedCategory[(currentRoundId + 1) / 7].SaveRound[currentRoundId % 7].SaveLayouts[i].SaveVideo[a].Bottom);
                    newGameObject.GetComponent<RectTransform>().offsetMax = new Vector2(ModelsScript.savedCategory[(currentRoundId + 1) / 7].SaveRound[currentRoundId % 7].SaveLayouts[i].SaveVideo[a].Right,
                                                                                        ModelsScript.savedCategory[(currentRoundId + 1) / 7].SaveRound[currentRoundId % 7].SaveLayouts[i].SaveVideo[a].Top);

                    newGameObject.transform.localScale = new Vector3(1f, 1f, 1f);

                    var videoObject = new GameObject();
                    videoObject.AddComponent<RectTransform>();
                    videoObject.AddComponent<CanvasRenderer>();
                    videoObject.AddComponent<RawImage>().texture = _renderTexture;

                    videoObject.transform.SetParent(newGameObject.transform);

                    videoObject.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
                    videoObject.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);

                    videoObject.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
                    videoObject.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);

                    videoObject.transform.localScale = new Vector3(1f, 1f, 1f);
                }
            }

            _questionObject.gameObject.SetActive(true);

            ModelsScript.mainMusic.Stop();

            if (!(_questionObject.GetChild(0).GetChild(0).GetChild(1).gameObject.GetComponentsInChildren<AudioSource>().Length > 0 ||
                _questionObject.GetChild(0).GetChild(0).GetChild(1).gameObject.GetComponentsInChildren<VideoPlayer>().Length > 0))
            {
                ModelsScript.supportMusic.clip = ModelsScript.allClips[3];
                ModelsScript.supportMusic.Play();
            }

            if (currentRoundId != 0 && (currentRoundId + 2) % 7 == 0)
            {
                btn.gameObject.SetActive(false);
                ModelsScript.allBtns[currentRoundId + 1].gameObject.SetActive(true);
            }

            btn.enabled = false;
            var tmp = btn.GetComponent<Image>().color;
            tmp.a = 0;
            btn.GetComponent<Image>().color = tmp;
            btn.GetComponentInChildren<Text>().text = string.Empty;

            if (currentRoundId == 0 || (currentRoundId + 1) % 7 != 0)
            {
                _countQuestionEnd++;
            }
        }
    }

    [Obsolete]
    private IEnumerator LoadAudio(string path, GameObject currentGameObject)
    {
        WWW request = new WWW(path);
        yield return request;

        AudioClip audioClip = request.GetAudioClip();
        currentGameObject.GetComponent<AudioSource>().clip = audioClip;
    }

    public static void GiveOutPrize(int commandId)
    {
        if (ModelsScript.Players[commandId].ProgressBattlePass >= 2)
        {
            if (ModelsScript.Players[commandId].ProgressBattlePass >= 2 && ModelsScript.Players[commandId].ProgressBattlePass < 4)
            {
                ModelsScript.Players[commandId].Items.Add(ModelsScript.Players[commandId].BattlePassItems[0]);
            }
            else if (ModelsScript.Players[commandId].ProgressBattlePass >= 4 && ModelsScript.Players[commandId].ProgressBattlePass < 6)
            {
                ModelsScript.Players[commandId].Items.Add(ModelsScript.Players[commandId].BattlePassItems[1]);
            }
            else if (ModelsScript.Players[commandId].ProgressBattlePass >= 6 && ModelsScript.Players[commandId].ProgressBattlePass < 7)
            {
                ModelsScript.Players[commandId].Items.Add(ModelsScript.Players[commandId].BattlePassItems[2]);
            }
            else if (ModelsScript.Players[commandId].ProgressBattlePass >= 7)
            {
                ModelsScript.Players[commandId].Items.Add(ModelsScript.Players[commandId].BattlePassItems[3]);
            }
            ModelsScript.Players[commandId].ProgressBattlePass = 0;
            ModelsScript.needUpdateCommandId = commandId;
            GenerateNewBattlePass(commandId);
        }
    }

    public void SetEnemyOnSword(int enemyId)
    {
        if (ModelsScript.giveQuestion)
        {
            ModelsScript.giveQuestion = false;
            ModelsScript.currentCommandIdResponds = enemyId;
        }
        else if (ModelsScript.attack[0] == 1)
        {
            if (ModelsScript.Players[enemyId].WearingArmor >= 1)
            {
                ModelsScript.attack[1] = 0;
            }
            else
            {
                ModelsScript.attack[2] = enemyId;

                ModelsScript.supportMusic.clip = ModelsScript.allClips[0];
                ModelsScript.supportMusic.Play();

                _wheelFortune.gameObject.SetActive(true);
            }

            ModelsScript.attack[0] = 0;
        }
    }

    private static void GenerateNewBattlePass(int commandId)
    {
        ModelsScript.Players[commandId].BattlePassItems = new List<ShopItem>
        {
            (ShopItem)UnityEngine.Random.Range(0,3),
            (ShopItem)UnityEngine.Random.Range(3,5),
            (ShopItem)UnityEngine.Random.Range(5,9),
            (ShopItem)UnityEngine.Random.Range(9,12)
        };
        ModelsScript.needUpdateCommandId = commandId;
    }

    private void UpdateStatistic(int commandId)
    {
        _allCommandStatistics[commandId].score.text = ModelsScript.Players[commandId].Score.ToString();
        _allCommandStatistics[commandId].scoreShop.text = ModelsScript.Players[commandId].ShopScore.ToString();

        for (int i = 0; i < _allCommandStatistics[commandId].allItems.childCount; i++)
        {
            _allCommandStatistics[commandId].allItems.GetChild(i).GetChild(1).GetComponent<Text>().text = ModelsScript.Players[commandId].Items.Count(item => item == (ShopItem)i).ToString();
        }

        _allCommandStatistics[commandId].battlePassSlider.value = 0.142857f * ModelsScript.Players[commandId].ProgressBattlePass;

        for (int i = 0; i < _allCommandStatistics[commandId].battlePassItemsObject.childCount; i++)
        {
            var shopItemId = (int)Enum.Parse(typeof(ShopItem), ModelsScript.Players[commandId].BattlePassItems[i].ToString());
            _allCommandStatistics[commandId].battlePassItemsObject.GetChild(i).GetComponent<Image>().sprite = _allItemsSprites[shopItemId];
        }
    }

    public void UpdateAllStatistic()
    {
        for (int a = 0; a < 3; a++)
        {
            _allCommandStatistics[a].score.text = ModelsScript.Players[a].Score.ToString();
            _allCommandStatistics[a].scoreShop.text = ModelsScript.Players[a].ShopScore.ToString();

            for (int i = 0; i < _allCommandStatistics[a].allItems.childCount; i++)
            {
                _allCommandStatistics[a].allItems.GetChild(i).GetChild(1).GetComponent<Text>().text = ModelsScript.Players[a].Items.Count(item => item == (ShopItem)i).ToString();
            }

            _allCommandStatistics[a].battlePassSlider.value = 0.142857f * ModelsScript.Players[a].ProgressBattlePass;

            for (int i = 0; i < _allCommandStatistics[a].battlePassItemsObject.childCount; i++)
            {
                var shopItemId = (int)Enum.Parse(typeof(ShopItem), ModelsScript.Players[a].BattlePassItems[i].ToString());
                _allCommandStatistics[a].battlePassItemsObject.GetChild(i).GetComponent<Image>().sprite = _allItemsSprites[shopItemId];
            }
        }
    }

    private IEnumerator StartAnimation(int numberAnimation)
    {
        _startAnimations[numberAnimation].SetTrigger("StartButtonsAnim");

        yield return new WaitForSeconds(9.9f);

        _startAnimations[numberAnimation + 1].SetTrigger("StartHeadersAnim");
    }
}

[System.Serializable]
public class StatisticCommand
{
    public TextMeshProUGUI score;
    public TextMeshProUGUI scoreShop;
    public Transform allItems;
    public Slider battlePassSlider;
    public Transform battlePassItemsObject;
}