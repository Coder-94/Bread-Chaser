using System;
using System.Collections;
using UnityEngine;

public class Warning : MonoBehaviour
{
    public Action WarnEnded;

    SpriteRenderer      _sprite;
    float               duration = 3f;
    public float        blinkInterval = 0.3f;
    void Start()
    {
        Init();
    }

    void Init()
    {
        _sprite = GetComponent<SpriteRenderer>();
        ToggleVisibility(false);
    }

    public void WarningInit()
    {
        StartCoroutine(Blinker());
    }

    IEnumerator Blinker()
    {
        float timer = 0f;

        while (timer < duration)
        {
            ToggleVisibility(true);

            yield return new WaitForSeconds(blinkInterval);

            ToggleVisibility(false);

            yield return new WaitForSeconds(blinkInterval);

            timer += blinkInterval * 2;
        }

        WarnEnded.Invoke();
        //Object Hiding
        ToggleVisibility(false);
    }

    public void ToggleVisibility(bool isVisible, bool hardStop = false)
    {
        if(hardStop)
            StopCoroutine(Blinker());
        _sprite.enabled = isVisible;
    }
}
