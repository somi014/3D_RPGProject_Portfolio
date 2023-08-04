using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class DamageDealer : MonoBehaviour
{
    [SerializeField] CameraShake cameraShake;

    private bool canDealDamage;

    [SerializeField] private int damage;
    [Range(0, 360)] public float viewAngle;
    public float viewRadius;

    public LayerMask targetMask;
    public LayerMask obstacleMask;

    public List<Transform> visibleTargets;
    public List<GameObject> hasDealtDamage;

    private StatAttribute stats;

    private void Start()
    {
        canDealDamage = false;
        visibleTargets = new List<Transform>();
        hasDealtDamage = new List<GameObject>();

        stats = GetComponentInParent<StatAttribute>();
    }

    private void OnEnable()
    {
        StartCoroutine(FindTargetsWithDelay(0.2f));
    }

    IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }

    private void FixedUpdate()
    {
        if (canDealDamage == true)
        {
            for (int i = 0; i < visibleTargets.Count; i++)
            {
                GameObject target = visibleTargets[i].gameObject;
                target.transform.TryGetComponent(out StatAttribute targetStat);

                if (targetStat.isDead == false)
                {
                    if (!hasDealtDamage.Contains(target))
                    {
                        if (cameraShake != null)
                        {
                            cameraShake.ShakeCamera(1f);
                        }

                        int tempDamage = damage + stats.TakeStats(Statistic.Damage).integer_value;
                        targetStat.TakeDamage(tempDamage);
                        hasDealtDamage.Add(target);                     //이미 공격한 타겟인지 구분하기 위해
                    }
                }
            }
        }
    }

    /// <summary>
    /// 범위 내에 타겟이 있는지
    /// </summary>
    private void FindVisibleTargets()
    {
        visibleTargets.Clear();

        // viewRadius를 반지름으로 한 원 영역 내 targetMask 레이어인 콜라이더를 모두 가져옴
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;

            // 플레이어와 forward와 target이 이루는 각이 설정한 각도 내라면
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle)
            {
                float dstToTarget = Vector3.Distance(transform.position, target.transform.position);

                // 타겟으로 가는 레이캐스트에 obstacleMask가 걸리지 않으면 visibleTargets에 Add
                if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
                {
                    visibleTargets.Add(target);
                }
            }
        }
    }

    /// <summary>
    /// y축 오일러 각을 3차원 방향 벡터로 변환
    /// </summary>
    /// <param name="angleDegrees"></param>
    /// <param name="angleIsGlobal"></param>
    /// <returns></returns>
    public Vector3 DirFromAngle(float angleDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleDegrees += transform.eulerAngles.y;
        }

        return new Vector3(Mathf.Cos((-angleDegrees + 90) * Mathf.Deg2Rad), 0, Mathf.Sin((-angleDegrees + 90) * Mathf.Deg2Rad));
    }
    public void StartDealDamage()
    {
        canDealDamage = true;
        hasDealtDamage.Clear();
    }

    public void EndDealDamage()
    {
        canDealDamage = false;
    }

}