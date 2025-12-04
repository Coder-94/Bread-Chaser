using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using static Define;
using static UnityEngine.GraphicsBuffer;

public class PlayerController : PlayerBase
{
    #region player control

    //input control ======================================================================================
    protected override void PlayerControl(Define.TouchEvent evt)
    {
        switch (CurrentState)
        {
            case Define.PlayerStatus.Running:
                Running(evt);
                break;
            case Define.PlayerStatus.LockOning:
                LockOn(evt);
                break;
            case Define.PlayerStatus.Attack:
                Attacking(evt);
                break;
            case Define.PlayerStatus.Jumping:
                if(_isJumping)
                    return;
                break;
        }
    }

    //run ======================================================================================
    void Running(Define.TouchEvent evt)
    {
        switch (evt)
        {
            case Define.TouchEvent.UpSwipe:
                CurrentState = Define.PlayerStatus.Jumping;
                break;
            case Define.TouchEvent.LeftSwipe:
                SideMove(evt);
                break;
            case Define.TouchEvent.RightSwipe:
                SideMove(evt);
                break;
            case Define.TouchEvent.StartHolding:
                CurrentState = Define.PlayerStatus.LockOning;
                break;
        }
    }

    //sideMove ======================================================================================
    void SideMove(Define.TouchEvent evt)
    {
        if(evt == Define.TouchEvent.LeftSwipe && _railPos != Define.PLRailPos.FirstRail)
        {
            _railPos--;
            CurrentState = Define.PlayerStatus.LeftMoving;
        }
        else if(evt == Define.TouchEvent.RightSwipe && _railPos != Define.PLRailPos.FourthRail)
        {
            _railPos++;
            CurrentState = Define.PlayerStatus.RightMoving;
        }
    }

    //lock on ======================================================================================
    protected void LockOn(Define.TouchEvent evt)
    {

        if(evt == Define.TouchEvent.HoldedFingerReleased)
        {
            _target = _targeting.GetComponent<LockOn>().GetTarget();

            if (_target == null)
                CurrentState = Define.PlayerStatus.Running;
            else
                CurrentState = Define.PlayerStatus.Attacking;

            _targeting = null;
        }
    }

    //atk ======================================================================================

    void Attacking(Define.TouchEvent evt)
    {
        if (evt == Define.TouchEvent.DownSwipe)
            CurrentState = Define.PlayerStatus.BackStepping;

        if (TargetNotDead)
        {
            if (!_touchBlock && evt == Define.TouchEvent.Tap)
            {
                if (_stat.EvolutionData[Define.IncreaseAbleStat.SkillDMG].FirstEvolve)
                {
                    _anim.SetTrigger(_hashedParams[(int)AnimParameters.TriggerRoundAtk]);
                    _touchBlock = true;
                }
                else
                {
                    int rand = UnityEngine.Random.Range(0, 2);

                    if (rand == 0)
                        _anim.SetTrigger(_hashedParams[(int)AnimParameters.TriggerLeftAtk]);
                    else
                        _anim.SetTrigger(_hashedParams[(int)AnimParameters.TriggerRightAtk]);

                    _touchBlock = true;
                }
                    
            }
        }
    }

    protected void Atk()
    {
        if (_target != null)
        {
            Vector3 dir = _target.transform.position - transform.position;
            dir.y = 0;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 10 * Time.deltaTime);
        }

        if (_target == null || _target.GetComponent<MobStat>().CurrentHp <= 0)
        {
            CurrentState = Define.PlayerStatus.BackStepping;
        }

    }

    #endregion

    #region player act

    //state control ======================================================================================
    protected override void PlayerActor() 
    {
        if(_stat.CurrentHp <= 0)
        {
            CurrentState = Define.PlayerStatus.Die;
            return;
        }

        switch (CurrentState)
        {
            case Define.PlayerStatus.Running:
                break;
            case Define.PlayerStatus.Attack:
                Atk();
                break;
            case Define.PlayerStatus.BackStepping:
                break;
            case Define.PlayerStatus.LockOning:
                break;
            case Define.PlayerStatus.Jumping:
                break;
            case Define.PlayerStatus.BossAtk:
                break;
        }
    }

    
    #endregion

    #region rb

    //rb control ======================================================================================
    protected override void RbControl()
    {
        switch (CurrentState)
        {
            case Define.PlayerStatus.Attacking:
                AttackRB();
                break;
            case Define.PlayerStatus.BackStepping:
                BackStepRB();
                break;
            case Define.PlayerStatus.LeftMoving:
                SideMoveRB();
                break;
            case Define.PlayerStatus.RightMoving:
                SideMoveRB();
                break;
            case Define.PlayerStatus.Damaged:
                BackStepRB();
                break;
            case Define.PlayerStatus.BossAtk:
                AttackRB();
                break;
        }

        //Mission Delay =====================================================================================
        while (_movementQueue.Count > 0)
        {
            Action action = _movementQueue.Dequeue();
            action?.Invoke();
        }
    }

    //sideMoving ======================================================================================
    void SideMoveRB()
    {
        Vector3 dest = new Vector3(Managers.Scene.CurrentScene.railLineX[(int)_railPos], 0, 0);
        Vector3 currentPos = _rb.position;

        Vector3 dir = dest - currentPos;

        float dist = dir.magnitude;

        if (Math.Abs(dir.x) > 0.01f)
        {
            float moveStep = 15f * Time.fixedDeltaTime;
            float moveAmount = Mathf.Min(moveStep, dist);

            Vector3 moveDir = dir.normalized * moveAmount;
            _rb.MovePosition(currentPos + moveDir);
        }
        else
        {
            CurrentState = Define.PlayerStatus.Running;
            _stat.InvincibleProcess(false, 0.5f);
        }
            
    }

    //rb atk ======================================================================================
    void AttackRB()
    {
        GameObject currentBoss = Managers.Scene.CurrentScene.CurrentBoss;

        if (_target != null)
        {
            Vector3 targetPos = _target.transform.position;
            Vector3 currentPos = _rb.position;

            targetPos.y = currentPos.y;

            Vector3 direction = (targetPos - currentPos).normalized;

            float dist = Vector3.Distance(currentPos, targetPos);

            if (dist > 1.0f)
            {
                float moveStep = 15f * Time.fixedDeltaTime;
                float moveAmount = Mathf.Min(moveStep, dist - 1.0f);

                Vector3 move = direction * moveAmount;
                _rb.MovePosition(currentPos + move);
            }
        }
        //bossmob Atk
        else if (_target == null && currentBoss != null)
        {
            Vector3 targetPos = currentBoss.transform.position;
            Vector3 currentPos = _rb.position;
            targetPos.z -= 1f;

            Vector3 direction = (targetPos - currentPos).normalized;

            float dist = Vector3.Distance(currentPos, targetPos);

            if (dist > 1.0f)
            {
                float moveStep = 15f * Time.fixedDeltaTime;
                float moveAmount = Mathf.Min(moveStep, dist - 1.0f);

                Vector3 move = direction * moveAmount;
                _rb.MovePosition(currentPos + move);

                _rb.useGravity = false;
                _camController.AtkSetting(true);
            }
        }
    }

    // rb backstep ======================================================================================
    void BackStepRB()
    {
        if (OriginPos == new Vector3(9999, 9999, 9999))
            return;

        Vector3 currentPos = _rb.position;
        Vector3 dir = OriginPos - currentPos;
        if(_rb.useGravity)
            dir.y = 0;

        float dist = dir.magnitude;

        if (dist > 0.01f)
        {
            float moveStep = 15f * Time.fixedDeltaTime;
            float moveAmount = Mathf.Min(moveStep, dist);

            Vector3 moveDir = dir.normalized * moveAmount;
            _rb.MovePosition(currentPos + moveDir);
        }
        else
        {
            if (OriginPos != new Vector3(9999, 9999, 9999))
                OriginPos = new Vector3(9999, 9999, 9999);

            if(!_rb.useGravity)
                _rb.useGravity = true;

            StartCoroutine(_stat.InvincibleProcess(false, 0.5f));
            CurrentState = Define.PlayerStatus.Running;
        }
            
    }
    #endregion

    #region jump

    //jump rb ======================================================================================
    protected override void JumpRB()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.AddForce(Vector3.up * 5f, ForceMode.Impulse);
    }

    #endregion


    #region anim Events

    public void OnAttack()
    {
        if(_target != null)
        {
            Stat targetStat = _target.GetComponent<MobStat>();

            float multiplier = (_state == Define.PlayerStatus.Attack) ? 0.7f : 1.0f;
            bool isTargetAlive = targetStat.OnEnemAttacked(gameObject, multiplier);

            TargetNotDead = isTargetAlive;

            _camController.AtkSetting(TargetNotDead);
            _camController.CamShake(10f, 5f, 0.2f);

            Vector3 targetPos = _target.GetComponent<MobController>().targetedPos.transform.position;
            targetPos.z -= 0.5f;

            HandleAttackEffects(targetStat);
        }

        if(_touchBlock)
            _touchBlock = false;
        if (CurrentState != Define.PlayerStatus.BackStepping)
            CurrentState = Define.PlayerStatus.Attack;
    }

    public void OnRoundAttack()
    {
        float   attackRange = 3.5f;
        int maxTargets = _stat.EvolutionData[Define.IncreaseAbleStat.SkillDMG].SecondEvolve ? 6 : 3;

        Collider[] enemiesInRange = new Collider[maxTargets];

        int hitCount = Physics.OverlapSphereNonAlloc(transform.position, attackRange, enemiesInRange, _enemyMask);

        if (hitCount > 0)
        {
            List<Transform> closestEnemies = enemiesInRange
                .Where(enemy => enemy != null)
                .OrderBy(enemy => Vector3.Distance(transform.position, enemy.transform.position))
                .Take(maxTargets)
                .Select(enemy => enemy.transform)
                .ToList();

            if (_punchEffect == null)
                _punchEffect = Managers.Resource.Instantiate("Effect/SkillDMGAtk");

            Vector3 pos = gameObject.transform.position;
            pos.y += 0.5f;
            _punchEffect.transform.position = pos;
            _punchEffect.GetComponent<ParticleSystem>().Play();

            foreach (Transform target in closestEnemies)
            {
                Stat targetStat = target.GetComponent<Stat>();

                if (targetStat != null)
                {
                    bool isAlive = targetStat.OnEnemAttacked(gameObject);

                    if (_target != null && target.gameObject == _target)
                    {
                        TargetNotDead = isAlive;
                        _camController.AtkSetting(TargetNotDead);
                    }

                    ApplyHitLogic(targetStat);
                }
            }

            if (_stat.EvolutionData[Define.IncreaseAbleStat.Atk].FirstEvolve)
                CurrentState = Define.PlayerStatus.BackStepping;

            if (_stat.IsAtkBuffed)
            {
                Managers.Sound.Play($"SE/HardHit");
                _camController.CamShake(10f, 10f, 0.2f);
            }
            else
            {
                Managers.Sound.Play($"SE/Hit");
                _camController.CamShake(10f, 5f, 0.2f);
            }
        }

        if (_touchBlock)
            _touchBlock = false;
        if (CurrentState != Define.PlayerStatus.BackStepping)
            CurrentState = Define.PlayerStatus.Attack;
    }

    public void OnBossAtk()
    {
        GameObject currentBoss = Managers.Scene.CurrentScene.CurrentBoss;
        if (currentBoss != null)
        {
            Stat targetStat = currentBoss.GetComponent<MobStat>();
            
            targetStat.OnBossAttacked();

            _camController.CamShake(10f, 20f, 0.2f);

            Vector3 targetPos = currentBoss.GetComponent<MobController>().targetedPos.transform.position;
            targetPos.z -= 0.5f;

            HandleAttackEffects(targetStat, true);
        }

        if (_touchBlock)
            _touchBlock = false;

        _camController.AtkSetting(false);
        CurrentState = Define.PlayerStatus.BackStepping;
    }

    #region effect

    void HandleAttackEffects(Stat targetStat, bool isBossAtk = false)
    {
        if (isBossAtk)
        {
            GameObject currentBoss = Managers.Scene.CurrentScene.CurrentBoss;

            if (currentBoss != null)
            {
                Vector3 bossTargetPos = currentBoss.GetComponent<MobController>().targetedPos.transform.position;

                if (_bossHitEffect == null)
                    _bossHitEffect = Managers.Resource.Instantiate("Effect/BossHit");

                _bossHitEffect.transform.position = bossTargetPos;
                _bossHitEffect.GetComponent<ParticleSystem>().Play();

                Managers.Sound.Play($"SE/HardHit");
            }
            return;
        }

        // ====================================================
        // _target Logic
        // ====================================================

        Vector3 targetPos = _target.GetComponent<MobController>().targetedPos.transform.position;
        targetPos.z -= 0.5f;

        ApplyHitLogic(targetStat);

        if (_stat.EvolutionData[Define.IncreaseAbleStat.Atk].FirstEvolve)
            CurrentState = Define.PlayerStatus.BackStepping;

        string effectPath = "Effect/DefaultAtk";

        if (_stat.EvolutionData[Define.IncreaseAbleStat.Atk].FirstEvolve)
            effectPath = "Effect/AtkAtk";

        if (_stat.EvolutionData[Define.IncreaseAbleStat.Hp].FirstEvolve)
            effectPath = "Effect/HpAtk";

        if (_stat.EvolutionData[Define.IncreaseAbleStat.MoveSpd].FirstEvolve)
            effectPath = "Effect/SpdAtk";

        if (_stat.IsAtkBuffed)
        {
            Managers.Sound.Play($"SE/HardHit");

            GameObject punchEffect = Managers.Resource.Instantiate("Effect/BuffAtk");
            punchEffect.transform.position = targetPos;
            punchEffect.GetComponent<ParticleSystem>().Play();

            return;
        }
        else
        {
            Managers.Sound.Play($"SE/Hit");

            GameObject punchEffect = Managers.Resource.Instantiate(effectPath);
            punchEffect.transform.position = targetPos;
            punchEffect.GetComponent<ParticleSystem>().Play();

            return;
        }
    }

    void ApplyHitLogic(Stat targetStat)
    {
        if (_stat.EvolutionData[Define.IncreaseAbleStat.MoveSpd].FirstEvolve)
        {
            targetStat.OnPoisoned(gameObject);
        }

        if (_stat.EvolutionData[Define.IncreaseAbleStat.Hp].FirstEvolve)
        {
            // 체력 관련 스킬 로직
        }
    }

    #endregion

    #region active skill

    public void ActiveSkill()
    { 
        Managers.Resource.Instantiate($"Effect/Skill/{EvolvedType}Skill", null, 1); 

        if(EvolvedType != Define.IncreaseAbleStat.Atk)
        {
            _camController.CamShake(10f, 20f, 0.2f);
            CurrentState = Define.PlayerStatus.Running;
        }
            
    }

    public void SkillOpen()
    {
        if (!IsSkillCool && CurrentState == Define.PlayerStatus.Running)
        {
            if (EvolvedType == Define.IncreaseAbleStat.Atk)
                _anim.SetTrigger(_hashedParams[(int)AnimParameters.TriggerPassive]);
            else
                CurrentState = Define.PlayerStatus.Channeling;

            if (CoolTime > 0)
            {
                StartCoroutine(CooldownCoroutine(CoolTime));
            }
        }
    }

    public void BossAtkTrigger() { CurrentState = Define.PlayerStatus.BossAtk; }

    private IEnumerator CooldownCoroutine(float duration)
    {
        IsSkillCool = true;

        yield return new WaitForSeconds(duration);

        IsSkillCool = false;
    }
    #endregion

    #endregion

}