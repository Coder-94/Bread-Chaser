using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class IceLandGimmick : BaseGimmick
{
    private int sequenceCount = 5;
    private float inputTimeLimit = 13.0f;
    private float damageOnFail = 6f;
    private float displayDuration = 0.5f;

    private List<int> _targetSequence = new List<int>();
    private int _displayIndex = 0;

    protected override void Init()
    {
        base.Init();
        minTime = 15.0f;
        maxTime = 19.0f;
    }

    protected override void ActivetGimmick()
    {
        if (_isGimmickActive) return;
        _isGimmickActive = true;

        _targetSequence.Clear();
        float[] rails = Managers.Scene.CurrentScene.railLineX;
        for (int i = 0; i < sequenceCount; i++)
        {
            _targetSequence.Add(Random.Range(0, rails.Length));
        }

        if (warning != null)
        {
            warning.warningDuration = displayDuration;

            warning.WarnEnded -= ShowNextSequence;
            warning.WarnEnded += ShowNextSequence;
        }

        _displayIndex = 0;
        ShowNextSequence();
    }


    private void ShowNextSequence()
    {
        if (_displayIndex >= sequenceCount)
        {
            if (warning != null)
            {
                warning.WarnEnded -= ShowNextSequence;
            }
            StartCoroutine(InputCheckRoutine());
            return;
        }


        float[] rails = Managers.Scene.CurrentScene.railLineX;
        int targetRailIdx = _targetSequence[_displayIndex];

        if (warning != null)
        {
            warning.transform.position = new Vector3(rails[targetRailIdx], warning.transform.position.y, warning.transform.position.z);

            warning.WarningInit(true);
        }

        _displayIndex++;
    }

    //ForDamage
    protected override void GimmickInstantiate()
    {
        Managers.Game.GetPlayer().GetComponent<PlayerStat>().OnPlAttacked(gameObject, damageOnFail);
    }

    IEnumerator InputCheckRoutine()
    {
        int currentIndex = 0;
        float timer = 0f;


        while (timer < inputTimeLimit && currentIndex < sequenceCount)
        {
            timer += Time.deltaTime;

            GameObject player = Managers.Game.GetPlayer();
            if (player == null) yield break;

            int playerRailIndex = GetClosestRailIndex(player.transform.position.x);

            if (playerRailIndex == _targetSequence[currentIndex])
            {
                currentIndex++;
                Managers.Sound.Play("SE/Correct");
                yield return new WaitForSeconds(0.3f);
            }

            yield return null;
        }


        if (currentIndex >= sequenceCount)
        {
        }
        else
        {
            Managers.Sound.Play("SE/NotCorrect");
            yield return new WaitForSeconds(0.3f);
            GimmickInstantiate();
        }

        _isGimmickActive = false;
    }

    private int GetClosestRailIndex(float playerX)
    {
        float[] rails = Managers.Scene.CurrentScene.railLineX;
        int closestIndex = -1;
        float minDist = float.MaxValue;

        for (int i = 0; i < rails.Length; i++)
        {
            float dist = Mathf.Abs(rails[i] - playerX);
            if (dist < minDist)
            {
                minDist = dist;
                closestIndex = i;
            }
        }
        return closestIndex;
    }
}
