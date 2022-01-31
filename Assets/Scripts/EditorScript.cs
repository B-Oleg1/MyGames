using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class EditorScript : MonoBehaviour
{
    [SerializeField]
    private Transform _allSavedGamesObject;

    [SerializeField]
    private Transform _allCategoryObject;

    [SerializeField]
    private Transform _editObject;

    [SerializeField]
    private GameObject _savingImage;

    [SerializeField]
    private RenderTexture _renderTexture;

    [SerializeField]
    private Sprite[] _controlMediaSprites;

    [SerializeField]
    private Sprite _whiteSprite;

    private SaveCategory[] _savedCategory = new SaveCategory[16];

    private GameObject _inputField;
    private GameObject _textInputField;
    private GameObject _placeholderInputField;

    private Vector2 _startDragMousePos;

    private GameObject _currentEditObject;
    private long _currentIdObject;

    private string _fileName = string.Empty;

    private int _currentRoundId = 0;
    private int _currentQuestionOrAnswer = 0;

    private string[] _directions = { "LeftBottom", "RightBottom", "LeftTop", "RightTop" };

    private string[] _randomWords = { "Approach", "Establish", "Attorney", "Tease", "Will", "Fortitude", "Torn", "Hero", "Catering", "Carefully",
        "Signal", "Totebook", "Orientalist", "Cable", "Distance", "Echo", "Hair", "Most", "Zombie", "Up" };

    private void Start()
    {
        LoadAllSaves();

        _textInputField = new GameObject();
        _textInputField.AddComponent<Text>();

        _placeholderInputField = new GameObject();
        _placeholderInputField.AddComponent<Text>();

        _inputField = new GameObject();
        _inputField.AddComponent<InputField>().textComponent = _textInputField.GetComponent<Text>();
        _inputField.GetComponent<InputField>().placeholder = _placeholderInputField.GetComponent<Text>();

        _textInputField.hideFlags = HideFlags.HideInHierarchy;
        _placeholderInputField.hideFlags = HideFlags.HideInHierarchy;
        _inputField.hideFlags = HideFlags.HideInHierarchy;
    }

    public void AddNewSave()
    {
        string randomName = _randomWords[UnityEngine.Random.Range(0, _randomWords.Length)] + _randomWords[UnityEngine.Random.Range(0, _randomWords.Length)];

        for (int i = 0; i < _savedCategory.Length; i++)
        {
            _savedCategory[i] = new SaveCategory();
            _savedCategory[i].Name = "Название категории";

            _savedCategory[i].SaveRound = new SaveRound[7];
            for (int a = 0; a < _savedCategory[i].SaveRound.Length; a++)
            {
                _savedCategory[i].SaveRound[a] = new SaveRound();
                _savedCategory[i].SaveRound[a].SaveLayouts = new SaveLayout[2];
                for (int b = 0; b < _savedCategory[i].SaveRound[a].SaveLayouts.Length; b++)
                {
                    _savedCategory[i].SaveRound[a].SaveLayouts[b] = new SaveLayout();
                    _savedCategory[i].SaveRound[a].SaveLayouts[b].SaveText = new List<SaveText>();
                    _savedCategory[i].SaveRound[a].SaveLayouts[b].SaveImage = new List<SaveImage>();
                    _savedCategory[i].SaveRound[a].SaveLayouts[b].SaveAudio = new List<SaveAudio>();
                    _savedCategory[i].SaveRound[a].SaveLayouts[b].SaveVideo = new List<SaveVideo>();
                }
            }
        }

        var json = JsonConvert.SerializeObject(_savedCategory);
        using (FileStream fileStream = new FileStream($"{Application.dataPath}/Saves/{randomName}.json", FileMode.Create))
        {
            using (StreamWriter sw = new StreamWriter(fileStream))
            {
                sw.WriteLine(json);
            }
        }

        LoadAllSaves();
    }

    public void LoadAllSaves()
    {
        for (int i = _allSavedGamesObject.GetChild(3).GetChild(0).childCount - 1; i >= 0; i--)
        {
            Destroy(_allSavedGamesObject.GetChild(3).GetChild(0).GetChild(i).gameObject);
        }

        if (!Directory.Exists(Application.dataPath + "/Saves"))
        {
            Directory.CreateDirectory(Application.dataPath + "/Saves");
        }
        string[] allFiles = Directory.GetFiles(Application.dataPath + "/Saves", "*.json");

        for (int i = 0; i < allFiles.Length; i++)
        {
            var button = Instantiate(Resources.Load<Button>("SavedConfButton"), _allSavedGamesObject.GetChild(3).GetChild(0));

            button.transform.GetChild(0).GetComponent<Text>().text = allFiles[i].Split('\\').Last().Split('.')[0];
            button.onClick.AddListener(() =>
            {
                string json = string.Empty;
                using (FileStream fileStream = new FileStream($"{Application.dataPath}/Saves/{button.transform.GetChild(0).GetComponent<Text>().text}.json", FileMode.Open))
                {
                    using (StreamReader sw = new StreamReader(fileStream))
                    {
                        json = sw.ReadToEnd();
                    }
                }
                _savedCategory = JsonConvert.DeserializeObject<SaveCategory[]>(json);

                _fileName = $"{button.transform.GetChild(0).GetComponent<Text>().text}.json";

                EditCategory();
            });
        }
    }

    public void EditCategory()
    {
        for (int i = 0; i < _savedCategory.Length; i++)
        {
            _allCategoryObject.GetChild(0).GetChild(0).GetChild(i).GetChild(0).GetComponent<InputField>().text = _savedCategory[i].Name;
        }

        _allCategoryObject.parent.gameObject.SetActive(true);
        _allCategoryObject.gameObject.SetActive(true);
        _editObject.gameObject.SetActive(false);
    }

    public void SaveGame()
    {
        _savingImage.SetActive(true);

        var json = JsonConvert.SerializeObject(_savedCategory);
        using (StreamWriter sw = new StreamWriter($"{Application.dataPath}/Saves/{_fileName}", false, Encoding.UTF8))
        {
            sw.Write(json);
        }

        _savingImage.SetActive(false);
    }

    public void OnClickEditButton(int roundId)
    {
        _currentRoundId = roundId;

        _currentQuestionOrAnswer = 0;
        _editObject.GetChild(0).GetChild(_currentQuestionOrAnswer).gameObject.SetActive(true);
        _editObject.GetChild(0).GetChild((_currentQuestionOrAnswer - 1) * -1).gameObject.SetActive(false);
        _editObject.GetChild(1).GetChild(_currentQuestionOrAnswer).gameObject.SetActive(true);
        _editObject.GetChild(1).GetChild((_currentQuestionOrAnswer - 1) * -1).gameObject.SetActive(false);

        _editObject.gameObject.SetActive(true);

        // TODO: Сделать загрузку всех объектов из класса
        for (int i = 0; i < _savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts.Length; i++)
        {
            for (int a = 0; a < _savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts[i].SaveText.Count; a++)
            {
                var newGameObject = new GameObject();
                newGameObject.AddComponent<Text>();
                newGameObject.GetComponent<Text>().text = _savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts[i].SaveText[a].Text;
                newGameObject.GetComponent<Text>().font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
                newGameObject.GetComponent<Text>().fontSize = _savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts[i].SaveText[a].FontSize;
                newGameObject.GetComponent<Text>().color = new Color(_savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts[i].SaveText[a].RedColor,
                                                                     _savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts[i].SaveText[a].GreenColor,
                                                                     _savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts[i].SaveText[a].BlueColor,
                                                                     _savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts[i].SaveText[a].AlphaColor);
                if (_savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts[i].SaveText[a].AlignmentText == "Left")
                {
                    newGameObject.GetComponent<Text>().alignment = TextAnchor.UpperLeft;
                }
                else if (_savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts[i].SaveText[a].AlignmentText == "Center")
                {
                    newGameObject.GetComponent<Text>().alignment = TextAnchor.UpperCenter;
                }
                else if (_savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts[i].SaveText[a].AlignmentText == "Right")
                {
                    newGameObject.GetComponent<Text>().alignment = TextAnchor.UpperRight;
                }

                newGameObject.transform.SetParent(_editObject.GetChild(0).GetChild(i));

                newGameObject.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
                newGameObject.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);

                newGameObject.GetComponent<RectTransform>().offsetMin = new Vector2(_savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts[i].SaveText[a].Left,
                                                                                    _savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts[i].SaveText[a].Bottom);
                newGameObject.GetComponent<RectTransform>().offsetMax = new Vector2(_savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts[i].SaveText[a].Right,
                                                                                    _savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts[i].SaveText[a].Top);

                newGameObject.transform.localScale = new Vector3(1f, 1f, 1f);

                AddUIOnNewObject(newGameObject, _savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts[i].SaveText[a].Id, i);
            }
            for (int a = 0; a < _savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts[i].SaveImage.Count; a++)
            {
                var newGameObject = new GameObject();
                newGameObject.AddComponent<Image>();

                if (File.Exists(_savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts[i].SaveImage[a].PathToImage))
                {
                    FileStream imgFile = new FileStream(_savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts[i].SaveImage[a].PathToImage, FileMode.Open);

                    byte[] imgByte = new byte[imgFile.Length];

                    imgFile.Read(imgByte, 0, imgByte.Length);

                    imgFile.Close();

                    Texture2D texture2d = new Texture2D(1920, 1080);
                    texture2d.LoadImage(imgByte);

                    Sprite spr = Sprite.Create(texture2d, new Rect(0, 0, texture2d.width, texture2d.height), Vector2.zero);
                    newGameObject.GetComponent<Image>().sprite = spr;
                }

                newGameObject.transform.SetParent(_editObject.GetChild(0).GetChild(i));

                newGameObject.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
                newGameObject.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);

                newGameObject.GetComponent<RectTransform>().offsetMin = new Vector2(_savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts[i].SaveImage[a].Left,
                                                                                    _savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts[i].SaveImage[a].Bottom);
                newGameObject.GetComponent<RectTransform>().offsetMax = new Vector2(_savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts[i].SaveImage[a].Right,
                                                                                    _savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts[i].SaveImage[a].Top);

                newGameObject.transform.localScale = new Vector3(1f, 1f, 1f);

                AddUIOnNewObject(newGameObject, _savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts[i].SaveImage[a].Id, i);
            }
            for (int a = 0; a < _savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts[i].SaveAudio.Count; a++)
            {
                var newGameObject = new GameObject();
                newGameObject.AddComponent<AudioSource>();

                if (File.Exists(_savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts[i].SaveAudio[a].PathToAudio))
                {
                    StartCoroutine(LoadAudio(_savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts[i].SaveAudio[a].PathToAudio, newGameObject));
                }

                newGameObject.transform.SetParent(_editObject.GetChild(0).GetChild(i));

                AddUIOnNewObject(newGameObject, _savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts[i].SaveAudio[a].Id, i);
            }
            for (int a = 0; a < _savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts[i].SaveVideo.Count; a++)
            {
                var newGameObject = new GameObject();
                newGameObject.AddComponent<RectTransform>();
                newGameObject.AddComponent<VideoPlayer>().source = VideoSource.Url;
                newGameObject.GetComponent<VideoPlayer>().targetTexture = _renderTexture;

                if (File.Exists(_savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts[i].SaveVideo[a].PathToVideo))
                {
                    newGameObject.GetComponent<VideoPlayer>().url = _savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts[i].SaveVideo[a].PathToVideo;
                    newGameObject.GetComponent<VideoPlayer>().Stop();
                }

                newGameObject.transform.SetParent(_editObject.GetChild(0).GetChild(i));

                newGameObject.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
                newGameObject.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);

                newGameObject.GetComponent<RectTransform>().offsetMin = new Vector2(_savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts[i].SaveVideo[a].Left,
                                                                                    _savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts[i].SaveVideo[a].Bottom);
                newGameObject.GetComponent<RectTransform>().offsetMax = new Vector2(_savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts[i].SaveVideo[a].Right,
                                                                                    _savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts[i].SaveVideo[a].Top);

                newGameObject.transform.localScale = new Vector3(1f, 1f, 1f);

                AddUIOnNewObject(newGameObject, _savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts[i].SaveVideo[a].Id, i);
            }
        }
    }

    public void OnChangeQuestionOrAnswer()
    {
        _editObject.GetChild(1).GetChild(2).GetChild(0).gameObject.SetActive(false);
        _editObject.GetChild(1).GetChild(2).GetChild(1).gameObject.SetActive(false);
        _editObject.GetChild(1).GetChild(2).GetChild(2).gameObject.SetActive(false);
        _editObject.GetChild(1).GetChild(2).GetChild(3).gameObject.SetActive(false);

        _editObject.GetChild(0).GetChild(_currentQuestionOrAnswer).gameObject.SetActive(false);
        _editObject.GetChild(1).GetChild(_currentQuestionOrAnswer).gameObject.SetActive(false);

        if (_currentQuestionOrAnswer == 0)
        {
            _currentQuestionOrAnswer = 1;
        }
        else
        {
            _currentQuestionOrAnswer = 0;
        }

        _editObject.GetChild(0).GetChild(_currentQuestionOrAnswer).gameObject.SetActive(true);
        _editObject.GetChild(1).GetChild(_currentQuestionOrAnswer).gameObject.SetActive(true);
    }

    public void AddText()
    {
        long objectId = DateTime.Now.Ticks;

        var newGameObject = new GameObject();
        newGameObject.AddComponent<Text>();
        newGameObject.GetComponent<Text>().text = "Text";
        newGameObject.GetComponent<Text>().font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
        newGameObject.GetComponent<Text>().color = new Color(1, 1, 1, 1);

        newGameObject.transform.SetParent(_editObject.GetChild(0).GetChild(_currentQuestionOrAnswer));

        newGameObject.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
        newGameObject.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);

        newGameObject.GetComponent<RectTransform>().offsetMin = new Vector2(150f, 150f);
        newGameObject.GetComponent<RectTransform>().offsetMax = new Vector2(-150f, -150f);

        newGameObject.transform.localScale = new Vector3(1f, 1f, 1f);

        _savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts[_currentQuestionOrAnswer].SaveText.Add(new SaveText()
        {
            Id = objectId,
            Text = "Text",
            FontSize = 14,
            RedColor = 1,
            GreenColor = 1,
            BlueColor = 1,
            AlphaColor = 1,
            AlignmentText = "Left",
            Left = 150,
            Right = -150,
            Top = -150,
            Bottom = 150
        });

        AddUIOnNewObject(newGameObject, objectId, _currentQuestionOrAnswer);
    }

    public void AddImage()
    {
        long objectId = DateTime.Now.Ticks;

        var newGameObject = new GameObject();
        newGameObject.AddComponent<Image>();

        newGameObject.transform.SetParent(_editObject.GetChild(0).GetChild(_currentQuestionOrAnswer));

        newGameObject.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
        newGameObject.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);

        newGameObject.GetComponent<RectTransform>().offsetMin = new Vector2(150f, 150f);
        newGameObject.GetComponent<RectTransform>().offsetMax = new Vector2(-150f, -150f);

        newGameObject.transform.localScale = new Vector3(1f, 1f, 1f);

        _savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts[_currentQuestionOrAnswer].SaveImage.Add(new SaveImage()
        {
            Id = objectId,
            PathToImage = string.Empty,
            Left = 150,
            Right = -150,
            Top = -150,
            Bottom = 150
        });

        AddUIOnNewObject(newGameObject, objectId, _currentQuestionOrAnswer);
    }

    public void AddAudio()
    {
        long objectId = DateTime.Now.Ticks;

        var newGameObject = new GameObject();
        newGameObject.AddComponent<AudioSource>();

        newGameObject.transform.SetParent(_editObject.GetChild(0).GetChild(_currentQuestionOrAnswer));

        _savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts[_currentQuestionOrAnswer].SaveAudio.Add(new SaveAudio()
        {
            Id = objectId,
            PathToAudio = string.Empty,
            Volume = 1
        });

        AddUIOnNewObject(newGameObject, objectId, _currentQuestionOrAnswer);
    }

    public void AddVideo()
    {
        long objectId = DateTime.Now.Ticks;

        var newGameObject = new GameObject();
        newGameObject.AddComponent<RectTransform>();
        newGameObject.AddComponent<VideoPlayer>().source = VideoSource.Url;
        newGameObject.GetComponent<VideoPlayer>().targetTexture = _renderTexture;

        newGameObject.transform.SetParent(_editObject.GetChild(0).GetChild(_currentQuestionOrAnswer));

        newGameObject.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
        newGameObject.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);

        newGameObject.GetComponent<RectTransform>().offsetMin = new Vector2(150, 150);
        newGameObject.GetComponent<RectTransform>().offsetMax = new Vector2(-150, -150);

        newGameObject.transform.localScale = new Vector3(1f, 1f, 1f);

        _savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts[_currentQuestionOrAnswer].SaveVideo.Add(new SaveVideo()
        {
            Id = objectId,
            PathToVideo = string.Empty,
            Left = 150,
            Right = -150,
            Top = -150,
            Bottom = 150,
            Volume = 1
        });

        AddUIOnNewObject(newGameObject, objectId, _currentQuestionOrAnswer);
    }

    private void AddUIOnNewObject(GameObject newGameObject, long objectId, int questOrAsnw)
    {
        string nameButton = string.Empty;

        // Add scalability buttons
        if (!newGameObject.GetComponent<AudioSource>())
        {
            newGameObject.AddComponent<EventTrigger>();

            EventTrigger.Entry startDrag = new EventTrigger.Entry();
            EventTrigger.Entry drag = new EventTrigger.Entry();

            startDrag.eventID = EventTriggerType.BeginDrag;
            startDrag.callback.AddListener((data) => _startDragMousePos = Input.mousePosition);

            drag.eventID = EventTriggerType.Drag;
            drag.callback.AddListener((data) => MoveObject((PointerEventData)data, Input.mousePosition));

            newGameObject.GetComponent<EventTrigger>().triggers.Add(startDrag);
            newGameObject.GetComponent<EventTrigger>().triggers.Add(drag);

            newGameObject.GetComponent<EventTrigger>().enabled = false;

            for (int i = 0; i < 4; i++)
            {
                GameObject resizingButton = new GameObject();
                resizingButton.AddComponent<RectTransform>();
                resizingButton.AddComponent<Image>();
                resizingButton.AddComponent<Button>();
                resizingButton.AddComponent<EventTrigger>();

                resizingButton.transform.SetParent(newGameObject.transform);

                resizingButton.GetComponent<RectTransform>().anchorMin = new Vector2(i % 2, i / 2);
                resizingButton.GetComponent<RectTransform>().anchorMax = new Vector2(i % 2, i / 2);

                resizingButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
                resizingButton.GetComponent<RectTransform>().sizeDelta = new Vector2(15, 15);

                resizingButton.transform.localScale = new Vector3(1f, 1f, 1f);

                resizingButton.SetActive(false);

                EventTrigger.Entry entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.Drag;

                string nameDir = _directions[i];
                entry.callback.RemoveAllListeners();
                entry.callback.AddListener((data) => OnMoveButton((PointerEventData)data, nameDir));
                resizingButton.GetComponent<EventTrigger>().triggers.Add(entry);
            }
        }

        if (newGameObject.GetComponent<Text>())
        {
            nameButton = "Text";

            // Add outline for text object
            GameObject strokeForText = new GameObject("StrokeForTextObject");

            strokeForText.AddComponent<Image>();
            strokeForText.GetComponent<Image>().sprite = _whiteSprite;
            strokeForText.GetComponent<Image>().type = Image.Type.Sliced;
            strokeForText.GetComponent<Image>().fillCenter = false;
            strokeForText.GetComponent<Image>().pixelsPerUnitMultiplier = 3;

            strokeForText.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
            strokeForText.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);

            strokeForText.transform.SetParent(newGameObject.transform);

            strokeForText.transform.localScale = new Vector3(1, 1, 1);

            strokeForText.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
            strokeForText.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);
        }
        else if(newGameObject.GetComponent<Image>())
        {
            nameButton = "Image";
        }
        else if (newGameObject.GetComponent<AudioSource>())
        {
            nameButton = "Audio";
        }
        else if (newGameObject.GetComponent<VideoPlayer>())
        {
            nameButton = "Video";

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

        var newElement = Instantiate(Resources.Load<GameObject>("NewElementObject"), _editObject.GetChild(1).GetChild(questOrAsnw).GetChild(0).GetChild(0));

        var button = newElement.transform.GetChild(0).GetComponent<Button>();
        button.transform.GetChild(0).GetComponent<Text>().text = $"{nameButton}#{(char)UnityEngine.Random.Range(65, 91)}{(char)UnityEngine.Random.Range(49, 58)}";
        button.onClick.AddListener(() =>
        {
            // Hide buttons for resizing
            if (_currentEditObject != null && !_currentEditObject.GetComponent<AudioSource>())
            {
                _currentEditObject.GetComponent<EventTrigger>().enabled = false;
                for (int i = 0; i < 4; i++)
                {
                    _currentEditObject.transform.GetChild(i).gameObject.SetActive(false);
                }
            }

            _currentEditObject = newGameObject;
            _currentIdObject = objectId;

            _editObject.GetChild(1).GetChild(2).GetChild(0).gameObject.SetActive(false);
            _editObject.GetChild(1).GetChild(2).GetChild(1).gameObject.SetActive(false);
            _editObject.GetChild(1).GetChild(2).GetChild(2).gameObject.SetActive(false);
            _editObject.GetChild(1).GetChild(2).GetChild(3).gameObject.SetActive(false);

            // Show button for resizing
            if (!_currentEditObject.GetComponent<AudioSource>())
            {
                newGameObject.GetComponent<EventTrigger>().enabled = true;

                for (int i = 0; i < 4; i++)
                {
                    newGameObject.transform.GetChild(i).gameObject.SetActive(true);
                }
            }

            // Update text with object prop
            if (newGameObject.GetComponent<Text>())
            {
                _editObject.GetChild(1).GetChild(2).GetChild(0).gameObject.SetActive(true);

                print(Screen.width + " " + Screen.height);

                _editObject.GetChild(1).GetChild(2).GetChild(0).GetChild(0).GetComponent<InputField>().text = Convert.ToInt32(newGameObject.GetComponent<RectTransform>().offsetMin.x * (Screen.width / 1920f)).ToString();
                _editObject.GetChild(1).GetChild(2).GetChild(0).GetChild(1).GetComponent<InputField>().text = Convert.ToInt32(newGameObject.GetComponent<RectTransform>().offsetMax.x * (Screen.width / 1920f) * -1).ToString();
                _editObject.GetChild(1).GetChild(2).GetChild(0).GetChild(2).GetComponent<InputField>().text = Convert.ToInt32(newGameObject.GetComponent<RectTransform>().offsetMax.y * (Screen.height / 1080f) * -1).ToString();
                _editObject.GetChild(1).GetChild(2).GetChild(0).GetChild(3).GetComponent<InputField>().text = Convert.ToInt32(newGameObject.GetComponent<RectTransform>().offsetMin.y * (Screen.height / 1080f)).ToString();
                _editObject.GetChild(1).GetChild(2).GetChild(0).GetChild(4).GetComponent<InputField>().text = newGameObject.GetComponent<Text>().text;
                _editObject.GetChild(1).GetChild(2).GetChild(0).GetChild(5).GetComponent<InputField>().text = newGameObject.GetComponent<Text>().fontSize.ToString();
                _editObject.GetChild(1).GetChild(2).GetChild(0).GetChild(6).GetComponent<Slider>().value = newGameObject.GetComponent<Text>().color.r;
                _editObject.GetChild(1).GetChild(2).GetChild(0).GetChild(7).GetComponent<Slider>().value = newGameObject.GetComponent<Text>().color.g;
                _editObject.GetChild(1).GetChild(2).GetChild(0).GetChild(8).GetComponent<Slider>().value = newGameObject.GetComponent<Text>().color.b;
                _editObject.GetChild(1).GetChild(2).GetChild(0).GetChild(9).GetComponent<Slider>().value = newGameObject.GetComponent<Text>().color.a;
            }
            else if (newGameObject.GetComponent<Image>())
            {
                _editObject.GetChild(1).GetChild(2).GetChild(1).gameObject.SetActive(true);
                
                _editObject.GetChild(1).GetChild(2).GetChild(1).GetChild(0).GetComponent<InputField>().text = Convert.ToInt32(newGameObject.GetComponent<RectTransform>().offsetMin.x * (Screen.width / 1920f)).ToString();
                _editObject.GetChild(1).GetChild(2).GetChild(1).GetChild(1).GetComponent<InputField>().text = Convert.ToInt32(newGameObject.GetComponent<RectTransform>().offsetMax.x * (Screen.width / 1920f) * -1).ToString();
                _editObject.GetChild(1).GetChild(2).GetChild(1).GetChild(2).GetComponent<InputField>().text = Convert.ToInt32(newGameObject.GetComponent<RectTransform>().offsetMax.y * (Screen.height / 1080f) * -1).ToString();
                _editObject.GetChild(1).GetChild(2).GetChild(1).GetChild(3).GetComponent<InputField>().text = Convert.ToInt32(newGameObject.GetComponent<RectTransform>().offsetMin.y * (Screen.height / 1080f)).ToString();
                _editObject.GetChild(1).GetChild(2).GetChild(1).GetChild(4).GetComponent<InputField>().text = _savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7]
                                                                                                                                .SaveLayouts[_currentQuestionOrAnswer].SaveImage
                                                                                                                                .FirstOrDefault(item => item.Id == objectId)
                                                                                                                                .PathToImage;
            }
            else if (newGameObject.GetComponent<AudioSource>())
            {
                _editObject.GetChild(1).GetChild(2).GetChild(2).gameObject.SetActive(true);

                _editObject.GetChild(1).GetChild(2).GetChild(2).GetChild(0).GetComponent<InputField>().text = _savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7]
                                                                                                                            .SaveLayouts[_currentQuestionOrAnswer].SaveAudio
                                                                                                                            .FirstOrDefault(item => item.Id == objectId)
                                                                                                                            .PathToAudio;
                _editObject.GetChild(1).GetChild(2).GetChild(2).GetChild(1).GetComponent<Slider>().value = _savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7]
                                                                                                                            .SaveLayouts[_currentQuestionOrAnswer].SaveAudio
                                                                                                                            .FirstOrDefault(item => item.Id == objectId)
                                                                                                                            .Volume;
            }
            else if (newGameObject.GetComponent<VideoPlayer>())
            {
                _editObject.GetChild(1).GetChild(2).GetChild(3).gameObject.SetActive(true);
                
                _editObject.GetChild(1).GetChild(2).GetChild(3).GetChild(0).GetComponent<InputField>().text = Convert.ToInt32(newGameObject.GetComponent<RectTransform>().offsetMin.x * (Screen.width / 1920f)).ToString();
                _editObject.GetChild(1).GetChild(2).GetChild(3).GetChild(1).GetComponent<InputField>().text = Convert.ToInt32(newGameObject.GetComponent<RectTransform>().offsetMax.x * (Screen.width / 1920f) * -1).ToString();
                _editObject.GetChild(1).GetChild(2).GetChild(3).GetChild(2).GetComponent<InputField>().text = Convert.ToInt32(newGameObject.GetComponent<RectTransform>().offsetMax.y * (Screen.height / 1080f) * -1).ToString();
                _editObject.GetChild(1).GetChild(2).GetChild(3).GetChild(3).GetComponent<InputField>().text = Convert.ToInt32(newGameObject.GetComponent<RectTransform>().offsetMin.y * (Screen.height / 1080f)).ToString();
                _editObject.GetChild(1).GetChild(2).GetChild(3).GetChild(4).GetComponent<InputField>().text = _savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7]
                                                                                                                            .SaveLayouts[_currentQuestionOrAnswer].SaveVideo
                                                                                                                            .FirstOrDefault(item => item.Id == objectId)
                                                                                                                            .PathToVideo;
                _editObject.GetChild(1).GetChild(2).GetChild(3).GetChild(5).GetComponent<Slider>().value = _savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7]
                                                                                                                            .SaveLayouts[_currentQuestionOrAnswer].SaveVideo
                                                                                                                            .FirstOrDefault(item => item.Id == objectId)
                                                                                                                            .Volume;
            }
        });

        var deleteButton = newElement.transform.GetChild(1).GetComponent<Button>();
        deleteButton.onClick.AddListener(() =>
        {
            if (_currentEditObject == newGameObject)
            {
                _editObject.GetChild(1).GetChild(2).GetChild(0).gameObject.SetActive(false);
                _currentEditObject = null;
            }

            if (newGameObject.GetComponent<Text>())
            {
                var deleteItem = _savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts[questOrAsnw]
                                                                                                                    .SaveText
                                                                                                                    .FirstOrDefault(item => item.Id == objectId);
                _savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts[questOrAsnw].SaveText.Remove(deleteItem);
            }
            else if (newGameObject.GetComponent<Image>())
            {
                var deleteItem = _savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts[questOrAsnw]
                                                                                                                    .SaveImage
                                                                                                                    .FirstOrDefault(item => item.Id == objectId);
                _savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts[questOrAsnw].SaveImage.Remove(deleteItem);
            }
            else if (newGameObject.GetComponent<AudioSource>())
            {
                var deleteItem = _savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts[questOrAsnw]
                                                                                                                    .SaveAudio
                                                                                                                    .FirstOrDefault(item => item.Id == objectId);
                _savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts[questOrAsnw].SaveAudio.Remove(deleteItem);
            }
            else if (newGameObject.GetComponent<VideoPlayer>())
            {
                var deleteItem = _savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts[questOrAsnw]
                                                                                                                    .SaveVideo
                                                                                                                    .FirstOrDefault(item => item.Id == objectId);
                _savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts[questOrAsnw].SaveVideo.Remove(deleteItem);
            }

            Destroy(newGameObject);
            Destroy(newElement);
        });

        var arrowUp = newElement.transform.GetChild(2).GetComponent<Button>();
        arrowUp.onClick.AddListener(() =>
        {
            if (newGameObject.transform.GetSiblingIndex() > 0)
            {
                newGameObject.transform.SetSiblingIndex(newGameObject.transform.GetSiblingIndex() - 1);
                newElement.transform.SetSiblingIndex(newElement.transform.GetSiblingIndex() - 1);
            }
        });

        var arrowDown = newElement.transform.GetChild(3).GetComponent<Button>();
        arrowDown.onClick.AddListener(() =>
        {
            if (newGameObject.transform.GetSiblingIndex() + 1 < newGameObject.transform.parent.childCount)
            {
                newGameObject.transform.SetSiblingIndex(newGameObject.transform.GetSiblingIndex() + 1);
                newElement.transform.SetSiblingIndex(newElement.transform.GetSiblingIndex() + 1);
            }
        });
    }

    public void OnEditInputText(InputField inputField)
    {
        if (inputField.text.Length > 0)
        {
            string nameAction = inputField.placeholder.GetComponent<Text>().text;
            if (nameAction.Split(' ').Length > 1 && nameAction.Split(' ')[0] == "Category")
            {
                _savedCategory[int.Parse(nameAction.Split(' ')[1]) - 1].Name = inputField.text;
            }

            switch (nameAction)
            {
                case "Text":
                    _currentEditObject.GetComponent<Text>().text = inputField.text;
                    _savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts[_currentQuestionOrAnswer].SaveText
                                                                                                                .FirstOrDefault(item => item.Id == _currentIdObject)
                                                                                                                .Text = inputField.text;
                    break;
                case "FontSize":
                    _currentEditObject.GetComponent<Text>().fontSize = int.Parse(inputField.text);
                    _savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts[_currentQuestionOrAnswer].SaveText
                                                                                                                .FirstOrDefault(item => item.Id == _currentIdObject)
                                                                                                                .FontSize = int.Parse(inputField.text);
                    break;
                case "PathToImage":
                    FileStream imgFile = new FileStream(inputField.text, FileMode.Open);

                    byte[] imgByte = new byte[imgFile.Length];

                    imgFile.Read(imgByte, 0, imgByte.Length);

                    imgFile.Close();

                    Texture2D texture2d = new Texture2D(1920, 1080);
                    texture2d.LoadImage(imgByte);

                    Sprite spr = Sprite.Create(texture2d, new Rect(0, 0, texture2d.width, texture2d.height), Vector2.zero);
                    _currentEditObject.GetComponent<Image>().sprite = spr;

                    _savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts[_currentQuestionOrAnswer].SaveImage
                                                                                                                .FirstOrDefault(item => item.Id == _currentIdObject)
                                                                                                                .PathToImage = inputField.text;
                    break;
                case "PathToAudio":
                    StartCoroutine(LoadAudio(inputField.text, _currentEditObject));

                    _savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts[_currentQuestionOrAnswer].SaveAudio
                                                                                                                .FirstOrDefault(item => item.Id == _currentIdObject)   
                                                                                                                .PathToAudio = inputField.text;
                    break;
                case "PathToVideo":
                    _currentEditObject.GetComponent<VideoPlayer>().source = VideoSource.Url;
                    _currentEditObject.GetComponent<VideoPlayer>().url = inputField.text;
                    _currentEditObject.GetComponent<VideoPlayer>().Stop();

                    _savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts[_currentQuestionOrAnswer].SaveVideo
                                                                                                                .FirstOrDefault(item => item.Id == _currentIdObject)
                                                                                                                .PathToVideo = inputField.text;
                    break;
                case "Left":
                    var left = _currentEditObject.GetComponent<RectTransform>().offsetMin;
                    left.x = Convert.ToInt32(int.Parse(inputField.text) * (1920f / Screen.width));
                    _currentEditObject.GetComponent<RectTransform>().offsetMin = left;

                    if (_currentEditObject.GetComponent<Text>())
                    {
                        print(int.Parse(inputField.text).ToString());
                        _editObject.transform.GetChild(1).GetChild(2).GetChild(0).GetChild(0).GetComponent<InputField>().text = int.Parse(inputField.text).ToString();
                        _savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts[_currentQuestionOrAnswer].SaveText
                                                                                                                .FirstOrDefault(item => item.Id == _currentIdObject)
                                                                                                                .Left = Convert.ToInt32(int.Parse(inputField.text) * (1920f / Screen.width));
                    }
                    else if (_currentEditObject.GetComponent<Image>())
                    {
                        _editObject.transform.GetChild(1).GetChild(2).GetChild(1).GetChild(0).GetComponent<InputField>().text = int.Parse(inputField.text).ToString();
                        _savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts[_currentQuestionOrAnswer].SaveImage
                                                                                                                .FirstOrDefault(item => item.Id == _currentIdObject)
                                                                                                                .Left = Convert.ToInt32(int.Parse(inputField.text) * (1920f / Screen.width));
                    }
                    else if (_currentEditObject.GetComponent<VideoPlayer>())
                    {
                        _editObject.transform.GetChild(1).GetChild(2).GetChild(3).GetChild(0).GetComponent<InputField>().text = int.Parse(inputField.text).ToString();
                        _savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts[_currentQuestionOrAnswer].SaveVideo
                                                                                                                .FirstOrDefault(item => item.Id == _currentIdObject)
                                                                                                                .Left = Convert.ToInt32(int.Parse(inputField.text) * (1920f / Screen.width));
                    }

                    break;
                case "Right":
                    var right = _currentEditObject.GetComponent<RectTransform>().offsetMax;
                    right.x = Convert.ToInt32(int.Parse(inputField.text) * (1920f / Screen.width)) * -1;
                    _currentEditObject.GetComponent<RectTransform>().offsetMax = right;

                    if (_currentEditObject.GetComponent<Text>())
                    {
                        _editObject.transform.GetChild(1).GetChild(2).GetChild(0).GetChild(1).GetComponent<InputField>().text = int.Parse(inputField.text).ToString();
                        _savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts[_currentQuestionOrAnswer].SaveText
                                                                                                                .FirstOrDefault(item => item.Id == _currentIdObject)
                                                                                                                .Right = Convert.ToInt32(int.Parse(inputField.text) * (1920f / Screen.width)) * -1;
                    }
                    else if (_currentEditObject.GetComponent<Image>())
                    {
                        _editObject.transform.GetChild(1).GetChild(2).GetChild(1).GetChild(1).GetComponent<InputField>().text = int.Parse(inputField.text).ToString();
                        _savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts[_currentQuestionOrAnswer].SaveImage
                                                                                                                .FirstOrDefault(item => item.Id == _currentIdObject)
                                                                                                                .Right = Convert.ToInt32(int.Parse(inputField.text) * (1920f / Screen.width)) * -1;
                    }
                    else if (_currentEditObject.GetComponent<VideoPlayer>())
                    {
                        _editObject.transform.GetChild(1).GetChild(2).GetChild(3).GetChild(1).GetComponent<InputField>().text = int.Parse(inputField.text).ToString();
                        _savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts[_currentQuestionOrAnswer].SaveVideo
                                                                                                                .FirstOrDefault(item => item.Id == _currentIdObject)
                                                                                                                .Right = Convert.ToInt32(int.Parse(inputField.text) * (1920f / Screen.width)) * -1;
                    }
                    break;
                case "Top":
                    var top = _currentEditObject.GetComponent<RectTransform>().offsetMax;
                    top.y = Convert.ToInt32(int.Parse(inputField.text) * (1080f / Screen.height)) * -1;
                    _currentEditObject.GetComponent<RectTransform>().offsetMax = top;

                    if (_currentEditObject.GetComponent<Text>())
                    {
                        _editObject.transform.GetChild(1).GetChild(2).GetChild(0).GetChild(2).GetComponent<InputField>().text = int.Parse(inputField.text).ToString();
                        _savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts[_currentQuestionOrAnswer].SaveText
                                                                                                                .FirstOrDefault(item => item.Id == _currentIdObject)
                                                                                                                .Top = Convert.ToInt32(int.Parse(inputField.text) * (1080f / Screen.height)) * -1;
                    }
                    else if (_currentEditObject.GetComponent<Image>())
                    {
                        _editObject.transform.GetChild(1).GetChild(2).GetChild(1).GetChild(2).GetComponent<InputField>().text = int.Parse(inputField.text).ToString();
                        _savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts[_currentQuestionOrAnswer].SaveImage
                                                                                                                .FirstOrDefault(item => item.Id == _currentIdObject)
                                                                                                                .Top = Convert.ToInt32(int.Parse(inputField.text) * (1080f / Screen.height)) * -1;
                    }
                    else if (_currentEditObject.GetComponent<VideoPlayer>())
                    {
                        _editObject.transform.GetChild(1).GetChild(2).GetChild(3).GetChild(2).GetComponent<InputField>().text = int.Parse(inputField.text).ToString();
                        _savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts[_currentQuestionOrAnswer].SaveVideo
                                                                                                                .FirstOrDefault(item => item.Id == _currentIdObject)
                                                                                                                .Top = Convert.ToInt32(int.Parse(inputField.text) * (1080f / Screen.height)) * -1;
                    }
                    break;
                case "Bottom":
                    var bottom = _currentEditObject.GetComponent<RectTransform>().offsetMin;
                    bottom.y = Convert.ToInt32(int.Parse(inputField.text) * (1080f / Screen.height));
                    _currentEditObject.GetComponent<RectTransform>().offsetMin = bottom;

                    if (_currentEditObject.GetComponent<Text>())
                    {
                        _editObject.transform.GetChild(1).GetChild(2).GetChild(0).GetChild(3).GetComponent<InputField>().text = int.Parse(inputField.text).ToString();
                        _savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts[_currentQuestionOrAnswer].SaveText
                                                                                                                .FirstOrDefault(item => item.Id == _currentIdObject)
                                                                                                                .Bottom = Convert.ToInt32(int.Parse(inputField.text) * (1080f / Screen.height));
                    }
                    else if (_currentEditObject.GetComponent<Image>())
                    {
                        _editObject.transform.GetChild(1).GetChild(2).GetChild(1).GetChild(3).GetComponent<InputField>().text = int.Parse(inputField.text).ToString();
                        _savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts[_currentQuestionOrAnswer].SaveImage
                                                                                                                .FirstOrDefault(item => item.Id == _currentIdObject)
                                                                                                                .Bottom = Convert.ToInt32(int.Parse(inputField.text) * (1080f / Screen.height));
                    }
                    else if (_currentEditObject.GetComponent<VideoPlayer>())
                    {
                        _editObject.transform.GetChild(1).GetChild(2).GetChild(3).GetChild(3).GetComponent<InputField>().text = int.Parse(inputField.text).ToString();
                        _savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts[_currentQuestionOrAnswer].SaveVideo
                                                                                                                .FirstOrDefault(item => item.Id == _currentIdObject)
                                                                                                                .Bottom = Convert.ToInt32(int.Parse(inputField.text) * (1080f / Screen.height));
                    }
                    break;
                default:
                    break;
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

    public void OnEditSlider(Slider slider)
    {
        switch (slider.name)
        {
            case "RedSlider":
                var rColor = _currentEditObject.GetComponent<Text>().color;
                rColor.r = slider.value;
                _currentEditObject.GetComponent<Text>().color = rColor;
                _savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts[_currentQuestionOrAnswer].SaveText
                                                                                                                .FirstOrDefault(item => item.Id == _currentIdObject)
                                                                                                                .RedColor = slider.value;
                break;
            case "GreenSlider":
                var gColor = _currentEditObject.GetComponent<Text>().color;
                gColor.g = slider.value;
                _currentEditObject.GetComponent<Text>().color = gColor;
                _savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts[_currentQuestionOrAnswer].SaveText
                                                                                                                .FirstOrDefault(item => item.Id == _currentIdObject)
                                                                                                                .GreenColor = slider.value;
                break;
            case "BlueSlider":
                var bColor = _currentEditObject.GetComponent<Text>().color;
                bColor.b = slider.value;
                _currentEditObject.GetComponent<Text>().color = bColor;
                _savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts[_currentQuestionOrAnswer].SaveText
                                                                                                                .FirstOrDefault(item => item.Id == _currentIdObject)
                                                                                                                .BlueColor = slider.value;
                break;
            case "AlphaSlider":
                var aColor = _currentEditObject.GetComponent<Text>().color;
                aColor.a = slider.value;
                _currentEditObject.GetComponent<Text>().color = aColor;
                _savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts[_currentQuestionOrAnswer].SaveText
                                                                                                                .FirstOrDefault(item => item.Id == _currentIdObject)
                                                                                                                .AlphaColor = slider.value;
                break;
            case "AudioSlider":
                _currentEditObject.GetComponent<AudioSource>().volume = slider.value;
                _savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts[_currentQuestionOrAnswer].SaveAudio
                                                                                                                .FirstOrDefault(item => item.Id == _currentIdObject)
                                                                                                                .Volume = slider.value;
                break;
            case "VideoSlider":
                _currentEditObject.GetComponent<VideoPlayer>().SetDirectAudioVolume(0, slider.value);
                _savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts[_currentQuestionOrAnswer].SaveVideo
                                                                                                                .FirstOrDefault(item => item.Id == _currentIdObject)
                                                                                                                .Volume = slider.value;
                break;
            default:
                break;
        }
    }

    public void OnEditButton(string action)
    {
        switch (action)
        {
            case "LeftText":
                _currentEditObject.GetComponent<Text>().alignment = TextAnchor.UpperLeft;
                _savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts[_currentQuestionOrAnswer].SaveText
                                                                                                                .FirstOrDefault(item => item.Id == _currentIdObject)
                                                                                                                .AlignmentText = "Left";
                break;
            case "CenterText":
                _currentEditObject.GetComponent<Text>().alignment = TextAnchor.UpperCenter;
                _savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts[_currentQuestionOrAnswer].SaveText
                                                                                                                .FirstOrDefault(item => item.Id == _currentIdObject)
                                                                                                                .AlignmentText = "Center";
                break;
            case "RightText":
                _currentEditObject.GetComponent<Text>().alignment = TextAnchor.UpperRight;
                _savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts[_currentQuestionOrAnswer].SaveText
                                                                                                                .FirstOrDefault(item => item.Id == _currentIdObject)
                                                                                                                .AlignmentText = "Right";
                break;
            default:
                break;
        }
    }

    public void ControlMedia(Button button)
    {
        if (button.name == "PlayOrStopButton")
        {
            if (_currentEditObject.GetComponent<AudioSource>())
            {
                if (_currentEditObject.GetComponent<AudioSource>().isPlaying)
                {
                    _currentEditObject.GetComponent<AudioSource>().Pause();
                    button.GetComponent<Image>().sprite = _controlMediaSprites[0];
                }
                else
                {
                    _currentEditObject.GetComponent<AudioSource>().Play();
                    button.GetComponent<Image>().sprite = _controlMediaSprites[1];
                }
            }
            else if (_currentEditObject.GetComponent<VideoPlayer>())
            {
                if (_currentEditObject.GetComponent<VideoPlayer>().isPlaying)
                {
                    _currentEditObject.GetComponent<VideoPlayer>().Pause();
                    button.GetComponent<Image>().sprite = _controlMediaSprites[0];
                }
                else
                {
                    _currentEditObject.GetComponent<VideoPlayer>().Play();
                    button.GetComponent<Image>().sprite = _controlMediaSprites[1];
                }
            }
        }
        else if (button.name == "ResetButton")
        {
            if (_currentEditObject.GetComponent<AudioSource>())
            {
                _currentEditObject.GetComponent<AudioSource>().time = 0;
            }
            else if (_currentEditObject.GetComponent<VideoPlayer>())
            {
                _currentEditObject.GetComponent<VideoPlayer>().time = 0;
            }
        }
    }

    public void MoveObject(PointerEventData pointerEventData, Vector2 mousePos)
    {
        int numberElement = -1;
        if (_currentEditObject != null)
        {
            if (_currentEditObject.GetComponent<Text>())
            {
                numberElement = 0;
            }
            else if (_currentEditObject.GetComponent<Image>())
            {
                numberElement = 1;
            }
            else if (_currentEditObject.GetComponent<VideoPlayer>())
            {
                numberElement = 3;
            }
        }

        if (mousePos.x > _startDragMousePos.x || mousePos.x < _startDragMousePos.x)
        {
            _inputField.GetComponent<InputField>().placeholder.GetComponent<Text>().text = "Left";
            _inputField.GetComponent<InputField>().text = Convert.ToInt32(int.Parse(_editObject.GetChild(1).GetChild(2).GetChild(numberElement).GetChild(0).GetComponent<InputField>().text) + Convert.ToInt32(mousePos.x - _startDragMousePos.x)).ToString();

            OnEditInputText(_inputField.GetComponent<InputField>());

            _inputField.GetComponent<InputField>().placeholder.GetComponent<Text>().text = "Right";
            _inputField.GetComponent<InputField>().text = Convert.ToInt32(int.Parse(_editObject.GetChild(1).GetChild(2).GetChild(numberElement).GetChild(1).GetComponent<InputField>().text) - Convert.ToInt32(mousePos.x - _startDragMousePos.x)).ToString();

            OnEditInputText(_inputField.GetComponent<InputField>());
        }
        if (mousePos.y > _startDragMousePos.y || mousePos.y < _startDragMousePos.y)
        {
            _inputField.GetComponent<InputField>().placeholder.GetComponent<Text>().text = "Top";
            _inputField.GetComponent<InputField>().text = Convert.ToInt32(int.Parse(_editObject.GetChild(1).GetChild(2).GetChild(numberElement).GetChild(2).GetComponent<InputField>().text) - Convert.ToInt32(mousePos.y - _startDragMousePos.y)).ToString();

            OnEditInputText(_inputField.GetComponent<InputField>());

            _inputField.GetComponent<InputField>().placeholder.GetComponent<Text>().text = "Bottom";
            _inputField.GetComponent<InputField>().text = Convert.ToInt32(int.Parse(_editObject.GetChild(1).GetChild(2).GetChild(numberElement).GetChild(3).GetComponent<InputField>().text) + Convert.ToInt32(mousePos.y - _startDragMousePos.y)).ToString();

            OnEditInputText(_inputField.GetComponent<InputField>());
        }

        _startDragMousePos = mousePos;
    }

    public void OnMoveButton(PointerEventData pointerEventData, string nameDirection)
    {
        if (nameDirection == "LeftTop")
        {
            _inputField.GetComponent<InputField>().placeholder.GetComponent<Text>().text = "Left";
            _inputField.GetComponent<InputField>().text = Convert.ToInt32(Input.mousePosition.x).ToString();

            OnEditInputText(_inputField.GetComponent<InputField>());

            _inputField.GetComponent<InputField>().placeholder.GetComponent<Text>().text = "Top";
            _inputField.GetComponent<InputField>().text = Convert.ToInt32(Screen.height - Input.mousePosition.y).ToString();
            
            OnEditInputText(_inputField.GetComponent<InputField>());
        }
        else if (nameDirection == "RightTop")
        {
            _inputField.GetComponent<InputField>().placeholder.GetComponent<Text>().text = "Right";
            _inputField.GetComponent<InputField>().text = Convert.ToInt32(Screen.width - Input.mousePosition.x).ToString();

            OnEditInputText(_inputField.GetComponent<InputField>());

            _inputField.GetComponent<InputField>().placeholder.GetComponent<Text>().text = "Top";
            _inputField.GetComponent<InputField>().text = Convert.ToInt32(Screen.height - Input.mousePosition.y).ToString();

            OnEditInputText(_inputField.GetComponent<InputField>());
        }
        else if (nameDirection == "LeftBottom")
        {
            _inputField.GetComponent<InputField>().placeholder.GetComponent<Text>().text = "Left";
            _inputField.GetComponent<InputField>().text = Convert.ToInt32(Input.mousePosition.x).ToString();

            OnEditInputText(_inputField.GetComponent<InputField>());

            _inputField.GetComponent<InputField>().placeholder.GetComponent<Text>().text = "Bottom";
            _inputField.GetComponent<InputField>().text = Convert.ToInt32(Input.mousePosition.y - (_editObject.GetChild(1).GetComponent<RectTransform>().rect.height * (Screen.height / 1080f))).ToString();

            OnEditInputText(_inputField.GetComponent<InputField>());
        }
        else if (nameDirection == "RightBottom")
        {
            _inputField.GetComponent<InputField>().placeholder.GetComponent<Text>().text = "Right";
            _inputField.GetComponent<InputField>().text = Convert.ToInt32(Screen.width - Input.mousePosition.x).ToString();

            OnEditInputText(_inputField.GetComponent<InputField>());

            _inputField.GetComponent<InputField>().text = Convert.ToInt32(Input.mousePosition.y - (_editObject.GetChild(1).GetComponent<RectTransform>().rect.height * (Screen.height / 1080f))).ToString();
            _inputField.GetComponent<InputField>().placeholder.GetComponent<Text>().text = "Bottom";
            
            OnEditInputText(_inputField.GetComponent<InputField>());
        }
    }

    public void DeleteAllObjects()
    {
        for (int i = 0; i < _editObject.GetChild(0).childCount; i++)
        {
            for (int a = 0; a < _editObject.GetChild(0).GetChild(i).childCount; a++)
            {
                Destroy(_editObject.GetChild(0).GetChild(i).GetChild(a).gameObject);
            }
        }

        for (int i = 0; i < 2; i++)
        {
            for (int a = 0; a < _editObject.GetChild(1).GetChild(i).GetChild(0).GetChild(0).childCount; a++)
            {
                Destroy(_editObject.GetChild(1).GetChild(i).GetChild(0).GetChild(0).GetChild(a).gameObject);
            }
        }
    }

    public void QuitFromEditor()
    {
        SceneManager.LoadScene(0);
    }
}

[Serializable]
public class SaveCategory
{
    public string Name { get; set; }
    public SaveRound[] SaveRound { get; set; }
}

[Serializable]
public class SaveRound
{
    public SaveLayout[] SaveLayouts { get; set; }
}

[Serializable]
public class SaveLayout
{
    public List<SaveText> SaveText { get; set; }
    public List<SaveImage> SaveImage { get; set; }
    public List<SaveAudio> SaveAudio { get; set; }
    public List<SaveVideo> SaveVideo { get; set; }
}

[Serializable]
public class SaveText
{
    public long Id { get; set; }
    public string Text { get; set; }
    public int FontSize { get; set; }
    public float RedColor { get; set; }
    public float GreenColor { get; set; }
    public float BlueColor { get; set; }
    public float AlphaColor { get; set; }
    public string AlignmentText { get; set; }
    public int Left { get; set; }
    public int Right { get; set; }
    public int Top { get; set; }
    public int Bottom { get; set; }

}

[Serializable]
public class SaveImage
{
    public long Id { get; set; }
    public string PathToImage { get; set; }
    public int Left { get; set; }
    public int Right { get; set; }
    public int Top { get; set; }
    public int Bottom { get; set; }
}

[Serializable]
public class SaveAudio
{
    public long Id { get; set; }
    public string PathToAudio { get; set; }
    public float Volume { get; set; }
}

[Serializable]
public class SaveVideo
{
    public long Id { get; set; }
    public string PathToVideo { get; set; }
    public float Volume { get; set; }
    public int Left { get; set; }
    public int Right { get; set; }
    public int Top { get; set; }
    public int Bottom { get; set; }
}