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

    private IEnumerator ienum;

    private float _maxTime = 15f;
    private float _time = 15f;

    private bool _coroutineIsStarted = false;

    void Start()
    {
        _canTurn = true;
        ienum = Timer();
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

        if (Input.GetKey(KeyCode.Alpha1))
        {
            ModelsScript.Players[ModelsScript.attack[1]].Score += ModelsScript.Players[ModelsScript.attack[2]].Score / 2;
            ModelsScript.Players[ModelsScript.attack[2]].Score -= ModelsScript.Players[ModelsScript.attack[2]].Score / 2;

            _prize = -1;
        }
        else if (Input.GetKey(KeyCode.Alpha2))
        {
            _prize = -1;

            _slider.gameObject.SetActive(false);
            _wheelFortune.gameObject.SetActive(false);
        }
    }

    private IEnumerator TurnWheel()
    {
        _canTurn = false;

        _numberOfTurns = Random.Range(500, 1000);

        _speed = 0.1f;

        for (int i = 0; i < _numberOfTurns; i++)
        {
            _wheelFortune.Rotate(0, 0, _speed * 100);

            if (i > Mathf.RoundToInt(_numberOfTurns * 0.5f))
            {
                _speed = 0.065f;
            }
            if (i > Mathf.RoundToInt(_numberOfTurns * 0.65f))
            {
                _speed = 0.035f;
            }
            if (i > Mathf.RoundToInt(_numberOfTurns * 0.85f))
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
            // Шерлок
            _allGames[0].gameObject.SetActive(true);
        }
        else if (_prize > 18.7f && _prize <= 78.6f)
        {
            // Слова
            _allGames[1].gameObject.SetActive(true);
            if (_allPastGames[1] <= 1)
            {
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
            // Города
            _allGames[2].gameObject.SetActive(true);

            _slider.gameObject.SetActive(true);
            _time = 5f;
            _maxTime = 5f;
        }
        else if (_prize > 138.7f && _prize <= 198.8f)
        {
            // Математика
            _allGames[3].gameObject.SetActive(true);
            if (_allPastGames[3] <= 1)
            {
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
            // Отличия
            _allGames[4].gameObject.SetActive(true);
            if (_allPastGames[4] <= 1)
            {
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
            // Фулл бай
            _allGames[5].gameObject.SetActive(true);
            if (_allPastGames[5] <= 1)
            {
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
