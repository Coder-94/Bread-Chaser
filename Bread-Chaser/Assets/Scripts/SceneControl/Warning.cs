using System;
using System.Collections;
using UnityEngine;

public class Warning : MonoBehaviour
{
    public Action WarnEnded;

    SpriteRenderer[] _sprites;
    public float        warningDuration = 3f;
    public float        blinkInterval = 0.3f;
    void Start()
    {
        Init();
    }

    void Init()
    {
        _sprites = GetComponentsInChildren<SpriteRenderer>(true);
        ToggleVisibility(false);
    }

    public void WarningInit(bool isKeepVisible = false)
    {
        if (_sprites == null || _sprites.Length == 0)
            _sprites = GetComponentsInChildren<SpriteRenderer>(true);

        StopAllCoroutines();
        StartCoroutine(Blinker(isKeepVisible));
    }

    IEnumerator Blinker(bool isKeepVisible = false)
    {
        Managers.Sound.Play("SE/Warning");

        if (isKeepVisible)
        {
            ToggleVisibility(true);

            yield return new WaitForSeconds(warningDuration);
        }
        else
        {
            float timer = 0f;

            while (timer < warningDuration)
            {
                ToggleVisibility(true);

                yield return new WaitForSeconds(blinkInterval);

                ToggleVisibility(false);

                yield return new WaitForSeconds(blinkInterval);

                timer += blinkInterval * 2;
            }
        }
        //Object Hiding
        ToggleVisibility(false);
        WarnEnded.Invoke();
    }

    public void ToggleVisibility(bool isVisible, bool hardStop = false)
    {
        if (_sprites != null)
        {
            foreach (var sprite in _sprites)
            {
                if (sprite != null)
                    sprite.enabled = isVisible;
            }
        }

        if (hardStop)
            StopCoroutine("Blinker");
    }
}
