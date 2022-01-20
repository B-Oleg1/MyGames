using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
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
    private Sprite _whiteSprite;

    private SaveCategory[] _savedCategory = new SaveCategory[16];

    private GameObject _inputField;
    private GameObject _textInputField;
    private GameObject _placeholderInputField;

    private GameObject _currentEditObject;
    private long _currentIdObject;

    private string _fileName = string.Empty;

    private int _currentRoundId = 0;
    private int _currentQuestionOrAnswer = 0;

    private string[] _randomWords = { "Approach", "Establish", "Attorney", "Tease", "Will", "Fortitude", "Torn", "Hero", "Catering", "Carefully",
        "Signal", "Totebook", "Orientalist", "Cable", "Distance", "Echo", "Hair", "Most", "Zombie", "Up" };

    private void Start()
    {
        LoadAllSaves();

        _textInputField.AddComponent<Text>();
        _placeholderInputField.AddComponent<Text>();
        _inputField.AddComponent<InputField>().textComponent = _textInputField.GetComponent<Text>();
        _inputField.GetComponent<InputField>().placeholder = _placeholderInputField.GetComponent<Text>();
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

                //Save game
                /*var json = JsonConvert.SerializeObject(_savedCategory);
                using (FileStream fileStream = new FileStream($"{Application.dataPath}/Saves/{button.transform.GetChild(0).GetComponent<Text>().text}.json", FileMode.Open))
                {
                    using (StreamWriter sw = new StreamWriter(fileStream))
                    {
                        sw.WriteLine(json);
                    }
                }*/
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
        var json = JsonConvert.SerializeObject(_savedCategory);
        using (FileStream fileStream = new FileStream($"{Application.dataPath}/Saves/{_fileName}", FileMode.Open))
        {
            using (StreamWriter sw = new StreamWriter(fileStream))
            {
                sw.WriteLine(json);
            }
        }
    }

    public void OnClickEditButton(int roundId)
    {
        _currentRoundId = roundId;

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

                newGameObject.transform.SetParent(_editObject.GetChild(0).GetChild(i));

                newGameObject.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
                newGameObject.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);

                newGameObject.GetComponent<RectTransform>().offsetMin = new Vector2(_savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts[i].SaveText[a].Left,
                                                                                    _savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts[i].SaveText[a].Bottom);
                newGameObject.GetComponent<RectTransform>().offsetMax = new Vector2(_savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts[i].SaveText[a].Right,
                                                                                    _savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts[i].SaveText[a].Top);

                newGameObject.transform.localScale = new Vector3(1f, 1f, 1f);

                print(1);

                AddUIOnNewObject(newGameObject, _savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts[i].SaveText[a].Id, i);
            }
            for (int a = 0; a < _savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts[i].SaveImage.Count; a++)
            {

            }
            for (int a = 0; a < _savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts[i].SaveAudio.Count; a++)
            {

            }
            for (int a = 0; a < _savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts[i].SaveVideo.Count; a++)
            {

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
    }

    public void AddVideo()
    {
    }

    private void AddUIOnNewObject(GameObject newGameObject, long objectId, int questOrAsnw)
    {
        string nameButton = string.Empty;

        string[] directions = { "LeftTop", "RightTop", "LeftBottom", "RightBottom" };
        for (int i = 0; i < 4; i++)
        {
            GameObject resizingButton = new GameObject();
            resizingButton.AddComponent<EventTrigger>();

            resizingButton.transform.SetParent(newGameObject.transform);

            resizingButton.GetComponent<RectTransform>().anchorMin = new Vector2(0, 1);
            resizingButton.GetComponent<RectTransform>().anchorMax = new Vector2(0, 1);

            resizingButton.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
            resizingButton.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);

            resizingButton.transform.localScale = new Vector3(1f, 1f, 1f);

            resizingButton.SetActive(false);

            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.Drag;

            entry.callback.AddListener((data) => OnMoveButton((PointerEventData)data, directions[i]));
            resizingButton.GetComponent<EventTrigger>().triggers.Add(entry);
        }

        if (newGameObject.GetComponent<Text>())
        {
            nameButton = "Текст";

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
            nameButton = "Картинка";
        }
        else if (newGameObject.GetComponent<AudioSource>())
        {
            nameButton = "Аудио";
        }
        else if (newGameObject.GetComponent<VideoPlayer>())
        {
            nameButton = "Видео";
        }

        var newElement = Instantiate(Resources.Load<GameObject>("NewElementObject"), _editObject.GetChild(1).GetChild(questOrAsnw).GetChild(0).GetChild(0));

        var button = newElement.transform.GetChild(0).GetComponent<Button>();
        button.transform.GetChild(0).GetComponent<Text>().text = nameButton;
        button.onClick.AddListener(() =>
        {
            // Hide buttons for resizing
            if (_currentEditObject is not null && !_currentEditObject.GetComponent<AudioSource>())
            {
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
                for (int i = 0; i < 4; i++)
                {
                    newGameObject.transform.GetChild(i).gameObject.SetActive(true);
                }
            }
            

            if (newGameObject.GetComponent<Text>())
            {
                _editObject.GetChild(1).GetChild(2).GetChild(0).gameObject.SetActive(true);

                _editObject.GetChild(1).GetChild(2).GetChild(0).GetChild(0).GetComponent<InputField>().text = newGameObject.GetComponent<Text>().text;
                _editObject.GetChild(1).GetChild(2).GetChild(0).GetChild(1).GetComponent<InputField>().text = newGameObject.GetComponent<Text>().fontSize.ToString();
                _editObject.GetChild(1).GetChild(2).GetChild(0).GetChild(2).GetComponent<InputField>().text = newGameObject.GetComponent<RectTransform>().offsetMin.x.ToString();
                _editObject.GetChild(1).GetChild(2).GetChild(0).GetChild(3).GetComponent<InputField>().text = (newGameObject.GetComponent<RectTransform>().offsetMax.x * -1).ToString();
                _editObject.GetChild(1).GetChild(2).GetChild(0).GetChild(4).GetComponent<InputField>().text = newGameObject.GetComponent<RectTransform>().offsetMin.y.ToString();
                _editObject.GetChild(1).GetChild(2).GetChild(0).GetChild(5).GetComponent<InputField>().text = (newGameObject.GetComponent<RectTransform>().offsetMax.y * -1).ToString();
            }
            else if (newGameObject.GetComponent<Image>())
            {
                _editObject.GetChild(1).GetChild(2).GetChild(1).gameObject.SetActive(true);

                _editObject.GetChild(1).GetChild(2).GetChild(1).GetChild(0).GetComponent<InputField>().text = _savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7]
                                                                                                                            .SaveLayouts[_currentQuestionOrAnswer].SaveImage
                                                                                                                            .FirstOrDefault(item => item.Id == objectId)
                                                                                                                            .PathToImage;
                _editObject.GetChild(1).GetChild(2).GetChild(1).GetChild(1).GetComponent<InputField>().text = newGameObject.GetComponent<RectTransform>().offsetMin.x.ToString();
                _editObject.GetChild(1).GetChild(2).GetChild(1).GetChild(2).GetComponent<InputField>().text = (newGameObject.GetComponent<RectTransform>().offsetMax.x * -1).ToString();
                _editObject.GetChild(1).GetChild(2).GetChild(1).GetChild(3).GetComponent<InputField>().text = newGameObject.GetComponent<RectTransform>().offsetMin.y.ToString();
                _editObject.GetChild(1).GetChild(2).GetChild(1).GetChild(4).GetComponent<InputField>().text = (newGameObject.GetComponent<RectTransform>().offsetMax.y * -1).ToString();
            }
            else if (newGameObject.GetComponent<AudioSource>())
            {
                _editObject.GetChild(1).GetChild(2).GetChild(2).gameObject.SetActive(true);

                _editObject.GetChild(1).GetChild(2).GetChild(2).GetChild(0).GetComponent<InputField>().text = _savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7]
                                                                                                                            .SaveLayouts[_currentQuestionOrAnswer].SaveAudio
                                                                                                                            .FirstOrDefault(item => item.Id == objectId)
                                                                                                                            .PathToAudio;
            }
            else if (newGameObject.GetComponent<VideoPlayer>())
            {
                _editObject.GetChild(1).GetChild(2).GetChild(3).gameObject.SetActive(true);

                _editObject.GetChild(1).GetChild(2).GetChild(3).GetChild(0).GetComponent<InputField>().text = _savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7]
                                                                                                                            .SaveLayouts[_currentQuestionOrAnswer].SaveVideo
                                                                                                                            .FirstOrDefault(item => item.Id == objectId)
                                                                                                                            .PathToVideo;
                _editObject.GetChild(1).GetChild(2).GetChild(3).GetChild(1).GetComponent<InputField>().text = newGameObject.GetComponent<RectTransform>().offsetMin.x.ToString();
                _editObject.GetChild(1).GetChild(2).GetChild(3).GetChild(2).GetComponent<InputField>().text = (newGameObject.GetComponent<RectTransform>().offsetMax.x * -1).ToString();
                _editObject.GetChild(1).GetChild(2).GetChild(3).GetChild(3).GetComponent<InputField>().text = newGameObject.GetComponent<RectTransform>().offsetMin.y.ToString();
                _editObject.GetChild(1).GetChild(2).GetChild(3).GetChild(4).GetComponent<InputField>().text = (newGameObject.GetComponent<RectTransform>().offsetMax.y * -1).ToString();
            }
        });

        var deleteButton = newElement.transform.GetChild(1).GetComponent<Button>();
        deleteButton.onClick.AddListener(() =>
        {
            if (_currentEditObject == newGameObject)
            {
                _editObject.GetChild(1).GetChild(2).GetChild(0).gameObject.SetActive(false);
            }

            if (newGameObject.GetComponent<Text>())
            {
                var deleteItem = _savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts[questOrAsnw]
                                                                                                                    .SaveText
                                                                                                                    .FirstOrDefault(item => item.Id == _currentIdObject);
                _savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts[questOrAsnw].SaveText.Remove(deleteItem);
            }
            else if (newGameObject.GetComponent<Image>())
            {
                var deleteItem = _savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts[questOrAsnw]
                                                                                                                    .SaveImage
                                                                                                                    .FirstOrDefault(item => item.Id == _currentIdObject);
                _savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts[questOrAsnw].SaveImage.Remove(deleteItem);
            }
            else if (newGameObject.GetComponent<AudioSource>())
            {
                var deleteItem = _savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts[questOrAsnw]
                                                                                                                    .SaveAudio
                                                                                                                    .FirstOrDefault(item => item.Id == _currentIdObject);
                _savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts[questOrAsnw].SaveAudio.Remove(deleteItem);
            }
            else if (newGameObject.GetComponent<VideoPlayer>())
            {
                var deleteItem = _savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts[questOrAsnw]
                                                                                                                    .SaveVideo
                                                                                                                    .FirstOrDefault(item => item.Id == _currentIdObject);
                _savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts[questOrAsnw].SaveVideo.Remove(deleteItem);
            }

            Destroy(newGameObject);
            Destroy(newElement);
        });
    }

    public void OnEditInputText(InputField inputField)
    {
        string nameAction = inputField.placeholder.GetComponent<Text>().text;
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
                FileStream audioFile = new FileStream(inputField.text, FileMode.Open);

                byte[] audioByte = new byte[audioFile.Length];

                audioFile.Read(audioByte, 0, audioByte.Length);

                audioFile.Close();

                float[] samples = new float[audioByte.Length / 4];

                Buffer.BlockCopy(audioByte, 0, samples, 0, audioByte.Length);

                int channels = 1;
                int sampleRate = 44100;

                AudioClip clip = AudioClip.Create("ClipName", samples.Length, channels, sampleRate, false);
                clip.SetData(samples, 0);

                _currentEditObject.GetComponent<AudioSource>().clip = clip;

                _savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts[_currentQuestionOrAnswer].SaveAudio
                                                                                                            .FirstOrDefault(item => item.Id == _currentIdObject)
                                                                                                            .PathToAudio = inputField.text;
                break;
            case "VolumeAudio":
                break;
            case "PathToVideo":
                _currentEditObject.GetComponent<VideoPlayer>().source = VideoSource.Url;
                _currentEditObject.GetComponent<VideoPlayer>().url = inputField.text;

                _savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts[_currentQuestionOrAnswer].SaveVideo
                                                                                                            .FirstOrDefault(item => item.Id == _currentIdObject)
                                                                                                            .PathToVideo = inputField.text;
                break;
            case "VolumeVideo":
                break;
            case "Left":
                var left = _currentEditObject.GetComponent<RectTransform>().offsetMin;
                left.x = int.Parse(inputField.text);
                _currentEditObject.GetComponent<RectTransform>().offsetMin = left;

                if (_currentEditObject.GetComponent<Text>())
                {
                    _savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts[_currentQuestionOrAnswer].SaveText
                                                                                                            .FirstOrDefault(item => item.Id == _currentIdObject)
                                                                                                            .Left = int.Parse(inputField.text);
                }
                else if (_currentEditObject.GetComponent<Image>())
                {
                    _savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts[_currentQuestionOrAnswer].SaveImage
                                                                                                            .FirstOrDefault(item => item.Id == _currentIdObject)
                                                                                                            .Left = int.Parse(inputField.text);
                }
                else if (_currentEditObject.GetComponent<VideoPlayer>())
                {
                    _savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts[_currentQuestionOrAnswer].SaveVideo
                                                                                                            .FirstOrDefault(item => item.Id == _currentIdObject)
                                                                                                            .Left = int.Parse(inputField.text);
                }

                break;
            case "Right":
                var right = _currentEditObject.GetComponent<RectTransform>().offsetMax;
                right.x = int.Parse(inputField.text) * -1;
                _currentEditObject.GetComponent<RectTransform>().offsetMax = right;

                if (_currentEditObject.GetComponent<Text>())
                {
                    _savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts[_currentQuestionOrAnswer].SaveText
                                                                                                            .FirstOrDefault(item => item.Id == _currentIdObject)
                                                                                                            .Right = int.Parse(inputField.text) * -1;
                }
                else if (_currentEditObject.GetComponent<Image>())
                {
                    _savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts[_currentQuestionOrAnswer].SaveImage
                                                                                                            .FirstOrDefault(item => item.Id == _currentIdObject)
                                                                                                            .Right = int.Parse(inputField.text) * -1;
                }
                else if (_currentEditObject.GetComponent<VideoPlayer>())
                {
                    _savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts[_currentQuestionOrAnswer].SaveVideo
                                                                                                            .FirstOrDefault(item => item.Id == _currentIdObject)
                                                                                                            .Right = int.Parse(inputField.text) * -1;
                }
                break;
            case "Top":
                var top = _currentEditObject.GetComponent<RectTransform>().offsetMax;
                top.y = int.Parse(inputField.text) * -1;
                _currentEditObject.GetComponent<RectTransform>().offsetMax = top;

                if (_currentEditObject.GetComponent<Text>())
                {
                    _savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts[_currentQuestionOrAnswer].SaveText
                                                                                                            .FirstOrDefault(item => item.Id == _currentIdObject)
                                                                                                            .Top = int.Parse(inputField.text) * -1;
                }
                else if (_currentEditObject.GetComponent<Image>())
                {
                    _savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts[_currentQuestionOrAnswer].SaveImage
                                                                                                            .FirstOrDefault(item => item.Id == _currentIdObject)
                                                                                                            .Top = int.Parse(inputField.text) * -1;
                }
                else if (_currentEditObject.GetComponent<VideoPlayer>())
                {
                    _savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts[_currentQuestionOrAnswer].SaveVideo
                                                                                                            .FirstOrDefault(item => item.Id == _currentIdObject)
                                                                                                            .Top = int.Parse(inputField.text) * -1;
                }
                break;
            case "Bottom":
                var bottom = _currentEditObject.GetComponent<RectTransform>().offsetMin;
                bottom.y = int.Parse(inputField.text);
                _currentEditObject.GetComponent<RectTransform>().offsetMin = bottom;

                if (_currentEditObject.GetComponent<Text>())
                {
                    _savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts[_currentQuestionOrAnswer].SaveText
                                                                                                            .FirstOrDefault(item => item.Id == _currentIdObject)
                                                                                                            .Bottom = int.Parse(inputField.text);
                }
                else if (_currentEditObject.GetComponent<Image>())
                {
                    _savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts[_currentQuestionOrAnswer].SaveImage
                                                                                                            .FirstOrDefault(item => item.Id == _currentIdObject)
                                                                                                            .Bottom = int.Parse(inputField.text);
                }
                else if (_currentEditObject.GetComponent<VideoPlayer>())
                {
                    _savedCategory[(_currentRoundId + 1) / 7].SaveRound[_currentRoundId % 7].SaveLayouts[_currentQuestionOrAnswer].SaveVideo
                                                                                                            .FirstOrDefault(item => item.Id == _currentIdObject)
                                                                                                            .Bottom = int.Parse(inputField.text);
                }
                break;
            default:
                break;
        }
    }

    public void OnMoveButton(PointerEventData pointerEventData, string nameDirection)
    {
        if (nameDirection == "LeftTop")
        {
            _inputField.GetComponent<InputField>().placeholder.name = "Left";
            _inputField.GetComponent<InputField>().text = Input.mousePosition.x.ToString();

            OnEditInputText(_inputField.GetComponent<InputField>());

            _inputField.GetComponent<InputField>().placeholder.name = "Top";
            _inputField.GetComponent<InputField>().text = Input.mousePosition.y.ToString();
        }
        else if (nameDirection == "RightTop")
        {
            _inputField.GetComponent<InputField>().placeholder.name = "Right";
            _inputField.GetComponent<InputField>().text = Input.mousePosition.x.ToString();

            OnEditInputText(_inputField.GetComponent<InputField>());

            _inputField.GetComponent<InputField>().placeholder.name = "Top";
            _inputField.GetComponent<InputField>().text = Input.mousePosition.y.ToString();
        }
        else if (nameDirection == "LeftBottom")
        {
            _inputField.GetComponent<InputField>().placeholder.name = "Left";
            _inputField.GetComponent<InputField>().text = Input.mousePosition.x.ToString();

            OnEditInputText(_inputField.GetComponent<InputField>());

            _inputField.GetComponent<InputField>().placeholder.name = "Bottom";
            _inputField.GetComponent<InputField>().text = Input.mousePosition.y.ToString();
        }
        else if (nameDirection == "RightBottom")
        {
            _inputField.GetComponent<InputField>().placeholder.name = "Right";
            _inputField.GetComponent<InputField>().text = Input.mousePosition.x.ToString();

            OnEditInputText(_inputField.GetComponent<InputField>());

            _inputField.GetComponent<InputField>().placeholder.name = "Bottom";
            _inputField.GetComponent<InputField>().text = Input.mousePosition.y.ToString();
        }
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