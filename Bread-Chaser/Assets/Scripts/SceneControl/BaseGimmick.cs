using System.Collections;
using UnityEngine;

public abstract class BaseGimmick : MonoBehaviour
{
    protected Coroutine _runningCoroutine;
    protected float minTime;
    protected float maxTime;
    protected Warning warning;
    protected bool _isGimmickActive = false;

    private void Start()
    {
        Init();
    }

    protected virtual void Init()
    {
        warning = Util.FindChild(gameObject, "Warning").GetComponent<Warning>();

        Managers.Game.StateAction -= DestroyCheck;
        Managers.Game.StateAction += DestroyCheck;
        Managers.Game.StateAction -= CoroutineStart;
        Managers.Game.StateAction += CoroutineStart;
    }

    protected IEnumerator RandomActionRoutine()
    {
        while (true)
        {
            float waitTime = Random.Range(minTime, maxTime);

            yield return new WaitForSeconds(waitTime);

            if (!_isGimmickActive)
            {
                ActivetGimmick();
            }
        }
    }

    void CoroutineStart(Define.SceneState sceneState)
    {
        if (sceneState == Define.SceneState.DefaultPlay)
            _runningCoroutine = StartCoroutine(RandomActionRoutine());
    }

    void DestroyCheck(Define.SceneState sceneState)
    {
        if (sceneState == Define.SceneState.Ending)
        {
            Managers.Game.StateAction -= DestroyCheck;
            Managers.Game.StateAction -= CoroutineStart;

            if (_runningCoroutine != null)
                StopCoroutine(_runningCoroutine);

            Destroy(gameObject);
        }
    }

    protected abstract void ActivetGimmick();
    protected abstract void GimmickInstantiate();
}
