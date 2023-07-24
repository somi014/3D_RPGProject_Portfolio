using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class DamageDealer : MonoBehaviour
{
    bool canDealDamage;
    //[SerializeField] bool isPlayer;

    [SerializeField] float weaponDamage;
    [Range(0, 360)] public float viewAngle;
    public float viewRadius;

    public LayerMask targetMask;
    public LayerMask obstacleMask;

    public List<Transform> visibleTargets;
    public List<GameObject> hasDealtDamage;

    StatAttribute stats;

    void Start()
    {
        canDealDamage = false;
        visibleTargets = new List<Transform>();
        hasDealtDamage = new List<GameObject>();

        stats = GetComponentInParent<StatAttribute>();

        //StartCoroutine(FindTargetsWithDelay(0.2f));
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

    void FixedUpdate()
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
                        targetStat.TakeDamage(stats.TakeStats(Statistic.Damage).integer_value);
                        hasDealtDamage.Add(target);
                    }
                }
            }
        }
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

    void FindVisibleTargets()
    {
        visibleTargets.Clear();
        // viewRadius�� ���������� �� �� ���� �� targetMask ���̾��� �ݶ��̴��� ��� ������
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;

            // �÷��̾�� forward�� target�� �̷�� ���� ������ ���� �����
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle )
            {
                float dstToTarget = Vector3.Distance(transform.position, target.transform.position);

                // Ÿ������ ���� ����ĳ��Ʈ�� obstacleMask�� �ɸ��� ������ visibleTargets�� Add
                if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
                {
                    visibleTargets.Add(target);
                }
            }
        }
    }

    // y�� ���Ϸ� ���� 3���� ���� ���ͷ� ��ȯ�Ѵ�.
    // ������ ������ ��¦ �ٸ��� ����. ����� ����.
    public Vector3 DirFromAngle(float angleDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleDegrees += transform.eulerAngles.y;
        }

        return new Vector3(Mathf.Cos((-angleDegrees + 90) * Mathf.Deg2Rad), 0, Mathf.Sin((-angleDegrees + 90) * Mathf.Deg2Rad));
    }
}