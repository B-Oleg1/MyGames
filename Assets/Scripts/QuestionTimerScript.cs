using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class QuestionTimerScript : MonoBehaviour
{
    [SerializeField]
    private Image _timer;

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
            _timer.fillAmount = 1;

            StartCoroutine(ienum);
        }
        if (Input.GetKey(KeyCode.E) && coroutineIsStarted)
        {
            StopCoroutine(ienum);
            coroutineIsStarted = false;
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            if (transform.GetChild(0).GetChild(0).GetChild(1).GetComponentInChildren<AudioSource>() &&
                !transform.GetChild(0).GetChild(0).GetChild(1).GetComponentInChildren<AudioSource>().isPlaying)
            {
                transform.GetChild(0).GetChild(0).GetChild(1).GetComponentInChildren<AudioSource>().Play();
            }
            else if (transform.GetChild(0).GetChild(0).GetChild(1).GetComponentInChildren<AudioSource>() &&
                transform.GetChild(0).GetChild(0).GetChild(1).GetComponentInChildren<AudioSource>().isPlaying)
            {
                transform.GetChild(0).GetChild(0).GetChild(1).GetComponentInChildren<AudioSource>().Pause();
            }

            if (transform.GetChild(0).GetChild(0).GetChild(1).GetComponentInChildren<VideoPlayer>() &&
                !transform.GetChild(0).GetChild(0).GetChild(1).GetComponentInChildren<VideoPlayer>().isPlaying)
            {
                transform.GetChild(0).GetChild(0).GetChild(1).GetComponentInChildren<VideoPlayer>().Play();
            }
            else if (transform.GetChild(0).GetChild(0).GetChild(1).GetComponentInChildren<VideoPlayer>() &&
                transform.GetChild(0).GetChild(0).GetChild(1).GetComponentInChildren<VideoPlayer>().isPlaying)
            {
                transform.GetChild(0).GetChild(0).GetChild(1).GetComponentInChildren<VideoPlayer>().Pause();
            }
        }

        if (Input.GetKey(KeyCode.Q))
        {
            transform.GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(true);
            transform.GetChild(0).GetChild(0).GetChild(1).gameObject.SetActive(false);
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
    }

    private IEnumerator End(int points)
    {
        int commandId = ModelsScript.currentCommandIdResponds;

        ScoringPoints(points, commandId);

        yield return new WaitForSeconds(1.5f);

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
        ModelsScript.needUpdateCommandId = commandId;
        ModelsScript.currentBonus.Clear();

        ModelsScript.mainMusic.Play();

        _questionCounted = false;

        for (int i = 0; i < transform.GetChild(0).GetChild(0).childCount; i++)
        {
            for (int a = 0; a < transform.GetChild(0).GetChild(0).GetChild(i).childCount; a++)
            {
                Destroy(transform.GetChild(0).GetChild(0).GetChild(i).GetChild(a).gameObject);
            }
        }

        transform.GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(false);
        transform.GetChild(0).GetChild(0).GetChild(1).gameObject.SetActive(true);

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
                ModelsScript.Players[ModelsScript.currentCommandIdResponds].Items.Add((ShopItem)Random.Range(0, 12));
            }
        }
    }

    private IEnumerator Timer()
    {
        while (time > 0)
        {
            time -= Time.deltaTime;
            _timer.fillAmount = time * 0.04f;
            yield return null;
        }
    }
}