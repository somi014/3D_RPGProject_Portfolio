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
                        hasDealtDamage.Add(target);                     //�̹� ������ Ÿ������ �����ϱ� ����
                    }
                }
            }
        }
    }

    /// <summary>
    /// ���� ���� Ÿ���� �ִ���
    /// </summary>
    private void FindVisibleTargets()
    {
        visibleTargets.Clear();

        // viewRadius�� ���������� �� �� ���� �� targetMask ���̾��� �ݶ��̴��� ��� ������
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;

            // �÷��̾�� forward�� target�� �̷�� ���� ������ ���� �����
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle)
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

    /// <summary>
    /// y�� ���Ϸ� ���� 3���� ���� ���ͷ� ��ȯ
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