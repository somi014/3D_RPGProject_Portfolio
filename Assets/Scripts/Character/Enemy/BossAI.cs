using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossAI : EnemyControl
{
    public bool bossStart = false;                 //보스전 시작인지

    private StatAttribute stats;
    private Animator anim;
    private StatAttribute player;
    private CapsuleCollider col;

    private bool attackLook = false;
    private bool isAttacking = false;                //공격 중인지 체크
    private int currencyHp = -1;
    public int attackCount;
    [HideInInspector] public float scanRange_gui;    //GUI 범위 표시용

    public bool stop = false;                        //테스트 용
    private bool jumpEnd;

    [Header("Hp Bar UI")]
    [SerializeField] private Image hpBar;

    [Header("Drop Item")]
    [SerializeField] private ItemDropList dropList;
    [SerializeField] private float itemDropRange = 2f;

    [Header("Particle")]
    [SerializeField] private ParticleSystem[] attackParticle;
    [SerializeField] private ParticleSystem groundParticle;

    protected override void Init()
    {
        if (TryGetComponent(out StatAttribute stats) == true)
            this.stats = stats;
        if (TryGetComponent(out Animator anim) == true)
            this.anim = anim;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<StatAttribute>();

        if (TryGetComponent(out CapsuleCollider col) == true)
            this.col = col;

        base.Init();
    }

    protected override void Spawn()
    {
        base.Spawn();

        currentState = EnemyState.IDLE;

        attackCount = 0;
        currencyHp = -1;

        deadDrop = false;

        anim.SetBool("Move", true);

        hpBar.fillAmount = Mathf.InverseLerp(0f, stats.lifePool.maxValue.integer_value, stats.lifePool.currentValue);
        hpBar.transform.parent.gameObject.SetActive(false);

        col.isTrigger = false;

    }

    #region state
    protected override void UpdateIdle()
    {
        if (stop == true)
        {
            return;
        }

        if (player.isDead == true)
        {
            return;
        }

        if (bossStart == false)
        {
            return;
        }

        float distance = (player.transform.position - transform.position).magnitude;

        //공격 가능 거리인지 
        bool attackRange = damageDealers[attackCount].viewRadius >= distance;
        _lockTarget = player.gameObject;

        if (attackRange == true)
        {
            currentState = EnemyState.ATTACK;
            anim.SetBool("Move", false);
            anim.SetTrigger("Attack");
        }
        else
        {
            currentState = EnemyState.CHASE;
            anim.SetBool("Move", true);
        }
    }

    protected override void UpdateMoving()
    {
        // 플레이어가 내 사정거리보다 가까우면 공격
        if (_lockTarget != null)
        {
            _destPos = _lockTarget.transform.position;
            float distance = (_destPos - transform.position).magnitude;
            if (distance <= damageDealers[attackCount].viewRadius)
            {
                currentState = EnemyState.ATTACK;
                anim.SetBool("Move", false);
                anim.SetTrigger("Attack");

                return;
            }
        }

        // 이동
        Vector3 dir = _destPos - transform.position;
        //if (dir.magnitude < 0.1f)
        //{
        //    currentState = EnemyState.IDLE;
        //    anim.SetBool("Move", true);
        //}
        //else        // 목적지까지의 거리가 매우 작다면(도착했다면) 이동 중이라는 상태를 false
        {
            float moveDist = Mathf.Clamp(speed * Time.deltaTime, 0, dir.magnitude);
            transform.position += dir.normalized * moveDist;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 10 * Time.deltaTime);

            anim.SetFloat("MoveSpeed", 2f);
        }
    }

    protected override void UpdateSkill()
    {
        if (player.isDead == true)
        {
            _lockTarget = null;
        }

        if (_lockTarget != null)
        {
            if (attackLook == true)
            {
                Vector3 dir = _lockTarget.transform.position - transform.position;
                Quaternion quat = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Lerp(transform.rotation, quat, 20 * Time.deltaTime);
            }

            if (isAttacking == false)
            {
                isAttacking = true;

                anim.SetInteger("AttackNum", attackCount);

                attackCount++;
                if (attackCount >= damageDealers.Length)
                {
                    attackCount = 0;
                }
            }
        }
        else
        {
            anim.SetBool("Move", true);
            anim.SetFloat("MoveSpeed", 0f);
            currentState = EnemyState.IDLE;
        }
    }

    protected override void UpdateDie()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f &&
                    anim.GetCurrentAnimatorStateInfo(0).IsName("Die"))
        {
            ItemSpawnManager.instance.SpawnItem(SelecteRandomPosition(), dropList.GetDrop());

            hpBar.gameObject.SetActive(false);

            gameObject.SetActive(false);
        }
    }
    #endregion

    protected override void UpdateHPBar()
    {
        if (bossStart == true)
        {
            hpBar.transform.parent.gameObject.SetActive(true);
        }

        if (currencyHp != stats.lifePool.currentValue)
        {
            hpBar.fillAmount = Mathf.InverseLerp(0f, stats.lifePool.maxValue.integer_value, stats.lifePool.currentValue);
            currencyHp = stats.lifePool.currentValue;
        }

        if (stats.isDead == true && deadDrop == false)
        {
            deadDrop = true;

            currentState = EnemyState.DIE;

            col.isTrigger = true;

            GameEventsManager.instance.playerEvents.ExperienceGained(5);
        }
    }

    private Vector3 SelecteRandomPosition()
    {
        Vector3 pos = transform.position;

        pos += Vector3.right * Random.Range(0, itemDropRange);
        pos += Vector3.forward * Random.Range(0, itemDropRange);

        return pos;
    }

    #region Animation Event
    public void EndAnimation()
    {
        isAttacking = false;
        if (stats.isDead == false)
        {
            currentState = EnemyState.IDLE;
        }
    }

    public void AttackStart()
    {
        attackLook = false;

        int temp = (attackCount - 1) < 0 ? (damageDealers.Length - 1) : (attackCount - 1);
        damageDealers[temp].StartDealDamage();
    }

    public void AttackEnd()
    {
        attackLook = true;

        int temp = (attackCount - 1) < 0 ? (damageDealers.Length - 1) : (attackCount - 1);
        damageDealers[temp].EndDealDamage();
    }

    private void AttackEffect()
    {
        for (int i = 0; i < attackParticle.Length; i++)
        {
            attackParticle[i].Play();
        }
    }

    private void GroundEffect()
    {
        groundParticle.Play();

        //camera shake
    }


    public void JumpMove()
    {
        jumpEnd = true;

        StartCoroutine(IEJumpMove());
    }

    public void JumpEnd()
    {
        jumpEnd = false;
    }

    private IEnumerator IEJumpMove()
    {
        Vector3 dir = transform.forward * 3f;
        do
        {
            float moveDist = Mathf.Clamp(speed * Time.deltaTime, 0, dir.magnitude);
            transform.position += dir.normalized * moveDist;

            yield return null;

        } while (jumpEnd == true && stats.isDead == false);
    }
    #endregion
}