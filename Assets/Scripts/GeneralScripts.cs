using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region Interfaces

public interface IDamageable
{
    void Damage(int damage);
    bool CheckKill();
    void Kill();
}

public interface IHealable
{
    void Heal(int heal);
}

#endregion
