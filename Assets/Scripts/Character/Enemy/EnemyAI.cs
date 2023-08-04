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
    private bool isAttacking = false;                //���� ������ üũ
    private int currencyHp = -1;
    private int attackCount;
    [HideInInspector] public float scanRange_gui;    //GUI ���� ǥ�ÿ�

    public bool stop = false;                        //�׽�Ʈ ��
    
    [Header("Hp Bar UI")]
    [SerializeField] private Image hpBar;

    private Vector3 originPos;

    [Header("Drop Item")]
    [SerializeField] private ItemDropList dropList;
    [SerializeField] private float itemDropRange = 2f;

    protected override void Init()
    {
        if (TryGetComponent(out StatAttribute stats) == true)
            this.stats = stats;
        if (TryGetComponent(out Animator anim) == true)
            this.anim = anim;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<StatAttribute>();

        spawn = transform.parent.GetComponent<EnemySpawn>();

        if (TryGetComponent(out CapsuleCollider col) == true)
            this.col = col;

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
            _lockTarget = player.gameObject;

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
        // �÷��̾ �� �����Ÿ����� ������ ����
        if (_lockTarget != null)
        {
            _destPos = _lockTarget.transform.position;
            float distance = (_destPos - transform.position).magnitude;
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
            _destPos = originPos;
        }

        // �̵�
        Vector3 dir = _destPos - transform.position;
        if (dir.magnitude < 0.1f)
        {
            currentState = EnemyState.IDLE;
            anim.SetBool("Move", true);
        }
        else        // ������������ �Ÿ��� �ſ� �۴ٸ�(�����ߴٸ�) �̵� ���̶�� ���¸� false
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
                if (attackCount >= damageDealers.Length - 1)
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

            //anim.SetBool("Die", true);
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
