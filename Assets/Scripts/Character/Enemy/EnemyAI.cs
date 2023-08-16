using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class EnemyAI : EnemyControl
{
    private StatAttribute stats;
    private Animator anim;
    private StatAttribute player;
    private EnemySpawn spawn;
    private CapsuleCollider col;

    private bool attackLook = false;
    private bool isAttacking = false;                //공격 중인지 체크
    private int currencyHp = -1;
    private int attackCount;
    [HideInInspector]
    public float scanRange_gui;                     //GUI 범위 표시용

    public bool stop = false;                        //테스트 용
    
    [Header("Hp Bar UI")]
    [SerializeField] 
    private Image hpBar;

    private Vector3 originPos;

    [Header("Drop Item")]
    [SerializeField] 
    private ItemDropList dropList;
    [SerializeField]
    private float itemDropRange = 2f;

    protected override void Init()
    {
        if (TryGetComponent(out StatAttribute statsAttribute) == true)
        {
            stats = statsAttribute;
        }
        if (TryGetComponent(out Animator animator) == true)
        {
            anim = animator;
        }
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<StatAttribute>();

        spawn = transform.parent.GetComponent<EnemySpawn>();

        if (TryGetComponent(out CapsuleCollider capsuleCollider) == true)
        {
            col = capsuleCollider;
        }

        scanRange_gui = scanRange;
        originPos = transform.position;

        base.Init();
    }

    protected override void Spawn()
    {
        base.Spawn();

        currentState = EnemyState.IDLE;

        currencyHp = -1;

        deadDrop = false;
        isReturn = false;

        anim.SetBool("Move", true);

        hpBar.fillAmount = Mathf.InverseLerp(0f, stats.lifePool.maxValue.integer_value, stats.lifePool.currentValue);

        col.isTrigger = false;

        originPos = transform.position;
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

        float distance = (player.transform.position - transform.position).magnitude;
        if (distance <= scanRange)
        {
            lockTarget = player.gameObject;

            currentState = EnemyState.CHASE;
            anim.SetBool("Move", true);
        }
        else
        {
            anim.SetBool("Move", true);
            Vector3 originDis = originPos - transform.position;
            if (originDis.magnitude >= 1f)
            {
                currentState = EnemyState.CHASE;
            }
            else
            {
                anim.SetFloat("MoveSpeed", 0f);
                isReturn = false;
            }
        }
    }

    protected override void UpdateMoving()
    {
        // 플레이어가 내 사정거리보다 가까우면 공격
        if (lockTarget != null)
        {
            destPos = lockTarget.transform.position;
            float distance = (destPos - transform.position).magnitude;
            if (distance <= attackRange)
            {
                currentState = EnemyState.ATTACK;
                anim.SetBool("Move", false);
                anim.SetTrigger("Attack");

                return;
            }
        }

        float originDis = (originPos - transform.position).magnitude;
        if (originDis > scanRange + 2f)
        {
            isReturn = true;
        }

        if (isReturn == true)
        {
            destPos = originPos;
        }

        // 이동
        Vector3 dir = destPos - transform.position;
        if (dir.magnitude < 0.1f)
        {
            currentState = EnemyState.IDLE;
            anim.SetBool("Move", true);
        }
        else        // 목적지까지의 거리가 매우 작다면(도착했다면) 이동 중이라는 상태를 false
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
            lockTarget = null;
        }

        if (lockTarget != null)
        {
            ReadyToAttack();
        }
        else
        {
            anim.SetBool("Move", true);
            anim.SetFloat("MoveSpeed", 0f);
            currentState = EnemyState.IDLE;
        }
    }

    private void ReadyToAttack()
    {
        if (attackLook == true)
        {
            Vector3 dir = lockTarget.transform.position - transform.position;
            Quaternion quat = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Lerp(transform.rotation, quat, 20 * Time.deltaTime);
        }

        if (isAttacking == false)
        {
            isAttacking = true;

            anim.SetInteger("AttackNum", attackCount);

            attackCount++;
            if (attackCount >= damageDealers.Length - 1)
            {
                attackCount = 0;
            }
        }
    }

    protected override void UpdateDie()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f &&
                    anim.GetCurrentAnimatorStateInfo(0).IsName("Die"))
        {
            ItemSpawnManager.instance.SpawnItem(SelecteRandomPosition(), dropList.GetDrop());

            gameObject.SetActive(false);
            spawn.ReturnObject(this.gameObject);
        }
    }
    #endregion

    protected override void UpdateHPBar()
    {
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
    #endregion
}

[CustomEditor(typeof(EnemyAI))]
public class EnemyEditor : Editor
{
    void OnSceneGUI()
    {
        EnemyAI enemy = (EnemyAI)target;

        //chase
        Handles.color = Color.yellow;
        Handles.DrawWireArc(enemy.transform.position, Vector3.up, Vector3.forward, 360, enemy.scanRange_gui);
    }
}
