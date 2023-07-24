using UnityEngine;

public struct CharacterStatus
{
    public float health_max;
    public float health_value { get; set; }
    public float attack_value;
    public float AttackValue { get; set; }
    public bool dead;
}

public class CharaterBase : MonoBehaviour
{
    public CharacterStatus status;
    private Animator animator;

    protected virtual void OnEnable()
    {
        status.dead = false;
        status.health_value = status.health_max;
    }

    public virtual void Recover(float heal)
    {
        if (status.dead == true)
            return;
        
        status.health_value += heal;
        if (status.health_value >= status.health_max)
        {
            status.health_value = status.health_max;
        }
    }
    
    public virtual void Damage(float damage)
    {
        if (status.dead == true)
            return;
        
        status.health_value -= damage;
        if (status.health_value <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        status.dead = true;
    }
}
