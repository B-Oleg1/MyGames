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
    private IEnumerator ienum = null;
    private bool coroutineIsStarted = false;

    private float time = 25f;

    private void Update()
    {
        if (Input.GetKey(KeyCode.R) && !coroutineIsStarted)
        {
            ienum = null;
            ienum = Timer();
            coroutineIsStarted = true;
            time = 25f;
            _slider.value = 1;
            if (ModelsScript.currentQuestion >= 49 && ModelsScript.currentQuestion <= 55 && !ModelsScript.allSceneWithQuestion[ModelsScript.currentQuestion].GetChild(1).GetChild(0)
                    .GetComponent<AudioSource>().isPlaying)
            {
                ModelsScript.allSceneWithQuestion[ModelsScript.currentQuestion].GetChild(1).GetChild(0)
                    .GetComponent<AudioSource>().Play();
            }
            StartCoroutine(ienum);
        }
        if (Input.GetKey(KeyCode.E) && coroutineIsStarted)
        {
            StopCoroutine(ienum);
            coroutineIsStarted = false;

            if (ModelsScript.currentQuestion >= 49 && ModelsScript.currentQuestion <= 55 && ModelsScript.allSceneWithQuestion[ModelsScript.currentQuestion].GetChild(1).GetChild(0)
                    .GetComponent<AudioSource>().isPlaying)
            {
                ModelsScript.allSceneWithQuestion[ModelsScript.currentQuestion].GetChild(1).GetChild(0)
                    .GetComponent<AudioSource>().Pause();
            }
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
            StartCoroutine(End(ModelsScript.currentPointsForQuestions));
        }
        else if (Input.GetKey(KeyCode.Alpha2) && !_questionCounted)
        {
            _questionCounted = true;
            StartCoroutine(End(ModelsScript.currentPointsForQuestions / 2));
        }
        else if (Input.GetKey(KeyCode.Alpha3) && !_questionCounted)
        {
            _questionCounted = true;
            StartCoroutine(End(ModelsScript.currentPointsForQuestions * -1));
        }

        //if (Input.GetKey(KeyCode.Alpha1) && !_questionCounted)
        //{
        //    _questionCounted = true;
        //    StartCoroutine(End(ModelsScript.currentPointsForQuestions, 0));
        //}
        //else if (Input.GetKey(KeyCode.Alpha2) && !_questionCounted)
        //{
        //    _questionCounted = true;
        //    StartCoroutine(End(ModelsScript.currentPointsForQuestions * -1, 0));
        //}
        //else if (Input.GetKey(KeyCode.Alpha3) && !_questionCounted)
        //{
        //    _questionCounted = true;
        //    StartCoroutine(End(ModelsScript.currentPointsForQuestions, 1));
        //}
        //else if (Input.GetKey(KeyCode.Alpha4) && !_questionCounted)
        //{
        //    _questionCounted = true;
        //    StartCoroutine(End(ModelsScript.currentPointsForQuestions * -1, 1));
        //}
        //else if (Input.GetKey(KeyCode.Alpha5) && !_questionCounted)
        //{
        //    _questionCounted = true;
        //    StartCoroutine(End(ModelsScript.currentPointsForQuestions, 2));
        //}
        //else if (Input.GetKey(KeyCode.Alpha6) && !_questionCounted)
        //{
        //    _questionCounted = true;
        //    StartCoroutine(End(ModelsScript.currentPointsForQuestions * -1, 2));
        //}
        //else if (Input.GetKey(KeyCode.Alpha8) && !_questionCounted)
        //{
        //    _questionCounted = true;
        //    StartCoroutine(End(ModelsScript.currentPointsForQuestions / 2, 0));
        //}
        //else if (Input.GetKey(KeyCode.Alpha9) && !_questionCounted)
        //{
        //    _questionCounted = true;
        //    StartCoroutine(End(ModelsScript.currentPointsForQuestions / 2, 1));
        //}
        //else if (Input.GetKey(KeyCode.Alpha0) && !_questionCounted)
        //{
        //    _questionCounted = true;
        //    StartCoroutine(End(ModelsScript.currentPointsForQuestions / 2, 2));
        //}
    }

    private IEnumerator End(int points)
    {
        int commandId = ModelsScript.currentCommandIdResponds;

        ScoringPoints(points, commandId);

        ModelsScript.needUpdateCommandId = commandId;

        yield return new WaitForSeconds(1.5f);

        ModelsScript.allSceneWithQuestion[ModelsScript.currentQuestion].gameObject.SetActive(false);
        _objectWithQuestions.gameObject.SetActive(false);

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

        for (int i = 0; i < ModelsScript.Players.Count; i++)
        {
            if (ModelsScript.Players[i].WearingArmor > 0)
            {
                ModelsScript.Players[i].WearingArmor--;
            }
        }

        if (!ModelsScript.currentBonus.Any(item => item == ShopItem.doublemove))
        {
            ModelsScript.currentCommandIdMove = (ModelsScript.currentCommandIdMove + 1) % 3;
        }
        ModelsScript.currentCommandIdResponds = ModelsScript.currentCommandIdMove;

        ModelsScript.currentBonus.Clear();

        ModelsScript.mainMusic.Play();

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
            ModelsScript.supportMusic.clip = ModelsScript.allClips[2];
            ModelsScript.supportMusic.Play();

            ModelsScript.Players[commandId].ShopScore += 0.5;

            ModelsScript.Players[commandId].ProgressBattlePass = 0;
        }
        else if (points > 0)
        {
            ModelsScript.supportMusic.clip = ModelsScript.allClips[1];
            ModelsScript.supportMusic.Play();

            ModelsScript.Players[commandId].ProgressBattlePass++;

            if (ModelsScript.Players[commandId].ProgressBattlePass < 3)
            {
                ModelsScript.Players[commandId].ShopScore += 1;
            }
            else if (ModelsScript.Players[commandId].ProgressBattlePass >= 3 && ModelsScript.Players[commandId].ProgressBattlePass < 5)
            {
                ModelsScript.Players[commandId].ShopScore += 1.5;
            }
            else if (ModelsScript.Players[commandId].ProgressBattlePass >= 5 && ModelsScript.Players[commandId].ProgressBattlePass < 6)
            {
                ModelsScript.Players[commandId].ShopScore += 2;
            }
            else if (ModelsScript.Players[commandId].ProgressBattlePass >= 6)
            {
                ModelsScript.Players[commandId].ShopScore += 2.5;

                if (ModelsScript.Players[commandId].ProgressBattlePass == 7)
                {
                    MainScript.GiveOutPrize(commandId);
                    ModelsScript.Players[commandId].ProgressBattlePass = 0;
                }
            }

            if ((ModelsScript.currentQuestion + 1) % 7 == 0)
            {
                ModelsScript.Players[commandId].ShopScore += 3;
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