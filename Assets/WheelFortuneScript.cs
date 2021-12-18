using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WheelFortuneScript : MonoBehaviour
{
    private int _numberOfTurns;
    private float _prize;

    private float _speed;

    private bool _canTurn;

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
    }

    private IEnumerator TurnWheel()
    {
        _canTurn = false;

        _numberOfTurns = Random.Range(500, 1000);

        _speed = 0.1f;

        for (int i = 0; i < _numberOfTurns; i++)
        {
            transform.Rotate(0, 0, _speed * 100);

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

        _prize = transform.eulerAngles.z;

        if (_prize >= 319 || _prize <= 18.7f)
        {
            // ������
        }
        else if (_prize > 18.7f && _prize <= 78.6f)
        {
            // �����
        }
        else if (_prize > 78.6f && _prize <= 138.7f)
        {
            // ����
        }
        else if (_prize > 138.7f && _prize <= 198.8f)
        {
            // ����������
        }
        else if (_prize > 198.8f && _prize <= 259.9f)
        {
            // �������
        }
        else if (_prize > 259.9f && _prize <= 259.9f)
        {
            // ���� ���
        }

        print(_prize);

        switch (_prize)
        {
            case 0:
                break;
            case 60:
                break;
            case 120:
                break;
            case 180:
                break;
            case 240:
                break;
            case 300:
                break;
            default:
                break;
        }

        _canTurn = true;
    }
}
