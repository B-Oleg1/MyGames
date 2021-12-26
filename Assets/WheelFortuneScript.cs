using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WheelFortuneScript : MonoBehaviour
{
    [SerializeField]
    private Transform _wheelFortune;

    [SerializeField]
    private Transform[] _allGames;

    [SerializeField]
    private Slider _slider;

    private int[] _allPastGames = new int[6];

    private int _numberOfTurns;
    private float _prize = -1;
    private float _speed;
    private bool _canTurn;

    private int currentGame = -1;

    private IEnumerator ienum;

    private float _maxTime = 15f;
    private float _time = 15f;

    private bool _coroutineIsStarted = false;

    private int _currentExample = 1;

    private DateTime _startTime;
    private DateTime _endTime;

    private int commandId = 0;

    void Start()
    {
        _canTurn = true;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && _canTurn)
        {
            StartCoroutine(TurnWheel());
        }

        if (Input.GetKeyDown(KeyCode.P) && _prize != -1)
        {
            StartSuperGame();
        }

        if (Input.GetKey(KeyCode.R) && !_coroutineIsStarted)
        {
            ienum = null;
            ienum = Timer();
            _coroutineIsStarted = true;
            _time = _maxTime;
            _slider.value = 1;
            StartCoroutine(ienum);
        }
        if (Input.GetKey(KeyCode.E) && _coroutineIsStarted)
        {
            StopCoroutine(ienum);
            _coroutineIsStarted = false;
        }

        if (Input.GetKey(KeyCode.K) && currentGame != -1)
        {
            if (_allGames[currentGame].childCount == 4)
            {
                _allGames[currentGame].GetChild(3).gameObject.SetActive(false);
            }
            _allGames[currentGame].GetChild(0).gameObject.SetActive(true);
            _allGames[currentGame].GetChild(1).gameObject.SetActive(true);
            _allGames[currentGame].GetChild(2).gameObject.SetActive(true);

            _slider.gameObject.SetActive(false);
        }

        if (Input.GetKey(KeyCode.Alpha1))
        {
            ModelsScript.Players[ModelsScript.attack[1]].Score += ModelsScript.Players[ModelsScript.attack[2]].Score / 2;
            ModelsScript.needUpdateCommandId = ModelsScript.attack[1];

            ModelsScript.Players[ModelsScript.attack[2]].Score -= ModelsScript.Players[ModelsScript.attack[2]].Score / 2;
            
            _slider.gameObject.SetActive(false);
            _allGames[currentGame].gameObject.SetActive(false);
            
            _prize = -1;
            currentGame = -1;

            for (int i = 0; i < 500; i++)
            {
                i++;
            }

            ModelsScript.needUpdateCommandId = ModelsScript.attack[2];

            gameObject.SetActive(false);
        }
        else if (Input.GetKey(KeyCode.Alpha2))
        {
            _slider.gameObject.SetActive(false);
            _allGames[currentGame].gameObject.SetActive(false);

            _prize = -1;
            currentGame = -1;

            gameObject.SetActive(false);
        }
    }

    public void StartMath(int mathId)
    {
        if (_currentExample >= 1 && _currentExample <= 11)
        {
            if (_currentExample >= 2)
            {
                _allGames[3].GetChild(3).GetChild(_allPastGames[3] - 1).GetChild(_currentExample - 1).gameObject.SetActive(false);
            }
            else
            {
                _startTime = DateTime.Now;
            }

            _allGames[3].GetChild(3).GetChild(_allPastGames[3] - 1).GetChild(_currentExample).gameObject.SetActive(true);
            _currentExample++;
        }
        else
        {
            _endTime = DateTime.Now;
            var totalTime = _endTime - _startTime;
            _allGames[3].GetChild(commandId).GetChild(1).GetComponent<InputField>().text = $"{totalTime.TotalSeconds} �";
            if (commandId == 0)
            {
                commandId++;
            }

            _allGames[3].GetChild(3).GetChild(_allPastGames[3] - 1).GetChild(_currentExample - 1).gameObject.SetActive(false);
            _currentExample = 1;
        }
    }

    private IEnumerator TurnWheel()
    {
        _canTurn = false;

        _numberOfTurns = UnityEngine.Random.Range(500, 1000);

        _speed = 0.1f;

        for (int i = 0; i < _numberOfTurns; i++)
        {
            _wheelFortune.Rotate(0, 0, _speed * 100);

            if (i > Mathf.RoundToInt(_numberOfTurns * 0.5f))
            {
                _speed = 0.065f;
            }
            if (i > Mathf.RoundToInt(_numberOfTurns * 0.6f))
            {
                _speed = 0.055f;
            }
            if (i > Mathf.RoundToInt(_numberOfTurns * 0.7f))
            {
                _speed = 0.045f;
            }
            if (i > Mathf.RoundToInt(_numberOfTurns * 0.75f))
            {
                _speed = 0.04f;
            }
            if (i > Mathf.RoundToInt(_numberOfTurns * 0.8f))
            {
                _speed = 0.03f;
            }
            if (i > Mathf.RoundToInt(_numberOfTurns * 0.85f))
            {
                _speed = 0.02f;
            }
            if (i > Mathf.RoundToInt(_numberOfTurns * 0.9f))
            {
                _speed = 0.01f;
            }

            yield return new WaitForSeconds(0.01f);
        }

        _prize = _wheelFortune.eulerAngles.z;

        _canTurn = true;
    }

    private void StartSuperGame()
    {
        foreach (var item in _allGames)
        {
            item.gameObject.SetActive(false);
        }

        if (_prize >= 319 || _prize <= 18.7f)
        {
            // ������
            currentGame = 0;
            _allGames[0].gameObject.SetActive(true);
        }
        else if (_prize > 18.7f && _prize <= 78.6f)
        {
            // �����
            currentGame = 1;
            _allGames[1].gameObject.SetActive(true);
            if (_allPastGames[1] <= 1)
            {
                _allGames[1].GetChild(3).gameObject.SetActive(true);
                _allGames[1].GetChild(3).GetChild(_allPastGames[1]).gameObject.SetActive(true);
                _allPastGames[1]++;
            }
            else
            {
                _allGames[1].GetChild(3).GetChild(0).gameObject.SetActive(true);
            }

            _slider.gameObject.SetActive(true);
            _time = 30f;
            _maxTime = 30f;
        }
        else if (_prize > 78.6f && _prize <= 138.7f)
        {
            // ������
            currentGame = 2;
            _allGames[2].gameObject.SetActive(true);

            _slider.gameObject.SetActive(true);
            _time = 5f;
            _maxTime = 5f;
        }
        else if (_prize > 138.7f && _prize <= 198.8f)
        {
            // ����������
            currentGame = 3;
            _allGames[3].gameObject.SetActive(true);
            if (_allPastGames[3] <= 1)
            {
                _allGames[3].GetChild(3).gameObject.SetActive(true);
                _allGames[3].GetChild(3).GetChild(_allPastGames[3]).gameObject.SetActive(true);
                _allPastGames[3]++;
            }
            else
            {
                _allGames[3].GetChild(3).GetChild(0).gameObject.SetActive(true);
            }
        }
        else if (_prize > 198.8f && _prize <= 259.9f)
        {
            // �������
            currentGame = 4;
            _allGames[4].gameObject.SetActive(true);
            if (_allPastGames[4] <= 1)
            {
                _allGames[4].GetChild(3).gameObject.SetActive(true);
                _allGames[4].GetChild(3).GetChild(_allPastGames[4]).gameObject.SetActive(true);
                _allPastGames[4]++;
            }
            else
            {
                _allGames[4].GetChild(3).GetChild(0).gameObject.SetActive(true);
            }

            _slider.gameObject.SetActive(true);
            _time = 25f;
            _maxTime = 25f;
        }
        else if (_prize > 259.9f && _prize < 319)
        {
            // ���� ���
            currentGame = 5;
            _allGames[5].gameObject.SetActive(true);
            if (_allPastGames[5] <= 1)
            {
                _allGames[5].GetChild(3).gameObject.SetActive(true);
                _allGames[5].GetChild(3).GetChild(_allPastGames[5]).gameObject.SetActive(true);
                _allPastGames[5]++;
            }
            else
            {
                _allGames[5].GetChild(3).GetChild(0).gameObject.SetActive(true);
            }

            _slider.gameObject.SetActive(true);
            _time = 15f;
            _maxTime = 15f;
        }
    }

    private IEnumerator Timer()
    {
        while (_time > 0)
        {
            _time -= Time.deltaTime;
            _slider.value = _time * (1 / _maxTime);
            yield return null;
        }
    }
}