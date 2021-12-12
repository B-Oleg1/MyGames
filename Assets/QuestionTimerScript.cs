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

    private void Update()
    {
        if (Input.GetKey(KeyCode.R))
        {
            StopCoroutine(Timer());
            StartTimer();
        }

        if (Input.GetKey(KeyCode.Alpha1))
        {
            End(ModelsScript.currentPointsForQuestions, 0);
        }
        else if (Input.GetKey(KeyCode.Alpha2))
        {
            End(ModelsScript.currentPointsForQuestions * -1, 0);
        }
        else if (Input.GetKey(KeyCode.Alpha3))
        {
            End(ModelsScript.currentPointsForQuestions, 1);
        }
        else if (Input.GetKey(KeyCode.Alpha4))
        {
            End(ModelsScript.currentPointsForQuestions * -1, 1);
        }
        else if (Input.GetKey(KeyCode.Alpha5))
        {
            End(ModelsScript.currentPointsForQuestions, 2);
        }
        else if (Input.GetKey(KeyCode.Alpha6))
        {
            End(ModelsScript.currentPointsForQuestions * -1, 2);
        }
    }

    private void End(int points, int commandId)
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

        ModelsScript.allSceneWithQuestion[ModelsScript.currentQuestion + 1].gameObject.SetActive(false);
        _objectWithQuestions.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    public void StartTimer()
    {
        _slider.value = 1;
        StartCoroutine(Timer());
    }

    private IEnumerator Timer()
    {
        while (_slider.value > 0)
        {
            _slider.value -= Time.deltaTime / 90;
            yield return null;
        }
    }
}
