using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    private float time = 25f;

    private void Start()
    {
        //ienum = Timer();
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.R) && !coroutineIsStarted)
        {
            ienum = null;
            ienum = Timer();
            coroutineIsStarted = true;
            time = 25f;
            _slider.value = 1;
            StartCoroutine(ienum);
        }
        if (Input.GetKey(KeyCode.E) && coroutineIsStarted)
        {
            StopCoroutine(ienum);
            coroutineIsStarted = false;
        }

        if (Input.GetKey(KeyCode.Q))
        {
            if (ModelsScript.allSceneWithQuestion[ModelsScript.currentQuestion].childCount == 3)
            {
                ModelsScript.allSceneWithQuestion[ModelsScript.currentQuestion].GetChild(2).GetComponent<Animator>().enabled = false;
            }
            ModelsScript.allSceneWithQuestion[ModelsScript.currentQuestion].GetChild(1).gameObject.SetActive(false);

            ModelsScript.allSceneWithQuestion[ModelsScript.currentQuestion].GetChild(0).gameObject.SetActive(true);
        }

        if (Input.GetKey(KeyCode.Alpha1) && !_questionCounted)
        {
            _questionCounted = true;
            StartCoroutine(End(ModelsScript.currentPointsForQuestions, 0));
        }
        else if (Input.GetKey(KeyCode.Alpha2) && !_questionCounted)
        {
            _questionCounted = true;
            StartCoroutine(End(ModelsScript.currentPointsForQuestions * -1, 1));
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
        ScoringPoints(points, commandId);

        ModelsScript.needUpdateCommandId = commandId;

        yield return new WaitForSeconds(1.5f);

        ModelsScript.allSceneWithQuestion[ModelsScript.currentQuestion].gameObject.SetActive(false);
        _objectWithQuestions.gameObject.SetActive(false);

        for (int i = 0; i < ModelsScript.lvlsWithFreeze.Count; i++)
        {
            ModelsScript.lvlsWithFreeze[i]--;
            if (ModelsScript.lvlsWithFreeze[i] == 0)
            {
                ModelsScript.allBtns[i].interactable = true;
                ModelsScript.lvlsWithFreeze.Remove(i);
            }
        }

        for (int i = 0; i < ModelsScript.Players.Count; i++)
        {
            if (ModelsScript.Players[i].WearingArmor > 0)
            {
                ModelsScript.Players[i].WearingArmor--;
            }
        }

        _questionCounted = false;

        gameObject.SetActive(false);
    }

    private void ScoringPoints(int points, int commandId)
    {
        if (ModelsScript.currentBonus.Any(item => item == ShopItem.x2))
        {
            ModelsScript.Players[commandId].Score += points * 2;
        }
        else if (ModelsScript.currentBonus.Any(item => item == ShopItem.x3))
        {
            ModelsScript.Players[commandId].Score += points * 3;
        }
        else if (ModelsScript.currentBonus.Any(item => item == ShopItem.x4))
        {
            ModelsScript.Players[commandId].Score += points * 4;
        }
        else
        {
            ModelsScript.Players[commandId].Score += points;
        }

        if (ModelsScript.lvlsWithBomb.Any(item => item == ModelsScript.currentQuestion) && !ModelsScript.currentBonus.Any(item => item == ShopItem.shield))
        {
            ModelsScript.Players[commandId].Score -= 100;
        }

        if (points < 0)
        {
            ModelsScript.Players[commandId].ProgressBattlePass = 0;
        }
        else if (points > 0)
        {
            ModelsScript.Players[commandId].ProgressBattlePass++;

            if (ModelsScript.Players[commandId].ProgressBattlePass < 3)
            {
                ModelsScript.Players[commandId].ShopScore += 0.5;
            }
            else if (ModelsScript.Players[commandId].ProgressBattlePass >= 3 && ModelsScript.Players[commandId].ProgressBattlePass < 5)
            {
                ModelsScript.Players[commandId].ShopScore += 1;
            }
            else if (ModelsScript.Players[commandId].ProgressBattlePass >= 5)
            {
                ModelsScript.Players[commandId].ShopScore += 1.5;

                if (ModelsScript.Players[commandId].ProgressBattlePass == 7)
                {
                    MainScript.GiveOutPrize(commandId);
                    ModelsScript.Players[commandId].ProgressBattlePass = 0;
                }
            }
        }
    }

    private IEnumerator Timer()
    {
        while (time > 0)
        {
            time -= Time.deltaTime;
            _slider.value = time * 0.04f;
            yield return null;
        }
    }
}