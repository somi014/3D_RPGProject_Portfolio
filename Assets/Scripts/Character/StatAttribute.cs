using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum Statistic
{
    Life,
    Energy,
    Damage,
    Armor,
    HealthRegeneration,
    Level,
    Experience
}

[Serializable]
public class StatsValue
{
    public Statistic statisticType;
    public bool typeFloat;
    public int integer_value;
    public int extra_integer_value;

    public float float_value;
    public float extra_float_value;

    public StatsValue(Statistic statisticType, int value = 0)
    {
        this.statisticType = statisticType;
        this.integer_value = value;
    }

    public StatsValue(Statistic statisticType, float float_value = 0)
    {
        this.statisticType = statisticType;
        this.float_value = float_value;
        typeFloat = true;
    }
}

[Serializable]
public class StatsGroup
{
    public List<StatsValue> stats;

    public StatsGroup()
    {
        stats = new List<StatsValue>();
    }

    public void Init()
    {
        stats.Add(new StatsValue(Statistic.Life, 100));
        stats.Add(new StatsValue(Statistic.Energy, 100));
        stats.Add(new StatsValue(Statistic.Damage, 25));
        stats.Add(new StatsValue(Statistic.Armor, 5));
        stats.Add(new StatsValue(Statistic.HealthRegeneration, 1));
        stats.Add(new StatsValue(Statistic.Level, 1));
        stats.Add(new StatsValue(Statistic.Experience, 10));
    }

    public StatsValue Get(Statistic statisticToGet)
    {
        return stats[(int)statisticToGet];
    }

    public void OriginSum(Statistic type, int value)
    {
        StatsValue statsValue = stats[(int)type];
        if (statsValue.typeFloat == false)
        {
            statsValue.integer_value += value;
        }
    }

    public void Sum(StatsValue toAdd)
    {
        StatsValue statsValue = stats[(int)toAdd.statisticType];
        if (toAdd.typeFloat == true)
        {
            statsValue.float_value += toAdd.float_value;
            statsValue.extra_float_value += toAdd.float_value;
        }
        else
        {
            statsValue.integer_value += toAdd.integer_value;
            statsValue.extra_integer_value += toAdd.integer_value;
        }
    }

    public void Subtract(StatsValue toSubtract)
    {
        StatsValue statsValue = stats[(int)toSubtract.statisticType];

        if (toSubtract.typeFloat == true)
        {
            statsValue.float_value -= toSubtract.float_value;
            statsValue.extra_float_value -= toSubtract.float_value;
        }
        else
        {
            statsValue.integer_value -= toSubtract.integer_value;
            statsValue.extra_integer_value -= toSubtract.integer_value;
        }
    }
}

#region Attribute
public enum Attribute
{
    Strength,
    Dexterity,
    Interlligence
}

[Serializable]
public class AttributeValue
{
    public Attribute attributeType;
    public int value;

    public AttributeValue(Attribute attributeType, int value = 0)
    {
        this.attributeType = attributeType;
        this.value = value;
    }
}

[Serializable]
public class AttributeGroup
{
    public List<AttributeValue> attributeValues;

    public AttributeGroup()
    {
        attributeValues = new List<AttributeValue>();
        Init();
    }

    public void Init()
    {
        attributeValues.Add(new AttributeValue(Attribute.Strength));
        attributeValues.Add(new AttributeValue(Attribute.Dexterity));
        attributeValues.Add(new AttributeValue(Attribute.Interlligence));
    }
    public AttributeValue Get(Attribute attributeToGet)
    {
        return attributeValues[(int)attributeToGet];
    }

    public void OriginSum(Attribute type, int value)
    {
        AttributeValue statsValue = attributeValues[(int)type];
        statsValue.value += value;
    }

    public void Sum(AttributeValue toAdd)
    {
        AttributeValue statsValue = attributeValues[(int)toAdd.attributeType];
        statsValue.value += toAdd.value;
    }

    public void Subtract(AttributeValue toSubtract)
    {
        AttributeValue statsValue = attributeValues[(int)toSubtract.attributeType];
        statsValue.value -= toSubtract.value;
    }
}
#endregion

[Serializable]
public class ValuePool
{
    public StatsValue maxValue;
    public int currentValue;

    public ValuePool(StatsValue maxValue)
    {
        this.maxValue = maxValue;
        this.currentValue = maxValue.integer_value;
    }

    public void Restore(int v)
    {
        currentValue += v;
        if (currentValue > maxValue.integer_value)
        {
            currentValue = maxValue.integer_value;
        }
    }
}

public class StatAttribute : MonoBehaviour
{
    [SerializeField]
    private AttributeGroup attributes;
    [SerializeField]
    private StatsGroup stats;
    public ValuePool lifePool;
    public ValuePool energyPool;
    public ValuePool experiencePool;

    public GameObject hudDamageText;
    public Transform hudPos;
    private Animator anim;

    public bool isDead;

    private void Awake()
    {
        attributes = new AttributeGroup();
        attributes.Init();

        stats = new StatsGroup();
        stats.Init();

        lifePool = new ValuePool(stats.Get(Statistic.Life));
        energyPool = new ValuePool(stats.Get(Statistic.Energy));
        experiencePool = new ValuePool(stats.Get(Statistic.Experience));
        experiencePool.currentValue = 0;

        anim = transform.GetComponent<Animator>();
    }

    private void OnEnable()
    {
        lifePool.currentValue = lifePool.maxValue.integer_value;
        energyPool.currentValue = energyPool.maxValue.integer_value;

        isDead = false;
    }

    public void LifeRegeneration()
    {
        StartCoroutine(IELifeRegeneration(1f));
    }

    IEnumerator IELifeRegeneration(float speed)
    {
        int regen = stats.Get(Statistic.HealthRegeneration).integer_value;

        for (int i = 0; i < 10; i++)
        {
            Heal(regen);
            yield return new WaitForSeconds(0.2f);
        }
    }

    public void Heal(int v)
    {
        lifePool.Restore(v);
    }

    public void TakeDamage(int damage)
    {
        if (isDead == true)
        {
            return;
        }

        damage = ApplyDefence(damage);                          //방어력에 따라 데미지 다르게
        lifePool.currentValue -= damage;

        GameObject hudText = Instantiate(hudDamageText);        // 생성할 텍스트 오브젝트
        hudText.transform.SetParent(hudPos.transform);
        hudText.transform.position = hudPos.position;           // 표시될 위치
        hudText.GetComponent<DamageText>().damage = damage;     // 데미지 전달

        CheckDeath();


        if (isDead == true)
        {
            anim.SetTrigger("Die");
            return;
        }

        if (transform.TryGetComponent(out PlayerStateManager player) == true)
        {
            if (player.currentState == player.idlingState)
            {
                player.GetComponent<PlayerStateManager>().SwitchState(player.damageState);
            }
            else
            {
                return;
            }
        }
        anim.SetTrigger("GetHit");
    }

    private int ApplyDefence(int damage)
    {
        damage -= stats.Get(Statistic.Armor).integer_value;

        if (damage <= 0)
        {
            damage = 1;
        }

        return damage;
    }

    private void CheckDeath()
    {
        if (lifePool.currentValue <= 0)
        {
            isDead = true;
        }
    }

    /// <summary>
    /// 스탯 반환
    /// </summary>
    /// <param name="statisticToGet"></param>
    /// <returns></returns>
    public StatsValue TakeStats(Statistic statisticToGet)
    {
        return stats.Get(statisticToGet);
    }

    #region 경험치 & 레벨 업
    public void GetExperience(int value)
    {
        experiencePool.currentValue += value;
        if (experiencePool.currentValue >= experiencePool.maxValue.integer_value)
        {
            int left = experiencePool.currentValue - experiencePool.maxValue.integer_value;
            experiencePool.currentValue = left;
            LevelUp(left);
        }
    }

    private void LevelUp(int value)
    {
        //레벨 업 했을 때 경험치 통 상승
        stats.OriginSum(Statistic.Level, 1);
        stats.OriginSum(Statistic.Experience, 10);

        GameEventsManager.instance.playerEvents.PlayerLevelChange(stats.stats[(int)Statistic.Level].integer_value);
        SoundManager.instance.Play(2);

        StatAttributeUp();

        if (experiencePool.currentValue >= experiencePool.maxValue.integer_value)               //초과한 경험치가 있을 때
        {
            int left = experiencePool.currentValue - experiencePool.maxValue.integer_value;
            LevelUp(left);
        }
    }

    /// <summary>
    /// 레벨업에 따른 능력 추가
    /// </summary>
    private void StatAttributeUp()
    {
        stats.OriginSum(Statistic.Life, 20);
        stats.OriginSum(Statistic.Energy, 20);
        stats.OriginSum(Statistic.Damage, 10);
        stats.OriginSum(Statistic.Armor, 8);

        ValuePool clone = new ValuePool(stats.Get(Statistic.Life));
        lifePool = clone;
        lifePool.Restore(lifePool.maxValue.integer_value);

        attributes.OriginSum(Attribute.Strength, 2);
        attributes.OriginSum(Attribute.Dexterity, 2);
        attributes.OriginSum(Attribute.Interlligence, 2);

        StatAttributeUIUpdate();
    }

    /// <summary>
    /// 스탯 창 업데이트
    /// </summary>
    public void StatAttributeUIUpdate()
    {
        for (int i = 0; i < stats.stats.Count; i++)
        {
            GameEventsManager.instance.playerEvents.StatChanged(this, stats.stats[i].statisticType);     //스탯 창 변경 표시
        }

        for (int i = 0; i < attributes.attributeValues.Count; i++)
        {
            GameEventsManager.instance.playerEvents.AttributeChanged(this, attributes.attributeValues[i].attributeType);
        }
    }
    #endregion

    #region 스탯 변화
    public void AddStats(List<StatsValue> statsValue)
    {
        for (int i = 0; i < statsValue.Count; i++)
        {
            StatsAdd(statsValue[i]);
        }
    }

    private void StatsAdd(StatsValue statsValue)
    {
        stats.Sum(statsValue);

        GameEventsManager.instance.playerEvents.StatChanged(this, statsValue.statisticType);        //스탯 창 변경 표시
    }

    public void SubtractStats(List<StatsValue> statsValues)
    {
        for (int i = 0; i < statsValues.Count; i++)
        {
            StatsSubtract(statsValues[i]);
        }
    }

    private void StatsSubtract(StatsValue statsValue)
    {
        stats.Subtract(statsValue);

        GameEventsManager.instance.playerEvents.StatChanged(this, statsValue.statisticType);        //스탯 창 변경 표시
    }
    #endregion

    #region Attribute Change
    public void AddAttributes(List<AttributeValue> attributesValues)
    {
        for (int i = 0; i < attributesValues.Count; i++)
        {
            AttributesAdd(attributesValues[i]);
        }
    }

    private void AttributesAdd(AttributeValue attributesValue)
    {
        attributes.Sum(attributesValue);

        GameEventsManager.instance.playerEvents.AttributeChanged(this, attributesValue.attributeType);
    }

    public void SubtractAttributes(List<AttributeValue> attributesValues)
    {
        for (int i = 0; i < attributesValues.Count; i++)
        {
            AttributesSubtract(attributesValues[i]);
        }
    }

    private void AttributesSubtract(AttributeValue attributesValue)
    {
        attributes.Subtract(attributesValue);

        GameEventsManager.instance.playerEvents.AttributeChanged(this, attributesValue.attributeType);
    }
    #endregion
}