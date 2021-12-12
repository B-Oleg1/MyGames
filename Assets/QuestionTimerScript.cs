using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestionTimerScript : MonoBehaviour
{
    [SerializeField]
    private Transform _objectWithQuestions;

    [SerializeField]
    private Slider _slider;

    private bool _questionCounted = false;
    private IEnumerator ienum;
    private bool coroutineIsStarted = false;

    private float time = 15f;

    private void Start()
    {
        ienum = Timer();
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.R) && !coroutineIsStarted)
        {
            coroutineIsStarted = true;
            time = 15f;
            _slider.value = 1;
            StartTimer();
        }

        if (Input.GetKey(KeyCode.E) && coroutineIsStarted)
        {
            StopCoroutine(ienum);
            coroutineIsStarted = false;
        }

        if (Input.GetKey(KeyCode.E))
        {
            StopCoroutine(ienum);
        }

        if (Input.GetKey(KeyCode.Alpha1) && !_questionCounted)
        {
            _questionCounted = true;
            StartCoroutine(End(ModelsScript.currentPointsForQuestions, 0));
        }
        else if (Input.GetKey(KeyCode.Alpha2) && !_questionCounted)
        {
            _questionCounted = true;
            StartCoroutine(End(ModelsScript.currentPointsForQuestions * -1, 0));
        }
        else if (Input.GetKey(KeyCode.Alpha3) && !_questionCounted)
        {
            _questionCounted = true;
            StartCoroutine(End(ModelsScript.currentPointsForQuestions, 1));
        }
        else if (Input.GetKey(KeyCode.Alpha4) && !_questionCounted)
        {
            _questionCounted = true;
            StartCoroutine(End(ModelsScript.currentPointsForQuestions * -1, 1));
        }
        else if (Input.GetKey(KeyCode.Alpha5) && !_questionCounted)
        {
            _questionCounted = true;
            StartCoroutine(End(ModelsScript.currentPointsForQuestions, 2));
        }
        else if (Input.GetKey(KeyCode.Alpha6) && !_questionCounted)
        {
            _questionCounted = true;
            StartCoroutine(End(ModelsScript.currentPointsForQuestions * -1, 2));
        }
    }

    private IEnumerator End(int points, int commandId)
    {
        ModelsScript.Players[commandId].Score += points;

        if (points < 0)
        {
            ModelsScript.Players[commandId].ProgressBattlePass = 0;
        }
        else if (points > 0 && ModelsScript.Players[commandId].ProgressBattlePass < 7)
        {
            ModelsScript.Players[commandId].ProgressBattlePass++;

            if (ModelsScript.Players[commandId].ProgressBattlePass < 3)
            {
                ModelsScript.Players[commandId].ShopScore += 0.5f;
            }
            else if (ModelsScript.Players[commandId].ProgressBattlePass >= 3 && ModelsScript.Players[commandId].ProgressBattlePass < 5)
            {
                ModelsScript.Players[commandId].ShopScore += 1;
            }
            else if (ModelsScript.Players[commandId].ProgressBattlePass >= 5)
            {
                ModelsScript.Players[commandId].ShopScore += 1.5f;
            }
        }
        else if (points > 0 && ModelsScript.Players[commandId].ProgressBattlePass == 7)
        {
            MainScript.GiveOutPrize(commandId);
            ModelsScript.Players[commandId].ProgressBattlePass = 0;
        }

        ModelsScript.needUpdateCommandId = commandId;

        for (int i = 1; i < ModelsScript.allSceneWithQuestion[ModelsScript.currentQuestion].childCount; i++)
        {
            ModelsScript.allSceneWithQuestion[ModelsScript.currentQuestion].GetChild(i).gameObject.SetActive(false);
        }
        ModelsScript.allSceneWithQuestion[ModelsScript.currentQuestion].GetChild(0).gameObject.SetActive(true);

        yield return new WaitForSeconds(3.75f);

        ModelsScript.allSceneWithQuestion[ModelsScript.currentQuestion].gameObject.SetActive(false);
        _objectWithQuestions.gameObject.SetActive(false);

        _questionCounted = false;

        gameObject.SetActive(false);
    }

    public void StartTimer()
    {
        StartCoroutine(ienum);
    }

    private IEnumerator Timer()
    {
        while (time > 0)
        {
            time -= Time.deltaTime;
            _slider.value = time * 0.06666666666666666666666666666667f;
            yield return null;
        }
    }
}