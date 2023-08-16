using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyState
{
    IDLE,
    CHASE,
    ATTACK,
    HIT,
    DIE
}

public abstract class EnemyControl : MonoBehaviour
{
    [SerializeField]
    protected EnemyState currentState = EnemyState.IDLE;

    [SerializeField]
    protected DamageDealer[] damageDealers;

    protected GameObject lockTarget;
    protected Vector3 destPos;

    [SerializeField] 
    protected float attackRange;
    [SerializeField]
    protected float scanRange;
    [SerializeField] 
    protected float speed;

    [Header("Check For Edit")]
    [SerializeField]
    protected bool deadDrop;
    [SerializeField] 
    protected bool isReturn;

    private void Awake()
    {
        Init();
    }

    private void OnEnable()
    {       
        Spawn();
    }

    protected virtual void Init() {  }
    protected virtual void Spawn() { StartCoroutine(IEUpdate()); }

    IEnumerator IEUpdate()
    {
        do
        {
            switch (currentState)
            {
                case EnemyState.IDLE:
                    UpdateIdle();
                    break;
                case EnemyState.CHASE:
                    UpdateMoving();
                    break;
                case EnemyState.ATTACK:
                    UpdateSkill();
                    break;
                case EnemyState.HIT:
                    break;
                case EnemyState.DIE:
                    UpdateDie();
                    break;
            }
            yield return null;
            
            UpdateHPBar();

        } while (true);
    }

    protected virtual void UpdateIdle() { }
    protected virtual void UpdateMoving() { }
    protected virtual void UpdateSkill() { }
    protected virtual void UpdateDie() { }
    protected virtual void UpdateHPBar() { }
}