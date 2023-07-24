using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : CharaterBase
{
    
    protected override void OnEnable()
    {
        base.OnEnable();
    }

    public override void Recover(float heal)
    {
        base.Recover(heal);
    }

    public override void Damage(float damage)
    {
        base.Damage(damage);
    }

    protected override void Die()
    {
        base.Die();
        Debug.Log("Player die");
    }
}